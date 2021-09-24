using System;
using System.IO;
using static System.Console;

namespace Mafs
{
    class Program
    {
        static void Main(string[] args)
        {
            WriteLine("Enter Expression:");
            bool isValid = false;
            ContainerExpression? expression = null;
            try
            {
                var expressionText = ReadLine();
                using StringReader expressionReader = new(expressionText);
                ExpressionParser parser = new(expressionReader);

                expression = parser.Parse();
                isValid = expression.IsValid;
            }
            catch (InvalidExpressionException ex)
            {
                WriteLine("Parsing error:");
                WriteLine(ex.Message);
            }
            if (!isValid)
            {
                WriteLine("Invalid expression");
            }
            else
            {
                ExpressionEvaluator expressionEvaluator = new(expression);
                WriteLine($"Evaluation result: {expressionEvaluator.Value}");
            }
        }
    }
}
