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
    public class Evaluator
    {
        public static Rank EvaluateCards(Card a, Card b, Card c, Card d,
                                            Card e)
        {
            return new Rank(Evaluator5.Evaluate_5cards(a, b, c, d, e));
        }

        public static Rank EvaluateCards(Card a, Card b, Card c, Card d,
                                            Card e, Card f)
        {
            return new Rank(Evaluator6.Evaluate_6cards(a, b, c, d, e, f));
        }

        public static Rank EvaluateCards(Card a, Card b, Card c, Card d,
                                            Card e, Card f, Card g)
        {
            return new Rank(Evaluator7.Evaluate_7cards(a, b, c, d, e, f, g));
        }

        public static Rank EvaluateHand(Hand hand)
        {
            if (DpTable.Suits[hand.GetSuitHash()] != 0)
            {
                return new Rank(HashTable.Flush[hand.GetSuitBinary()[DpTable.Suits[hand.GetSuitHash()] - 1]]);
            }

            var hash = Hash.Hash_quinary(hand.GetQuinary(), hand.Size());

            switch (hand.Size())
            {
                case 5: return new Rank(HashTable5.NoFlush5[hash]);
                case 6: return new Rank(HashTable6.NoFlush6[hash]);
                case 7: return new Rank(HashTable7.NoFlush7[hash]);
            }

            return new Rank(HashTable5.NoFlush5[hash]);
        }

        public static Rank EvaluateOmahaCards(Card c1, Card c2, Card c3, Card c4, Card c5,
                                                Card h1, Card h2, Card h3, Card h4)
        {
            return new Rank(EvaluatorOmaha.Evaluate_omaha_cards(c1, c2, c3, c4, c5, h1, h2, h3, h4));
        }

        public static Rank EvaluateOmaha6Cards(Card c1, Card c2, Card c3, Card c4, Card c5,
                                                Card h1, Card h2, Card h3, Card h4, Card h5, Card h6)
        {
            return new Rank(EvaluatorOmaha6.Evaluate_omaha_6_cards(c1, c2, c3, c4, c5, h1, h2, h3, h4, h5, h6));
        }

        public static Rank EvaluateOmaha6CardsByOmaha(Card c1, Card c2, Card c3, Card c4, Card c5,
                                                        Card h1, Card h2, Card h3, Card h4, Card h5, Card h6)
        {
            return new Rank(EvaluatorOmaha6.Evaluate_omaha_6_cards_by_omaha(c1, c2, c3, c4, c5, h1, h2, h3, h4, h5, h6));
        }

        public static Rank EvaluateOmaha6CardsByHoldem(Card c1, Card c2, Card c3, Card c4, Card c5,
                                                        Card h1, Card h2, Card h3, Card h4, Card h5, Card h6)
        {
            return new Rank(EvaluatorOmaha6.Evaluate_omaha_6_cards_by_holdem(c1, c2, c3, c4, c5, h1, h2, h3, h4, h5, h6));
        }
    }
}
