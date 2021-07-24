using System;
using System.Collections.Generic;
using System.Linq;

// https://www.codewars.com/kata/59b47ff18bcb77a4d1000076/train/csharp
namespace Experiments
{
    public class Tile
    {
        public Tile(char symbol, (int x, int y) coordinates)
        {
            Symbol = symbol;
            Coordinates = coordinates;
        }
        
        public char Symbol { get; }
        public bool IsStation => Symbol == 'S';
        public bool IsEmpty => Symbol == ' ';
        public (int x, int y) Coordinates { get; }
        
        public Train? Train { get; set; }
        public Tile? Previous { get; set; }
        public Tile? Next { get; set; }
        public int Position { get; set; } = -1;

        public (int x, int y)[] NextPossibleCoordinates((int x, int y)? from = null)
        {
            var nextCoord =
                (Coordinates.x - (from?.x ?? Previous.Coordinates.x), Coordinates.y - (from?.y ?? Previous.Coordinates.y));
            
            var newDirections = _charToNewDirections[(Symbol, nextCoord)].ToArray();

            return newDirections.Any()
                ? newDirections
                : char.IsLetter(Symbol) // NOTE: train or station
                    ? _charToNewDirections[('-', nextCoord)].ToArray()
                    : throw new InvalidOperationException("Symbol not found in hashmap");
        }

        private static readonly ILookup<(char ch, (int dx, int dy) incomingDir), (int dx, int dy)> _charToNewDirections =
            MixOutputAndInput(GetDirections()).ToLookup(t => (t.ch, input: t.@from), t => t.to);

        public override string ToString() => $"{nameof(Symbol)}: {Symbol}, {nameof(Coordinates)}: {Coordinates}, {nameof(Position)}: {Position}";
        private static IEnumerable<(char ch, (int dx, int dy) from, (int dx, int dy) to)> GetDirections()
        {
            //            (0, 1)
            //     (-1, 1)  |    (1, 1)
            //              |
            // (-1, 0) -----+----- (1, 0)
            //              |
            //    (-1, -1)  |   (1, -1)
            //           (0, -1)
            
            var charToDeltaDirection = new (char ch, (int x, int y) from, (int x, int y) to)[]
            {
                ('|', (0, -1), (0, 1)),
                ('-', (-1, 0), (1, 0)),
                ('X', (-1, -1), (1, 1)),
                ('X', (1, -1), (-1, 1)),
                ('+', (0, -1), (0, 1)),
                ('+', (1, 0), (0, 1)),
                ///////////////////
                ('/', (-1, 0), (0, 1)),
                ('/', (-1, 0), (1, 1)),
                ///////////////////
                ('/', (-1, -1), (0, 1)),
                ('/', (-1, -1), (1, 1)),
                ('/', (-1, -1), (1, 0)),
                //////////////////
                ('/', (0, -1), (1, 1)),
                ('/', (0, -1), (1, 0)),
            };

            foreach (var tpl in charToDeltaDirection)
            {
                yield return tpl;

                if (tpl.ch == '/')
                    yield return ('\\', (-tpl.@from.x, -tpl.@from.y), (-tpl.to.x, -tpl.to.y));

                if (tpl.ch == 'X' || tpl.ch == '+')
                    yield return ('S', tpl.@from, tpl.to);
            }
        }

        private static IEnumerable<(char ch, T from, T to)> MixOutputAndInput<T>(IEnumerable<(char ch, T from, T to)> enumerable)
        {
            foreach (var tpl in enumerable)
            {
                yield return (tpl.ch, tpl.from, tpl.to);
                yield return (tpl.ch, tpl.to, tpl.from);
            }
        }
    }

    public class Train
    {
        public Train(string str)
        {
            Char = char.ToUpper(str[0]);
            Length = str.Length;
            HasClockwiseDirection = char.IsLower(str[0]);
        }
        public char Char { get; set; }
        public bool HasClockwiseDirection { get; set; }
        public int Length { get; set; }

        public int Position { get; set; }
        public int TimeToWait { get; set; } = 0;
        public bool IsExpress => Char == 'X';
    }
    
    public class Field
    {
        private readonly Tile[][] _field;
        private Tile? TryGetTile((int x, int y) point) => IsInBounds(point) ? _field[point.y][point.x] : null;
        private bool IsInBounds((int x, int y) point) =>
            0 <= point.y && point.y < _field.Length && 0 <= point.x && point.x < _field[0].Length;

        private Tile ZeroTile { get; }

        private readonly Dictionary<char, Train> Trains = new Dictionary<char, Train>();

        public Field(string track, string firstTrain, string secondTrain)
        {
            _field = track
                     .Split('\n')
                     .Select((str, i) => str.ToCharArray().Select((ch, j) => new Tile(ch, (j, i))).ToArray())
                     .ToArray();

            ZeroTile = _field[0].FirstOrDefault(tile => tile.Symbol != ' ')
                       ?? throw new InvalidOperationException("Zero tile not found");

            Trains[char.ToUpper(firstTrain[0])] = new Train(firstTrain);
            Trains[char.ToUpper(secondTrain[0])] = new Train(secondTrain);
        }

        public Field CreateTrack()
        {
            var current = TryGetTile((ZeroTile.Coordinates.x + 1, ZeroTile.Coordinates.y));

            if (current is null || current.Symbol != '-')
                throw new InvalidOperationException($"Invalid track on tile: <{ZeroTile}>");

            ZeroTile.Next = current;
            ZeroTile.Position = 0;
            
            current.Previous = ZeroTile;
            current.Position = 1;

            while (current != ZeroTile)
            {
                var nextTile = current.NextPossibleCoordinates()
                                      .Select(TryGetTile)
                                      .FirstOrDefault(tile =>
                                          tile != null && tile.NextPossibleCoordinates(current.Coordinates).Any());

                if (nextTile is null)
                    throw new InvalidOperationException($"Invalid track on tile: <{current}>");

                current.Next = nextTile;
                nextTile.Previous = current;
                nextTile.Position = current.Position + 1;
                current = nextTile;

                if (Trains.TryGetValue(char.ToUpper(current.Symbol), out var train))
                {
                    // TODO: logic with trains
                }
            }

            return this;
        }
    }
    
    public class Dinglemouse
    {
        public static int TrainCrash(
            string trackStr, string aTrain, int aTrainPos, string bTrain, int bTrainPos, int limit)
        {
            var track = new Field(trackStr, aTrain, bTrain).CreateTrack();

            for (var step = 1; step <= limit; step++)
            {
                throw new NotImplementedException();
            }

            throw new NotImplementedException();
        }
    }
}
