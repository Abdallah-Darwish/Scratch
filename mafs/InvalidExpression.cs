using System;
namespace Mafs
{
    [System.Serializable]
    public class InvalidExpressionException : System.Exception
    {
        public InvalidExpressionException(string message) : base(message) { }
        public InvalidExpressionException(int index, string message, System.Exception inner) : base(message, inner) { }
        protected InvalidExpressionException(
            System.Runtime.Serialization.SerializationInfo info,
            System.Runtime.Serialization.StreamingContext context) : base(info, context) { }
    }

}
