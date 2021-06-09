using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection.Emit;
using System.Transactions;
using BenchmarkDotNet.Toolchains.Results;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Experiments.Completed
{
    // https://www.codewars.com/kata/59b47ff18bcb77a4d1000076
    public class Dinglemouse
    {
        public static int TrainCrash(
            string trackStr, string aTrain, int aTrainPos, string bTrain, int bTrainPos, int limit)
        {
            var track = new TrackBuilder(trackStr, aTrain, bTrain).Build();

            for (int step = 1; step <= limit; step++)
                if (track.MakeStep().IsCollisionDetected())
                    return step;

            return -1;
        }

        public class Track
        {
            public int Length { get; set; }
            public Train TrainA { get; } = new ();
            public Train TrainB { get; } = new ();
            public List<int> StationPositions { get; } = new(); //TODO: to hashset

            public class Train
            {
                public char Char { get; set; }
                public int Position { get; set; }
                public int Length { get; set; }
                public bool IsStraghtDirection { get; set; } // TODO: init in ctor
                public int TimeToWait { get; set; } = 0;

                public bool IsExpress => Char == 'X';

                public int TailPosition => IsStraghtDirection
                    ? Position - (Length - 1)
                    : Position + (Length - 1);
            }

            public bool TryGetTrain(char ch, out Train train) =>
                (train = TrainA).Char == ch || (train = TrainB).Char == ch;

            public Track MakeStep()
            {
                if (!AtStation(TrainA))
                    Move(TrainA);

                if (!AtStation(TrainA))
                    Move(TrainB);

                return this;

                void Move(Train train) =>
                    train.Position = (train.Position + (train.IsStraghtDirection ? 1 : -1)) % Length;

                bool AtStation(Train train)
                {

                    throw new NotImplementedException();

                    if (train.TimeToWait == 0 && StationPositions.Contains(train.Position))
                    {
                    }
                }
            }

            public bool IsCollisionDetected() =>
                TrainA.Position == TrainB.Position
                || TrainA.Position == TrainB.TailPosition
                || TrainB.Position == TrainA.TailPosition;
        }

        //TODO: Move to track class, set name like Builder or Factory
        //TODO: Hide properites, 
        public class TrackBuilder
        {
            private readonly char[][] _field;

            private char GetChar((int y, int x) point) => _field[point.y][point.x];
            private bool IsEmpty((int y, int x) point) => GetChar(point) == ' ';

            private (int y, int x) Start { get; }
            private (int y, int x) Current { get; set; }
            private (int y, int x) Previous { get; set; }

            private char CurrentChar => GetChar(Current);
            private char PreviousChar => GetChar(Previous);

            private Track Track { get; } = new();

            public TrackBuilder(string track, string aTrain, string bTrain)
            {
                _field = track.Split('\n').Select(str => str.ToCharArray()).ToArray();

                Start = (0, _field[0].TakeWhile(x => x != ' ').Count());

                Current = (Start.y, Start.x + 1);
                Previous = Current;

                Track.TrainA.Length = aTrain.Length;
                Track.TrainB.Length = bTrain.Length;
            }

            public Track Build()
            {
                for (var i = 1;; i++)
                {
                    if (IsEmpty(Current))
                        throw new InvalidOperationException(
                            $"Current ({Current.y}, {Current.x}) symbol is 'space'");

                    if (CurrentChar == 'S')
                    {
                        Track.StationPositions.Add(i);
                    }
                    else if (Track.TryGetTrain(CurrentChar, out var train))
                    {
                        train.Position = i;
                        train.IsStraghtDirection = PreviousChar == char.ToLower(train.Char);
                    }

                    var next = NextStep();
                    Previous = Current;
                    Current = next;

                    if (Current == Start)
                    {
                        Track.Length = i;

                        break;
                    }
                }

                return Track;
            }

            // TODO: prlly add new type Coordinate - implement ToString, ==, ((int, int)), 
            // TODO: switch by CurrentChar and PreviousChar
            private (int y, int x) NextStep() => (PreviousChar, CurrentChar) switch
            {
                (_, '-') => (Current.y, Current.x + (Current.x - Previous.x)),
                (_, '|') => (Current.y + (Current.y - Previous.y), Current.x),
                ('\\', var ch) when
                    ch == 'X' || ch == 'S' || ch == '\\' => GetDelta(Current, Previous),//TODO: count delta
                ('/', var ch) when
                    ch == 'X' || ch == 'S' => (Current.y - (Current.y - Previous.y), Current.x + 1),
            };

            private (int y, int x) GetDelta((int y, int x) tpl1, (int y, int x) tpl2) =>
                (tpl1.y - tpl2.y, tpl1.x - tpl2.x);
        }
    }
}
