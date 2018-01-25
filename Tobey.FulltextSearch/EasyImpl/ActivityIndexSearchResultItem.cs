using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tobey.FulltextSearch.EasyImpl
{
    public class ActivityIndexSearchResultItem : IndexSearchResultItem
    {
        /// <summary>
        /// 活动类别
        /// </summary>
        public string ActivityTypes { get; set; }

        /// <summary>
        /// 城市ID
        /// </summary>
        public Guid CityId { get; set; }

        /// <summary>
        /// 活动地址
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// 活动日期
        /// </summary>
        public string ActivityDate { get; set; }

        /// <summary>
        /// 源链接
        /// </summary>
        public string Url { get; set; }

        /// <summary>
        /// 采集源名称
        /// </summary>
        public string SourceName { get; set; }

        /// <summary>
        /// 采集源主站地址
        /// </summary>
        public string SourceUrl { get; set; }

        /// <summary>
        /// 采集源官方热线
        /// </summary>
        public string SourceOfficialHotline { get; set; }
    }
}
