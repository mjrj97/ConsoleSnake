using System;
using System.Collections.Generic;
using System.Threading;

namespace Snake
{
    class Program
    {
        // Size of the display
        const int sizeX = 16;
        const int sizeY = 16;

        // Game values
        const int FLOOR = 1;
        const int APPLE = 0;
        const int FPS = 2;

        // List of presses since start
        static readonly List<char> presses = new();

        // Gameplay Grid
        static readonly byte[,] grid = new byte[sizeX, sizeY];

        // Random object
        static readonly Random random = new();

        static void Main()
        {
            DateTime lastTime = DateTime.Now;
            Clear();

            // Place apple
            grid[5, 3] = APPLE;

            // Place snake
            grid[2, 3] = 2;
            grid[3, 3] = 3;

            Console.CursorVisible = false;
            Thread keyLogger = new(GetKeyPress);
            keyLogger.Start();
            presses.Add('d');

            // Gameplay loop
            while (true)
            {
                if ((DateTime.Now - lastTime) > TimeSpan.FromSeconds(1.0/((double)FPS)))
                {
                    Move();
                    Render();
                    lastTime = DateTime.Now;
                }
            }
        }

        static void Move()
        {
            // Find the head
            int headX = 0;
            int headY = 0;

            int largestValue = FLOOR + 1;

            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    if (grid[x,y] > largestValue)
                    {
                        headX = x;
                        headY = y;
                        largestValue = grid[x, y];
                    }
                }
            }

            // Use the last key press
            switch (presses[^1])
            {
                case 'w':
                    headY--;
                    break;
                case 'a':
                    headX--;
                    break;
                case 's':
                    headY++;
                    break;
                case 'd':
                    headX++;
                    break;
            }

            // Clamp the snake to the grid and make it appear on the other side if moving into wall
            headX %= sizeX;
            headY %= sizeY;
            if (headX < 0)
                headX += sizeX;
            if (headY < 0)
                headY += sizeY;

            // Eat the apple or move the entire snake
            if (grid[headX,headY] == APPLE)
            {
                grid[headX, headY] = (byte)(largestValue + 1);
                PlaceApple();
            }
            else
            {
                for (int x = 0; x < sizeX; x++)
                {
                    for (int y = 0; y < sizeY; y++)
                    {
                        if (grid[x, y] > FLOOR)
                            grid[x, y]--;
                    }
                }
                grid[headX, headY] = (byte)(largestValue);
            }
        }

        static void PlaceApple()
        {
            int x = random.Next(0, sizeX-1);
            int y = random.Next(0, sizeY-1);
            if (grid[x,y] == FLOOR)
            {
                grid[x, y] = 0;
            }
            else
            {
                PlaceApple();
            }
        }

        static void Render()
        {
            Console.SetCursorPosition(0, 0);
            int head = 0;
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    if (grid[x, y] > head)
                        head = grid[x, y];
                }
            }

            Console.ForegroundColor = ConsoleColor.White;
            for (int i = 0; i < sizeX * 2 + 2; i++)
            {
                Console.Write("-");
            }
            Console.Write("\n");
            for (int y = 0; y < sizeX; y++)
            {
                Console.Write("|");
                for (int x = 0; x < sizeY; x++)
                {
                    if (grid[x,y] == head)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("O ");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else if (grid[x, y] > FLOOR)
                    {
                        Console.ForegroundColor = ConsoleColor.Green;
                        Console.Write("# ");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else if (grid[x, y] == APPLE)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.Write("a ");
                        Console.ForegroundColor = ConsoleColor.White;
                    }
                    else
                        Console.Write("  ");
                }
                Console.Write("|\n");
            }
            for (int i = 0; i < sizeX * 2 + 2; i++)
            {
                Console.Write("-");
            }
            Console.Write("\n");
            Console.ForegroundColor = ConsoleColor.Black;
        }

        static void Clear()
        {
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    grid[x, y] = FLOOR;
                }
            }
        }

        static void GetKeyPress()
        {
            while (true)
            {
                ConsoleKeyInfo info = Console.ReadKey();
                presses.Add(info.KeyChar);
            }
        }
    }
}
