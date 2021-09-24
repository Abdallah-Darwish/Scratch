using System;
using System.IO;
using System.Threading.Tasks;

namespace Mafs
{
    public class OperationExpression : Expression
    {
        public char Value { get; private set; }
        public override async Task Write(TextWriter w)
        {
            await w.WriteAsync('+');
        }
        public override bool CanBeFirst => Value == '+' || Value == '-';

        public override bool IsValid => true;

        internal override bool CanBeFollowedBy(Expression? ex)
        {
            return ex switch
            {
                OperationExpression op => op.Value == '+' || op.Value == '-',
                BracketExpression or NumberExpression => true,
                _ => false,
            };
        }
        internal override void Initialize(ExpressionParser p)
        {
            if (p.CurrentChar != '+' && p.CurrentChar != '-' && p.CurrentChar != '*' && p.CurrentChar != '/')
            {
                throw new InvalidOperationException($"{nameof(p.CurrentChar)} must be one of [+, -, *, /].");
            }
            Value = p.CurrentChar;
            p.Advance();
        }
    }
}