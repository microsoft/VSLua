using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageModel.Syntax
{
    struct ElseBlock
    {
        private Syntax.Keyword elseKeyword;
        private Syntax.Block block;
    }

    struct ElseIfBlock
    {
        private Syntax.Keyword elseIfKeyword;
        private Syntax.Expression exp;
        private Syntax.Keyword thenKeyword;
        private Syntax.Block elseIfBlock;
    }

    internal class IfNode : StatNode
    {

        private Syntax.Keyword ifKeyword;
        //TODO: just keep reading till then keyword and pass all those tokens to exp
        private Syntax.Expression exp; //TODO: finish defining this class
        private Syntax.Keyword thenKeyword;
        private Syntax.Block ifBlock;
        private List<ElseIfBlock> elseIfList;
        private Syntax.Keyword elseKeyword;
        private ElseBlock elseBlock;
        private Syntax.Keyword endKeyword;

        public IfNode():base(StatKind.If)
        {
        }
    }
}
