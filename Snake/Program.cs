using System;
using System.Collections.Generic;
using System.Threading;

namespace Snake
{
    class Program
    {
        static int sizeX = 16;
        static int sizeY = 16;

        static List<char> presses = new List<char>();
        static byte[,] grid = new byte[sizeX, sizeY];

        static void Main(string[] args)
        {
            DateTime lastTime = DateTime.Now;
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    grid[x, y] = 1;
                }
            }

            grid[5, 3] = 0;

            grid[2, 3] = 2;
            grid[3, 3] = 3;

            Console.CursorVisible = false;
            presses.Add('d');
            Thread keyLogger = new Thread(GetKeyPress);
            keyLogger.Start();

            ConsoleKeyInfo info = Console.ReadKey(false);
            while (true)
            {
                if ((DateTime.Now - lastTime) > TimeSpan.FromSeconds(0.5))
                {
                    Move();
                    Render();
                    lastTime = DateTime.Now;
                }
            }
        }

        public static void Move()
        {
            int headX = 0;
            int headY = 0;

            int largestValue = 2;

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

            switch (presses[presses.Count-1])
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

            headX = headX % sizeX;
            headY = headY % sizeY;

            if (headX < 0)
                headX = headX + sizeX;
            if (headY < 0)
                headY = headY + sizeY;

            if (grid[headX,headY] == 0)
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
                        if (grid[x, y] > 1)
                            grid[x, y]--;
                    }
                }
                grid[headX, headY] = (byte)(largestValue);
            }
        }

        public static void PlaceApple()
        {
            Random random = new Random();
            int x = random.Next(0, sizeX-1);
            int y = random.Next(0, sizeY-1);
            if (grid[x,y] == 1)
            {
                grid[x, y] = 0;
            }
            else
            {
                PlaceApple();
            }
        }

        public static void Render()
        {
            Console.SetCursorPosition(0, 0);
            int highestValue = 0;
            for (int x = 0; x < sizeX; x++)
            {
                for (int y = 0; y < sizeY; y++)
                {
                    if (grid[x, y] > highestValue)
                        highestValue = grid[x, y];
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
                    if (grid[x,y] == highestValue)
                        Console.Write("O ");
                    else if (grid[x, y] > 1 && grid[x,y] != highestValue)
                    {
                        Console.Write("# ");
                    }
                    else if (grid[x, y] == 0)
                        Console.Write("a ");
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

        public static void GetKeyPress()
        {
            while (true)
            {
                ConsoleKeyInfo info = Console.ReadKey();
                presses.Add(info.KeyChar);
            }
        }
    }
}
