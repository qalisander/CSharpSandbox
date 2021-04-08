using System;
using System.Collections.Generic;
using System.Linq;

namespace Experiments
{
public class Sudoku
{
    private readonly int[][] _sudokuData;
    private int Length { get; set; }
    private int LengthSqrt { get; set; }
    public Sudoku(int[][] sudokuData)
    {
        _sudokuData = sudokuData;
        Length = _sudokuData.Length;
        LengthSqrt = (int) Math.Sqrt(Length);  
    }
    public bool IsValid()
    { 
        foreach (var arr in _sudokuData)
            if (arr.Length != Length)
                return false;
        for (var i = 0; i < Length; i++)
            if (!SequenceIsValid(HorizontalSequence(i)) || !SequenceIsValid(VerticalSequence(i)))
                return false;
        for (var i = 0; i < Length; i += LengthSqrt)
            for (var j = 0; j < Length; j += LengthSqrt)
                if (!SequenceIsValid(SquareSequence(i, j)))
                    return false;
        return true;
    }
    public IEnumerable<int> HorizontalSequence(int i) => _sudokuData[i];
    public IEnumerable<int> VerticalSequence(int j) => 
        Enumerable.Range(0, Length).Select(i => _sudokuData[i][j]);
    public IEnumerable<int> SquareSequence(int iOrigin, int jOrigin)
    {
        for (var i = iOrigin; i < LengthSqrt + iOrigin; i++)
            for (var j = jOrigin;j < LengthSqrt + jOrigin; j++)
                yield return _sudokuData[i][j];
    }
    public bool SequenceIsValid(IEnumerable<int> enumerable) =>
        Enumerable.SequenceEqual(enumerable.OrderBy(x => x), Enumerable.Range(1, Length));
}
}