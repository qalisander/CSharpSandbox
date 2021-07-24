using System;
using System.Collections.Generic;
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
        
        // public Dir[] PossibleDirs { get; }
        public char Symbol { get; }
        
        public Train? Train { get; set; }

        private static ILookup<(char ch, (int dx, int dy) incomingDir), (int dx, int dy)> charToNewPossibleDir =
            MixOutputAndInput(GetDirections()).ToLookup(t => (t.ch, t.input), t => t.output);

        private static IEnumerable<(char ch, (int dx, int dy) input, (int dx, int dy) output)> GetDirections()
        {
            //            (0, 1)
            //     (-1, 1)  |    (1, 1)
            //              |
            // (-1, 0) -----+----- (1, 0)
            //              |
            //    (-1, -1)  |   (1, -1)
            //           (0, -1)
            
            var charToDeltaDirection = new (char ch, (int x, int y) input, (int x, int y) output)[]
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
                    yield return ('\\', (-tpl.input.x, -tpl.input.y), (-tpl.output.x, -tpl.output.y));

                if (tpl.ch == 'X' || tpl.ch == '+')
                    yield return ('S', tpl.input, tpl.output);
            }
        }

        private static IEnumerable<(char ch, T input, T output)> MixOutputAndInput<T>(IEnumerable<(char ch, T input, T output)> enumerable)
        {
            foreach (var tpl in enumerable)
            {
                yield return (tpl.ch, tpl.input, tpl.output);
                yield return (tpl.ch, tpl.output, tpl.input);
            }
        }
    }

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

    // NOTE:
    // Field
    //      Tile[][] _filed 
    //      Track
    //      Train
    //      
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
    }
    
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

            throw new NotImplementedException();
        }
    }
}
