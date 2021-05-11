using System.Collections.Generic;
using System.Linq;

namespace PokerHandEvaluator
{
    public class Deck
    {
        static readonly string[] rankList = new string[] { "2", "3", "4", "5", "6", "7", "8", "9", "T", "J", "Q", "K", "A" };
        static readonly string[] suitList = new string[] { "c", "d", "h", "s" };

        public static IEnumerable<Card> All()
        {
            return rankList.SelectMany(r => suitList.Select(s => new Card(r + s)));
        }
    }
}