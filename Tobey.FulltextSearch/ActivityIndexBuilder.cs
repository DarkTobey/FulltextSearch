using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tobey.FulltextSearch
{
    /// <summary>
    /// 活动数据索引创建器
    /// </summary>
    public class ActivityIndexBuilder : IIndexBuilder<ActivityIndexContent>
    {
        public const string MODULETYPE = "活动";

        /// <summary>
        /// 创建索引
        /// </summary>
        /// <param name="activityIndexContents"></param>
        public void BuildIndex(List<ActivityIndexContent> activityIndexContents)
        {
            var indexManager = new IndexManager();
            var indexContents = activityIndexContents.Select(activityIndexContent => new IndexContent
            {
                ModuleType = MODULETYPE,
                TableName = activityIndexContent.TableName,
                RowId = activityIndexContent.RowId,
                Title = activityIndexContent.Title,
                IndexTextContent = activityIndexContent.InformationContent,
                CollectTime = activityIndexContent.CollectTime,
                Tag1 = new IndexContentStringValue
                {
                    // 活动分类
                    Value = activityIndexContent.GetActivityTypeStr()
                },
                Tag2 = new IndexContentStringValue
                {
                    // 源链接
                    Value = activityIndexContent.Url
                },
                Tag3 = new IndexContentStringValue
                {
                    // 采集源名称
                    Value = activityIndexContent.SourceName,
                    Index = IndexEnum.UseAnalyzerIndex
                },
                Tag4 = new IndexContentStringValue
                {
                    // 采集源官方热线
                    Value = activityIndexContent.SourceOfficialHotline
                },
                Tag5 = new IndexContentStringValue
                {
                    // 采集源主站地址
                    Value = activityIndexContent.SourceUrl
                },
                Tag6 = new IndexContentStringValue()
                {
                    // 采集活动举办城市ID
                    Value = activityIndexContent.CityId.ToString().ToLower(),
                    Index = IndexEnum.NotUseAnalyzerButIndex
                },
                Tag7 = new IndexContentStringValue()
                {
                    // 采集活动举办地址
                    Value = string.IsNullOrEmpty(activityIndexContent.Address) ? "" : activityIndexContent.Address
                },
                Tag8 = new IndexContentStringValue()
                {
                    // 采集活动举办时间
                    Value = activityIndexContent.ActivityDate.HasValue ? activityIndexContent.ActivityDate.Value.ToString("yyyy年MM月dd日") : ""
                }
            }).ToList();
            indexManager.BuildIndex(indexContents);
        }

        /// <summary>
        /// 删除索引
        /// </summary>
        /// <param name="tableName"></param>
        /// <param name="rowID"></param>
        public void DeleteIndex(string tableName, string rowID)
        {
            var indexManager = new IndexManager();
            indexManager.DeleteIndex(MODULETYPE, tableName, rowID);
        }

        /// <summary>
        /// 更新索引
        /// </summary>
        /// <param name="indexContents"></param>
        public void UpdateIndex(List<ActivityIndexContent> indexContents)
        {
            foreach (var indexContent in indexContents)
            {
                if (indexContent.RowId != Guid.Empty &&
                    indexContent.TableName != null)
                {
                    // 删除索引
                    this.DeleteIndex(indexContent.TableName,
                        indexContent.RowId.ToString().ToLower());
                }
            }

            // 添加索引
            this.BuildIndex(indexContents);
        }
    }
}
