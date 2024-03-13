using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class Program
{
    static List<City> ReadFile(string fileName)
    {
        List<City> cities = new List<City>();

        try
        {
            using (StreamReader reader = new StreamReader(fileName))
            {
                while (!reader.EndOfStream)
                {
                    string line = reader.ReadLine();
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        string[] parts = line.Split(new char[] { '\t' }, StringSplitOptions.RemoveEmptyEntries);

                        if (parts.Length == 3)
                        {
                            double[] locations = CoordinateConverter(ParseCoordinateToDouble(parts[1]), ParseCoordinateToDouble(parts[2]), 0);

                            City city = new City(parts[0], locations[0], locations[1]);
                            cities.Add(city);
                        }
                    }
                }
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
        }
        return cities;
    }

    static double[] CoordinateConverter(double latitude, double longitude, double h)
    {
        double a = 6378137.0;
        double b = 6356752.3;
        double e2 = 1 - (Math.Pow(b, 2) / Math.Pow(a, 2));
        double latr = latitude / 90 * 0.5 * Math.PI;
        double lonr = longitude / 180 * Math.PI;
        double Nphi = a / Math.Sqrt(1 - e2 * Math.Pow(Math.Sin(latr), 2));

        double[] locations = new double[3];
        locations[0] = (Nphi + h) * Math.Cos(latr) * Math.Cos(lonr);
        locations[1] = (Nphi + h) * Math.Cos(latr) * Math.Sin(lonr);

        return locations;
    }

    static double ParseCoordinateToDouble(string coordinate)
    {
        string[] parts = coordinate.Split('°', '\'');

        double degrees = double.Parse(parts[0]);
        double minutes = double.Parse(parts[1]);

        double result = degrees + minutes / 60.0;

        if (coordinate.Contains("W") || coordinate.Contains("S"))
        {
            result = -result;
        }

        return result;
    }

    static int[,] CalculateDistances(List<City> cities)
    {
        int numberOfCities = cities.Count;
        int[,] distances = new int[numberOfCities, numberOfCities];

        for (int i = 0; i < numberOfCities; i++)
        {
            for (int j = i; j < numberOfCities; j++)
            {
                if (i == j)
                {
                    distances[i, j] = 0;
                }
                else
                {
                    double distance = Math.Sqrt(Math.Pow(cities[i].LocationX - cities[j].LocationX, 2) + Math.Pow(cities[i].LocationY - cities[j].LocationY, 2));
                    distances[i, j] = (int)Math.Ceiling(distance / 1000.0);
                    distances[j, i] = (int)Math.Ceiling(distance / 1000.0);
                }
            }
        }

        return distances;
    }

    private static List<int> AStarAlgorithm(int startCity, int goalCity, int[,] GoogleDistances, int[,] HeuristicDistances )
    {
        var openSet = new List<Node>();
        var closedSet = new HashSet<int>();

        var startNode = new Node { Index = startCity, G = 0, H = HeuristicDistances[startCity, goalCity], Parent = null };
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            var currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
                if (openSet[i].F < currentNode.F || (openSet[i].F == currentNode.F && openSet[i].H < currentNode.H))
                    currentNode = openSet[i];

            if (currentNode.Index == goalCity)
                return ReconstructPath(currentNode);

            openSet.Remove(currentNode);
            closedSet.Add(currentNode.Index);

            for (int i = 0; i < GoogleDistances.GetLength(0); i++)
            {
                if (GoogleDistances[currentNode.Index, i] > 0)
                {
                    if (closedSet.Contains(i))
                        continue;

                    var NewGCost = currentNode.G + GoogleDistances[currentNode.Index, i];

                    var neighbor = openSet.Find(n => n.Index == i);
                    if (neighbor == null || NewGCost < neighbor.G)
                    {
                        if (neighbor != null)
                            openSet.Remove(neighbor);

                        neighbor = new Node { Index = i };
                        neighbor.Parent = currentNode;
                        neighbor.G = NewGCost;
                        neighbor.H = HeuristicDistances[i, goalCity];
                        openSet.Add(neighbor);
                    }
                }
            }
        }
        return new List<int>();
    }

    private static List<int> ReconstructPath(Node node)
    {
        List<int> path = new List<int>();
        while (node != null)
        {
            path.Insert(0, node.Index);
            node = node.Parent;
        }
        return path;
    }

    private static void PrintPath(List<int> path, List<City> cities)
    {
        Console.WriteLine("\nZnaleziona ścieżka:");
        Console.Write(cities[path[0]].Name);
        for (int i = 1; i < path.Count; i++)
            Console.Write(" -> " + cities[path[i]].Name);
        Console.WriteLine();
    }

    private static void AStarAlgorithmPuzzle(int[,] initialState, int[,] goalState)
    {
        var openSet = new List<NodePuzzle>();
        var closedSet = new HashSet<int[,]>();

        var startNode = new NodePuzzle(initialState, 0, CalculateHeuristic(initialState, goalState), null);
        openSet.Add(startNode);

        while (openSet.Count > 0)
        {
            var currentNode = openSet[0];
            for (int i = 1; i < openSet.Count; i++)
                if (openSet[i].F < currentNode.F)
                    currentNode = openSet[i];

            if (IsGoalState(currentNode.State, goalState))
            {
                Console.WriteLine("Znaleziono rozwiązanie:");
                PrintSolution(currentNode);
                return;
            }

            openSet.Remove(currentNode);
            closedSet.Add(currentNode.State);

            List<int[,]> neighbors = GetNeighbors(currentNode.State);
            foreach (var neighborState in neighbors)
            {
                if (closedSet.Contains(neighborState))
                    continue;

                int g = currentNode.G + 1;
                int h = CalculateHeuristic(neighborState, goalState);
                NodePuzzle neighborNode = new NodePuzzle(neighborState, g, h, currentNode);

                var existingNode = openSet.Find(n => n.State.Cast<int>().SequenceEqual(neighborNode.State.Cast<int>()));
                if (existingNode == null || g < existingNode.G)
                {
                    if (existingNode != null)
                        openSet.Remove(existingNode);

                    openSet.Add(neighborNode);
                }
            }
        }
        Console.WriteLine("Nie udało się znaleźć rozwiązania.");
    }

    static List<int[,]> GetNeighbors(int[,] state)
    {
        List<int[,]> neighbors = new List<int[,]>();
        int zeroRow = 0, zeroCol = 0;

        for (int i = 0; i < state.GetLength(0); i++)
            for (int j = 0; j < state.GetLength(1); j++)
                if (state[i, j] == 0)
                {
                    zeroRow = i;
                    zeroCol = j;
                    break;
                }

        if (zeroRow > 0)
            neighbors.Add(SwapTiles(state, zeroRow, zeroCol, zeroRow - 1, zeroCol));

        if (zeroRow < state.GetLength(0) - 1)
            neighbors.Add(SwapTiles(state, zeroRow, zeroCol, zeroRow + 1, zeroCol));

        if (zeroCol > 0)
            neighbors.Add(SwapTiles(state, zeroRow, zeroCol, zeroRow, zeroCol - 1));

        if (zeroCol < state.GetLength(1) - 1)
            neighbors.Add(SwapTiles(state, zeroRow, zeroCol, zeroRow, zeroCol + 1));

        return neighbors;
    }

    static int[,] SwapTiles(int[,] state, int row1, int col1, int row2, int col2)
    {
        int[,] newState = (int[,])state.Clone();
        int temp = newState[row1, col1];
        newState[row1, col1] = newState[row2, col2];
        newState[row2, col2] = temp;
        return newState;
    }

    static int CalculateHeuristic(int[,] initialState, int[,] goalState)
    {
        int heuristic = 0;
        for (int i = 0; i < initialState.GetLength(0); i++)
            for (int j = 0; j < initialState.GetLength(1); j++)
                if (initialState[i, j] != goalState[i, j] && initialState[i, j] != 0)
                    heuristic++;
        return heuristic;
    }

    static bool IsGoalState(int[,] initialState, int[,] goalState)
    {
        for (int i = 0; i < initialState.GetLength(0); i++)
            for (int j = 0; j < initialState.GetLength(1); j++)
                if (initialState[i, j] != goalState[i, j])
                    return false;
        return true;
    }

    static void PrintSolution(NodePuzzle node)
    {
        List<NodePuzzle> path = new List<NodePuzzle>();
        while (node != null)
        {
            path.Add(node);
            node = node.Parent;
        }
        for (int i = path.Count - 1; i >= 0; i--)
        {
            PrintState(path[i].State);
        }
    }

    static void PrintState(int[,] state)
    {
        Console.WriteLine();
        for (int i = 0; i < state.GetLength(0); i++)
        {
            for (int j = 0; j < state.GetLength(1); j++)
                Console.Write(state[i, j] + " ");
            Console.WriteLine();
        } 
    }

    public static void Main()
    {
        Console.WriteLine("######################### Zadanie 1: ######################### \n");

        List <City> cities = ReadFile("Cities.txt");
        int[,] HeuristicDistances = CalculateDistances(cities); //Odległość między miastami w linii prostej

        //Długości tras między miastami z map Google wybierane po najszybszej trasie
        //Tam gdzie trasa między miastami przebiega przez inne miasto, długość trasy między miastem początkowym a końcowym jest równa 0
        int[,] GoogleDistances = new int[,]
        {
        //             Warszawa Kraków  Łódź    Wrocław Poznań  Gdańsk  Szczeci Bydgosz Toruń   Katowice
        /*Warszawa*/    { 0,    294,    136,    0,      0,      341,    0,      0,      259,    295 },
        /*Kraków*/      { 294,  0,      0,      0,      0,      0,      0,      0,      0,      81  },
        /*Łódź*/        { 136,  0,      0,      221,    214,    0,      0,      0,      183,    203 },
        /*Wrocław*/     { 0,    0,      221,    0,      181,    0,      392,    0,      0,      195 },
        /*Poznań*/      { 0,    0,      214,    181,    0,      0,      264,    138,    0,      0   },
        /*Gdańsk*/      { 341,  0,      0,      0,      0,      0,      368,    167,    170,    0   },
        /*Szczecin*/    { 0,    0,      0,      392,    264,    368,    0,      259,    0,      0   },
        /*Bydgoszcz*/   { 0,    0,      0,      0,      138,    167,    259,    0,      46,     0   },
        /*Toruń*/       { 259,  0,      183,    0,      0,      170,    0,      46,     0,      0   },
        /*Katowice*/    { 295,  81,     203,    195,    0,      0,      0,      0,      0,      0   }
        };

        Console.WriteLine("Miasta:");
        for (int i = 0; i < cities.Count(); i++)
            Console.WriteLine(i + 1 + " - " + cities[i].Name);

        int startCity = 0;
        int endCity = 0;

        try
        {
            Console.WriteLine("\nPodaj numer miasta początkowego: ");
            startCity = int.Parse(Console.ReadLine()) - 1;

            Console.WriteLine("Podaj numer miasta końcowego: ");
            endCity = int.Parse(Console.ReadLine()) - 1;

            if (startCity < 0 || startCity > cities.Count() - 1 || endCity < 0 || endCity > cities.Count() - 1)
            {
                Console.WriteLine("\nNieprawidłowe miasto. Program zostanie zakończony.");
                return;
            }
        }
        catch (Exception)
        {
            Console.WriteLine("\nPodano nieprawidłowe dane. Program zostanie zakończony.");
            return;
        }

        List<int> path = AStarAlgorithm(startCity, endCity, GoogleDistances, HeuristicDistances);

        if (path.Count > 0)
            PrintPath(path, cities);
        else
            Console.WriteLine("Nie udało się znaleźć ścieżki.");

        Console.WriteLine("\n######################### Zadanie 2: #########################\n");

        int[,] initialState = {
            {1, 2, 3},
            {0, 4, 6},
            {7, 5, 8}
        };

        int[,] goalState = {
            { 1, 2, 3 },
            { 4, 5, 6 },
            { 7, 8, 0 }
        };

        AStarAlgorithmPuzzle(initialState, goalState);
    }
}