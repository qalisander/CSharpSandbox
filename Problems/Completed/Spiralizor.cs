using System;
using System.Collections.Generic;
using System.Linq;

namespace Problems
{
    public static class Spiralizor
    {
        // public static int[,] Spiralize(int size)
        // {
        //     var dirs = new[] { new Id(0, 1), new Id(1, 0), new Id(0, -1), new Id(-1, 0) };
        //     var currDirId = 0;
        //
        //     var arr = new int[size, size];
        //     for (int i = 0; i < size; i++)
        //         for (int j = 0; j < size; j++)
        //             arr[i, j] = 1;
        //
        //     Id curr = new Id(1, 0);
        //     arr[curr.I, curr.J] = 0;
        //
        //     while (true)
        //     {
        //         if (CanMoveNext(curr))
        //         {
        //             curr += CurrDir();
        //
        //             if (arr[curr.I, curr.J] == 0)
        //                 break;
        //
        //             arr[curr.I, curr.J] = 0;
        //         }
        //         else
        //         {
        //             ChangeDir();
        //
        //             if (!CanMoveNext(curr))
        //                 break;
        //         }
        //     };
        //
        //     return arr;
        //
        //     bool CanMoveNext(Id curr) => IsRightDir(curr + CurrDir() * 2);
        //
        //     bool IsRightDir(Id id) =>
        //         0 <= id.I && id.I < size && 0 <= id.J && id.J < size
        //         && arr[id.I, id.J] == 1;
        //
        //     void ChangeDir() => currDirId = (currDirId + 1) % dirs.Length;
        //
        //     Id CurrDir() => dirs[currDirId];
        // }
        //
        // private readonly struct Id
        // {
        //     public readonly int I;
        //     public readonly int J;
        //
        //     public Id(int i, int j)
        //     {
        //         I = i;
        //         J = j;
        //     }
        //
        //     public static Id operator +(Id left, Id right) =>
        //         new Id(left.I + right.I, left.J + right.J);
        //
        //     public static Id operator *(Id crdnt, int num) =>
        //         new Id(crdnt.I * num, crdnt.J * num);
        // }


        public static int[,] Spiralize(int size)
        {
            var spiral = new int[size, size];
            foreach (var p in Walk(-2, 0, 1, 0, size + 1))
            {
                spiral[p.y, p.x] = 1;
            }
            return spiral;      
        }
  
        private static IEnumerable<(int x, int y)> Walk(int x, int y, int dx, int dy, int l)
        {
            if (l <= 0) yield break;
            for (var i = 0; i < l; i++) 
            {
                x += dx;
                y += dy;
                if (x >= 0) yield return (x, y);
                if (l == 1) yield break;
            }
            foreach (var next in Walk(x, y, -dy, dx, dy == 0 ? l - 2 : l)) 
            {
                yield return next;
            }
        }
    }
}
