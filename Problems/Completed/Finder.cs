using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.CompilerServices;

namespace Problems
{
    public class Finder
    {
        public static bool PathFinder(string maze)
        {
            if(maze == ".")
                return true;

            var charArray = maze.Split('\n').Select(str => str.ToCharArray()).ToArray();
            var currDots = new Queue<(int x, int y)>();

            var dirs = new(int i, int j)[] { (-1, 0), (1, 0), (0, 1), (0, -1) };
            currDots.Enqueue((0,0));
            bool wFound = false;
            do
            {
                var (i, j) = currDots.Dequeue();

                foreach (var dir in dirs)
                    CheckField(i + dir.i, j + dir.j);

                void CheckField(int iCheck, int jCheck)
                {
                    if (IsOutRange(iCheck, jCheck))
                        return;

                    if (IsEndPoint(iCheck, jCheck))
                        wFound = true;

                    if (charArray[iCheck][jCheck] != '.')
                        return;

                    currDots.Enqueue((iCheck, jCheck));
                    charArray[iCheck][jCheck] = 'C';
                }

                bool IsEndPoint(int i, int j) =>
                    i == charArray.Length - 1 && j == charArray[^1].Length - 1;

                bool IsOutRange(int iCheck, int jCheck) =>
                    0 > iCheck || iCheck >= charArray.Length || 0 > jCheck || jCheck >= charArray[j].Length;

            } while (wFound == false && currDots.Count != 0);

            Console.WriteLine(string.Join('\n', charArray.Select(arr => string.Concat(arr))));

            return wFound;
        }

        // private static bool PathFinder(int[][] maze, int x = 0, int y = 0) =>
        //     (x >= 0 && x < maze[0].Length) 
        //     && (y >= 0 && y < maze.Length)
        //     && (maze[y][x] == 0) 
        //     && ((x + 1 == maze[0].Length && y + 1 == maze.Length) 
        //         || (maze[y][x] = -1) == -1
        //             && (PathFinder(maze, x + 1, y) 
        //                 || PathFinder(maze, x - 1, y) 
        //                 || PathFinder(maze, x, y + 1) 
        //                 || PathFinder(maze, x, y - 1)));
        
        // public static bool PathFinder(string maze) => 
        //     PathFinder(maze.Split('\n').Select(
        //         line => line .Select(c => '.' - c).ToArray()
        //     ).ToArray());
    }
}
