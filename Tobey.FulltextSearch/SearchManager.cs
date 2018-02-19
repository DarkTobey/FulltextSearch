using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Tokenattributes;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using PanGu.HighLight;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tobey.FulltextSearch
{
    public class SearchManager
    {
        private static readonly string _IndexDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, System.Configuration.ConfigurationManager.AppSettings["IndexStoreDir"]);
        private static SearchManager _Instance = new SearchManager();
        private static ResultFormatter _Formatter = new ResultFormatter()
        {
            FragmentSize = 100,
            HTMLFormatter = new SimpleHTMLFormatter()
        };


        private SearchManager()
        {
        }

        public static SearchManager Instance()
        {
            return _Instance;
        }

        public void SetResultFormatter(ResultFormatter formatter)
        {
            _Formatter = formatter;
        }

        public SearchResult<RecordInfo> Search(string keywords, int start, int length)
        {
            return Search(keywords, null, null, null, start, length);
        }

        public SearchResult<RecordInfo> Search(string keywords, string[] fields, Dictionary<string, string> filters, List<ResultOrderBy> sorts, int start, int length)
        {
            if (string.IsNullOrEmpty(keywords))
                return null;

            keywords = ReplaceSensitiveWords(keywords);


            Stopwatch stopWatch = new Stopwatch();
            stopWatch.Start();


            Query query = MakeSearchQuery(keywords, fields, new JiebaAnalyzer());
            Filter luceneFilter = MakeSearchFilter(filters);
            Sort sort = MakeSort(sorts);

            FSDirectory fsDirectory = FSDirectory.Open(new DirectoryInfo(_IndexDir));
            IndexReader indexReader = IndexReader.Open(fsDirectory, true);
            IndexSearcher indexSearcher = new IndexSearcher(indexReader);

            TopDocs topDocs = indexSearcher.Search(query, luceneFilter, start + length, sort);
            ScoreDoc[] hits = topDocs.ScoreDocs;

            if (hits.Length < 1)
                return null;


            int end = hits.Length < length ? start + hits.Length : start + length;
            List<RecordInfo> data = new List<RecordInfo>();
            for (int i = start; i < end; i++)
            {
                ScoreDoc scoredoc = hits[i];
                Document doc = indexSearcher.Doc(scoredoc.Doc);

                RecordInfo info = new RecordInfo();
                info.RowId = doc.Get("RowId");

                string title = doc.Get("Title");
                string body = doc.Get("Body");
                string lightTitle = Highlight(keywords, title);
                string ligthBody = Highlight(keywords, body);
                info.Title = string.IsNullOrEmpty(lightTitle) ? title : lightTitle;
                info.Body = string.IsNullOrEmpty(ligthBody) ? body : ligthBody;

                data.Add(info);
            }


            stopWatch.Stop();
            Console.Write("检索耗时:" + stopWatch.ElapsedMilliseconds * 1.0 / 1000 + "秒");


            return new SearchResult<RecordInfo>
            {
                Start = start,
                Length = length,
                Total = hits.Length,
                Data = data,
            };
        }

        private string ReplaceSensitiveWords(string key)
        {
            string[] sensitiveWords = new string[] {
                "+", "-" , "<" , ">",
                "(", "（" , ")" , "）",
                ":", "：" , "!" , "！",
                "[", "【" , "]" , "】",
                "{", "}" , "~" , "*",
                "?", "？" , "^" , "……",
            };

            foreach (var word in sensitiveWords)
            {
                key = key.Replace(word, "");
            }
            return key;
        }

        private Query MakeSearchQuery(string key, string[] fields, Analyzer analyzer)
        {
            if (fields == null || !fields.Any())
            {
                fields = new string[] { "Title", "Body" };
            }

            BooleanQuery query = new BooleanQuery();
            QueryParser parser = new MultiFieldQueryParser(Lucene.Net.Util.Version.LUCENE_30, fields, analyzer);
            query.Add(parser.Parse(key), Occur.MUST);
            return query;
        }

        private Filter MakeSearchFilter(Dictionary<string, string> filter)
        {
            Filter luceneFilter = null;
            if (filter != null && filter.Keys.Any())
            {
                var booleanQuery = new BooleanQuery();
                foreach (KeyValuePair<string, string> keyValuePair in filter)
                {
                    var termQuery = new TermQuery(new Term(keyValuePair.Key, keyValuePair.Value));
                    booleanQuery.Add(termQuery, Occur.MUST);
                }
                luceneFilter = new QueryWrapperFilter(booleanQuery);
            }
            return luceneFilter;
        }

        private Sort MakeSort(List<ResultOrderBy> sorts)
        {
            Sort result = Sort.RELEVANCE;

            if (sorts != null)
            {
                result = new Sort(sorts.Select(x => new SortField(x.FieldName, SortField.FLOAT, x.IsDesc)).ToArray());
            }

            return result;
        }

        private string[] SplitWords(string key)
        {
            List<string> result = new List<string>();
            JiebaAnalyzer analyzer = new JiebaAnalyzer();
            TokenStream tokenStream = analyzer.TokenStream("", new StringReader(key));

            bool hasNext;
            ITermAttribute ita;
            while (hasNext = tokenStream.IncrementToken())
            {
                ita = tokenStream.GetAttribute<ITermAttribute>();
                result.Add(ita.Term);
                hasNext = tokenStream.IncrementToken();
            }

            return result.ToArray();
        }

        private string Highlight(string key, string content)
        {
            Highlighter highlighter = new Highlighter(_Formatter.HTMLFormatter, new PanGu.Segment())
            {
                FragmentSize = _Formatter.FragmentSize
            };

            return highlighter.GetBestFragment(key, content);
        }
    }

}
