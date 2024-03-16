using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

public class Program
{
    public static int MinimaxAlphaBeta(int[] values, int depth, int h, int nodeIndex,  int alpha, int beta, bool maximizingPlayer)
    {
        if (nodeIndex < 0 || nodeIndex >= values.Length)
        {
            throw new Exception("Podano niepoprawną ilość wartości");
        }

        if (depth == 0)
            return values[nodeIndex];

        int childCount;
        if (depth == h)
            childCount = (int)Math.Ceiling(values.Length / Math.Pow(2, depth - 1));
        else
            childCount = 2;

        if (maximizingPlayer)
        {
            int bestValue = int.MinValue;
            for (int i = 0; i < childCount; i++)
            {
                bestValue = Math.Max(bestValue, MinimaxAlphaBeta(values, depth - 1, h, nodeIndex * 2 + i, alpha, beta, false));
                alpha = Math.Max(alpha, bestValue);
                if (beta <= alpha)
                    break;
            }
            return bestValue;
        }
        else
        {
            int bestValue = int.MaxValue;
            for (int i = 0; i < childCount; i++)
            {
                bestValue = Math.Min(bestValue, MinimaxAlphaBeta(values, depth - 1, h, nodeIndex * 2 + i, alpha, beta, true));
                beta = Math.Min(beta, bestValue);
                if (beta <= alpha)
                    break;
            }
            return bestValue;
        }
    }

    public static void Main()
    {
        int[] values = { 4, 6, 7, 9, 1, 2, 0, 1, 8, 1, 9, 2 };
        int depth = 3;

        try
        {
            Console.WriteLine("Wartość najlepszego ruchu: " + MinimaxAlphaBeta(values, depth, depth, 0, int.MinValue, int.MaxValue, true));
        }
        catch (Exception e)
        {
            Console.WriteLine("Błąd: " + e.Message);
        }
    }
}