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
    public class Card
    {
        readonly int id_;
        string name_ = "";

        public Card(int id)
        {
            id_ = id;
        }
        
        public Card(string name)
        {
            var rankMap = new Dictionary<char, int>
            {
              {'2', 0}, {'3', 1}, {'4', 2}, {'5', 3},
              {'6', 4}, {'7', 5}, {'8', 6}, {'9', 7},
              {'T', 8}, {'J', 9}, {'Q', 10}, {'K', 11}, {'A', 12},
              {'t', 8}, {'j', 9}, {'q', 10}, {'k', 11}, {'a', 12},
            };
            var suitMap = new Dictionary<char, int>
            {
              {'C', 0}, {'D', 1}, {'H', 2}, {'S', 3},
              {'c', 0}, {'d', 1}, {'h', 2}, {'s', 3},
            };

            if (name.Length < 2)
            {
                throw new ArgumentException("name.Length < 2 :" + name);
            }

            id_ = rankMap[name[0]] * 4 + suitMap[name[1]];
            name_ = name;
        }

        public Card(char[] name) : this(new string(name)) {}

        public static implicit operator int(Card c) => c.id_;

        public override string ToString()
        {
            if (name_ == "")
            {
                name_ = new string(new char[] {
                                                "23456789TJQKA"[id_ >> 2], 
                                                "cdhs"[id_ & 0x3]
                                            });
            }

            return name_;
        }
    }
}