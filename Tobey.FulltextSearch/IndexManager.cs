using Lucene.Net.Analysis;
using Lucene.Net.Documents;
using Lucene.Net.Index;
using Lucene.Net.Search;
using Lucene.Net.Store;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tobey.FulltextSearch
{
    public class IndexManager
    {
        private static readonly string _IndexDir = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, System.Configuration.ConfigurationManager.AppSettings["IndexStoreDir"]);
        private static readonly IndexManager _Instance = new IndexManager();

        private static IndexWriter _IndexWriter;
        private static FSDirectory _FSDirectory;

        private IndexManager()
        {
        }

        public static IndexManager Instance()
        {
            if (_IndexWriter != null)
            {
                _IndexWriter.Dispose();
            }

            if (_FSDirectory != null)
            {
                _FSDirectory.Dispose();
            }

            return _Instance;
        }

        public void Add(List<RecordInfo> records)
        {
            if (_FSDirectory == null)
            {
                _FSDirectory = FSDirectory.Open(new DirectoryInfo(_IndexDir));
            }
            if (_IndexWriter == null)
            {
                Analyzer analyzer = new JiebaAnalyzer();
                _IndexWriter = new IndexWriter(_FSDirectory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED);
            }
            lock (_IndexDir)
            {
                foreach (RecordInfo record in records)
                {
                    Document doc = AnalyzerDocument(record);
                    _IndexWriter.AddDocument(doc);
                }
                _IndexWriter.Commit();
                _IndexWriter.Optimize();

                _IndexWriter.Dispose();
                _FSDirectory.Dispose();
            }
        }

        public void Update(RecordInfo record)
        {
            if (_FSDirectory == null)
            {
                _FSDirectory = FSDirectory.Open(new DirectoryInfo(_IndexDir));
            }
            if (_IndexWriter == null)
            {
                Analyzer analyzer = new JiebaAnalyzer();
                _IndexWriter = new IndexWriter(_FSDirectory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED);
            }
            lock (_IndexDir)
            {
                BooleanQuery query = new BooleanQuery()
                {
                    {new TermQuery(new Term("ModuleType", record.ModuleType)), Occur.MUST},
                    {new TermQuery(new Term("TableName", record.TableName)), Occur.MUST},
                    {new TermQuery(new Term("RowId", record.RowId.ToString())), Occur.MUST}
                };
                _IndexWriter.DeleteDocuments(query);

                Document document = AnalyzerDocument(record);
                _IndexWriter.AddDocument(document);
                _IndexWriter.Commit();
                _IndexWriter.Optimize();

                _IndexWriter.Dispose();
                _FSDirectory.Dispose();
            }
        }

        public void Delete(string moduleType, string tableName, string rowID)
        {
            if (_FSDirectory == null)
            {
                _FSDirectory = FSDirectory.Open(new DirectoryInfo(_IndexDir));
            }
            if (_IndexWriter == null)
            {
                Analyzer analyzer = new JiebaAnalyzer();
                _IndexWriter = new IndexWriter(_FSDirectory, analyzer, IndexWriter.MaxFieldLength.UNLIMITED);
            }
            lock (_IndexDir)
            {
                BooleanQuery query = new BooleanQuery()
                {
                    {new TermQuery(new Term("ModuleType", moduleType)), Occur.MUST},
                    {new TermQuery(new Term("TableName", tableName)), Occur.MUST},
                    {new TermQuery(new Term("RowId", rowID)), Occur.MUST}
                };
                _IndexWriter.DeleteDocuments(query);
                _IndexWriter.Commit();
                _IndexWriter.Optimize();

                _IndexWriter.Dispose();
                _FSDirectory.Dispose();
            }
        }

        private Document AnalyzerDocument(RecordInfo record)
        {
            Document doc = new Document();
            doc.Add(new Field("ModuleType", record.ModuleType, Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("TableName", record.TableName, Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("RowId", record.RowId, Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("CollectTime", record.CollectTime.ToString("yyyy-MM-dd HH:mm:ss"), Field.Store.YES, Field.Index.NOT_ANALYZED));
            doc.Add(new Field("Title", record.Title, Field.Store.YES, Field.Index.ANALYZED));
            doc.Add(new Field("Body", record.Body, Field.Store.YES, Field.Index.ANALYZED));

            if (record.StringTags != null && record.StringTags.Any())
            {
                foreach (var tag in record.StringTags)
                {
                    doc.Add(new Field(tag.Name, tag.Value, tag.Store, tag.Index));
                }
            }

            if (record.FloatTags != null && record.FloatTags.Any())
            {
                foreach (var tag in record.FloatTags)
                {
                    NumericField field = new NumericField(tag.Name, tag.Store, tag.Index != Field.Index.NO);
                    field = field.SetFloatValue(tag.Value);
                    doc.Add(field);
                }
            }

            return doc;
        }
    }
}
