using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tobey.FulltextSearch
{
    public class PagedIndexSearchResult<TIndexSearchResultItem> where TIndexSearchResultItem : IndexSearchResultItem
    {
        internal int PageIndex;

        public double Elapsed { get; internal set; }
        public int PageSize { get; internal set; }
        public int TotalRecords { get; internal set; }

        internal void Add<TIndexSearchResultItem>(TIndexSearchResultItem indexSearchResultItem) where TIndexSearchResultItem : IndexSearchResultItem
        {
            throw new NotImplementedException();
        }
    }
}
