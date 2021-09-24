using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Mafs
{
    public class BracketExpression : ContainerExpression
    {
        protected override bool IsStart(ExpressionParser p) => p.CurrentChar == '(';
        protected override bool IsEnd(ExpressionParser p) => p.CurrentChar == ')';
        protected override void HandleStart(ExpressionParser p) => p.Advance();
        protected override void HandleEnd(ExpressionParser p) => p.Advance();
        protected override async Task WriteStart(TextWriter w) => await w.WriteAsync('(');
        protected override async Task WriteEnd(TextWriter w) => await w.WriteAsync(')');
        public override bool IsValid => SubExpressions.Count != 0 && base.IsValid;
    }
}