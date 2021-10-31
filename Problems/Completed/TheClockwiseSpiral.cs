using System;
using System.Collections;
using System.Collections.Generic;

public class TheClockwiseSpiral
{
    public static int[,] CreateSpiral(int N)
    {
        var ans = new int[N, N];

        var i = 0;
        foreach (var point in GetPoints(-1, 0, 1, 0, N))
            ans[point.y, point.x] = ++i;

        return ans;
    }

    public static IEnumerable<(int x, int y)> GetPoints(int x, int y, int dX, int dY, int N)
    {
        if (N == 0)
            yield break;

        for (int i = 0; i < N; i++)
            yield return (x += dX, y += dY);

        foreach (var point in GetPoints(x, y, -dY, dX, dY == 0 ? --N : N))
            yield return point;
    }
}
