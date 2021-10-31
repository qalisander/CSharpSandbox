using System.Collections.Generic;
using System.Linq;
using BenchmarkDotNet.Attributes;

namespace Problems.Completed
{
    public class DoubleLinear
    {
        [Benchmark]
        public static int DblLinear(int n)
        {
            // var list = new SortedList<int, bool>();
            var set = new SortedSet<int>();

            var queue = new Queue<(int val, bool isMinPoint)>();
            queue.Enqueue((1, true));

            while (true)
            {
                var q = queue.Dequeue();
                // list[x.val] = true; //O(list.Count)
                set.Add(q.val);                                                  //O(log(set.Count))
                if (q.isMinPoint && set.TakeWhile(s => s <= q.val).Count() >= n) // O(n)
                    break;

                queue.Enqueue(Func2X(q));
                queue.Enqueue(Func3X(q));
            }

            return set.ElementAt(n);

            static (int x, bool isMinPoint) Func2X((int x, bool isMinPoint) t) => (2 * t.x + 1, t.isMinPoint);
            static (int x, bool isMinPoint) Func3X((int x, bool isMinPoint) t) => (3 * t.x + 1, false);
        }
    }
}
// W                   W                   W                   C    
//   E               S   E               S   E               S      
//     A           I       A           I       A           I        
//       E       C           E       C           E       C          
//         D   S               D   S               D   S            
//           I                   I                   I              
    