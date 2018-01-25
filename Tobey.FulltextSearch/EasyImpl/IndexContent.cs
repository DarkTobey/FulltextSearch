using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tobey.FulltextSearch.EasyImpl
{
    public class IndexContent : BaseIndexContent
    {
        public IndexContent()
        {
            Tag1 = new IndexContentStringValue();
            Tag2 = new IndexContentStringValue();
            Tag3 = new IndexContentStringValue();
            Tag4 = new IndexContentStringValue();
            Tag5 = new IndexContentStringValue();
            Tag6 = new IndexContentStringValue();
            Tag7 = new IndexContentStringValue();
            Tag8 = new IndexContentStringValue();
            FloatTag9 = new IndexContentFloatValue();
            FloatTag10 = new IndexContentFloatValue();
        }

        /// <summary>
        /// 预留1
        /// </summary>
        public IndexContentStringValue Tag1 { get; set; }

        /// <summary>
        /// 预留2
        /// </summary>
        public IndexContentStringValue Tag2 { get; set; }

        /// <summary>
        /// 预留3
        /// </summary>
        public IndexContentStringValue Tag3 { get; set; }

        /// <summary>
        /// 预留4
        /// </summary>
        public IndexContentStringValue Tag4 { get; set; }

        /// <summary>
        /// 预留5
        /// </summary>
        public IndexContentStringValue Tag5 { get; set; }

        /// <summary>
        /// 预留6
        /// </summary>
        public IndexContentStringValue Tag6 { get; set; }

        /// <summary>
        /// 预留7
        /// </summary>
        public IndexContentStringValue Tag7 { get; set; }

        /// <summary>
        /// 预留8
        /// </summary>
        public IndexContentStringValue Tag8 { get; set; }

        /// <summary>
        /// 预留9(数值型)
        /// </summary>
        public IndexContentFloatValue FloatTag9 { get; set; }

        /// <summary>
        /// 预留10(数值型)
        /// </summary>
        public IndexContentFloatValue FloatTag10 { get; set; }
    }

    /// <summary>
    /// 索引值及方式
    /// </summary>
    public class IndexContentStringValue
    {
        public IndexContentStringValue()
        {
            Value = "";
            Store = true;
            Index = IndexEnum.NotIndex;
        }

        /// <summary>
        /// 字符值
        /// </summary>
        public string Value { get; set; }

        /// <summary>
        /// 是否存储
        /// </summary>
        public bool Store { get; set; }

        /// <summary>
        /// 索引&分词方式
        /// </summary>
        public IndexEnum Index { get; set; }
    }

    /// <summary>
    /// 索引值及方式
    /// </summary>
    public class IndexContentFloatValue
    {
        public IndexContentFloatValue()
        {
            Value = 0;
            Store = true;
            Index = IndexEnum.NotIndex;
        }

        /// <summary>
        /// 字符值
        /// </summary>
        public float Value { get; set; }

        /// <summary>
        /// 是否存储
        /// </summary>
        public bool Store { get; set; }

        /// <summary>
        /// 是否索引且分词
        /// </summary>
        public IndexEnum Index { get; set; }
    }

}
