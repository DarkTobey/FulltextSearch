using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Analysis.Tokenattributes;

namespace Tobey.FulltextSearch
{
    class Program
    {
        static void Main(string[] args)
        {
            NeiZhiFenCi();
            JieBaFenCi();

            Console.ReadLine();
        }

        static void NeiZhiFenCi()
        {
            string txt = "标准分词,一元分词";

            Analyzer analyzer = new StandardAnalyzer(Lucene.Net.Util.Version.LUCENE_30);
            TokenStream tokenStream = analyzer.TokenStream("", new StringReader(txt));

            string result = string.Empty;
            bool hasNext;
            ITermAttribute ita;
            while (hasNext = tokenStream.IncrementToken())
            {
                ita = tokenStream.GetAttribute<ITermAttribute>();
                result += ita.Term + " | ";
                hasNext = tokenStream.IncrementToken();
            }
            Console.WriteLine(result);
        }

        static void JieBaFenCi()
        {
            string txt = "测试一下结巴分词";

            JiebaForLuceneAnalyzer analyzer = new JiebaForLuceneAnalyzer();
            TokenStream tokenStream = analyzer.TokenStream("", new StringReader(txt));

            string result = string.Empty;
            bool hasNext;
            ITermAttribute ita;
            while (hasNext = tokenStream.IncrementToken())
            {
                ita = tokenStream.GetAttribute<ITermAttribute>();
                result += ita.Term + " | ";
                hasNext = tokenStream.IncrementToken();
            }
            Console.WriteLine(result);
        }


    }
}
