using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace PokerHandEvaluator
{
    public class HashTableGeneratorOmaha6NoFlush
    {
        static short[] NoFlush_omaha6 = new short[113589125]; //6175 * 18395  Dp[1,13,5] * Dp[1,13,6]

        static List<byte[]> qBoardList = new List<byte[]>(6175); //6175   Dp[1,13,5]
        static List<byte[]> qHoleList = new List<byte[]>(18395); //18395  Dp[1,13,6]

        public static void Create()
        {
            CreateTargetQList(qBoardList, 5);
            CreateTargetQList(qHoleList, 6);
            CreateTable();
            Serialize();
            //Output();
        }

        static void CreateTargetQList(List<byte[]> qList, int k)
        {
            var q = new byte[13];
            var maxQ = 1220703125; //5^13

            for (var i = 0; i < maxQ; i++)
            {
                if (q.Aggregate(0, (m, n) => m + n) == k)
                {
                    var c = new byte[13];
                    Array.Copy(q, c, 13);
                    qList.Add(c);
                }

                //quinary++
                for (var j = 0; j < 13; j++)
                {
                    q[j]++;

                    if (q[j] == 5)
                    {
                        q[j] = 0;
                    }
                    else
                    {
                        break;
                    }
                }
            }
        }

        static void CreateTable()
        {
            foreach (var qBoard in qBoardList)
            {
                var boardHash = Hash.Hash_quinary(qBoard, 5);

                foreach (var qHole in qHoleList)
                {
                    if (qBoard.Zip(qHole, (b, h) => b + h).Where(e => 5 <= e).Any())
                    {
                        //Duplicate
                        continue;
                    }

                    var holeIndexs = new List<int>();
                    for (var i = 0; i < 13; i++)
                    {
                        if (0 < qHole[i])
                        {
                            for (var j = 0; j < qHole[i]; j++)
                            {
                                holeIndexs.Add(i);
                            }
                        }
                    }

                    var omahaHoleIndexsList = Util.Combinations(holeIndexs, 4);
                    var hashOmahaHoleList = omahaHoleIndexsList.Select(e =>
                                                                        {
                                                                            var quinaryHole = new byte[13];
                                                                            quinaryHole[e[0]]++;
                                                                            quinaryHole[e[1]]++;
                                                                            quinaryHole[e[2]]++;
                                                                            quinaryHole[e[3]]++;
                                                                            return Hash.Hash_quinary(quinaryHole, 4);
                                                                        });
                    var valueNoFlushOmahaList = hashOmahaHoleList.Select(e => TablesOmaha.NoFlush_omaha[boardHash * 1820 + e]);
                    var bestValueNoFlush = valueNoFlushOmahaList.Min();

                    if (bestValueNoFlush == 0)
                    {
                        throw new System.ArgumentException($"NoFlush value 0  board[{String.Join(" ", qBoard.Select(e => e.ToString()))}] hole[{String.Join(" ", qHole.Select(e => e.ToString()))}]");
                    }

                    var holeHash = Hash.Hash_quinary(qHole, 6);
                    NoFlush_omaha6[boardHash * 18395 + holeHash] = bestValueNoFlush;
                }
            }
        }

        static void Serialize()
        {
            IFormatter formatter = new BinaryFormatter();
            using (var stream = new FileStream("no_flush_omaha_6.bin", FileMode.Create, FileAccess.Write, FileShare.None))
            {
                formatter.Serialize(stream, NoFlush_omaha6);
            }
        }

        static void Output()
        {
            List<short> line = new List<short>();
            foreach (var value in NoFlush_omaha6)
            {
                line.Add(value);

                if (line.Count() == 8)
                {
                    Console.WriteLine("  " + String.Join(",  ", line) + ",");
                    line.Clear();
                }
            }
            if (1 <= line.Count())
            {
                Console.WriteLine("  " + String.Join(",  ", line) + ",");
            }
        }

    }
}