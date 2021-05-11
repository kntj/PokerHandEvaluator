using System;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using PokerHandEvaluator;

namespace PokerHandEvaluatorTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestValue()
        {
            {
                var c =  (new string [] { "4c", "4s", "4h", "Ac", "As" }).Select(e => new Card(e)).ToList();
                var h = (new string [] { "9c", "9d", "8c", "8d", "7c", "7d" }).Select(e => new Card(e)).ToList();

                var r = Evaluator.EvaluateOmaha6Cards(c[0], c[1] ,c[2], c[3], c[4], h[0], h[1], h[2], h[3], h[4], h[5]);

                Assert.AreEqual(r.Value(), 292);
            }

            {
                var c =  (new string [] { "9c", "4c", "9d", "Ah", "Kc" }).Select(e => new Card(e)).ToList();
                var h = (new string [] { "4s", "9h", "Qc", "Jc", "Tc", "2c" }).Select(e => new Card(e)).ToList();

                var r = Evaluator.EvaluateOmaha6Cards(c[0], c[1] ,c[2], c[3], c[4], h[0], h[1], h[2], h[3], h[4], h[5]);

                Assert.AreEqual(r.Value(), 236);
            }
        }

        [TestMethod]
        public void TestFlush()
        {
            //hole 5 suit
            {
                var c =  (new string [] { "Ad", "Kd", "Qd", "Jd", "Td" }).Select(e => new Card(e)).ToList();
                var h = (new string [] { "8d", "7d", "6d", "5d", "4d", "Ac" }).Select(e => new Card(e)).ToList();

                var r = Evaluator.EvaluateOmaha6Cards(c[0], c[1] ,c[2], c[3], c[4], h[0], h[1], h[2], h[3], h[4], h[5]);

                Assert.IsTrue(r.IsFlush());
            }

            //hole 6 suit
            {
                var c =  (new string [] { "Ad", "Kd", "Qd", "Jd", "Td" }).Select(e => new Card(e)).ToList();
                var h = (new string [] { "8d", "7d", "6d", "5d", "4d", "3d" }).Select(e => new Card(e)).ToList();

                var r = Evaluator.EvaluateOmaha6Cards(c[0], c[1] ,c[2], c[3], c[4], h[0], h[1], h[2], h[3], h[4], h[5]);

                Assert.IsTrue(r.IsFlush());
            }
        }

        [DataTestMethod()]
        [DataRow(100000)]
        public void TestRandom(int value)
        {
            foreach (var i in Enumerable.Range(0, value))
            {
                var card11 = Deck.All().OrderBy(e => Guid.NewGuid()).Take(11).ToList();

                var rank = Evaluator.EvaluateOmaha6Cards(
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

                var rankByOmaha = Evaluator.EvaluateOmaha6CardsByOmaha(
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

                var rankByHoldem = Evaluator.EvaluateOmaha6CardsByHoldem(
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

                Assert.AreEqual(rank.Value(), rankByOmaha.Value(), " omaha6:" + rank.Value() + "/" + rank.DescribeRank() + " omaha:" + rankByOmaha.Value() + "/" + rankByOmaha.DescribeRank());
                Assert.AreEqual(rank.Value(), rankByHoldem.Value(), " omaha6:" + rank.Value() + "/" + rank.DescribeRank() + " holdem:" + rankByHoldem.Value() + "/" + rankByHoldem.DescribeRank());
            }
        }
    }
}
