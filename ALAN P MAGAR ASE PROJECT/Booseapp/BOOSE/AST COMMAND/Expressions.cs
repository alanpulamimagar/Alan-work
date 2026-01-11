using System;
using BOOSE.Runtime;

namespace BOOSE.Ast
{
    public sealed class LiteralExpression : IExpression
    {
        private readonly Value _value;
        public LiteralExpression(Value value) => _value = value;
        public Value Eval(ExecutionContext ctx) => _value;
    }

    public sealed class IdentifierExpression : IExpression
    {
        private readonly string _name;
        public IdentifierExpression(string name) => _name = name;
        public Value Eval(ExecutionContext ctx) => ctx.Get(_name);
        public override string ToString() => _name;
    }

    public sealed class UnaryExpression : IExpression
    {
        private readonly string _op;
        private readonly IExpression _expr;
        public UnaryExpression(string op, IExpression expr)
        {
            _op = op;
            _expr = expr;
        }

        public Value Eval(ExecutionContext ctx)
        {
            var v = _expr.Eval(ctx);
            return _op switch
            {
                "-" => v.Kind == ValueKind.Real ? Value.FromReal(-v.AsReal()) : Value.FromInt(-v.AsInt()),
                "!" => Value.FromBoolean(!v.AsBoolean()),
                "+" => v,
                _ => throw new InvalidOperationException($"Unsupported unary operator '{_op}'")
            };
        }
    }

    public sealed class BinaryExpression : IExpression
    {
        private readonly string _op;
        private readonly IExpression _left;
        private readonly IExpression _right;

        public BinaryExpression(string op, IExpression left, IExpression right)
        {
            _op = op;
            _left = left;
            _right = right;
        }

        public Value Eval(ExecutionContext ctx)
        {
            var a = _left.Eval(ctx);
            var b = _right.Eval(ctx);

            // String concatenation
            if (_op == "+" && (a.Kind == ValueKind.String || b.Kind == ValueKind.String))
                return Value.FromString(a.AsString() + b.AsString());

            switch (_op)
            {
                case "+":
                    return PromoteNumeric(a, b, (x, y) => x + y, (x, y) => x + y);
                case "-":
                    return PromoteNumeric(a, b, (x, y) => x - y, (x, y) => x - y);
                case "*":
                    return PromoteNumeric(a, b, (x, y) => x * y, (x, y) => x * y);
                case "/":
                    // In BOOSE examples, division is integer when both ints, else real.
                    if (a.Kind == ValueKind.Int && b.Kind == ValueKind.Int)
                        return Value.FromInt(a.AsInt() / b.AsInt());
                    return Value.FromReal(a.AsReal() / b.AsReal());

                case "<":
                    return Value.FromBoolean(a.AsReal() < b.AsReal());
                case ">":
                    return Value.FromBoolean(a.AsReal() > b.AsReal());
                case "<=":
                    return Value.FromBoolean(a.AsReal() <= b.AsReal());
                case ">=":
                    return Value.FromBoolean(a.AsReal() >= b.AsReal());
                case "==":
                    return Value.FromBoolean(a.AsString() == b.AsString());
                case "!=":
                    return Value.FromBoolean(a.AsString() != b.AsString());

                case "&&":
                    return Value.FromBoolean(a.AsBoolean() && b.AsBoolean());
                case "||":
                    return Value.FromBoolean(a.AsBoolean() || b.AsBoolean());

                default:
                    throw new InvalidOperationException($"Unsupported binary operator '{_op}'");
            }
        }

        private static Value PromoteNumeric(Value a, Value b, Func<int, int, int> intOp, Func<double, double, double> realOp)
        {
            if (a.Kind == ValueKind.Real || b.Kind == ValueKind.Real)
                return Value.FromReal(realOp(a.AsReal(), b.AsReal()));
            return Value.FromInt(intOp(a.AsInt(), b.AsInt()));
        }
    }
}
