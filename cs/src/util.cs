using System.Collections.Generic;
using System.Linq;
using MoreLinq;

namespace PokerHandEvaluator
{
    public class Util
    {
        public static IEnumerable<List<T>> Combinations<T>(List<T> src, int n)
        {
            return src.Subsets(n).Select(e => new List<T>(e));
        }

        public static uint Choose(int n, int r)
        {
            return DpTable.Choose[n, r];
        }
    }
}
