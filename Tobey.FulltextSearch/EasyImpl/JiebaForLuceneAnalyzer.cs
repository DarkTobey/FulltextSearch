using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using JiebaNet.Segmenter;
using Lucene.Net.Analysis;

namespace Tobey.FulltextSearch.EasyImpl
{
    /// <summary>
    /// 基于LuceneNet扩展的JieBa分析器
    /// </summary>
    public class JiebaForLuceneAnalyzer : Analyzer
    {
        private static ISet<string> StopWords;
        protected static readonly ISet<string> DefaultStopWords = StopAnalyzer.ENGLISH_STOP_WORDS_SET;

        static JiebaForLuceneAnalyzer()
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
            TokenStream result = new JiebaForLuceneTokenizer(seg, reader);
            result = new LowerCaseFilter(result);
            result = new StopFilter(true, result, StopWords);
            return result;
        }
    }
}
