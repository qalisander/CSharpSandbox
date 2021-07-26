#nullable enable
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

// https://www.codewars.com/kata/59b47ff18bcb77a4d1000076/train/csharp
namespace Experiments.Completed
{
    public class Tile
    {
        private readonly Field _field;
        public Tile(char symbol, (int x, int y) coordinates, Field field)
        {
            _field = field;
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

        public IEnumerable<Tile?> NextPossibleTile(Tile fromTile)
        {
            return CharToNewTileDeltas[(Symbol, delta: Delta(this, fromTile))]
                   .AddToAll(Coordinates)
                   .Select(_field.TryGetTile)
                   .Where(nextTile => nextTile != null)
                   .Where(nextTile => IsDiagonal(this) && IsDiagonalDelta(Delta(this, nextTile)) && IsDiagonal(nextTile)
                                      || IsDiagonal(this) && !IsDiagonalDelta(Delta(this, nextTile)) && !IsDiagonal(nextTile)
                                      || IsVerticalOrHorizontal(this));

            static bool IsVerticalOrHorizontal(Tile tile) => "S|-+".Contains(tile.Symbol);
            static bool IsDiagonal(Tile tile) => "S/\\X".Contains(tile.Symbol);
            
            static (int dx, int dy) Delta(Tile from, Tile to) => 
                (to.Coordinates.x - from.Coordinates.x, to.Coordinates.y - from.Coordinates.y);

            bool IsDiagonalDelta((int dx, int dy) delta) => (delta.dx + delta.dy) % 2 == 0;
        }

        private static readonly ILookup<(char ch, (int dx, int dy) incomingDir), (int dx, int dy)> CharToNewTileDeltas = GetDirections().MixFromAndTo().ToLookup(t => (t.ch, input: t.@from), t => t.to);

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
                ('/', (-1, 1), (1, -1)),
                ('/', (-1, 1), (1, 0)),
                ('/', (-1, 1), (0, -1)),
                ///////////////////
                ('/', (-1, 0), (0, -1)),
                ('/', (-1, 0), (1, -1)),
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

        public override string ToString() => $"{nameof(Symbol)}: '{Symbol}', {nameof(Coordinates)}: {Coordinates}, {nameof(Position)}: {Position}";
    }

    public class Train
    {
        private readonly Field _field;
        public Train( string str, Field field, int initPosition = -1)
        {
            _field = field;
            InitPosition = initPosition;
            Char = char.ToUpper(str[0]);
            Length = str.Length;
            IsClockwise = char.IsLower(str[0]);
            Head = field.GetByPosition(initPosition);
            Tail = field.GetByPosition(IsClockwise ? initPosition - Length + 1: initPosition + Length - 1);
        }
        public char Char { get; }
        public bool IsClockwise { get; }
        public int Length { get; }
        public Tile? Head { get; set; }
        public Tile? Tail { get; set; }
        public int InitPosition { get; }
        public int TimeToWait { get; set; } = 0;
        public bool IsExpress => Char == 'X';
        public string HighlightedOnFieldStr => _field.Highlight(GetTiles().ToArray(), Char, $"train: {this}");

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
                throw new InvalidOperationException($"Train has invalid length: {actualLength}\nTrain info:\n{HighlightedOnFieldStr}");
        }

        public IEnumerable<Tile> GetTiles() => _field.GetChaindedTiles(Tail, Head, IsClockwise);

        public override string ToString() =>
            $"{nameof(Char)}: {Char}, {nameof(InitPosition)}: {InitPosition}, {nameof(Length)}: {Length}, {nameof(TimeToWait)}: {TimeToWait}"
            + $"\n{nameof(Head)}: {Head},\n{nameof(Tail)}: {Tail}";
    }
    
    public class Field
    {
        private readonly Tile[][] _field;
        public Tile? TryGetTile((int x, int y) point) => IsInBounds(point) ? _field[point.y][point.x] : null;
        private bool IsInBounds((int x, int y) point) =>
            0 <= point.y && point.y < _field.Length && 0 <= point.x && point.x < _field[0].Length;

        private Tile ZeroTile { get; }

        private readonly List<Train> _trains = new List<Train>();
        public IEnumerable<Train> Trains => _trains;
        public int TotalLength => ZeroTile.Previous.Position + 1;

        public string ChainedTilesStr =>
            Highlight(GetChaindedTiles(ZeroTile, ZeroTile.Previous).ToArray(), '$', "Chained tiles:\n");

        public Field(string track)
        {
            var strings = track.Split(new []{'\r','\n'}, StringSplitOptions.RemoveEmptyEntries);
            var maxLength = strings.Max(str => str.Length);

            _field = strings
                     .Select((str, i) =>
                         str.PadRight(maxLength, ' ').ToCharArray().Select((ch, j) => 
                             new Tile(ch, (j, i), this)).ToArray())
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

            var nextTile = current.NextPossibleTile(current.Previous!)
                                  .FirstOrDefault(tile =>
                                      tile != null && tile.NextPossibleTile(current).Any());

            if (nextTile is null || nextTile.IsEmpty)
                throw new InvalidOperationException($"Invalid tile!\n{Highlight(current)}>");

            // NOTE: processing crossing tile
            if (nextTile.Position != -1 && nextTile != ZeroTile)
                nextTile = new Tile(nextTile.Symbol, nextTile.Coordinates, this);

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
            Trains.First().HighlightedOnFieldStr + '\n' + Trains.Last().HighlightedOnFieldStr;
        
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
        
        public IEnumerable<Tile> GetChaindedTiles(Tile from, Tile iclusiveTo, bool IsClockwise = true)
        {
            for (var tile = from; tile != iclusiveTo; tile = IsClockwise ? tile!.Next : tile!.Previous)
                yield return tile;

            yield return iclusiveTo;
        }

        public override string ToString() => Highlight(Array.Empty<Tile>(), ' ', "current field\n");
    }
    
    public static class Dinglemouse
    {
        public static bool HasPrint = false;
        
        public static int TrainCrash(string trackStr, string aTrain, int aTrainPos, string bTrain, int bTrainPos, int limit)
        {
            // NOTE: use Console.SetCursorPosition to animate
            
            Console.WriteLine(new string('_', 40)
                              + '\n'
                              + "InputInfo:\n"
                              + trackStr
                              + $"\naTrain: {aTrain}, aTrainPos: {aTrainPos}, bTrain: {bTrain}, bTrainPos: {bTrainPos}, limit: {limit}\n"
                              + new string('_', 20)
                              + '\n');
            
            var field = new Field(trackStr).CreateTrack().SetTrainInfo(aTrain, aTrainPos, bTrain, bTrainPos);

            int ans = -1;
            for (var i = 0; i <= limit; i++)
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
                Console.WriteLine(field + field.ChainedTilesStr + field.HighlightTrains());

            return ans;
        }

        public static IEnumerable<(int, int)> AddToAll(this IEnumerable<(int x, int y)> enumerable, (int x, int y) term) =>
            enumerable.Select(tpl => (tpl.x + term.x, tpl.y + term.y));
        
        public static IEnumerable<(V ch, T from, T to)> MixFromAndTo<V, T>(this IEnumerable<(V ch, T from, T to)> enumerable)
        {
            foreach (var tpl in enumerable)
            {
                yield return (tpl.ch, tpl.@from, tpl.to);
                yield return (tpl.ch, tpl.to, tpl.@from);
            }
        }
    }
}
