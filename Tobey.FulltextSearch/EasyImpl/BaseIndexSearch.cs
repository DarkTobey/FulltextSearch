using JiebaNet.Segmenter;
using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.QueryParsers;
using Lucene.Net.Search;
using Lucene.Net.Store;
using PanGu;
using PanGu.HighLight;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tobey.FulltextSearch.EasyImpl
{
    public abstract class BaseIndexSearch<TIndexSearchResultItem> where TIndexSearchResultItem : IndexSearchResultItem
    {
        /// <summary>
        /// 索引存储目录
        /// </summary>
        private static readonly string IndexStorePath = ConfigurationManager.AppSettings["IndexStorePath"];
        private readonly string[] fieldsToSearch;
        protected static readonly SimpleHTMLFormatter formatter = new SimpleHTMLFormatter("<em>", "</em>");
        private static IndexSearcher indexSearcher = null;

        /// <summary>
        /// 索引内容命中片段大小
        /// </summary>
        public int FragmentSize { get; set; }

        /// <summary>
        /// 构造方法
        /// </summary>
        /// <param name="fieldsToSearch">搜索文本字段</param>
        protected BaseIndexSearch(string[] fieldsToSearch)
        {
            FragmentSize = 100;
            this.fieldsToSearch = fieldsToSearch;
        }

        /// <summary>
        /// 创建搜索结果实例
        /// </summary>
        /// <returns></returns>
        protected abstract TIndexSearchResultItem CreateIndexSearchResultItem();

        /// <summary>
        /// 修改搜索结果（主要修改tag字段对应的属性）
        /// </summary>
        /// <param name="indexSearchResultItem">搜索结果项实例</param>
        /// <param name="content">用户搜索内容</param>
        /// <param name="docIndex">索引库位置</param>
        /// <param name="doc">当前位置内容</param>
        /// <returns>搜索结果</returns>
        protected abstract void ModifyIndexSearchResultItem(ref TIndexSearchResultItem indexSearchResultItem, string content, int docIndex, Document doc);

        /// <summary>
        /// 修改筛选器（各模块）
        /// </summary>
        /// <param name="filter"></param>
        protected abstract void ModifySearchFilter(ref Dictionary<string, string> filter);

        /// <summary>
        /// 全库搜索
        /// </summary>
        /// <param name="content">搜索文本内容</param>
        /// <param name="filter">查询内容限制条件,默认为null，不限制条件.</param>
        /// <param name="fieldSorts">对字段进行排序</param>
        /// <param name="pageIndex">查询结果当前页，默认为1</param>
        /// <param name="pageSize">查询结果每页结果数，默认为20</param>
        public PagedIndexSearchResult<TIndexSearchResultItem> Search(string content
            , Dictionary<string, string> filter = null, List<FieldSort> fieldSorts = null
            , int pageIndex = 1, int pageSize = 20)
        {
            try
            {
                if (!string.IsNullOrEmpty(content))
                {
                    content = ReplaceIndexSensitiveWords(content);
                    content = GetKeywordsSplitBySpace(content,
                        new JiebaForLuceneTokenizer(new JiebaSegmenter(), content));
                }
                if (string.IsNullOrEmpty(content) || pageIndex < 1)
                {
                    throw new Exception("输入参数不符合要求（用户输入为空，页码小于等于1）");
                }

                var stopWatch = new Stopwatch();
                stopWatch.Start();

                Analyzer analyzer = new JiebaForLuceneAnalyzer();
                // 索引条件创建
                var query = MakeSearchQuery(content, analyzer);
                // 筛选条件构建
                filter = filter == null ? new Dictionary<string, string>() : new Dictionary<string, string>(filter);
                ModifySearchFilter(ref filter);
                Filter luceneFilter = MakeSearchFilter(filter);

                #region------------------------------执行查询---------------------------------------

                TopDocs topDocs;
                if (indexSearcher == null)
                {
                    var dir = new DirectoryInfo(IndexStorePath);
                    FSDirectory entityDirectory = FSDirectory.Open(dir);
                    IndexReader reader = IndexReader.Open(entityDirectory, true);
                    indexSearcher = new IndexSearcher(reader);
                }
                else
                {
                    IndexReader indexReader = indexSearcher.IndexReader;
                    if (!indexReader.IsCurrent())
                    {
                        indexSearcher.Dispose();
                        indexSearcher = new IndexSearcher(indexReader.Reopen());
                    }
                }
                // 收集器容量为所有
                int totalCollectCount = pageIndex * pageSize;
                Sort sort = GetSortByFieldSorts(fieldSorts);
                topDocs = indexSearcher.Search(query, luceneFilter, totalCollectCount, sort ?? Sort.RELEVANCE);

                #endregion

                #region-----------------------返回结果生成-------------------------------

                ScoreDoc[] hits = topDocs.ScoreDocs;
                var start = (pageIndex - 1) * pageSize + 1;
                var end = Math.Min(totalCollectCount, hits.Count());

                var result = new PagedIndexSearchResult<TIndexSearchResultItem>
                {
                    PageIndex = pageIndex,
                    PageSize = pageSize,
                    TotalRecords = topDocs.TotalHits
                };

                for (var i = start; i <= end; i++)
                {
                    var scoreDoc = hits[i - 1];
                    var doc = indexSearcher.Doc(scoreDoc.Doc);

                    var indexSearchResultItem = CreateIndexSearchResultItem();
                    indexSearchResultItem.DocIndex = scoreDoc.Doc;
                    indexSearchResultItem.ModuleType = doc.Get("ModuleType");
                    indexSearchResultItem.TableName = doc.Get("TableName");
                    indexSearchResultItem.RowId = Guid.Parse(doc.Get("RowId"));
                    if (!string.IsNullOrEmpty(doc.Get("CollectTime")))
                    {
                        indexSearchResultItem.CollectTime = DateTime.Parse(doc.Get("CollectTime"));
                    }
                    var title = GetHighlighter(formatter, FragmentSize).GetBestFragment(content, doc.Get("Title"));
                    indexSearchResultItem.Title = string.IsNullOrEmpty(title) ? doc.Get("Title") : title;
                    var text = GetHighlighter(formatter, FragmentSize)
                        .GetBestFragment(content, doc.Get("IndexTextContent"));
                    indexSearchResultItem.Content = string.IsNullOrEmpty(text)
                        ? (doc.Get("IndexTextContent").Length > 100
                            ? doc.Get("IndexTextContent").Substring(0, 100)
                            : doc.Get("IndexTextContent"))
                        : text;
                    ModifyIndexSearchResultItem(ref indexSearchResultItem, content, scoreDoc.Doc, doc);
                    result.Add(indexSearchResultItem);
                }
                stopWatch.Stop();
                result.Elapsed = stopWatch.ElapsedMilliseconds * 1.0 / 1000;

                return result;

                #endregion
            }
            catch (Exception exception)
            {
                //LogUtils.ErrorLog(exception);
                return null;
            }
        }

        private Sort GetSortByFieldSorts(List<FieldSort> fieldSorts)
        {
            if (fieldSorts == null)
            {
                return null;
            }
            return new Sort(fieldSorts.Select(fieldSort => new SortField(fieldSort.FieldName, SortField.FLOAT, !fieldSort.Ascend)).ToArray());
        }

        private static Filter MakeSearchFilter(Dictionary<string, string> filter)
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

        private Query MakeSearchQuery(string content, Analyzer analyzer)
        {
            var query = new BooleanQuery();
            // 总查询参数
            // 属性查询
            if (!string.IsNullOrEmpty(content))
            {
                QueryParser parser = new MultiFieldQueryParser(Lucene.Net.Util.Version.LUCENE_30, fieldsToSearch, analyzer);
                Query queryObj;
                try
                {
                    queryObj = parser.Parse(content);
                }
                catch (ParseException parseException)
                {
                    throw new Exception("在FileLibraryIndexSearch中构造Query时出错。", parseException);
                }
                query.Add(queryObj, Occur.MUST);
            }
            return query;
        }

        private string GetKeywordsSplitBySpace(string keywords, JiebaForLuceneTokenizer jiebaForLuceneTokenizer)
        {
            var result = new StringBuilder();

            var words = jiebaForLuceneTokenizer.Tokenize(keywords);

            foreach (var word in words)
            {
                if (string.IsNullOrWhiteSpace(word.Word))
                {
                    continue;
                }

                result.AppendFormat("{0} ", word.Word);
            }

            return result.ToString().Trim();
        }

        private string ReplaceIndexSensitiveWords(string str)
        {
            str = str.Replace("+", "");
            str = str.Replace("+", "");
            str = str.Replace("-", "");
            str = str.Replace("-", "");
            str = str.Replace("!", "");
            str = str.Replace("！", "");
            str = str.Replace("(", "");
            str = str.Replace(")", "");
            str = str.Replace("（", "");
            str = str.Replace("）", "");
            str = str.Replace(":", "");
            str = str.Replace("：", "");
            str = str.Replace("^", "");
            str = str.Replace("[", "");
            str = str.Replace("]", "");
            str = str.Replace("【", "");
            str = str.Replace("】", "");
            str = str.Replace("{", "");
            str = str.Replace("}", "");
            str = str.Replace("{", "");
            str = str.Replace("}", "");
            str = str.Replace("~", "");
            str = str.Replace("~", "");
            str = str.Replace("*", "");
            str = str.Replace("*", "");
            str = str.Replace("?", "");
            str = str.Replace("？", "");
            return str;
        }

        protected Highlighter GetHighlighter(Formatter formatter, int fragmentSize)
        {
            var highlighter = new Highlighter(formatter, new Segment()) { FragmentSize = fragmentSize };
            return highlighter;
        }
    }


    public class FieldSort
    {
        public bool Ascend { get; internal set; }
        public string FieldName { get; internal set; }
    }
}
