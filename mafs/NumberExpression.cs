using System;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace Mafs
{
    public class NumberExpression : Expression
    {
        public override bool CanBeFirst => true;

        public double Value { get; private set; }

        public override bool IsValid => true;

        public override Task Write(TextWriter w)
        {
            w.Write(Value);
            return Task.CompletedTask;
        }

        internal override bool CanBeFollowedBy(Expression ex)
        {
            switch (ex)
            {
                case null:
                case OperationExpression:
                return true;
                case NumberExpression:
                case BracketExpression:
                default:
                    return false;
            }
        }

        internal override void Initialize(ExpressionParser p)
        {
            if(char.IsNumber(p.CurrentChar))
            {
                throw new InvalidOperationException($"{p.CurrentChar} must be a digit.");
            }
            StringBuilder sb = new();
            bool dotSeen = false;
            while(char.IsNumber(p.CurrentChar) || (!dotSeen && p.CurrentChar == '.'))
            {
                dotSeen = p.CurrentChar == '.';
                sb.Append(p.CurrentChar);
                p.Advance();
            }
            if(p.CurrentChar == '.')
            {
                throw new InvalidExpressionException("A dot was encounterd more than once in a number");
            }
        }
    }
}
