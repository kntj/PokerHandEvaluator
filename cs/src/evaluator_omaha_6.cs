using System.Linq;

namespace PokerHandEvaluator
{
    class EvaluatorOmaha6
    {
        static readonly ushort[] binaries_by_id = new ushort[] 
        {
            0x1,  0x1,  0x1,  0x1,
            0x2,  0x2,  0x2,  0x2,
            0x4,  0x4,  0x4,  0x4,
            0x8,  0x8,  0x8,  0x8,
            0x10,  0x10,  0x10,  0x10,
            0x20,  0x20,  0x20,  0x20,
            0x40,  0x40,  0x40,  0x40,
            0x80,  0x80,  0x80,  0x80,
            0x100,  0x100,  0x100,  0x100,
            0x200,  0x200,  0x200,  0x200,
            0x400,  0x400,  0x400,  0x400,
            0x800,  0x800,  0x800,  0x800,
            0x1000,  0x1000,  0x1000,  0x1000,
        };

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

        internal static int Evaluate_omaha_6_cards(int c1, int c2, int c3, int c4, int c5,
                                                    int h1, int h2, int h3, int h4, int h5, int h6)
        {
            TablesOmaha6.Init();

            var value_flush = 10000;
            var value_noflush = 10000;
            var suit_count_board = new int[4];
            var suit_count_hole = new int[4];
            var board_hash = 0;
            var hole_hash = 0;

            suit_count_board[c1 & 0x3]++;
            suit_count_board[c2 & 0x3]++;
            suit_count_board[c3 & 0x3]++;
            suit_count_board[c4 & 0x3]++;
            suit_count_board[c5 & 0x3]++;

            suit_count_hole[h1 & 0x3]++;
            suit_count_hole[h2 & 0x3]++;
            suit_count_hole[h3 & 0x3]++;
            suit_count_hole[h4 & 0x3]++;
            suit_count_hole[h5 & 0x3]++;
            suit_count_hole[h6 & 0x3]++;

            for (var i = 0; i < 4; i++)
            {
                if (suit_count_board[i] >= 3 && suit_count_hole[i] >= 2)
                {
                    var suit_binary_board = new uint[4];

                    suit_binary_board[c1 & 0x3] |= binaries_by_id[c1];
                    suit_binary_board[c2 & 0x3] |= binaries_by_id[c2];
                    suit_binary_board[c3 & 0x3] |= binaries_by_id[c3];
                    suit_binary_board[c4 & 0x3] |= binaries_by_id[c4];
                    suit_binary_board[c5 & 0x3] |= binaries_by_id[c5];

                    var suit_binary_hole = new uint[4];
                    suit_binary_hole[h1 & 0x3] |= binaries_by_id[h1];
                    suit_binary_hole[h2 & 0x3] |= binaries_by_id[h2];
                    suit_binary_hole[h3 & 0x3] |= binaries_by_id[h3];
                    suit_binary_hole[h4 & 0x3] |= binaries_by_id[h4];
                    suit_binary_hole[h5 & 0x3] |= binaries_by_id[h5];
                    suit_binary_hole[h6 & 0x3] |= binaries_by_id[h6];

                    if (suit_count_board[i] == 3 && suit_count_hole[i] == 2)
                    {
                        value_flush = HashTable.Flush[suit_binary_board[i] | suit_binary_hole[i]];
                    }
                    else if (suit_count_hole[i] <= 4)
                    {
                        var padding = new uint[] { 0x0000, 0x2000, 0x6000 };

                        suit_binary_board[i] |= padding[5 - suit_count_board[i]];
                        suit_binary_hole[i] |= padding[4 - suit_count_hole[i]];

                        board_hash = Hash_binary(suit_binary_board[i], 15, 5);
                        hole_hash = Hash_binary(suit_binary_hole[i], 15, 4);

                        value_flush = TablesOmaha.Flush_omaha[board_hash * 1365 + hole_hash];
                    }
                    else
                    {
                        var padding = new uint[] { 0x0000, 0x2000, 0x6000 };

                        suit_binary_board[i] |= padding[5 - suit_count_board[i]];
                        suit_binary_hole[i] |= padding[6 - suit_count_hole[i]];

                        board_hash = Hash_binary(suit_binary_board[i], 15, 5);
                        hole_hash = Hash_binary(suit_binary_hole[i], 14, 6);

                        value_flush = TablesOmaha6.Flush_omaha6[board_hash * 3003 + hole_hash];  
                    }

                    break;
                }
            }

            var quinary_board = new byte[13];
            var quinary_hole = new byte[13];

            quinary_board[(c1 >> 2)]++;
            quinary_board[(c2 >> 2)]++;
            quinary_board[(c3 >> 2)]++;
            quinary_board[(c4 >> 2)]++;
            quinary_board[(c5 >> 2)]++;

            quinary_hole[(h1 >> 2)]++;
            quinary_hole[(h2 >> 2)]++;
            quinary_hole[(h3 >> 2)]++;
            quinary_hole[(h4 >> 2)]++;
            quinary_hole[(h5 >> 2)]++;
            quinary_hole[(h6 >> 2)]++;

            board_hash = Hash.Hash_quinary(quinary_board, 5);
            hole_hash = Hash.Hash_quinary(quinary_hole, 6);

            value_noflush = TablesOmaha6.NoFlush_omaha6[board_hash * 18395 + hole_hash];

            if (value_flush < value_noflush)
            {
                return value_flush;
            }
            else
            {
                return value_noflush;
            }
        }

        internal static int Evaluate_omaha_6_cards_by_omaha(int c1, int c2, int c3, int c4, int c5,
                                                            int h1, int h2, int h3, int h4, int h5, int h6)
        {
            var combinationsHoleCard = new int[][] 
            {
                //15 6C4
                new int[] {h1, h2, h3, h4},
                new int[] {h1, h2, h3, h5},
                new int[] {h1, h2, h3, h6},
                new int[] {h1, h2, h4, h5},
                new int[] {h1, h2, h4, h6},
                new int[] {h1, h2, h5, h6},
                new int[] {h1, h3, h4, h5},
                new int[] {h1, h3, h4, h6},
                new int[] {h1, h3, h5, h6},
                new int[] {h1, h4, h5, h6},
                new int[] {h2, h3, h4, h5},
                new int[] {h2, h3, h4, h6},
                new int[] {h2, h3, h5, h6},
                new int[] {h2, h4, h5, h6},
                new int[] {h3, h4, h5, h6},
            };

            var bestValue = combinationsHoleCard
                            .Select(e => 
                                EvaluatorOmaha.Evaluate_omaha_cards(c1, c2, c3, c4, c5, e[0], e[1], e[2], e[3]))
                            .Min();

            return bestValue;
        }

        internal static int Evaluate_omaha_6_cards_by_holdem(int c1, int c2, int c3, int c4, int c5,
                                                                int h1, int h2, int h3, int h4, int h5, int h6)
        {
            var combinationsCommunityCard = new int[][] 
            {
                //10 5C3
                new int[] {c1, c2, c3},
                new int[] {c1, c2, c4},
                new int[] {c1, c2, c5},
                new int[] {c1, c3, c4},
                new int[] {c1, c3, c5},
                new int[] {c1, c4, c5},
                new int[] {c2, c3, c4},
                new int[] {c2, c3, c5},
                new int[] {c2, c4, c5},
                new int[] {c3, c4, c5},
            };

            var combinationsHoleCard = new int[][] 
            {
                //15 6C2
                new int[] {h1, h2},
                new int[] {h1, h3},
                new int[] {h1, h4},
                new int[] {h1, h5},
                new int[] {h1, h6},
                new int[] {h2, h3},
                new int[] {h2, h4},
                new int[] {h2, h5},
                new int[] {h2, h6},
                new int[] {h3, h4},
                new int[] {h3, h5},
                new int[] {h3, h6},
                new int[] {h4, h5},
                new int[] {h4, h6},
                new int[] {h5, h6},
            };

            var beatValue = combinationsCommunityCard
                            .SelectMany(cc => 
                                combinationsHoleCard
                                .Select(ch => 
                                    Evaluator5.Evaluate_5cards(cc[0], cc[1], cc[2], ch[0], ch[1])))
                            .Min();

            return beatValue;
        }
    }
}