using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tobey.FulltextSearch.EasyImpl
{
    public class IndexManager
    {
        /// <summary>
        /// 索引存储目录
        /// </summary>
        public static readonly string IndexStorePath = ConfigurationManager.AppSettings["IndexStorePath"];
        private IndexWriter indexWriter;
        private FSDirectory entityDirectory;

        public IndexManager()
        {
            if (entityDirectory != null)
            {
                entityDirectory.Dispose();
            }
            if (indexWriter != null)
            {
                indexWriter.Dispose();
            }
        }

        /// <summary>
        /// 对内容新增索引
        /// </summary>
        public void BuildIndex(List<IndexContent> indexContents)
        {
            try
            {
                if (entityDirectory == null)
                {
                    entityDirectory = FSDirectory.Open(new DirectoryInfo(IndexStorePath));
                }
                if (indexWriter == null)
                {
                    Analyzer analyzer = new JiebaForLuceneAnalyzer();
                    indexWriter = new IndexWriter(entityDirectory, analyzer, IndexWriter.MaxFieldLength.LIMITED);
                }
                lock (IndexStorePath)
                {
                    foreach (var indexContent in indexContents)
                    {
                        var doc = GetDocument(indexContent);
                        indexWriter.AddDocument(doc);
                    }
                    indexWriter.Commit();
                    indexWriter.Optimize();
                    indexWriter.Dispose();
                }
            }
            catch (Exception exception)
            {
                //LogUtils.ErrorLog(exception);
            }
            finally
            {
                if (entityDirectory != null)
                {
                    entityDirectory.Dispose();
                }
                if (indexWriter != null)
                {
                    indexWriter.Dispose();
                }
            }
        }

        /// <summary>
        /// 删除索引
        /// </summary>
        /// <param name="moduleType"></param>
        /// <param name="tableName">可空</param>
        /// <param name="rowID"></param>
        public void DeleteIndex(string moduleType, string tableName, string rowID)
        {
            try
            {
                if (entityDirectory == null)
                {
                    entityDirectory = FSDirectory.Open(new DirectoryInfo(IndexStorePath));
                }
                if (indexWriter == null)
                {
                    Analyzer analyzer = new JiebaForLuceneAnalyzer();
                    indexWriter = new IndexWriter(entityDirectory, analyzer, IndexWriter.MaxFieldLength.LIMITED);
                }
                lock (IndexStorePath)
                {
                    var query = new BooleanQuery
                      {
                          {new TermQuery(new Term("ModuleType", moduleType)), Occur.MUST},
                          {new TermQuery(new Term("RowId", rowID)), Occur.MUST}
                      };
                    if (!string.IsNullOrEmpty(tableName))
                    {
                        query.Add(new TermQuery(new Term("TableName", tableName)), Occur.MUST);
                    }

                    indexWriter.DeleteDocuments(query);
                    indexWriter.Commit();
                    indexWriter.Optimize();
                    indexWriter.Dispose();
                }
            }
            catch (Exception exception)
            {
                //LogUtils.ErrorLog(exception);
            }
            finally
            {
                if (entityDirectory != null)
                {
                    entityDirectory.Dispose();
                }
                if (indexWriter != null)
                {
                    indexWriter.Dispose();
                }
            }
        }

        /// <summary>
        /// 更新索引
        /// </summary>
        /// <param name="indexContent"></param>
        public void UpdateIndex(IndexContent indexContent)
        {
            try
            {
                if (entityDirectory == null)
                {
                    entityDirectory = FSDirectory.Open(new DirectoryInfo(IndexStorePath));
                }
                if (indexWriter == null)
                {
                    Analyzer analyzer = new JiebaForLuceneAnalyzer();
                    indexWriter = new IndexWriter(entityDirectory, analyzer, IndexWriter.MaxFieldLength.LIMITED);
                }
                lock (IndexStorePath)
                {
                    var query = new BooleanQuery
                      {
                          {new TermQuery(new Term("ModuleType", indexContent.ModuleType)), Occur.MUST},
                          {new TermQuery(new Term("RowId", indexContent.RowId.ToString())), Occur.MUST}
                      };
                    if (!string.IsNullOrEmpty(indexContent.TableName))
                    {
                        query.Add(new TermQuery(new Term("TableName", indexContent.TableName)), Occur.MUST);
                    }

                    indexWriter.DeleteDocuments(query);

                    var document = GetDocument(indexContent);
                    indexWriter.AddDocument(document);

                    indexWriter.Commit();
                    indexWriter.Optimize();
                    indexWriter.Dispose();
                }
            }
            catch (Exception exception)
            {
                //LogUtils.ErrorLog(exception);
            }
            finally
            {
                if (entityDirectory != null)
                {
                    entityDirectory.Dispose();
                }
                if (indexWriter != null)
                {
                    indexWriter.Dispose();
                }
            }
        }

        private Document GetDocument(IndexContent indexContent)
        {
            var doc = new Document();
            doc.Add(new Field("ModuleType", indexContent.ModuleType, Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("TableName", indexContent.TableName, Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("RowId", indexContent.RowId.ToString().ToLower(), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("Title", indexContent.Title, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("IndexTextContent", ReplaceIndexSensitiveWords(indexContent.IndexTextContent), Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("CollectTime", indexContent.CollectTime.ToString("yyyy-MM-dd HH:mm:ss"), Field.Store.YES, Field.Index.NO));

            // 预留
            doc.Add(new Field("Tag1", indexContent.Tag1.Value, GetStoreEnum(indexContent.Tag1.Store)
                , GetIndexEnum(indexContent.Tag1.Index)));
            doc.Add(new Field("Tag2", indexContent.Tag2.Value, GetStoreEnum(indexContent.Tag2.Store)
                , GetIndexEnum(indexContent.Tag2.Index)));
            doc.Add(new Field("Tag3", indexContent.Tag3.Value, GetStoreEnum(indexContent.Tag3.Store)
                , GetIndexEnum(indexContent.Tag3.Index)));
            doc.Add(new Field("Tag4", indexContent.Tag4.Value, GetStoreEnum(indexContent.Tag4.Store)
                , GetIndexEnum(indexContent.Tag4.Index)));
            doc.Add(new Field("Tag5", indexContent.Tag5.Value, GetStoreEnum(indexContent.Tag5.Store)
                , GetIndexEnum(indexContent.Tag5.Index)));
            doc.Add(new Field("Tag6", indexContent.Tag6.Value, GetStoreEnum(indexContent.Tag6.Store)
                , GetIndexEnum(indexContent.Tag6.Index)));
            doc.Add(new Field("Tag7", indexContent.Tag7.Value, GetStoreEnum(indexContent.Tag7.Store)
                , GetIndexEnum(indexContent.Tag7.Index)));
            doc.Add(new Field("Tag8", indexContent.Tag8.Value, GetStoreEnum(indexContent.Tag8.Store)
                , GetIndexEnum(indexContent.Tag8.Index)));
            var field = new NumericField("FloatTag9", GetStoreEnum(indexContent.FloatTag9.Store),
                indexContent.FloatTag9.Index != IndexEnum.NotIndex);
            field = field.SetFloatValue(indexContent.FloatTag9.Value);
            doc.Add(field);
            field = new NumericField("FloatTag10", GetStoreEnum(indexContent.FloatTag10.Store),
                indexContent.FloatTag10.Index != IndexEnum.NotIndex);
            field = field.SetFloatValue(indexContent.FloatTag10.Value);
            doc.Add(field);
            return doc;
        }

        /// <summary>
        /// 权益方法，临时使用
        /// 去除文本中非索引文本
        /// </summary>
        /// <param name="str"></param>
        /// <returns></returns>
        private string ReplaceIndexSensitiveWords(string str)
        {
            for (var i = 0; i < 3; i++)
            {
                str = str.Replace(" ", "");
                str = str.Replace("　", "").Replace("\n", "");
            }
            return str;
        }

        private Field.Index GetIndexEnum(IndexEnum index)
        {
            switch (index)
            {
                case IndexEnum.NotIndex:
                    return Field.Index.NO;
                case IndexEnum.NotUseAnalyzerButIndex:
                    return Field.Index.NOT_ANALYZED;
                case IndexEnum.UseAnalyzerIndex:
                    return Field.Index.ANALYZED;
                default:
                    return Field.Index.NO;
            }
        }

        private Field.Store GetStoreEnum(bool store)
        {
            return store ? Field.Store.YES : Field.Store.NO;
        }
    }

    public enum IndexEnum
    {
        NotIndex,
        NotUseAnalyzerButIndex,
        UseAnalyzerIndex,
    }

}
