using JiebaNet.Segmenter;
using Lucene.Net.Analysis.Tokenattributes;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tobey.FulltextSearch
{
    public class JiebaTokenizer : Lucene.Net.Analysis.Tokenizer
    {
        private readonly ITermAttribute termAtt;
        private readonly IOffsetAttribute offsetAtt;
        private readonly ITypeAttribute typeAtt;

        private int position = -1;
        private readonly JiebaSegmenter segmenter;
        private readonly List<JiebaNet.Segmenter.Token> tokens;

        public JiebaTokenizer(JiebaSegmenter seg, TextReader input)
            : this(seg, input.ReadToEnd())
        {
        }

        public JiebaTokenizer(JiebaSegmenter seg, string input)
        {
            segmenter = seg;
            termAtt = AddAttribute<ITermAttribute>();
            offsetAtt = AddAttribute<IOffsetAttribute>();
            typeAtt = AddAttribute<ITypeAttribute>();
            tokens = segmenter.Tokenize(input, TokenizerMode.Search).ToList();
        }

        public override bool IncrementToken()
        {
            ClearAttributes();
            position++;
            if (position < tokens.Count)
            {
                var token = tokens[position];
                termAtt.SetTermBuffer(token.Word);
                offsetAtt.SetOffset(token.StartIndex, token.EndIndex);
                typeAtt.Type = "Jieba";
                return true;
            }

            End();
            return false;
        }

        public IEnumerable<JiebaNet.Segmenter.Token> Tokenize(string text, TokenizerMode mode = TokenizerMode.Search)
        {
            return segmenter.Tokenize(text, mode);
        }
    }
}
