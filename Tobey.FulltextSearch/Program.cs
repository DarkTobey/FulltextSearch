using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;

using Lucene.Net.Analysis;
using Lucene.Net.Analysis.Standard;
using Lucene.Net.Analysis.Tokenattributes;
using Lucene.Net.Index;
using Lucene.Net.Documents;
using Lucene.Net.Store;
using Lucene.Net.Search;

namespace Tobey.FulltextSearch
{
    class Program
    {
        static void Main(string[] args)
        {
            List<RecordInfo> records = new List<RecordInfo>();
            DirectoryInfo dirInfo = new DirectoryInfo("E:/总结");
            foreach (var file in dirInfo.GetFiles("*.txt"))
            {
                records.Add(new RecordInfo()
                {
                    RowId = Guid.NewGuid().ToString("N"),
                    Title = file.Name,
                    Body = File.ReadAllText(file.FullName),
                    TableName = "test",
                    ModuleType = "demo",
                    CollectTime = DateTime.Now,
                });
            }

            IndexManager.Instance().Add(records);
            SearchResult<RecordInfo> result = SearchManager.Instance().Search("测试", 0, 10);

            Console.ReadLine();
        }
    }

    class Demo
    {
        static void JieBaTest()
        {
            string txt = "测试一下结巴分词";

            JiebaAnalyzer analyzer = new JiebaAnalyzer();
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

        static void CreateIndexByData()
        {
            //索引文档保存位置  
            string indexPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FullText/IndexData");

            //FSDirectory: 指定索引库文件存放文件位置
            FSDirectory directory = FSDirectory.Open(new DirectoryInfo(indexPath), new NativeFSLockFactory());

            //IndexReader:对索引库进行读取的类
            bool isExist = IndexReader.IndexExists(directory); //是否存在索引库文件夹以及索引库特征文件
            if (isExist)
            {
                //如果索引目录被锁定（比如索引过程中程序异常退出或另一进程在操作索引库），则解锁
                //Q:存在问题 如果一个用户正在对索引库写操作 此时是上锁的 而另一个用户过来操作时 将锁解开了 于是产生冲突 解决方法后续
                if (IndexWriter.IsLocked(directory))
                {
                    IndexWriter.Unlock(directory);
                }
            }

            //IndexWriter:向索引库写入索引 (这里指定使用结巴分词进行切词,不设置最大写入长度)
            //补充:使用IndexWriter打开directory时会自动对索引库文件上锁
            IndexWriter writer = new IndexWriter(directory, new JiebaAnalyzer(), !isExist, IndexWriter.MaxFieldLength.UNLIMITED);

            //测试数据
            List<Info> contentList = GetTestTxt();

            //遍历数据源 将数据转换成为文档对象 存入索引库
            foreach (var info in contentList)
            {
                //文档对象，一条记录对应索引库中的一个文档
                Document document = new Document();

                //向文档中添加字段(字段,值,是否保存字段原始值,是否针对该列创建索引)

                //--所有字段的值都将以字符串类型保存 因为索引库只存储字符串类型数据

                //Field.Store:表示是否保存字段原值。

                //指定Field.Store.YES的字段在检索时才能用document.Get取出原值  

                //Field.Index.NOT_ANALYZED:指定不按照分词后的结果保存--是否按分词后结果保存取决于是否对该列内容进行模糊查询

                //Field.Index.ANALYZED:指定文章内容按照分词后结果保存 否则无法实现后续的模糊查询

                //WITH_POSITIONS_OFFSETS:指示不仅保存分割后的词 还保存词之间的距离

                document.Add(new Field("ID", info.ID.ToString(), Field.Store.YES, Field.Index.NOT_ANALYZED));

                document.Add(new Field("Title", info.Title, Field.Store.YES, Field.Index.ANALYZED));

                document.Add(new Field("Body", info.Body, Field.Store.YES, Field.Index.ANALYZED));

                writer.AddDocument(document); //文档写入索引库
            }

            //会自动解锁
            writer.Close();

            //不要忘了Close，否则索引结果搜不到
            directory.Close();
        }

        static void SearchFromIndexData(string searchKey)
        {
            //索引文档保存位置  
            string indexPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "FullText/IndexData");

            FSDirectory directory = FSDirectory.Open(new DirectoryInfo(indexPath), new NoLockFactory());
            IndexReader reader = IndexReader.Open(directory, true);
            IndexSearcher searcher = new IndexSearcher(reader);

            //搜索条件
            PhraseQuery query = new PhraseQuery();

            //把用户输入的关键字进行分词
            string[] words = SplitWords(searchKey);
            foreach (string word in words)
            {
                query.Add(new Term("Body", word));
            }

            //TopScoreDocCollector盛放查询结果的容器
            TopScoreDocCollector collector = TopScoreDocCollector.Create(1000, true);

            //根据query查询条件进行查询，查询结果放入collector容器
            searcher.Search(query, null, collector);

            //TopDocs 指定0到GetTotalHits() 即所有查询结果中的文档 如果TopDocs(20,10)则意味着获取第20-30之间文档内容 达到分页的效果
            ScoreDoc[] docs = collector.TopDocs(0, collector.TotalHits).ScoreDocs;

            //展示数据实体对象集合
            List<Info> bookResult = new List<Info>();
            for (int i = 0; i < docs.Length; i++)
            {
                //得到查询结果文档的id（Lucene内部分配的id）
                int docId = docs[i].Doc;

                //根据文档id来获得文档对象Document
                Document doc = searcher.Doc(docId);

                Info book = new Info();
                book.ID = doc.Get("ID");
                book.Title = doc.Get("Title");
                book.Body = HightLight(searchKey, doc.Get("Body"));//搜索关键字高亮显示 使用盘古提供高亮插件

                bookResult.Add(book);
            }

        }

        static List<Info> GetTestTxt()
        {
            List<Info> result = new List<Info>();

            string dir = "E:/总结";
            DirectoryInfo dirInfo = new DirectoryInfo(dir);
            foreach (var file in dirInfo.GetFiles("*.txt"))
            {
                result.Add(new Info()
                {
                    ID = DateTime.Now.ToFileTime().ToString(),
                    Title = file.Name,
                    Body = File.ReadAllText(file.FullName),
                });
            }
            return result;
        }

        /// <summary>
        /// 分词
        /// </summary>
        /// <param name="content"></param>
        /// <returns></returns>
        static string[] SplitWords(string content)
        {
            List<string> result = new List<string>();
            JiebaAnalyzer analyzer = new JiebaAnalyzer();
            TokenStream tokenStream = analyzer.TokenStream("", new StringReader(content));

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

        /// <summary>
        /// 搜索结果中关键词高亮显示
        /// </summary>
        /// <param name="keyword"></param>
        /// <param name="content"></param>
        /// <returns></returns>
        static string HightLight(string keyword, string content)
        {
            //创建HTMLFormatter,参数为高亮单词的前后缀
            PanGu.HighLight.SimpleHTMLFormatter simpleHTMLFormatter = new PanGu.HighLight.SimpleHTMLFormatter("<em>", "<em>");

            //创建 Highlighter ，输入HTMLFormatter 和 盘古分词对象Semgent
            PanGu.HighLight.Highlighter highlighter = new PanGu.HighLight.Highlighter(simpleHTMLFormatter, new PanGu.Segment());

            //设置每个摘要段的字符数
            highlighter.FragmentSize = 1000;

            //获取最匹配的摘要段
            return highlighter.GetBestFragment(keyword, content);
        }

        class Info
        {
            public string ID { get; set; }

            public string Title { get; set; }

            public string Body { get; set; }
        }
    }
}
