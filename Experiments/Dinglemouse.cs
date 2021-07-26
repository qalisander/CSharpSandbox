#nullable enable
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

        public IEnumerable<(int x, int y)> NextPossibleCoordinates((int x, int y) from)
        {
            var delta = (from.x - Coordinates.x, from.y - Coordinates.y);
            var newCoordinates = CharToNewTileDeltas[(Symbol, delta)].AddToAll(Coordinates).ToArray();

            return newCoordinates.Any()
                ? newCoordinates
                : char.IsLetter(Symbol) // NOTE: train or station
                    ? CharToNewTileDeltas[('-', delta)].AddToAll(Coordinates).ToArray()
                    : Enumerable.Empty<(int x, int y)>();
        }

        private static readonly ILookup<(char ch, (int dx, int dy) incomingDir), (int dx, int dy)> CharToNewTileDeltas =
            MixFromAndTo(GetDirections()).ToLookup(t => (t.ch, input: t.@from), t => t.to);

        private static IEnumerable<(char ch, (int dx, int dy) from, (int dx, int dy) to)> GetDirections()
        {
            //           (0, -1)            --------->
            //    (-1, -1)  |    (1, -1)    |        X
            //              |               |
            // (-1, 0) -----+----- (1, 0)   |
            //              |               V Y
            //     (-1, 1)  |   (1, 1)     
            //            (0, 1)            
            
            var charToDeltaDirection = new (char ch, (int x, int y) from, (int x, int y) to)[]
            {
                ('|', (0, 1), (0, -1)),
                ('-', (-1, 0), (1, 0)),
                ('X', (-1, 1), (1, -1)),
                ('X', (-1, -1), (1, 1)),
                ///////////////////
                ('/', (-1, 0), (0, -1)),
                ('/', (-1, 0), (1, -1)),
                ///////////////////
                ('/', (-1, 1), (0, -1)),
                ('/', (-1, 1), (1, -1)),
                ('/', (-1, 1), (1, 0)),
                //////////////////
                ('/', (0, 1), (1, -1)),
                ('/', (0, 1), (1, 0)),
            };

            foreach (var tpl in charToDeltaDirection)
            {
                if (tpl.ch == '/')
                    yield return ('\\', (-tpl.@from.x, tpl.@from.y), (-tpl.to.x, tpl.to.y)); // NOTE: reflection respective to axis y

                if (tpl.ch == '-' || tpl.ch == '|')
                {
                    yield return ('+', tpl.from, tpl.to);
                    yield return ('S', tpl.from, tpl.to);
                }
                
                if (tpl.ch == 'X')
                    yield return ('S', tpl.@from, tpl.to);

                yield return tpl;
            }
        }

        private static IEnumerable<(V ch, T from, T to)> MixFromAndTo<V, T>(IEnumerable<(V ch, T from, T to)> enumerable)
        {
            foreach (var tpl in enumerable)
            {
                yield return (tpl.ch, tpl.from, tpl.to);
                yield return (tpl.ch, tpl.to, tpl.from);
            }
        }
        
        public override string ToString() => $"{nameof(Symbol)}: '{Symbol}', {nameof(Coordinates)}: {Coordinates}, {nameof(Position)}: {Position}";
    }

    public class Train
    {
        private readonly Field _field;
        public Train( string str, Field field, int position = -1)
        {
            _field = field;
            Position = position;
            Char = char.ToUpper(str[0]);
            Length = str.Length;
            IsClockwise = char.IsLower(str[0]);
            Head = field.GetByPosition(position);
            Tail = field.GetByPosition(IsClockwise ? position - Length + 1: position + Length - 1);
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
                Head = IsClockwise ? Head.Next : Head.Previous;
                Tail = IsClockwise ? Tail.Next : Tail.Previous;

                TimeToWait = Head.IsStation ? Length - 1 : 0;
            }
            else
            {
                TimeToWait--;
            }

            var actualLength = GetTiles().Count();
            if (actualLength != Length)
                throw new InvalidOperationException($"Train has invalid length: {actualLength}\nTrain info:\n{HighlightOnField()}");
        }
        
        public IEnumerable<Tile> GetTiles()
        {
            for (var tile = Tail; tile != Head; tile = IsClockwise ? tile!.Next : tile!.Previous)
                yield return tile;

            yield return Head;
        }
        
        public override string ToString() =>
            $"{nameof(Char)}: {Char}, {nameof(Position)}: {Position}, {nameof(Length)}: {Length}, {nameof(TimeToWait)}: {TimeToWait}"
            + $"\n{nameof(Head)}: {Head},\n{nameof(Tail)}: {Tail}";
        
        public string HighlightOnField() => _field.Highlight(GetTiles().ToArray(), Char, $"train: {this}");
    }
    
    public class Field
    {
        private readonly Tile[][] _field;
        private Tile? TryGetTile((int x, int y) point) => IsInBounds(point) ? _field[point.y][point.x] : null;
        private bool IsInBounds((int x, int y) point) =>
            0 <= point.y && point.y < _field.Length && 0 <= point.x && point.x < _field[0].Length;

        private Tile ZeroTile { get; }

        private readonly List<Train> _trains = new List<Train>();
        public IEnumerable<Train> Trains => _trains;
        public int TotalLength => ZeroTile.Previous.Position + 1;

        public Field(string track)
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
                current.Position = current.Previous.Position + 1;

                var nextTile = GetNextTile(current);

                current.Next = nextTile;
                nextTile.Previous = current;
                current = nextTile;
            }

            return this;
        }

        private Tile GetNextTile(Tile current)
        {

            var nextTile = current.NextPossibleCoordinates(current.Previous!.Coordinates)
                                  .Select(TryGetTile)
                                  .FirstOrDefault(tile =>
                                      tile != null && tile.NextPossibleCoordinates(current.Coordinates).Any());

            if (nextTile is null || nextTile.IsEmpty)
                throw new InvalidOperationException($"Invalid tile!\n{Highlight(current)}>");

            // NOTE: processing crossing tile
            if (nextTile.Position != -1 && nextTile != ZeroTile)
                nextTile = new Tile(nextTile.Symbol, nextTile.Coordinates);

            return nextTile;
        }
        
        public Field SetTrainInfo(string firstTrain, int firstTrainPos, string secondTrain, int secondTrainPos)
        {
            _trains.Add(new Train(firstTrain, this, firstTrainPos));
            _trains.Add(new Train(secondTrain, this, secondTrainPos));
            return this;
        }

        public bool CollisionDetected() =>
            Trains.SelectMany(train => train.GetTiles())
                  .GroupBy(tile => (tile.Coordinates, 1))
                  .Any(g => g.Count() > 1);

        private string Highlight(Tile tile) =>
            Highlight(new[] { tile }, '@', $"current tile: {tile}");

        public string HighlightTrains() =>
            Trains.First().HighlightOnField() + '\n' + Trains.Last().HighlightOnField();
        
        public string Highlight(Tile[] tilesToHighlight, char placeholder, string message) =>
            new StringBuilder()
                .AppendJoin(null, _field.SelectMany(tiles =>
                    tiles.Select(tile =>
                        tilesToHighlight.Any(t => t.Coordinates == tile.Coordinates) ? placeholder : tile.Symbol).Append('\n')))
                .Append($"\t{placeholder} - {message}\n")
                .ToString();

        public Tile GetByPosition(int position)
        {
            position = (position + TotalLength) % TotalLength;
            
            for (Tile tile = ZeroTile;; tile = tile.Next)
                if (tile.Position == position)
                    return tile;
        }

        public override string ToString() => Highlight(Array.Empty<Tile>(), ' ', "current field\n");
    }
    
    public static class Dinglemouse
    {
        public static bool HasPrint = false;
        
        public static int TrainCrash(string trackStr, string aTrain, int aTrainPos, string bTrain, int bTrainPos, int limit)
        {
            Console.WriteLine(new string('-', 25)
                              + '\n'
                              + "InputInfo:\n"
                              + trackStr
                              + $"\naTrain: {aTrain}, aTrainPos: {aTrainPos}, bTrain: {bTrain}, bTrainPos: {bTrainPos}, limit: {limit}\n"
                              + new string('-', 10)
                              + '\n');
            
            var field = new Field(trackStr).CreateTrack().SetTrainInfo(aTrain, aTrainPos, bTrain, bTrainPos);

            int ans = -1;
            for (var i = 0; i < limit; i++)
            {
                if (field.CollisionDetected())
                {
                    ans = i;
                    break;
                }

                foreach (var train in field.Trains)
                    train.Move();
            }

            if (HasPrint)
                Console.WriteLine(field + field.HighlightTrains());

            return ans;
        }

        public static IEnumerable<(int, int)> AddToAll(this IEnumerable<(int x, int y)> enumerable, (int x, int y) term) =>
            enumerable.Select(tpl => (tpl.x + term.x, tpl.y + term.y));
    }
}
