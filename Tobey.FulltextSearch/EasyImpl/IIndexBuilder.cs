using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tobey.FulltextSearch.EasyImpl
{
    /// <summary>
    /// 各子模块内容索引构建器接口
    /// </summary>
    public interface IIndexBuilder<TIndexContent>
    {
        /// <summary>
        /// 将内容集合建立索引
        /// </summary>
        void BuildIndex(List<TIndexContent> indexContents);

        /// <summary>
        /// 删除索引
        /// </summary>
        void DeleteIndex(string tableName, string rowID);

        /// <summary>
        /// 更新索引
        /// </summary>
        /// <param name="indexContents"></param>
        void UpdateIndex(List<TIndexContent> indexContents);
    }
}
