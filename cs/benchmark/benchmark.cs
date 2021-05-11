using System;
using System.Collections.Generic;
using System.Linq;
using PokerHandEvaluator;

namespace Benchmark
{
    public interface IEvaluator
    {
        public Rank EvaluateOmaha6Cards(Card c1, Card c2, Card c3, Card c4, Card c5,
                                    Card h1, Card h2, Card h3, Card h4, Card h5, Card h6);
    }

    public class EvaluatorOmaha6Impl : IEvaluator
    {
        public Rank EvaluateOmaha6Cards(Card c1, Card c2, Card c3, Card c4, Card c5,
                                    Card h1, Card h2, Card h3, Card h4, Card h5, Card h6)
        {
            return Evaluator.EvaluateOmaha6Cards(c1, c2, c3, c4, c5,
                                                    h1, h2, h3, h4, h5, h6);
        }
    }

    public class EvaluatorOmaha6ByOmahaImpl : IEvaluator
    {
        public Rank EvaluateOmaha6Cards(Card c1, Card c2, Card c3, Card c4, Card c5,
                                    Card h1, Card h2, Card h3, Card h4, Card h5, Card h6)
        {
            return Evaluator.EvaluateOmaha6CardsByOmaha(c1, c2, c3, c4, c5,
                                                        h1, h2, h3, h4, h5, h6);
        }
    }

    public class EvaluatorOmaha6ByHoldemImpl : IEvaluator
    {
        public Rank EvaluateOmaha6Cards(Card c1, Card c2, Card c3, Card c4, Card c5,
                                    Card h1, Card h2, Card h3, Card h4, Card h5, Card h6)
        {
            return Evaluator.EvaluateOmaha6CardsByHoldem(c1, c2, c3, c4, c5,
                                                            h1, h2, h3, h4, h5, h6);
        }
    }

    public class Benchmark
    {
        public static void Start()
        {
            var count = 1000000;
            var rankTotalList = new List<long>();
            var elapsedList = new List<TimeSpan>();

            var sw = new System.Diagnostics.Stopwatch();
            sw.Start();
            sw.Stop();

            Console.WriteLine("create test data");
            var testList = new List<List<Card>>();
            foreach (var i in Enumerable.Range(0, count))
            {
                testList.Add(Deck.All().OrderBy(e => Guid.NewGuid()).Take(11).ToList());
            }

            Console.WriteLine("test start");

            var evalList = new List<IEvaluator>() { new EvaluatorOmaha6Impl(), new EvaluatorOmaha6ByOmahaImpl(), new EvaluatorOmaha6ByHoldemImpl() };
            var descList = new List<string>() { "omaha6", "byOmaha", "byHoldem" };

            foreach (var eval in evalList)
            {
                var rankTotal = 0L;

                sw.Restart();
                foreach (var card11 in testList)
                {
                    var rank = eval.EvaluateOmaha6Cards(
                                                        card11[0],
                                                        card11[1],
                                                        card11[2],
                                                        card11[3],
                                                        card11[4],
                                                        card11[5],
                                                        card11[6],
                                                        card11[7],
                                                        card11[8],
                                                        card11[9],
                                                        card11[10]);
                    rankTotal += rank.Value();
                }
                sw.Stop();

                elapsedList.Add(sw.Elapsed);
                rankTotalList.Add(rankTotal);
            }

            for (var i = 0; i < rankTotalList.Count; i++)
            {
                Console.WriteLine(descList[i] + ":" + elapsedList[i] + " rankTotal:" + rankTotalList[i]);
            }
        }
    }
}