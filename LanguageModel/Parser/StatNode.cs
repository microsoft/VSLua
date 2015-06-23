using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageModel
{   
    internal class StatNode : SyntaxNode
    {
        public enum StatKind
        {
            Semicolon,
            Assignment,
            Label,
            Break,
            Goto,
            Do,
            While,
            Repeat,
            If,
            For,
            FunctionDeclaration,        //function funcname funcbody
            LocalFunctionDeclaration,
            LocalAssignment
        }

        private StatKind kind;

        public StatNode(StatKind kind) : base()
        {
            this.kind = kind;
        }

        public StatNode(int start, int end, StatKind kind ) : base(start,end)
        {
            this.kind = kind;
        }        
    }
}
