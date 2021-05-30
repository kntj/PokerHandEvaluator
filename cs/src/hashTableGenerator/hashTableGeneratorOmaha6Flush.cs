using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace PokerHandEvaluator
{
    public class HashTableGeneratorOmaha6Flush
    {
        static short[] Flush_omaha6 = new short[9018009]; //3003 * 3003  15C5 * 14C6

        static readonly ushort[] binaries_by_id = new ushort[]
        {
            0x1,
            0x2,
            0x4,
            0x8,
            0x10,
            0x20,
            0x40,
            0x80,
            0x100,
            0x200,
            0x400,
            0x800,
            0x1000,
        };

        static readonly uint[] padding = new uint[] { 0x0000, 0x2000, 0x6000 };

        public static void Create()
        {
            CreateTable();
            WriteBinary();
            //Output();
        }

        static int Hash_binary(uint binary, int len, int k)
        {
            int sum = 0;

            for (var i = 0; i < len; i++)
            {
                if ((binary & (1 << i)) != 0)
                {
                    if (len - i - 1 >= k)
                    {
                        sum += (int)DpTable.Choose[len - i - 1, k];
                    }

                    k--;

                    if (k == 0)
                    {
                        break;
                    }
                }
            }

            return sum;
        }

        static void CreateTable()
        {
            var suit_board_5_flush = Util.Combinations(binaries_by_id.Select(e => (uint)e).ToList(), 5); //1287 13C5
            var suit_board_4_flush = Util.Combinations(binaries_by_id.Select(e => (uint)e).ToList(), 4); //715 13C4
            var suit_board_3_flush = Util.Combinations(binaries_by_id.Select(e => (uint)e).ToList(), 3); //286 13C3

            var suit_board_all_flush = suit_board_5_flush.Concat(suit_board_4_flush).Concat(suit_board_3_flush); //2288


            var suit_hole_6_flush = Util.Combinations(binaries_by_id.Select(e => (uint)e).ToList(), 6); //1716 13C6
            var suit_hole_5_flush = Util.Combinations(binaries_by_id.Select(e => (uint)e).ToList(), 5); //1287 13C5

            var suit_hole_all_flush = suit_hole_6_flush.Concat(suit_hole_5_flush); //3003


            foreach (var binariesBoard in suit_board_all_flush)
            {
                var binaryBoard = binariesBoard.Aggregate((m, n) => m |= n);
                var paddingBinaryBoard = binaryBoard | padding[5 - binariesBoard.Count()];
                var hashBoard = Hash_binary(paddingBinaryBoard, 15, 5);

                foreach (var binariesHole in suit_hole_all_flush)
                {
                    var binaryHole = binariesHole.Aggregate((m, n) => m |= n);
                    if ((binaryBoard & binaryHole) != 0)
                    {
                        //Duplicate
                        continue;
                    }

                    var omahaHoleList = Util.Combinations(binariesHole, 4);
                    var binaryOmahaHoleList = omahaHoleList.Select(e => e.Aggregate((m, n) => m |= n));
                    var hashOmahaHoleList = binaryOmahaHoleList.Select(e => Hash_binary(e, 15, 4));
                    var valueFlushOmahaList = hashOmahaHoleList.Select(e => TablesOmaha.Flush_omaha[hashBoard * 1365 + e]);
                    var bestValueFlush = valueFlushOmahaList.Min();

                    if (bestValueFlush == 0)
                    {
                        throw new System.ArgumentException($"flush value 0  board[{binaryBoard.ToString()}] hole[{ binaryHole.ToString()}]");
                    }

                    var paddingBinaryHole = binaryHole | padding[6 - binariesHole.Count()];
                    var hashHole = Hash_binary(paddingBinaryHole, 14, 6);

                    Flush_omaha6[hashBoard * 3003 + hashHole] = bestValueFlush;
                }
            }
        }

        static void WriteBinary()
        {
            if (BitConverter.IsLittleEndian == false)
            {
                throw new InvalidOperationException("not little endian");
            }

            using (var writer = new BinaryWriter(File.Open("flush_omaha_6.bin", FileMode.Create, FileAccess.Write, FileShare.None)))
            {
                var byteLen = Buffer.ByteLength(Flush_omaha6);
                var byteArr = new Byte[byteLen];
                Buffer.BlockCopy(Flush_omaha6, 0, byteArr, 0, byteLen);

                writer.Write(byteArr);
            }
        }

        static void Output()
        {
            List<short> line = new List<short>();
            foreach (var value in Flush_omaha6)
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