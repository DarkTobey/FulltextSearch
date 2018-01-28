using JiebaNet.Segmenter;
using Lucene.Net.Analysis;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tobey.FulltextSearch
{
    public class JiebaAnalyzer : Lucene.Net.Analysis.Analyzer
    {
        private static ISet<string> StopWords;
        private static readonly ISet<string> DefaultStopWords = StopAnalyzer.ENGLISH_STOP_WORDS_SET;

        static JiebaAnalyzer()
        {
            var stopWordsFile = Path.GetFullPath(JiebaNet.Analyser.ConfigManager.StopWordsFile);
            if (File.Exists(stopWordsFile))
            {
                StopWords = new HashSet<string>();
                var lines = File.ReadAllLines(stopWordsFile);
                foreach (var line in lines)
                {
                    StopWords.Add(line.Trim());
                }
            }
            else
            {
                StopWords = DefaultStopWords;
            }
        }

        public override TokenStream TokenStream(string fieldName, TextReader reader)
        {
            var seg = new JiebaSegmenter();
            TokenStream result = new JiebaTokenizer(seg, reader);
            result = new LowerCaseFilter(result);
            result = new StopFilter(true, result, StopWords);
            return result;
        }
    }
}
