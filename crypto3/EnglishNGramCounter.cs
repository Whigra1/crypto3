using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace crypto1
{
    public class EnglishNGramCounter
    {
        private Dictionary<string, long> BiGrams { get; set; }
        public Dictionary<string, long> TriGrams { get; set; }
        private Dictionary<string, long> QuadGrams { get; set; }
        public EnglishNGramCounter(int biGramsCount = 5000, int triGramCount = 5000, int quadGrams = 200)
        {
            BiGrams = LoadGrams(Path.GetFullPath("./english_bigrams.txt"), biGramsCount);
            TriGrams = LoadGrams(Path.GetFullPath("./english_trigrams.txt"), triGramCount);
            QuadGrams = LoadGrams(Path.GetFullPath("./english_quadgrams.txt"), quadGrams);
        }

        private Dictionary<string, long> LoadGrams(string path, int n)
        {
            var quadGramLine = File
                .ReadLines(Path.GetFullPath(path))
                .Take(n)
                .ToList();
            var res = new Dictionary<string, long>(n);
            foreach (var s in quadGramLine)
            {
                var splitLine = s.ToLower().Split(" ");
                if (splitLine.Length != 2)  continue;
                res[splitLine[0]] = long.Parse(splitLine[1]);
            }
            return res;
        }
        
        public double CountBiGrams(string text)
        {
            var (quadDictionary, totalQuadgrams) = FindAllGrams(text, BiGrams, 2);
            return quadDictionary.Sum(keyValuePair => Math.Log(keyValuePair.Value / (double) totalQuadgrams));
        }
        
        public double CountTriGrams(string text)
        {
            var (quadDictionary, totalQuadgrams) = FindAllGrams(text, TriGrams, 3);
            return quadDictionary.Sum(keyValuePair => Math.Log(keyValuePair.Value / (double) totalQuadgrams));
        }
        
        public double CountQuadGrams(string text)
        {
            var (quadDictionary, totalQuadgrams) = FindAllGrams(text, QuadGrams, 4);
            return quadDictionary.Sum(keyValuePair => Math.Log(keyValuePair.Value / (double) totalQuadgrams));
        }
        
        private (Dictionary<string, long>, long) FindAllGrams(string text, Dictionary<string, long> grams, int offsetStep)
        {
            var res = new Dictionary<string, long>(50);
            var totalCount = 0L;
            for (int offset = 0; offset < text.Length; offset += offsetStep)
            {
                var subString = offset + offsetStep > text.Length ? text[offset..] : text[offset..(offset + offsetStep)];
                if (!grams.ContainsKey(subString)) continue;
                if (res.ContainsKey(subString))
                    res[subString]++;
                else
                    res[subString] = 1;
                totalCount++;
            }
            return (res, totalCount);
        }
    }
}