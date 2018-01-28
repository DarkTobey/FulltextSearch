using PanGu.HighLight;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tobey.FulltextSearch
{
    public class SearchResult
    {
        public int Start { get; set; }

        public int Length { get; set; }

        public int Total { get; set; }

        public IEnumerable<RecordInfo> Data { get; set; }
    }

    public class ResultOrderBy
    {
        public string FieldName { get; set; }

        public bool IsDesc { get; set; }
    }

    public class ResultFormatter
    {
        public int FragmentSize { get; set; }

        public SimpleHTMLFormatter HTMLFormatter { get; set; }
    }
}
