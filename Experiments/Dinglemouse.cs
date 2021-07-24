using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

// https://www.codewars.com/kata/59b47ff18bcb77a4d1000076/train/csharp
namespace Experiments
{
    public class Tile
    {
        public Tile(char symbol)
        {
            // PossibleDirs = GetDir(symbol);
            Symbol = symbol;
        }
        
        public Dir[] PossibleDirs { get; }
        public char Symbol { get; }
        
        public Train? Train { get; set; }
        
        private (int x, int y)[] GetPossibleDirs(char symbol) => symbol switch
        {
            // '|' => new[] { Dir.Up | Dir.Down },
            // '\\' => new[]
            // {
            //     Dir.Up | Dir.DownRight,
            //     Dir.Up | Dir.Right,
            //     Dir.UpLeft | Dir.Right,
            //     Dir.UpLeft | Dir.DownRight,
            //     Dir.UpLeft | Dir.Down,
            // },
            // '/' => new[]
            // {
            //     Dir.Up | Dir.DownLeft,
            //     Dir.Up | Dir.Left,
            //     Dir.UpRight | Dir.Left,
            //     Dir.UpRight | Dir.DownLeft,
            //     Dir.UpRight | Dir.Down,
            // },
            // '-' => new[] { Dir.Left | Dir.Right },
            // 'X' => new[] { Dir.UpRight | Dir.DownLeft, Dir.UpLeft | Dir.DownRight },
            // '+' => new[] { Dir.Left | Dir.Right, Dir.Up | Dir.Down },
            // 'S' => GetPossibleDirs('X').Append<>(GetPossibleDirs('+')).ToArray(),
            // var ch when char.IsLetter(ch) => new[] { Dir.Left | Dir.Right },
            // var ch => throw new NotSupportedException($"Not supported symbol: {ch}"),
        };
        
        // private Dir[] GetPossibleDirs(char symbol) => symbol switch
        // {
        //     '|' => new[] { Dir.Up | Dir.Down },
        //     '\\' => new[]
        //     {
        //         Dir.Up | Dir.DownRight,
        //         Dir.Up | Dir.Right,
        //         Dir.UpLeft | Dir.Right,
        //         Dir.UpLeft | Dir.DownRight,
        //         Dir.UpLeft | Dir.Down,
        //     },
        //     '/' => new[]
        //     {
        //         Dir.Up | Dir.DownLeft,
        //         Dir.Up | Dir.Left,
        //         Dir.UpRight | Dir.Left,
        //         Dir.UpRight | Dir.DownLeft,
        //         Dir.UpRight | Dir.Down,
        //     },
        //     '-' => new[] { Dir.Left | Dir.Right },
        //     'X' => new[] { Dir.UpRight | Dir.DownLeft, Dir.UpLeft | Dir.DownRight },
        //     '+' => new[] { Dir.Left | Dir.Right, Dir.Up | Dir.Down },
        //     'S' => GetPossibleDirs('X').Append<>(GetPossibleDirs('+')).ToArray(),
        //     var ch when char.IsLetter(ch) => new[] { Dir.Left | Dir.Right },
        //     var ch => throw new NotSupportedException($"Not supported symbol: {ch}"),
        // };
    }
    //
    // [Flags]
    // public enum Dir
    // {
    //     Up = 1 << 0,
    //     UpRight = 1 << 1,
    //     Right = 1 << 2,
    //     DownRight = 1 << 3,
    //     Down = 1 << 4,
    //     DownLeft = 1 << 5,
    //     Left = 1 << 6,
    //     UpLeft = 1 << 7,
    // }
    
    public class Train
    {
        public Train(char c, int length)
        {
            Char = c;
            Length = length;
        }
        public char Char { get; set; }
        public int Position { get; set; }
        public int Length { get; set; }
        public bool HasClockwiseDirection { get; set; }
        public int TimeToWait { get; set; } = 0;
        public bool IsExpress => Char == 'X';

        // public int TailPosition => IsStraghtDirection
        //     ? Position - (Length - 1)
        //     : Position + (Length - 1);
    }

    // https://www.codewars.com/kata/59b47ff18bcb77a4d1000076

    // NOTE:
    // Field
    //      Tile[][] _filed 
    //      Track
    //      Train
    //      
    public class Dinglemouse
    {
        public static int TrainCrash(
            string trackStr, string aTrain, int aTrainPos, string bTrain, int bTrainPos, int limit)
        {
            var track = new Field(trackStr, aTrain, bTrain).Build();

            for (var step = 1; step <= limit; step++)
            {
                if (track.MakeStep().IsCollisionDetected())
                    return step;
            }

            return -1;
        }
    }
    public class Field
    {
        private readonly Tile[][] _field;
        private Tile GetTile((int x, int y) point) => _field[point.y][point.x]; // TODO: check for valid x and y
        private Tile ZeroTile { get; }

        private Dictionary<string, Train> Trains = new Dictionary<string, Train>(StringComparer.OrdinalIgnoreCase);

        public Field(string track, string aTrain, string bTrain)
        {
            _field = track
                         .Split('\n')
                         .Select((str, i) => str.ToCharArray().Select(ch => new Tile(ch)).ToArray())
                         .ToArray();

            ZeroTile = _field[0].FirstOrDefault(tile => tile.Symbol != ' ')
                       ?? throw new InvalidOperationException("Zero tile not found");

            Trains[aTrain[0].ToString()] = new Train(aTrain[0], aTrain.Length);
            Trains[bTrain[0].ToString()] = new Train(bTrain[0], bTrain.Length);

            Tile CreateTile(char ch, int i, int j)
            {
                if (char.IsLetter(ch) && Trains.ContainsKey(ch.ToString()))
                {
                    
                }
            }
        }


        
        private (int y, int x) Start { get; }
        private (int y, int x) Current { get; set; }
        private (int y, int x) Previous { get; set; }

    //     private char CurrentChar => GetChar(Current);
    //     private char PreviousChar => GetChar(Previous);
    //
    //     private Track Track { get; } = new Track();
    //
    //     // private readonly char[][] _field;
    //
    //     // private char GetChar((int y, int x) point) => _field[point.y][point.x];
    //     private bool IsEmpty((int y, int x) point) => GetChar(point) == ' ';
    //
    //     public Track Build()
    //     {
    //         for (var i = 1;; i++)
    //         {
    //             if (IsEmpty(Current))
    //             {
    //                 throw new InvalidOperationException(
    //                     $"Current ({Current.y}, {Current.x}) symbol is 'space'");
    //             }
    //
    //             if (CurrentChar == 'S')
    //                 Track.StationPositions.Add(i);
    //             else if (Track.TryGetTrain(CurrentChar, out var train))
    //             {
    //                 train.Position = i;
    //                 train.IsStraghtDirection = PreviousChar == char.ToLower(train.Char);
    //             }
    //
    //             var next = NextStep();
    //             Previous = Current;
    //             Current = next;
    //
    //             if (Current == Start)
    //             {
    //                 Track.Length = i;
    //
    //                 break;
    //             }
    //         }
    //
    //         return Track;
    //     }
    //
    //     // TODO: prlly add new type Coordinate - implement ToString, ==, ((int, int)), 
    //     // TODO: switch by CurrentChar and PreviousChar
    //     private (int y, int x) NextStep() => (PreviousChar, CurrentChar) switch
    //     {
    //         (_, '-') => (Current.y, Current.x + (Current.x - Previous.x)),
    //         (_, '|') => (Current.y + (Current.y - Previous.y), Current.x),
    //         ('\\', var ch) when
    //             ch == 'X' || ch == 'S' || ch == '\\' => GetDelta(Current, Previous), //TODO: count delta
    //         ('/', var ch) when
    //             ch == 'X' || ch == 'S' => (Current.y - (Current.y - Previous.y), Current.x + 1),
    //     };
    //
    //     private (int y, int x) GetDelta((int y, int x) tpl1, (int y, int x) tpl2) =>
    //         (tpl1.y - tpl2.y, tpl1.x - tpl2.x);
    // }
    // public class Track
    // {
    //     public int Length { get; set; }
    //     public Train TrainA { get; } = new Train();
    //     public Train TrainB { get; } = new Train();
    //     public List<int> StationPositions { get; } = new List<int>(); //TODO: to hashset
    //
    //     public bool TryGetTrain(char ch, out Train train) =>
    //         (train = TrainA).Char == ch || (train = TrainB).Char == ch;
    //
    //     public Track MakeStep()
    //     {
    //         if (!AtStation(TrainA))
    //             Move(TrainA);
    //
    //         if (!AtStation(TrainA))
    //             Move(TrainB);
    //
    //         return this;
    //
    //         void Move(Train train) =>
    //             train.Position = (train.Position + (train.IsStraghtDirection ? 1 : -1)) % Length;
    //
    //         bool AtStation(Train train)
    //         {
    //
    //             throw new NotImplementedException();
    //
    //             if (train.TimeToWait == 0 && StationPositions.Contains(train.Position))
    //             {
    //             }
    //         }
    //     }
    //
    //     public bool IsCollisionDetected() =>
    //         TrainA.Position == TrainB.Position
    //         || TrainA.Position == TrainB.TailPosition
    //         || TrainB.Position == TrainA.TailPosition;
    //
    //     public class Train
    //     {
    //         public char Char { get; set; }
    //         public int Position { get; set; }
    //         public int Length { get; set; }
    //         public bool IsStraghtDirection { get; set; } // TODO: init in ctor
    //         public int TimeToWait { get; set; } = 0;
    //
    //         public bool IsExpress => Char == 'X';
    //
    //         public int TailPosition => IsStraghtDirection
    //             ? Position - (Length - 1)
    //             : Position + (Length - 1);
    //     }
    }
}
