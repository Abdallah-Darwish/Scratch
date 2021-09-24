using System.Threading.Tasks;
using System.IO;
using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Mafs
{
    public partial class ContainerExpression : Expression
    {
        protected virtual bool IsEnd(ExpressionParser p) => p.IsEOF;
        protected virtual bool IsStart(ExpressionParser p) => true;
        protected virtual void HandleStart(ExpressionParser p) { }
        protected virtual void HandleEnd(ExpressionParser p) { }
        protected virtual Task WriteStart(TextWriter w) => Task.CompletedTask;
        protected virtual Task WriteEnd(TextWriter w) => Task.CompletedTask;

        private readonly List<Expression> _subExpressions = new();
        public IReadOnlyList<Expression> SubExpressions => _subExpressions;
        public override bool CanBeFirst => true;

        public override bool IsValid
        {
            get
            {
                if (_subExpressions.Count == 0) { return true; }
                if (!_subExpressions[0].CanBeFirst) { return false; }
                for (int i = 0; i < _subExpressions.Count - 1; i++)
                {
                    if (!_subExpressions[i].CanBeFollowedBy(_subExpressions[i + 1])) { return false; }
                }
                return _subExpressions.Last().CanBeFollowedBy(null);
            }
        }

        public override async Task Write(TextWriter w)
        {
            await WriteStart(w);
            foreach (var se in SubExpressions)
            {
                await se.Write(w);
            }
            await WriteEnd(w);
        }
        /// <param name="ex">Null means this is the final expression in group</param>
        internal override bool CanBeFollowedBy(Expression? ex)
        {
            return ex switch
            {
                null or OperationExpression or NumberExpression => true,
                _ => false,
            };
        }
        internal override void Initialize(ExpressionParser p)
        {
            if (!IsStart(p))
            {
                throw new InvalidOperationException($"Expression is not valid for container of type {this.GetType().Name}");
            }
            HandleStart(p);

            Expression? innerExpression;
            while (!p.IsEOF && !IsEnd(p))
            {
                innerExpression = p.ParseExpression();
                if (innerExpression != null)
                {
                    _subExpressions.Add(innerExpression);
                }
            }
            if (!IsEnd(p))
            {
                throw new InvalidExpressionException("Expression is not valid container expression");
            }
            HandleEnd(p);
        }
    }
}
