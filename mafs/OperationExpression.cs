using System;
using System.IO;
using System.Threading.Tasks;

namespace Mafs
{
    public class OperationExpression : Expression
    {
        public char Operation { get;private set; }
        public override async Task Write(TextWriter w)
        {
            await w.WriteAsync('+');
        }
        public override bool CanBeFirst => Operation == '+' || Operation == '-';

        public override bool IsValid => true;

        internal override bool CanBeFollowedBy(Expression ex)
        {
            switch (ex)
            {
                case OperationExpression op:
                    return (Operation == '+' || Operation == '-') && (op.Operation == '+' || op.Operation == '-');
                case BracketExpression:
                case NumberExpression:
                    return true;
                case null:
                default:
                    return false;
            }
        }
        internal override void Initialize(ExpressionParser p)
        {
            if(p.CurrentChar != '+' && p.CurrentChar != '-' && p.CurrentChar != '*' && p.CurrentChar != '/')
            {
                throw new InvalidOperationException($"{nameof(p.CurrentChar)} must be one of [+, -, *, /].");
            }
            Operation = p.CurrentChar;
            p.Advance();
        }
    }
}