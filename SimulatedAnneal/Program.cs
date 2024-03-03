using System;

class Program
{
    static Random random = new Random();

    public static void Swap<T>(IList<T> list, int indexA, int indexB)
    {
        T tmp = list[indexA];
        list[indexA] = list[indexB];
        list[indexB] = tmp;
    }

    static double Function(double x)
    {
        return 3 * Math.Sin(Math.PI * x / 5) + Math.Sin(Math.PI * x);
    }

    static double AcceptanceProbabilityMinValue(double deltaE, double temperature)
    {
        if (deltaE < 0)
            return 1.0;
        return Math.Exp(-deltaE / temperature);
    }

    static double SimulatedAnnealingAlgorithmMinValue(double lowerBound, double upperBound, int maxEpochs, int numTrialsPerEpoch, double initialTemperature, double coolingRate)
    {
        double currentTemperature = initialTemperature;

        double currentSolution = random.NextDouble() * (upperBound - lowerBound) + lowerBound;

        for (int epoch = 0; epoch < maxEpochs; epoch++)
        {
            for (int trial = 0; trial < numTrialsPerEpoch; trial++)
            {
                double rangeMin = Math.Max(lowerBound, currentSolution - 2 * currentTemperature);
                double rangeMax = Math.Min(upperBound, currentSolution + 2 * currentTemperature);

                double trialSolution = random.NextDouble() * (rangeMax - rangeMin) + rangeMin;

                if (Function(trialSolution) < Function(currentSolution))
                    currentSolution = trialSolution;
                else
                {
                    double deltaE = Function(trialSolution) - Function(currentSolution);
                    if (AcceptanceProbabilityMinValue(deltaE, currentTemperature) > random.NextDouble())
                        currentSolution = trialSolution;
                }
            }

            currentTemperature *= coolingRate;
        }

        return currentSolution;
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
        locations[2] = ((b * b) / (a * a) * Nphi + h) * Math.Sin(latr);

        return locations;
    }

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

                            City city = new City(parts[0], locations[0], locations[1], locations[2]);
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

    static double[,] CalculateDistances(List<City> cities)
    {
        int numberOfCities = cities.Count;
        double[,] distances = new double[numberOfCities, numberOfCities];

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
                    double distance = Math.Sqrt(Math.Pow(cities[i].LocationX - cities[j].LocationX, 2) + Math.Pow(cities[i].LocationY - cities[j].LocationY, 2) + Math.Pow(cities[i].LocationZ - cities[j].LocationZ, 2));
                    distances[i, j] = distance;
                    distances[j, i] = distance;
                }
            }
        }

        return distances;
    }

    static double CalculateTotalDistance(List<int> solution, double[,] distances)
    {
        double totalDistance = 0;
        for (int i = 0; i < solution.Count - 1; i++)
        {
            totalDistance += distances[solution[i], solution[i + 1]];
        }

        if (solution[solution.Count - 1] != solution[0])
            totalDistance += distances[solution[solution.Count - 1], solution[0]];
        return totalDistance;
    }

    static double AcceptanceProbabilityTSP(double deltaE, double temperature)
    {
        if (deltaE < 0)
            return 1.0;
        return 100*Math.Exp(-deltaE / temperature);
    }

    static List<int> SimulatedAnnealingAlgorithmTSP(int maxEpochs, int numTrialsPerEpoch, double initialTemperature, double coolingRate, List<City> cities, double[,] distances)
    {
        double currentTemperature = initialTemperature;

        List<int> currentSolution = Enumerable.Range(0, cities.Count).ToList();
        currentSolution = currentSolution.OrderBy(x => Random.Shared.Next()).ToList();

        for (int epoch = 0; epoch < maxEpochs; epoch++)
        {
            for (int trial = 0; trial < numTrialsPerEpoch; trial++)
            {
                List<int> trialSolution = new List<int>(currentSolution);
                int index1 = random.Next(0, trialSolution.Count);
                int index2 = random.Next(0, trialSolution.Count);
                while (index1 == index2)
                {
                    index2 = random.Next(currentSolution.Count);
                }
                Swap(trialSolution, index1, index2);


                if (CalculateTotalDistance(trialSolution, distances) < CalculateTotalDistance(currentSolution, distances))
                    currentSolution = trialSolution;
                else
                {
                    double deltaE = CalculateTotalDistance(trialSolution, distances) - CalculateTotalDistance(currentSolution, distances);
                    if (AcceptanceProbabilityTSP(deltaE, currentTemperature) > random.Next(1, 101))
                        currentSolution = trialSolution;
                }
            }

            currentTemperature *= coolingRate;
        }

        currentSolution.Add(currentSolution[0]);

        return currentSolution;
    }

    static void PrintRoute(List<int> route, List<City> cities, double[,] distances)
    {
        Console.WriteLine("Znaleziona najkrótsza trasa: ");

        for (int i = 0; i < route.Count - 1; i++)
        {
            Console.Write(cities[route[i]].Name + " ---" + Math.Ceiling(distances[route[i], route[i + 1]] / 1000.0) + "--> ");
        }
        Console.Write(cities[route[route.Count - 1]].Name);
        Console.Write("\t Suma:" + Math.Ceiling(CalculateTotalDistance(route, distances) / 1000.0) + "km");
        Console.WriteLine();
    }

    static void Main()
    {
        double lowerBound = 0;
        double upperBound = 10;
        int maxEpochs = 1000;
        int numTrialsPerEpoch = 500;
        double initialTemperature = 1;
        double coolingRate = 0.9;
        
        double result = SimulatedAnnealingAlgorithmMinValue(lowerBound, upperBound, maxEpochs, numTrialsPerEpoch, initialTemperature, coolingRate);

        Console.WriteLine("Znalezione minimum dla x = " + result);
        Console.WriteLine("Znaleziona wartość minimalna funkcji = " + Function(result));
        Console.WriteLine();

        Console.WriteLine("Problem komiwojażera (odległości i suma podane w kilometrach zaokrąglone w górę do cyfry jedności)\n");

        List<City> cities = ReadFile("Cities.txt");
        double[,] distances = CalculateDistances(cities);

        List<int> resultRoute = SimulatedAnnealingAlgorithmTSP(maxEpochs, numTrialsPerEpoch, initialTemperature, coolingRate, cities, distances);
        PrintRoute(resultRoute, cities, distances);
    }
}