using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
        public Tile? Previous { get; set; }
        public Tile? Next { get; set; }
        public int Position { get; set; } = -1;

        public IEnumerable<(int x, int y)> NextPossibleCoordinates((int x, int y)? from = null)
        {
            from ??= Previous.Coordinates;
            
            var delta = (from.Value.x - Coordinates.x, from.Value.y - Coordinates.y);
            var newCoordinates = CharToNewTileDeltas[(Symbol, delta)].AddToAll(Coordinates).ToArray();

            return newCoordinates.Any()
                ? newCoordinates
                : char.IsLetter(Symbol) // NOTE: train or station
                    ? CharToNewTileDeltas[('-', delta)].AddToAll(Coordinates).ToArray()
                    : Enumerable.Empty<(int x, int y)>();
        }

        private static readonly ILookup<(char ch, (int dx, int dy) incomingDir), (int dx, int dy)> CharToNewTileDeltas =
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
        public Train(string str, int position = -1)
        {
            Position = position;
            Char = char.ToUpper(str[0]);
            Length = str.Length;
            IsClockwise = char.IsLower(str[0]);
        }
        public char Char { get; }
        public bool IsClockwise { get; }
        public int Length { get; }
        public Tile? Head { get; set; }
        public Tile? Tail { get; set; }

        public int Position { get; set; } // TODO: Position should use Head tile
        public int TimeToWait { get; set; } = 0;
        public bool IsExpress => Char == 'X';

        public void Move()
        {
            if (IsExpress || TimeToWait == 0)
            {
                Head = Head.Next;
                Tail = Tail.Next;

                TimeToWait = Head.IsStation ? Length : 0;
            }
            else
            {
                TimeToWait--;
            }
        }
    }
    
    public class Field
    {
        private readonly Tile[][] _field;
        private Tile? TryGetTile((int x, int y) point) => IsInBounds(point) ? _field[point.y][point.x] : null;
        private bool IsInBounds((int x, int y) point) =>
            0 <= point.y && point.y < _field.Length && 0 <= point.x && point.x < _field[0].Length;

        private Tile ZeroTile { get; }

        private readonly Dictionary<char, Train> _charToTrain = new Dictionary<char, Train>();
        public IEnumerable<Train> Trains => _charToTrain.Values;

        public Field(string track, string firstTrain, int firstTrainPos, string secondTrain, int secondTrainPos)
        {
            var strings = track.Split(new []{'\r','\n'}, StringSplitOptions.RemoveEmptyEntries);
            var maxLength = strings.Max(str => str.Length);

            _field = strings
                     .Select((str, i) =>
                         str.PadRight(maxLength, ' ').ToCharArray().Select((ch, j) => 
                             new Tile(ch, (j, i))).ToArray())
                     .ToArray();

            ZeroTile = _field[0].FirstOrDefault(tile => tile.Symbol != ' ')
                       ?? throw new InvalidOperationException("Zero tile not found");

            _charToTrain[char.ToUpper(firstTrain[0])] = new Train(firstTrain, firstTrainPos);
            _charToTrain[char.ToUpper(secondTrain[0])] = new Train(secondTrain, secondTrainPos);
        }

        public Field CreateTrack()
        {
            var current = TryGetTile((ZeroTile.Coordinates.x + 1, ZeroTile.Coordinates.y));

            if (current is null || current.Symbol != '-')
                throw new InvalidOperationException($"Invalid tile!\n{Highlight(ZeroTile)}>");

            ZeroTile.Next = current;
            ZeroTile.Position = 0;
            
            current.Previous = ZeroTile;
            current.Position = 1;

            while (current != ZeroTile)
            {
                SetTrainInfo(current);

                var nextTile = GetNextTile(current);

                current.Next = nextTile;
                nextTile.Previous = current;
                nextTile.Position = current.Position + 1;

                current = nextTile;
            }

            return this;
        }
        
        private Tile GetNextTile(Tile current)
        {

            var nextTile = current.NextPossibleCoordinates()
                                  .Select(TryGetTile)
                                  .FirstOrDefault(tile =>
                                      tile != null && tile.NextPossibleCoordinates(current.Coordinates).Any());

            if (nextTile is null || nextTile.IsEmpty)
                throw new InvalidOperationException($"Invalid tile!\n{Highlight(current)}>");

            // NOTE: process crossing tile
            if (nextTile.Position != -1)
                nextTile = new Tile(nextTile.Symbol, nextTile.Coordinates);

            return nextTile;
        }

        private void SetTrainInfo(Tile current)
        {
            if (!_charToTrain.TryGetValue(char.ToUpper(current.Symbol), out var train))
                return;

            var isTrainHead = char.IsUpper(current.Symbol);

            if (isTrainHead && current.Position != train.Position)
            {
                train.Position = train.Position == -1
                    ? current.Position
                    : throw new InvalidOperationException(
                        $"Train <{train.Char}> expected to have position: {train.Position} but has: {current.Position}");
            }

            if (isTrainHead)
            {
                train.Head = current;

                if (train.Length == 1)
                    train.Tail = current;
            }
            else if (train.Head is null)
            {
                train.Tail ??= current;
            }
            else
            {
                train.Tail = current;
            }
        }

        public bool CollisionDetected()
        {
            return Trains.SelectMany(GetCoordinates)
                         .GroupBy(point => point)
                         .Any(g => g.Count() > 1);

            IEnumerable<(int x, int y)> GetCoordinates(Train train)
            {
                for (var tile = train.Tail; tile != train.Head!.Next; tile = train.IsClockwise ? tile!.Next : tile!.Previous)
                {
                    yield return tile!.Coordinates;
                }
            }
        }

        public string Highlight(Tile tileToHighlight) =>
            new StringBuilder()
                .AppendJoin(null, _field.SelectMany(tiles =>
                    tiles.Select(tile => tileToHighlight.Coordinates == tile.Coordinates ? '@' : tile.Symbol).Append('\n')))
                .Append($"  @ - current tile: {tileToHighlight}\n")
                .ToString();
    }
    
    public static class Dinglemouse
    {
        public static int TrainCrash(string trackStr, string aTrain, int aTrainPos, string bTrain, int bTrainPos, int limit)
        {
            var field = new Field(trackStr, aTrain, aTrainPos, bTrain, bTrainPos).CreateTrack();

            for (var i = 0; i < limit; i++)
            {
                if (field.CollisionDetected())
                    return i;

                foreach (var train in field.Trains)
                    train.Move();
            }

            return -1;
        }

        public static IEnumerable<(int, int)> AddToAll(this IEnumerable<(int x, int y)> enumerable, (int x, int y) term) =>
            enumerable.Select(tpl => (tpl.x + term.x, tpl.y + term.y));
    }
}
