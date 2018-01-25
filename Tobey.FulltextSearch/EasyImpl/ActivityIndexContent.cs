using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tobey.FulltextSearch.EasyImpl
{
    public class ActivityIndexContent
    {
        /// <summary>
        /// 关联表格名
        /// </summary>
        public string TableName { get; set; }

        /// <summary>
        /// 关联表格行ID
        /// </summary>
        public Guid RowId { get; set; }

        /// <summary>
        /// 采集分析时间
        /// </summary>
        public DateTime CollectTime { get; set; }

        /// <summary>
        /// 标题
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        /// 详情
        /// </summary>
        public string InformationContent { get; set; }

        /// <summary>
        /// 活动类别
        /// </summary>
        public List<ActivityType> ActivityTypes { get; set; }

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
        public DateTime? ActivityDate { get; set; }

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

        /// <summary>
        /// 获取类型
        /// </summary>
        /// <returns></returns>
        public string GetActivityTypeStr()
        {
            throw new NotImplementedException();
        }
    }
}
