using System;

public class Program
{
    private static List<int> AStarAlgorithm(int startCity, int goalCity, int[,] distances)
    {
        var openSet = new List<Node>();
        var closedSet = new HashSet<int>();

        var startNode = new Node { Index = startCity, G = 0, H = distances[startCity, goalCity], Parent = null };
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

            for (int i = 0; i < distances.GetLength(0); i++)
            {
                if (distances[currentNode.Index, i] > 0)
                {
                    if (closedSet.Contains(i))
                        continue;

                    var tentativeGCost = currentNode.G + distances[currentNode.Index, i];

                    var neighbor = openSet.Find(n => n.Index == i);
                    if (neighbor == null || tentativeGCost < neighbor.G)
                    {
                        if (neighbor != null)
                            openSet.Remove(neighbor);

                        neighbor = new Node { Index = i };
                        neighbor.Parent = currentNode;
                        neighbor.G = tentativeGCost;
                        neighbor.H = distances[i, goalCity];
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

    private static void PrintPath(List<int> path, List<string> cities)
    {
        Console.WriteLine("\nZnaleziona ścieżka:");
        Console.Write(cities[path[0]]);
        for (int i = 1; i < path.Count; i++)
            Console.Write(" -> " + cities[path[i]]);
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
                Console.WriteLine("Znaleziono rozwiązanie!");
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
            Console.WriteLine();
        }
    }

    static void PrintState(int[,] state)
    {
        for (int i = 0; i < state.GetLength(0); i++)
        {
            for (int j = 0; j < state.GetLength(1); j++)
                Console.Write(state[i, j] + " ");
            Console.WriteLine();
        }
    }

    public static void Main()
    {
        Console.WriteLine("Zadanie 1:");

        List<string> cities = new List<string>
        { "Warszawa", "Kraków", "Łódź", "Wrocław", "Poznań", "Gdańsk", "Szczecin", "Bydgoszcz", "Toruń", "Katowice" };

        //Długości tras między miastami z map Google wybierame po najszybszej trasie
        //Tam gdzie trasa między miastami przebiega przez inne miasto, długość trasy jest równa 0
        int[,] distances = new int[,]
        {
            { 0,    294,    136,    0,      0,      341,    0,      0,      259,    295 },
            { 294,  0,      0,      0,      0,      0,      0,      0,      0,      81  },
            { 136,  0,      0,      221,    214,    0,      0,      0,      183,    203 },
            { 0,    0,      221,    0,      181,    0,      392,    0,      0,      195 },
            { 0,    0,      214,    181,    0,      0,      264,    138,    0,      0   },
            { 341,  0,      0,      0,      0,      0,      368,    167,    170,    0   },
            { 0,    0,      0,      392,    264,    368,    0,      259,    0,      0   },
            { 0,    0,      0,      0,      138,    167,    259,    0,      46,     0   },
            { 259,  0,      183,    0,      0,      170,    0,      46,     0,      0 },
            { 295,  81,     203,    195,    0,      0,      0,      0,      0,      0   }
        };

        for (int i = 0; i < cities.Count(); i++)
            Console.WriteLine(i + ". " + cities[i]);

        int startCity = 0;
        int endCity = 0;

        try
        {
            Console.WriteLine("\nPodaj numer miasta początkowego: ");
            startCity = int.Parse(Console.ReadLine());

            Console.WriteLine("Podaj numer miasta końcowego: ");
            endCity = int.Parse(Console.ReadLine());

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

        List<int> path = AStarAlgorithm(startCity, endCity, distances);

        if (path.Count > 0)
            PrintPath(path, cities);
        else
            Console.WriteLine("Nie udało się znaleźć ścieżki.");

        Console.WriteLine("\nZadanie 2:");

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