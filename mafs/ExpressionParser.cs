using System.Threading.Tasks;
using System.IO;
using System.Text;
using System;
using System.Collections.Generic;

namespace Mafs
{
    public class ExpressionParser
    {
        public TextReader Source { get; }
        public bool IsEOF { get; private set; }
        public char CurrentChar { get; private set; }
        public bool Advance(bool ignoreWhiteSpaces = true)
        {
            int c = Source.Read();
            if (ignoreWhiteSpaces)
            {
                while (c != -1 && char.IsWhiteSpace((char)c))
                {
                    c = Source.Read();
                }
            }
            CurrentChar = c == -1 ? '\0' : (char)c;
            IsEOF = c == -1;
            return !IsEOF;
        }
        public ContainerExpression Parse()
        {
            if (IsEOF) { return new ContainerExpression(); }
            ContainerExpression res = new();
            res.Initialize(this);
            return res;
        }
        //Advance must be called before
        public Expression? ParseExpression()
        {
            if (char.IsWhiteSpace(CurrentChar)) { Advance(); }
            if (IsEOF) { return null; }

            Expression res;
            if (CurrentChar == '(') { res = new BracketExpression(); }
            else if (CurrentChar == '+' || CurrentChar == '-' || CurrentChar == '*' || CurrentChar == '/')
            {
                res = new OperationExpression();
            }
            else if (char.IsNumber(CurrentChar)) { res = new NumberExpression(); }
            else
            {
                throw new InvalidExpressionException($"'{CurrentChar}' can't be parsed");
            }
            res.Initialize(this);
            return res;
        }
        /*
      BracketExpression (Exp)
      NumberExperssion Number, can be float
      OperandExpression Op
      */
        public ExpressionParser(TextReader src)
        {
            Source = src;
            IsEOF = src.Peek() == -1;
            CurrentChar = IsEOF ? '\0' : (char)src.Read();
        }
    }
}
