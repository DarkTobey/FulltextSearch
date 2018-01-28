using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tobey.FulltextSearch
{
    public class RecordInfo
    {
        /// <summary>
        /// 所属系统模块
        /// </summary>
        public string ModuleType { get; set; }

        /// <summary>
        /// 记录所在的表名
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 记录的主键ID
        /// </summary>
        public string RowId { get; set; }

        /// 检索标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///检索文本
        /// </summary>
        public string Body { get; set; }

        /// <summary>
        /// 记录的创建时间
        /// </summary>
        public DateTime CollectTime { get; set; }

        /// <summary>
        /// 其他字段
        /// </summary>
        public IEnumerable<RecordStringTag> StringTags { get; set; }

        /// <summary>
        /// 其他字段
        /// </summary>
        public IEnumerable<RecordFloatTag> FloatTags { get; set; }
    }

    public class RecordStringTag
    {
        public RecordStringTag()
        {
            Name = string.Empty;
            Value = string.Empty;
            Store = Lucene.Net.Documents.Field.Store.NO;
            Index = Lucene.Net.Documents.Field.Index.NO;
        }

        /// <summary>
        /// 名称
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 字符值
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 是否存储
        /// </summary>
        public Lucene.Net.Documents.Field.Store Store { get; set; }

        /// <summary>
        /// 索引&分词方式
        /// </summary>
        public Lucene.Net.Documents.Field.Index Index { get; set; }
    }

    public class RecordFloatTag
    {
        public RecordFloatTag()
        {
            Name = string.Empty;
            Value = 0;
            Store = Lucene.Net.Documents.Field.Store.NO;
            Index = Lucene.Net.Documents.Field.Index.NO;
        }

        public string Name { get; set; }

        /// <summary>
        /// 浮点值
        /// </summary>
        public float Value { get; set; }

        /// <summary>
        /// 是否存储
        /// </summary>
        public Lucene.Net.Documents.Field.Store Store { get; set; }

        /// <summary>
        /// 索引&分词方式
        /// </summary>
        public Lucene.Net.Documents.Field.Index Index { get; set; }
    }
}
