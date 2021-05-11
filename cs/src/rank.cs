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

using System;
using System.Collections.Generic;

namespace PokerHandEvaluator
{
    public class Rank : IComparer<Rank>, IComparable<Rank>
    {
        static readonly string[] rankCategoryDescription = new string[] 
        {
            "",
            "Straight Flush",
            "Four of a Kind",
            "Full House",
            "Flush",
            "Straight",
            "Three of a Kind",
            "Two Pair",
            "One Pair",
            "High Card",
        };

        readonly int value_;

        public Rank(int value)
        {
            value_ = value;
        }

        public enum RankCategory
        {
            // FIVE_OF_A_KIND = 0, // Reserved
            STRAIGHT_FLUSH = 1,
            FOUR_OF_A_KIND,
            FULL_HOUSE,
            FLUSH,
            STRAIGHT,
            THREE_OF_A_KIND,
            TWO_PAIR,
            ONE_PAIR,
            HIGH_CARD,
        }

        public static bool operator <(Rank t, Rank other) => t.value_ >= other.value_;
        public static bool operator <=(Rank t, Rank other) => t.value_ > other.value_;
        public static bool operator >(Rank t, Rank other) => t.value_ <= other.value_;
        public static bool operator >=(Rank t, Rank other) => t.value_ < other.value_;
        public static bool operator ==(Rank t, Rank other) => t.value_ == other.value_;
        public static bool operator !=(Rank t, Rank other) => t.value_ != other.value_;

        public override bool Equals(Object obj)
        {
            if ((obj == null) || !this.GetType().Equals(obj.GetType()))
            {
                return false;
            }
            else
            {
                return value_ == ((Rank)obj).value_;
            }
        }

        public override int GetHashCode()
        {
            return value_;
        }

        public int Compare(Rank x, Rank y)
        {
            return y.value_ - x.value_;
        }

        public int CompareTo(Rank other)
        {
            return Compare(this, other);
        }
        
        public int Value()
        { 
            return value_;
        }

        public RankCategory Category()
        {
            if (value_ > 6185) return RankCategory.HIGH_CARD;        // 1277 high card
            if (value_ > 3325) return RankCategory.ONE_PAIR;         // 2860 one pair
            if (value_ > 2467) return RankCategory.TWO_PAIR;         //  858 two pair
            if (value_ > 1609) return RankCategory.THREE_OF_A_KIND;  //  858 three-kind
            if (value_ > 1599) return RankCategory.STRAIGHT;         //   10 straights
            if (value_ > 322) return RankCategory.FLUSH;            // 1277 flushes
            if (value_ > 166) return RankCategory.FULL_HOUSE;       //  156 full house
            if (value_ > 10) return RankCategory.FOUR_OF_A_KIND;   //  156 four-kind
            return RankCategory.STRAIGHT_FLUSH;
        }

        public string DescribeCategory()
        {
            return rankCategoryDescription[(int)Category()];
        }

        public string DescribeRank()
        {
            return Rank7462.Description[value_, 1];
        }

        public string DescribeSampleHand()
        {
            return Rank7462.Description[value_, 0];
        }

        public bool IsFlush()
        {
            switch (Category())
            {
                case RankCategory.STRAIGHT_FLUSH:
                case RankCategory.FLUSH:
                    return true;
                default:
                    return false;
            }
        }
    }
}