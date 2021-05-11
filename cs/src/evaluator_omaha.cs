/*
 *  Copyright 2016-2019 Henry Lee
 *
 *  Licensed under the Apache License, Version 2.0 (the "License");
 *  you may not use this file except in compliance with the License.
 *  You may obtain a copy of the License at
 *
 *      http://www.apache.org/licenses/LICENSE-2.0
 *
 *  Unless required by applicable law or agreed to in writing, software
 *  distributed under the License is distributed on an "AS IS" BASIS,
 *  WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 *  See the License for the specific language governing permissions and
 *  limitations under the License.
 */

namespace PokerHandEvaluator
{
    class EvaluatorOmaha
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

        static int Hash_binary(uint binary, int k)
        {
            int sum = 0;
            const int len = 15;

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

        /*
         * Card id, ranged from 0 to 51.
         * The two least significant bits represent the suit, ranged from 0-3.
         * The rest of it represent the rank, ranged from 0-12.
         * 13 * 4 gives 52 ids.
         *
         * The first five parameters are the community cards on the board
         * The last four parameters are the hole cards of the player
         */
        internal static int Evaluate_omaha_cards(int c1, int c2, int c3, int c4, int c5,
                                                    int h1, int h2, int h3, int h4)
        {
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

                    if (suit_count_board[i] == 3 && suit_count_hole[i] == 2)
                    {
                        value_flush = HashTable.Flush[suit_binary_board[i] | suit_binary_hole[i]];
                    }
                    else
                    {
                        var padding = new uint[] { 0x0000, 0x2000, 0x6000 };

                        suit_binary_board[i] |= padding[5 - suit_count_board[i]];
                        suit_binary_hole[i] |= padding[4 - suit_count_hole[i]];

                        board_hash = Hash_binary(suit_binary_board[i], 5);
                        hole_hash = Hash_binary(suit_binary_hole[i], 4);

                        value_flush = TablesOmaha.Flush_omaha[board_hash * 1365 + hole_hash];
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

            board_hash = Hash.Hash_quinary(quinary_board, 5);
            hole_hash = Hash.Hash_quinary(quinary_hole, 4);

            value_noflush = TablesOmaha.NoFlush_omaha[board_hash * 1820 + hole_hash];

            if (value_flush < value_noflush)
            {
                return value_flush;
            }
            else
            {
                return value_noflush;
            }
        }
    }
}