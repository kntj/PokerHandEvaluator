using System;
using PokerHandEvaluator;

namespace HashTableGenerator
{
    class Program
    {
        static void Main(string[] args)
        {
            HashTableGeneratorOmaha6Flush.Create();
            HashTableGeneratorOmaha6NoFlush.Create();
        }
    }
}
