/*
 *  Copyright 2016 Henry Lee
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
    public class Evaluator8
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

        static readonly ushort[] suitbit_by_id = new ushort[]
        {
          0x1,  0x8,  0x40,  0x200,
          0x1,  0x8,  0x40,  0x200,
          0x1,  0x8,  0x40,  0x200,
          0x1,  0x8,  0x40,  0x200,
          0x1,  0x8,  0x40,  0x200,
          0x1,  0x8,  0x40,  0x200,
          0x1,  0x8,  0x40,  0x200,
          0x1,  0x8,  0x40,  0x200,
          0x1,  0x8,  0x40,  0x200,
          0x1,  0x8,  0x40,  0x200,
          0x1,  0x8,  0x40,  0x200,
          0x1,  0x8,  0x40,  0x200,
          0x1,  0x8,  0x40,  0x200,
        };

        /*
         * Card id, ranged from 0 to 51.
         * The two least significant bits represent the suit, ranged from 0-3.
         * The rest of it represent the rank, ranged from 0-12.
         * 13 * 4 gives 52 ids.
         */
        internal static int Evaluate_8cards(int a, int b, int c, int d, int e, int f, int g, int h)
        {
            var suit_hash = 0;
            var value_flush = 10000;
            var value_noflush = 10000;

            suit_hash += suitbit_by_id[a];
            suit_hash += suitbit_by_id[b];
            suit_hash += suitbit_by_id[c];
            suit_hash += suitbit_by_id[d];
            suit_hash += suitbit_by_id[e];
            suit_hash += suitbit_by_id[f];
            suit_hash += suitbit_by_id[g];
            suit_hash += suitbit_by_id[h];

            if (DpTable.Suits[suit_hash] != 0)
            {
                var suit_binary = new int[4];
                suit_binary[a & 0x3] |= binaries_by_id[a];
                suit_binary[b & 0x3] |= binaries_by_id[b];
                suit_binary[c & 0x3] |= binaries_by_id[c];
                suit_binary[d & 0x3] |= binaries_by_id[d];
                suit_binary[e & 0x3] |= binaries_by_id[e];
                suit_binary[f & 0x3] |= binaries_by_id[f];
                suit_binary[g & 0x3] |= binaries_by_id[g];
                suit_binary[h & 0x3] |= binaries_by_id[h];

                value_flush = HashTable.Flush[suit_binary[DpTable.Suits[suit_hash] - 1]];
            }

            var quinary = new byte[13];
            quinary[(a >> 2)]++;
            quinary[(b >> 2)]++;
            quinary[(c >> 2)]++;
            quinary[(d >> 2)]++;
            quinary[(e >> 2)]++;
            quinary[(f >> 2)]++;
            quinary[(g >> 2)]++;
            quinary[(h >> 2)]++;

            var hash = Hash.Hash_quinary(quinary, 8);

            value_noflush = HashTable8.NoFlush8[hash];

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
