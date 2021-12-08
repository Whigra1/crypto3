using System.Collections.Generic;
using System.Globalization;
using System.Text;
using crypto1;

namespace Lab2
{
    using System;
    using System.IO;
    using System.Linq;

    class Program
    {
        private static string TestString;
        
        private static EnglishNGramCounter _nGram = new(200, 800);
        
        private static string[] _lines = File.ReadAllLines(Path.GetFullPath("./data.txt"));

        private static void Main(string[] args)
        {
            TestString = _lines[^1];
            ToBruteForceOrNotToBruteForce();
        }

        private static byte[] BytesFromHexString(string text)
        {
            return text
                .Chunk(2)
                .Select(hexNum => byte.Parse(new string(hexNum), NumberStyles.HexNumber))
                .ToArray();
        }

        private static string TryHack(string line, string text)
        {
            var byteText = Encoding.UTF8.GetBytes(text);
            var res = Xor(Xor(BytesFromHexString(TestString), BytesFromHexString(line)), byteText);
            return Encoding.UTF8.GetString(res);
        }

        private static byte[] Xor(byte[] text1, byte[] text2)
        {
            var (b1, b2) = text1.Length < text2.Length ? (text1, text2) : (text2, text1);
            return b1.Select((b, index) => (byte) (b ^ b2[index])).ToArray();
        }

        private static void ToBruteForceOrNotToBruteForce()
        {
            var text = "";
            for (int i = 0; i < 7; i++)
            {
                text += FindBestTriGram(text);
            }
            
            foreach (var line in _lines)
            {
                
                Console.WriteLine(TryHack(line, "and lose the name"));
            } 
            
        }
        private static string FindBestTriGram(string text)
        {
            var bestTriGram = "";
            var bestTriGramScore = 0;
            foreach (var triGramsKey in _nGram.TriGrams.Keys)
            {
                var triGramCount = _lines
                    .Count(_line =>
                        {
                            var res = TryHack(_line, text + triGramsKey);
                            return _nGram.TriGrams.ContainsKey(res[^3..]);
                        }
                    );
                if (triGramCount > bestTriGramScore)
                {
                    bestTriGram = triGramsKey;
                    bestTriGramScore = triGramCount;
                }
                if (triGramCount > 12)
                    return triGramsKey;
            }

            return bestTriGram;
        }
    }
}