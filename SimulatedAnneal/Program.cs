using System;

class Program
{
    static Random random = new Random();

    static double Function(double x)
    {
        return 3 * Math.Sin(Math.PI * x / 5) + Math.Sin(Math.PI * x);
    }

    static double AcceptanceProbability(double deltaE, double temperature)
    {
        if (deltaE < 0)
            return 1.0;
        return Math.Exp(-deltaE / temperature);
    }

    static double SimulatedAnnealingAlgorithm(double lowerBound, double upperBound, int maxEpochs, int numTrialsPerEpoch, double initialTemperature, double coolingRate)
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

                if (trialSolution == 0)
                    Console.WriteLine("0");

                if (Function(trialSolution) < Function(currentSolution))
                    currentSolution = trialSolution;
                else
                {
                    double deltaE = Function(trialSolution) - Function(currentSolution);
                    if (AcceptanceProbability(deltaE, currentTemperature) > random.NextDouble())
                        currentSolution = trialSolution;
                }
            }

            currentTemperature *= coolingRate;
        }

        return currentSolution;
    }

    static void Main()
    {
        double lowerBound = 0;
        double upperBound = 10;
        int maxEpochs = 5;
        int numTrialsPerEpoch = 3;
        double initialTemperature = 1;
        double coolingRate = 0.9;
        
        double result = SimulatedAnnealingAlgorithm(lowerBound, upperBound ,maxEpochs, numTrialsPerEpoch, initialTemperature, coolingRate);

        Console.WriteLine("Znalezione minimum dla x = " + result);
        Console.WriteLine("Znaleziona wartość minimalna funkcji = " + Function(result));
    }
}