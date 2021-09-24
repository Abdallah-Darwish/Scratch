using System.Threading.Tasks;
using System.IO;
using System.Text;

namespace Mafs
{
    public abstract class Expression
    {
        public abstract bool CanBeFirst { get; }
        public abstract bool IsValid { get; }
        public async Task<string> ToStringAsync()
        {
            StringBuilder sb = new();
            await using StringWriter sw = new(sb);
            await Write(sw);
            return sb.ToString();
        }
        public abstract Task Write(TextWriter w);
        internal abstract bool CanBeFollowedBy(Expression? ex);
        internal abstract void Initialize(ExpressionParser p);
    }
}
