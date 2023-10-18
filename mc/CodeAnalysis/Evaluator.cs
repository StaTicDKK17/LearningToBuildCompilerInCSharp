using mc.CodeAnalysis.Binding;
using mc.CodeAnalysis.Syntax;

namespace Minsk.CodeAnalysis;

internal sealed class Evaluator
{
    public Evaluator(BoundExpression root)
    {
        this._root = root;
    }

    public readonly BoundExpression _root;

    public int Evalate()
    {
        return EvaluateExpression(_root);
    }

    private int EvaluateExpression(BoundExpression node)
    {
        if (node is BoundLiteralExpression n)
            return (int)n.Value;

        if (node is BoundUnaryExpression u)
        {
            var operand = EvaluateExpression(u.Operand);

            return u.OperatorKind switch
            {
                BoundUnaryOperatorKind.Identity => operand,
                BoundUnaryOperatorKind.Negation => -operand,
                _ => throw new Exception($"Unexpected unary operator {u.OperatorKind}"),
            };
        }

        if (node is BoundBinaryExpression b)
        {
            var left = EvaluateExpression(b.Left);
            var right = EvaluateExpression(b.Right);

            switch (b.OperatorKind)
            {
                case BoundBinaryOperatorKind.Addition:
                    return left + right;
                case BoundBinaryOperatorKind.Subtraction:
                    return left - right;
                case BoundBinaryOperatorKind.Multiplication:
                    return left * right;
                case BoundBinaryOperatorKind.Division:
                    return left / right;
                default:
                    throw new Exception($"Unexpected binary operator {b.OperatorKind}");
            }
        }

        throw new Exception($"Unexpected node {node.Kind}");
    }
}