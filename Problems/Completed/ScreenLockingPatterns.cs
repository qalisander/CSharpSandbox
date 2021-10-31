using System;

namespace Problems.Completed
{
    // NOTE: https://www.codewars.com/kata/585894545a8a07255e0002f1/train/csharp
    public class ScreenLockingPatterns
    {
        public static bool[,] DotIsPeeked = new bool[3, 3];

        public static int CountPatternsFrom(char firstDot, int length)
        {
            var (x, y) = ParseToIndex(firstDot);

            return CountPatterns(x, y, length);
        }
        public static int CountPatterns(int x, int y, int length)
        {
            if (length == 1)
                return 1;
            if (length == 0)
                return 0;

            var count = 0;

            DotIsPeeked[x,y] = true;

            for (int x2 = 0; x2 < 3; x2++)
            {
                for (int y2 = 0; y2 < 3; y2++)
                {
                    if (DotIsPeeked[x2, y2])
                        continue;

                    if (TryGetMid(x, y, x2, y2, out var midX, out var midY)
                        && !DotIsPeeked[midX, midY])
                        continue;

                    count += CountPatterns(x2, y2, length - 1);
                }
            }

            DotIsPeeked[x, y] = false;

            return count;
        }

        public static bool TryGetMid(int x, int y, int x2, int y2, out int midX, out int midY)
        {
            var avgX = Math.Abs(x + x2) / 2.0m;
            var avgY = Math.Abs(y + y2) / 2.0m;

            midX = (int)avgX;
            midY = (int)avgY;

            return avgX == midX && avgY == midY;
        }

        public static (int x, int y) ParseToIndex(char dot) => dot switch
        {
            'A' => (0, 0),
            'B' => (1, 0),
            'C' => (2, 0),
            'D' => (0, 1),
            'E' => (1, 1),
            'F' => (2, 1),
            'G' => (0, 2),
            'H' => (1, 2),
            'I' => (2, 2),
            _ => (-1, -1)
        };
    }
}
