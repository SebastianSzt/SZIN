using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;

namespace Zadanie1
{
    internal class Program
    {
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

        static double[] CoordinateConverter(double longitude, double latitude, double h)
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

        static double[][] CalculateDistances(List<City> cities)
        {
            int numberOfCities = cities.Count;
            double[][] distances = new double[numberOfCities][];

            for (int i = 0; i < numberOfCities; i++)
            {
                distances[i] = new double[numberOfCities];
                for (int j = 0; j < numberOfCities; j++)
                {
                    if (i == j)
                    {
                        distances[i][j] = 0;
                    }
                    else
                    {
                        double distance = Math.Sqrt(Math.Pow(cities[i].LocationX - cities[j].LocationX, 2) + Math.Pow(cities[i].LocationY - cities[j].LocationY, 2) + Math.Pow(cities[i].LocationZ - cities[j].LocationZ, 2));
                        distances[i][j] = distance;
                    }
                }
            }

            return distances;
        }

        static void TSP(List<City> cities, double[][] distances)
        {
            Console.WriteLine("Problem komiwojażera (odległości i suma podane w kilometrach zaokrąglone w górę do cyfry jedności)\n");
            double[] routeSum = new double[cities.Count];
            for (int i = 0; i < cities.Count; i++)
            {
                List<string> cityNames = new List<string>();

                for (int j = 0; j < cities.Count; j++)
                        cityNames.Add(cities[j].Name);

                Console.Write(i + 1 + ". " + cities[i].Name);
                cityNames.RemoveAt(cityNames.IndexOf(cities[i].Name));

                int prefix = i;

                while (cityNames.Count > 0)
                {
                    double minDistance = Double.MaxValue;
                    int minPosition = -1;
                    for (int j = 0; j < cities.Count; j++)
                    {
                        if (distances[prefix][j] < minDistance && cityNames.IndexOf(cities[j].Name) != -1)
                        {
                            minDistance = distances[prefix][j];
                            minPosition = j;
                        }
                    }
                    prefix = minPosition;
                    routeSum[i] += minDistance;
                    Console.Write(" ---" + Math.Ceiling(minDistance / 1000.0) + "--> " + cities[prefix].Name);
                    cityNames.RemoveAt(cityNames.IndexOf(cities[prefix].Name));
                }
                routeSum[i] += distances[prefix][i];
                Console.Write(" ---" + Math.Ceiling(distances[prefix][i] / 1000.0) + "--> " + cities[i].Name);
                Console.Write("\t Suma:" + Math.Ceiling(routeSum[i] / 1000.0) + "km");
                Console.WriteLine();
                Console.WriteLine();
            }

            double minRoute = routeSum.Min();
            List<int> index = new List<int>();
            for (int i = 0; i < routeSum.Length; i++)
            {
                if (routeSum[i] == minRoute)
                {
                    index.Add(i);
                }
            }

            Console.WriteLine();
            Console.Write("Ściezka/i z najmniejszą długością: ");
            for (int i = 0; i < index.Count; i++)
            {
                Console.Write((index[i] + 1) + " ");
            }
            Console.WriteLine("(" + Math.Ceiling(minRoute / 1000.0) + "km)");
        }

        private static void Main()
        {
            List<City> cities = ReadFile("Cities.txt");

            double[][] distances = CalculateDistances(cities);

            //Console.Write("\t\t");
            //for (int i = 0; i < cities.Count; i++)
            //{
            //    Console.Write($"{cities[i].Name,-15}");
            //}
            //Console.WriteLine("\n");

            //for (int i = 0; i < cities.Count; i++)
            //{
            //    Console.Write($"{cities[i].Name,-15}\t");
            //    for (int j = 0; j < cities.Count; j++)
            //    {
            //        double distanceInKilometers = Math.Ceiling(distances[i][j] / 1000.0);
            //        Console.Write($"{distanceInKilometers,-15}");
            //    }
            //    Console.WriteLine();
            //}

            //Console.WriteLine("\n\n");

            TSP(cities, distances);
        }
    }
}