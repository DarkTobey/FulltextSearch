using Lucene.Net.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tobey.FulltextSearch
{
      public class IndexSearch : BaseIndexSearch<IndexSearchResultItem>
      {
          public IndexSearch()
              : base(new[] { "IndexTextContent", "Title" })
          {
          }
  
          protected override IndexSearchResultItem CreateIndexSearchResultItem()
          {
              return new IndexSearchResultItem();
          }
  
          protected override void ModifyIndexSearchResultItem(ref IndexSearchResultItem indexSearchResultItem, string content,
              int docIndex, Document doc)
          {
              //不做修改
          }
  
          protected override void ModifySearchFilter(ref Dictionary<string, string> filter)
          {
              //不做筛选条件修改
          }
      }
}
