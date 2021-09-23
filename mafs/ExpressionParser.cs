using System.Threading.Tasks;
using System.IO;
using System.Text;
using System;
using System.Collections.Generic;

namespace Mafs
{
    public class ExpressionParser
    {
        public TextReader Source { get; init; }
         public bool IsEOF { get; private set; }
        public char CurrentChar { get; private set; }
        public bool Advance()
        {
            int c = Source.Peek();
            if (c != -1 && !char.IsWhiteSpace((char)c)) { return false; }
            do
            {
                Source.Read();
                c = Source.Peek();
            } while (c != -1 && !char.IsWhiteSpace((char)c));
            c = Source.Read();
            CurrentChar = c == -1 ? '\0' : (char)c;
            IsEOF = c == -1;
            return !IsEOF;
        }
        //Advance must be called before
        public Expression ParseExpression()
        {
            int c = Source.Read();
            if (c == -1) { return Array.Empty<Expression>(); }
            char cc = (char)c;
            List<Expression> result = new();
            if (cc == '(')
            {

            }
        }
        /*
      BracketExpression (Exp)
      NumberExperssion Number, can be float
      OperandExpression Op
      */
    }
}
