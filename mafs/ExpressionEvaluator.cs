using System.Threading.Tasks;
using System.IO;
using System.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;

namespace Mafs
{
    public class ExpressionEvaluator
    {
        private class ExpressionEvaluationResult
        {
            public Expression Expression;
            public double Result;
            public bool IsEvaluated;
            public ExpressionEvaluationResult(Expression exp)
            {
                Expression = exp;
            }
        }
        private readonly LinkedList<ExpressionEvaluationResult> _subExpressions = new();
        public ContainerExpression Expression { get; }
        private void EvaluateSigns(LinkedListNode<ExpressionEvaluationResult> numNode)
        {
            if (numNode.Value.IsEvaluated) { return; }
            numNode.Value.IsEvaluated = true;
            double val = (numNode.Value.Expression as NumberExpression)!.Value;
            LinkedListNode<ExpressionEvaluationResult>? nd = numNode.Previous;
            while (nd?.Value.Expression is OperationExpression && nd.Previous?.Value.Expression is (null or OperationExpression))
            {
                char op = (nd.Value.Expression as OperationExpression)!.Value;
                if (op == '-') { val *= -1; }
                var cnd = nd;
                nd = nd.Previous;
                _subExpressions.Remove(cnd);
            }
            numNode.Value.Result = val;
        }
        private void EvaluateNonOperation(LinkedListNode<ExpressionEvaluationResult> expNode)
        {
            if (expNode.Value.IsEvaluated) { return; }
            expNode.Value.IsEvaluated = true;
            if (expNode.Value.Expression is NumberExpression numExp)
            {
                expNode.Value.Result = numExp.Value;
            }
            else if (expNode.Value.Expression is ContainerExpression bExp)
            {
                ExpressionEvaluator evaluator = new(bExp);
                expNode.Value.Result = evaluator.Value;
            }
        }
        private void EvaluateOperation(LinkedListNode<ExpressionEvaluationResult> opNode)
        {
            if (opNode.Value.IsEvaluated) { return; }
            opNode.Value.IsEvaluated = true;

            EvaluateNonOperation(opNode.Previous!);
            EvaluateNonOperation(opNode.Next!);
            double left = opNode.Previous!.Value.Result, right = opNode.Next!.Value.Result;
            _subExpressions.Remove(opNode.Previous);
            _subExpressions.Remove(opNode.Next);

            var op = (opNode.Value.Expression as OperationExpression)!.Value;
            opNode.Value.Result = op switch
            {
                '+' => left + right,
                '-' => left - right,
                '*' => left * right,
                '/' => left / right,
                _ => double.NaN
            };
        }
        private void Evaluate()
        {
            //1- Evalute numbers signs
            for (LinkedListNode<ExpressionEvaluationResult>? nd = _subExpressions.First; nd != null; nd = nd!.Next)
            {
                if (nd.Value.Expression is NumberExpression)
                {
                    EvaluateSigns(nd);
                }
            }

            //2- Evaluate * /
            for (LinkedListNode<ExpressionEvaluationResult>? nd = _subExpressions.First; nd != null; nd = nd!.Next)
            {
                if (nd.Value.Expression is OperationExpression opExp && (opExp.Value == '*' || opExp.Value == '/'))
                {
                    EvaluateOperation(nd);
                }
            }

            //3- Evaluet + -
            for (LinkedListNode<ExpressionEvaluationResult>? nd = _subExpressions.First; nd != null; nd = nd!.Next)
            {
                if (nd.Value.Expression is OperationExpression)
                {
                    EvaluateOperation(nd);
                }
            }

            _value = _subExpressions.First!.Value.Result;
            _subExpressions.Clear();
        }
        private bool _isEvaluated = false;
        private double _value;
        public double Value
        {
            get
            {
                if (!_isEvaluated)
                {
                    Evaluate();
                    _isEvaluated = true;
                }
                return _value;
            }
        }
        public ExpressionEvaluator(ContainerExpression exp)
        {
            Expression = exp;

            foreach (var subExp in exp.SubExpressions)
            {
                _subExpressions.AddLast(new ExpressionEvaluationResult(subExp));
            }
        }
    }
}