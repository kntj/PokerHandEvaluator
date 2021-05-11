using System;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;

namespace PokerHandEvaluator
{
    class TablesOmaha6
    {
        static bool initialized = false;
        
        internal static short[] Flush_omaha6;
        internal static short[] NoFlush_omaha6;

        internal static void Init()
        {
            if (initialized)
            {
                return;
            }
            initialized = true;

            Flush_omaha6 = new short[9018009]; //3003 * 3003  15C5 * 14C6
            NoFlush_omaha6 = new short[113589125]; //6175 * 18395  Dp[1,13,5] * Dp[1,13,6]

            ReadBinary();
            //ReadText();
        }

        static void ReadBinary()
        {
            ReadResourceBinary("PokerHandEvaluator.Resources.flush_omaha_6.bin", Flush_omaha6);
            ReadResourceBinary("PokerHandEvaluator.Resources.no_flush_omaha_6.bin", NoFlush_omaha6);
        }

        static void ReadResourceBinary(string resourceName, short[] hashTable)
        {
            var assembly = Assembly.GetExecutingAssembly();

            IFormatter formatter = new BinaryFormatter();  
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                var src = (short[])formatter.Deserialize(stream);
                Array.Copy(src, hashTable, src.Length); 
            }
        }

        static void ReadText()
        {
            ReadResourceText("PokerHandEvaluator.Resources.flush_omaha_6.txt", Flush_omaha6);
            ReadResourceText("PokerHandEvaluator.Resources.no_flush_omaha_6.txt", NoFlush_omaha6);
        }

        static void ReadResourceText(string resourceName, short[] hashTable)
        {
            var assembly = Assembly.GetExecutingAssembly();

            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                using (StreamReader sr = new StreamReader(stream))
                {
                    var index = 0;
                    while (sr.Peek() >= 0)
                    {
                        var line = sr.ReadLine();
                        var values = line.Split(",");
                        for (var i = 0; i < values.Length; i++)
                        {
                            if (values[i].Trim() != "")
                            {
                                hashTable[index] = short.Parse(values[i].Trim());
                                index++;
                            }
                        }
                    }
                }
            }
        }
    }
}