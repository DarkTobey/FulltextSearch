using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tobey.FulltextSearch
{
    /// <summary>
    /// 全库搜索结果单项内容
    /// </summary>
    public class IndexSearchResultItem
    {
        /// <summary>
        /// 内容索引
        /// </summary>
        public int DocIndex { get; set; }

        /// <summary>
        /// 模块类别
        /// </summary>
        public string ModuleType { get; set; }

        /// <summary>
        /// 表名
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 行号
        /// </summary>
        public Guid RowId { get; set; }

        /// <summary>
        /// 文档标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 文档内容片段
        /// </summary>
        public string Content { get; set; }

        /// <summary>
        /// 创建时间
        /// </summary>
        public DateTime? CollectTime { get; set; }
    }
}
