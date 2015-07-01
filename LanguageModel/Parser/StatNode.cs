using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LanguageModel
{   
    internal class StatNode
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

        public SyntaxNode Parse(IEnumerator<Token> tokenEnumerator)
        {
            switch (tokenEnumerator.Current.Text)
            {
                case "break":
                    //TODO: Implement
                    break;
                case "goto":
                    //TODO: Implement
                    break;
                case "do":
                    //TODO: Implement
                    break;
                case "while":
                    //TODO: Implement
                    break;
                case "repeat":
                    //TODO: Implement
                    break;
                case "if":
                    break;
                case "for":
                    //TODO: Implement
                    break;
                case "function":
                    //TODO: Implement
                    break;
                case "local":
                    //TODO: Implement
                    break;
                default:
                    //TODO: Implement
                    //not the beginning of a statement?
                    break;
            }
            return null; //TODO: change
        }
    }
}
