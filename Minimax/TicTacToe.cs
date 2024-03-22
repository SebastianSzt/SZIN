using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

class TicTacToe
{
    static char EMPTY = ' ';
    static char COMPUTER = 'O';
    static char PLAYER = 'X';

    static void MakeMove(char[,] board)
    {
        int bestScore = int.MinValue;
        int bestMoveRow = -1;
        int bestMoveCol = -1;

        for (int i = 0; i < 3; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                if (board[i, j] == EMPTY)
                {
                    board[i, j] = COMPUTER;
                    int score = MinMax(board, 0, false);
                    board[i, j] = EMPTY;
                    if (score > bestScore)
                    {
                        bestScore = score;
                        bestMoveRow = i;
                        bestMoveCol = j;
                    }
                }
            }
        }
        board[bestMoveRow, bestMoveCol] = COMPUTER;
    }

    static int MinMax(char[,] board, int depth, bool isMaximizing)
    {
        int score = Evaluate(board);

        if (score != 0)
            return score;

        if (CheckDraw(board))
            return 0;

        if (isMaximizing)
        {
            int bestScore = int.MinValue;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (board[i, j] == EMPTY)
                    {
                        board[i, j] = COMPUTER;
                        bestScore = Math.Max(bestScore, MinMax(board, depth + 1, false));
                        board[i, j] = EMPTY;
                    }
                }
            }
            return bestScore;
        }
        else
        {
            int bestScore = int.MaxValue;
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (board[i, j] == EMPTY)
                    {
                        board[i, j] = PLAYER;
                        bestScore = Math.Min(bestScore, MinMax(board, depth + 1, true));
                        board[i, j] = EMPTY;
                    }
                }
            }
            return bestScore;
        }
    }

    static int Evaluate(char[,] board)
    {
        if (CheckWin(board, COMPUTER))
            return 1;
        else if (CheckWin(board, PLAYER))
            return -1;
        else
            return 0;
    }

    static bool CheckWin(char[,] board, char player)
    {
        for (int i = 0; i < 3; i++)
            if ((board[i, 0] == player && board[i, 1] == player && board[i, 2] == player) || (board[0, i] == player && board[1, i] == player && board[2, i] == player))
                return true;

        if ((board[0, 0] == player && board[1, 1] == player && board[2, 2] == player) || (board[0, 2] == player && board[1, 1] == player && board[2, 0] == player))
            return true;

        return false;
    }

    static bool CheckDraw(char[,] board)
    {
        for (int i = 0; i < 3; i++)
            for (int j = 0; j < 3; j++)
                if (board[i, j] == EMPTY)
                    return false;
        return true;
    }

    static void DrawBoard(char[,] board)
    {
        Console.WriteLine("-------------");
        for (int i = 0; i < 3; i++)
        {
            Console.Write("| ");
            for (int j = 0; j < 3; j++)
            {
                if (board[i, j] == EMPTY)
                {
                    Console.Write("  | ");
                }
                else
                {
                    Console.Write(board[i, j] + " | ");
                }
            }
            Console.WriteLine();
            Console.WriteLine("-------------");
        }
    }

    public static void Main()
    {
        char[,] board = new char[3, 3]
        {
            {' ', ' ', ' '},
            {' ', ' ', ' '},
            {' ', ' ', ' '}
        };

        bool playerTurn = true;
        bool gameEnd = false;

        DrawBoard(board);

        while (!gameEnd)
        {
            if (playerTurn)
            {
                Console.WriteLine();
                Console.WriteLine("Podaj współrzędne ruchu w formacie 'wiersz kolumna': ");
                string[] input = Console.ReadLine().Split(' ');
                Console.WriteLine();
                int row = -1;
                int col = -1;
                try
                {
                    row = int.Parse(input[0]) - 1;
                    col = int.Parse(input[1]) - 1;
                }
                catch (Exception)
                { }

                if (row >= 0 && row < 3 && col >= 0 && col < 3 && board[row, col] == EMPTY)
                {
                    board[row, col] = PLAYER;
                    playerTurn = false;
                    Console.Clear();
                    Console.WriteLine("Twój ruch:");
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Nieprawidłowy ruch, spróbuj ponownie.");
                    Console.WriteLine();
                    DrawBoard(board);
                    continue;
                }
            }
            else
            {
                playerTurn = true;
                Console.WriteLine();
                Console.WriteLine("Ruch komputera:");
                MakeMove(board);
            }

            DrawBoard(board);

            if (CheckWin(board, COMPUTER))
            {
                Console.WriteLine("Komputer wygrał.");
                gameEnd = true;
            }
            else if (CheckWin(board, PLAYER))
            {
                Console.WriteLine("Wygrałeś.");
                gameEnd = true;
            }
            else if (CheckDraw(board))
            {
                Console.WriteLine("Remis.");
                gameEnd = true;
            }
        }
    }
}