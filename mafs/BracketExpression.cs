using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Mafs
{
    public class BracketExpression : Expression
    {
        private readonly List<Expression> _subExpressions = new();
        public IReadOnlyList<Expression> SubExpressions => _subExpressions;
        public override bool CanBeFirst => true;

        public override bool IsValid
        {
            get
            {
                if (!_subExpressions[0].CanBeFirst) { return false; }
                for (int i = 0; i < _subExpressions.Count - 1; i++)
                {
                    if (!_subExpressions[i].CanBeFollowedBy(_subExpressions[i + 1])) { return false; }
                }
                return true;
            }
        }

        public override async Task Write(TextWriter w)
        {
            await w.WriteAsync('(');
            foreach (var se in SubExpressions)
            {
                await se.Write(w);
            }
            await w.WriteAsync(')');
        }
        /// <param name="ex">Null means this is the final expression in group</param>
        internal override bool CanBeFollowedBy(Expression? ex)
        {
            switch (ex)
            {
                case null:
                case OperationExpression:
                case NumberExpression:
                    return true;
                case BracketExpression:
                default:
                    return false;
            }
        }

        internal override void Initialize(ExpressionParser p)
        {
            if (p.CurrentChar != '(')
            {
                throw new InvalidOperationException($"{nameof(p.CurrentChar)} must be '('.");
            }
            p.Advance();
            while (p.CurrentChar != ')')
            {
                _subExpressions.Add(p.ParseExpression());
            }

            if (p.IsEOF)
            {
                throw new InvalidExpressionException("Not matching pair of brackets.");
            }
            if (_subExpressions.Count == 0)
            {
                throw new InvalidExpressionException("Empty Brackets.");
            }
            p.Advance();
        }
    }
}