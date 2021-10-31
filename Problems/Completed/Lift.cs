
using System;
using System.Collections.Generic;
using System.Linq;

namespace Problems.Completed
{
    // NOTE: https://www.codewars.com/kata/58905bfa1decb981da00009e/train/csharp
    public class Lift
    {
        public static int[] TheLift(int[][] queues, int capacity)
        {
            var levels = queues.Select((arr, id) =>
            {
                return new Level
                {
                    Id = id,
                    Upward = new Queue<int>(arr.Where(to => to > id)),
                    Downward = new Queue<int>(arr.Where(to => to < id))
                };
            }).ToList();

             var elevator = new Elevator(0, levels.Count, capacity);

            foreach (var level in levels)
                CallOnLevel(level, elevator);

            while (elevator.IsWorking)
            {
                var levelId = elevator.CurrentLevel;

                elevator.ComeinAndGo(levels[levelId]);

                CallOnLevel(levels[levelId],
                            elevator);
            }

            return elevator.MovementLog.ToArray();
        }

        private static void CallOnLevel(
            Level level,
            Elevator elevator)
        {
            if (level.Upward.Count != 0)
                elevator.Call(Direction.Up, level.Id);

            if (level.Downward.Count != 0)
                elevator.Call(Direction.Down, level.Id);
        }
    }

    [Flags]
    public enum Direction : byte
    {
        None = 0,
        Up = 1,
        Down = 1 << 1,
        All = Up | Down,
    }

    public class Level
    {
        public int Id;
        public Queue<int> Upward;
        public Queue<int> Downward;
    }

    public class Elevator
    {
        private readonly int _capacity;
        private readonly int _levelsAmount;
        private int _currentLevel;
        private Direction _direction;

        //BUG: with counting whole quantity of persons inside elevator
        private readonly List<int> _insideCalls;
        //NOTE: maybe use _outsideCalls.Reverse();
        private readonly List<Direction> _outsideCalls;

        public Elevator(int currentLevel, int levelsAmount, int capacity)
        {
            _direction = Direction.Up;
            IsWorking = true;

            _capacity = capacity;
            _insideCalls = new List<int>();
            

            CurrentLevel = currentLevel;
            _levelsAmount = levelsAmount;
            _outsideCalls = Enumerable.Repeat(Direction.None, levelsAmount).ToList();
        }

        public int CurrentLevel
        {
            get => _currentLevel;
            set
            {
                MovementLog.Add(value);
                _currentLevel = value;
            }
        }

        public bool IsWorking { get; private set; }

        public List<int> MovementLog { get; } = new List<int>();

        public void Call(Direction direction, int fromLevel)
        {
            _outsideCalls[fromLevel] |= direction;
        }

        public void ComeinAndGo(Level level)
        {

            PushIntoLift(level);

            var tryGetNearestSameWay = TryGetNearestSameWay(out var nearestSameWay);
            if (_insideCalls.Count != 0)
            {
                //NOTE: take last when direction is down cause _insideCalls are sorted in ascending
                var nearestInside = _direction == Direction.Up
                    ? _insideCalls.Min()
                    : _insideCalls.Max();

                var deltaI = nearestInside - _currentLevel;
                var deltaSW = nearestSameWay - _currentLevel;

                if (!tryGetNearestSameWay || Math.Abs(deltaI) <= Math.Abs(deltaSW))
                {
                    _insideCalls.RemoveAll(c => c == nearestInside);
                    //TODO: create iterator and remove logging from elevator class
                    CurrentLevel = nearestInside;
                }
                else
                    CurrentLevel = nearestSameWay;

                return;
            }

            if (tryGetNearestSameWay)
            {
                CurrentLevel = nearestSameWay;
                return;
            }

            if (TryGetDistantOppositeWay(out var distant))
            {
                CurrentLevel = distant;
                return;
            }

            //NOTE: not every time we should assign currentLevel
            _direction ^= Direction.All;

            if (_outsideCalls.Count(d => d == Direction.None) == _levelsAmount)
            {
                if (CurrentLevel != 0)
                    CurrentLevel = 0;

                IsWorking = false;
            }
        }

        private void PushIntoLift(Level level)
        {
            //TODO: think out changing direction with 
            Queue<int> queue;
            if (_direction == Direction.Up)
                queue = level.Upward;
            else if (_direction == Direction.Down)
                queue = level.Downward;
            else
                return;//NOTE: If there is no button pushed but elevator's stopped

            if (queue.Count == 0)
                return;

            while (_insideCalls.Count != _capacity)
            {
                if (!queue.TryDequeue(out var person))
                    break;

                _insideCalls.Add(person);
            }

            //lift discards current button
            _outsideCalls[CurrentLevel] ^= _direction;
        }

        private bool TryGetNearestSameWay(out int index)
        {
            index = CurrentLevel;

            while (true)
            {
                if (_direction == Direction.Up)
                    index++;
                else
                    index--;

                if (index < 0 || index == _levelsAmount)
                {
                    index = -1;
                    return false;
                }

                if (_outsideCalls[index].HasFlag(_direction))
                    return true;
            }
        }

        private bool TryGetDistantOppositeWay(out int index)
        {
            if (_direction == Direction.Up)
                index = _levelsAmount;
            else
                index = -1;

            while (true)
            {
                if (_direction == Direction.Up)
                    index--;
                else
                    index++;

                if (index == CurrentLevel)
                {
                    index = -1;
                    return false;
                }

                if (_outsideCalls[index].HasFlag(_direction ^ Direction.All))
                    return true;
            }
        }
    }
}
