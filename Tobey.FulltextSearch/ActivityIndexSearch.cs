using Lucene.Net.Documents;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tobey.FulltextSearch
{
    public class ActivityIndexSearch : BaseIndexSearch<ActivityIndexSearchResultItem>
    {
        public ActivityIndexSearch()
            : base(new[] { "IndexTextContent", "Title" })
        {
        }

        protected override ActivityIndexSearchResultItem CreateIndexSearchResultItem()
        {
            return new ActivityIndexSearchResultItem();
        }

        protected override void ModifyIndexSearchResultItem(ref ActivityIndexSearchResultItem indexSearchResultItem, string content,
            int docIndex, Document doc)
        {
            indexSearchResultItem.ActivityTypes = doc.Get("Tag1");
            indexSearchResultItem.Url = doc.Get("Tag2");
            indexSearchResultItem.SourceName = doc.Get("Tag3");
            indexSearchResultItem.SourceOfficialHotline = doc.Get("Tag4");
            indexSearchResultItem.SourceUrl = doc.Get("Tag5");
            indexSearchResultItem.CityId = new Guid(doc.Get("Tag6"));
            indexSearchResultItem.Address = doc.Get("Tag7");
            indexSearchResultItem.ActivityDate = doc.Get("Tag8");
        }

        protected override void ModifySearchFilter(ref Dictionary<string, string> filter)
        {
            filter.Add("ModuleType", "活动");
        }
    }
}
