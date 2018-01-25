using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tobey.FulltextSearch.EasyImpl
{
    public class BaseIndexContent
    {
        /// <summary>
        /// （对应DB表名）
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// （对应DB主键）
        /// </summary>
        public Guid RowId { get; set; }

        /// <summary>
        /// （对应DB数据创建时间）
        /// </summary>
        public DateTime CollectTime { get; set; }

        /// <summary>
        /// （所属系统模块）
        /// </summary>
        public string ModuleType { get; set; }

        /// <summary>
        /// （检索标题）
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// （检索文本）
        /// </summary>
        public string IndexTextContent { get; set; }
    }
}
