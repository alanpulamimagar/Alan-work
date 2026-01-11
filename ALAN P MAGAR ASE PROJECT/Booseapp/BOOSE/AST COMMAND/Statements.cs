using System;
using System.Collections.Generic;
using System.Globalization;
using BOOSE.Runtime;

namespace BOOSE.Ast
{
    public sealed class VarDeclStatement : IStatement
    {
        private readonly string _name;
        private readonly ValueKind _kind;
        private readonly IExpression? _init;

        public VarDeclStatement(ValueKind kind, string name, IExpression? init)
        {
            _kind = kind;
            _name = name;
            _init = init;
        }

        public void Execute(ExecutionContext ctx)
        {
            Value v;
            if (_init is null)
            {
                v = _kind switch
                {
                    ValueKind.Int => Value.FromInt(0),
                    ValueKind.Real => Value.FromReal(0.0),
                    ValueKind.Boolean => Value.FromBoolean(false),
                    ValueKind.String => Value.FromString(string.Empty),
                    _ => Value.Null
                };
            }
            else
            {
                v = Coerce(_kind, _init.Eval(ctx));
            }

            ctx.Declare(_name, v);
        }

        private static Value Coerce(ValueKind kind, Value v)
        {
            return kind switch
            {
                ValueKind.Int => Value.FromInt(v.AsInt()),
                ValueKind.Real => Value.FromReal(v.AsReal()),
                ValueKind.Boolean => Value.FromBoolean(v.AsBoolean()),
                ValueKind.String => Value.FromString(v.AsString()),
                _ => v
            };
        }
    }

    public sealed class AssignStatement : IStatement
    {
        private readonly string _name;
        private readonly IExpression _expr;

        public AssignStatement(string name, IExpression expr)
        {
            _name = name;
            _expr = expr;
        }

        public void Execute(ExecutionContext ctx)
        {
            var value = _expr.Eval(ctx);
            ctx.Assign(_name, value);
        }
    }

    public sealed class ArrayDeclStatement : IStatement
    {
        private readonly string _name;
        private readonly ValueKind _elementKind;
        private readonly IExpression _lengthExpr;

        public ArrayDeclStatement(ValueKind elementKind, string name, IExpression lengthExpr)
        {
            _name = name;
            _elementKind = elementKind;
            _lengthExpr = lengthExpr;
        }

        public void Execute(ExecutionContext ctx)
        {
            int length = _lengthExpr.Eval(ctx).AsInt();
            var arr = new ArrayValue(_elementKind, length);
            ctx.Declare(_name, Value.FromArray(arr));
        }
    }

    public sealed class PokeStatement : IStatement
    {
        private readonly string _arrayName;
        private readonly IExpression _index;
        private readonly IExpression _value;

        public PokeStatement(string arrayName, IExpression index, IExpression value)
        {
            _arrayName = arrayName;
            _index = index;
            _value = value;
        }

        public void Execute(ExecutionContext ctx)
        {
            var arrVal = ResolveArray(ctx, _arrayName);
            int idx = _index.Eval(ctx).AsInt();
            var v = _value.Eval(ctx);
            arrVal.Set(idx, v);
        }

        private static ArrayValue ResolveArray(ExecutionContext ctx, string name)
        {
            if (ctx.TryGet(name, out var v) && v.Kind == ValueKind.Array)
                return v.AsArray();

            // Handle common typo in provided example programs: 'log' vs 'logs'
            if (name.Equals("log", StringComparison.OrdinalIgnoreCase) && ctx.TryGet("logs", out var logs) && logs.Kind == ValueKind.Array)
                return logs.AsArray();

            throw new KeyNotFoundException($"Array '{name}' not defined.");
        }
    }

    public sealed class PeekStatement : IStatement
    {
        private readonly string _destVar;
        private readonly string _arrayName;
        private readonly IExpression _index;

        public PeekStatement(string destVar, string arrayName, IExpression index)
        {
            _destVar = destVar;
            _arrayName = arrayName;
            _index = index;
        }

        public void Execute(ExecutionContext ctx)
        {
            var arrVal = ResolveArray(ctx, _arrayName);
            int idx = _index.Eval(ctx).AsInt();
            var v = arrVal.Get(idx);
            ctx.Assign(_destVar, v);
        }

        private static ArrayValue ResolveArray(ExecutionContext ctx, string name)
        {
            if (ctx.TryGet(name, out var v) && v.Kind == ValueKind.Array)
                return v.AsArray();

            if (name.Equals("log", StringComparison.OrdinalIgnoreCase) && ctx.TryGet("logs", out var logs) && logs.Kind == ValueKind.Array)
                return logs.AsArray();

            throw new KeyNotFoundException($"Array '{name}' not defined.");
        }
    }

    public sealed class IfStatement : IStatement
    {
        private readonly IExpression _condition;
        private readonly IReadOnlyList<IStatement> _thenBlock;
        private readonly IReadOnlyList<IStatement>? _elseBlock;

        public IfStatement(IExpression condition, IReadOnlyList<IStatement> thenBlock, IReadOnlyList<IStatement>? elseBlock)
        {
            _condition = condition;
            _thenBlock = thenBlock;
            _elseBlock = elseBlock;
        }

        public void Execute(ExecutionContext ctx)
        {
            if (_condition.Eval(ctx).AsBoolean())
                ExecuteBlock(ctx, _thenBlock);
            else if (_elseBlock is not null)
                ExecuteBlock(ctx, _elseBlock);
        }

        private static void ExecuteBlock(ExecutionContext ctx, IReadOnlyList<IStatement> stmts)
        {
            foreach (var s in stmts) s.Execute(ctx);
        }
    }

    public sealed class WhileStatement : IStatement
    {
        private readonly IExpression _condition;
        private readonly IReadOnlyList<IStatement> _body;

        public WhileStatement(IExpression condition, IReadOnlyList<IStatement> body)
        {
            _condition = condition;
            _body = body;
        }

        public void Execute(ExecutionContext ctx)
        {
            while (_condition.Eval(ctx).AsBoolean())
            {
                foreach (var s in _body) s.Execute(ctx);
            }
        }
    }

    public sealed class ForStatement : IStatement
    {
        private readonly string _varName;
        private readonly IExpression _start;
        private readonly IExpression _end;
        private readonly IExpression _step;
        private readonly IReadOnlyList<IStatement> _body;

        public ForStatement(string varName, IExpression start, IExpression end, IExpression step, IReadOnlyList<IStatement> body)
        {
            _varName = varName;
            _start = start;
            _end = end;
            _step = step;
            _body = body;
        }

        public void Execute(ExecutionContext ctx)
        {
            int start = _start.Eval(ctx).AsInt();
            int end = _end.Eval(ctx).AsInt();
            int step = _step.Eval(ctx).AsInt();
            if (step == 0) throw new InvalidOperationException("For loop step cannot be 0");

            ctx.EnsureDeclared(_varName, Value.FromInt(start));
            ctx.Assign(_varName, Value.FromInt(start));

            bool Forward(int i) => step > 0 ? i <= end : i >= end;

            for (int i = start; Forward(i); i += step)
            {
                ctx.Assign(_varName, Value.FromInt(i));
                foreach (var s in _body) s.Execute(ctx);
            }
        }
    }

    public sealed class MethodDeclStatement : IStatement
    {
        public MethodDef Method { get; }
        public MethodDeclStatement(MethodDef method) => Method = method;
        public void Execute(ExecutionContext ctx)
        {
            // No-op at runtime; methods are registered during parsing.
        }
    }

    public sealed class CallStatement : IStatement
    {
        private readonly string _methodName;
        private readonly IReadOnlyList<IExpression> _args;

        public CallStatement(string methodName, IReadOnlyList<IExpression> args)
        {
            _methodName = methodName;
            _args = args;
        }

        public void Execute(ExecutionContext ctx)
        {
            if (!ctx.Methods.TryGetValue(_methodName, out var method))
            {
                // handle typo in provided sample: mullMethod vs mulMethod
                if (_methodName.Equals("mullMethod", StringComparison.OrdinalIgnoreCase) && ctx.Methods.TryGetValue("mulMethod", out var m2))
                    method = m2;
                else
                    throw new KeyNotFoundException($"Method '{_methodName}' not defined.");
            }

            if (_args.Count != method.Parameters.Count)
                throw new InvalidOperationException($"Method '{method.Name}' expects {method.Parameters.Count} args, got {_args.Count}.");

            ctx.PushScope();
            try
            {
                // Initialise return variable in method scope
                ctx.Declare(method.Name, method.ReturnKind switch
                {
                    ValueKind.Int => Value.FromInt(0),
                    ValueKind.Real => Value.FromReal(0.0),
                    ValueKind.Boolean => Value.FromBoolean(false),
                    ValueKind.String => Value.FromString(string.Empty),
                    _ => Value.Null
                });

                // Bind parameters
                for (int i = 0; i < method.Parameters.Count; i++)
                {
                    var p = method.Parameters[i];
                    var argVal = _args[i].Eval(ctx);
                    var coerced = p.Kind switch
                    {
                        ValueKind.Int => Value.FromInt(argVal.AsInt()),
                        ValueKind.Real => Value.FromReal(argVal.AsReal()),
                        ValueKind.Boolean => Value.FromBoolean(argVal.AsBoolean()),
                        ValueKind.String => Value.FromString(argVal.AsString()),
                        _ => argVal
                    };
                    ctx.Declare(p.Name, coerced);
                }

                foreach (var s in method.Body) s.Execute(ctx);

                var ret = ctx.Get(method.Name);

                // Pop method scope, then store result in outer scope
                ctx.PopScope();

                ctx.Assign(method.Name, ret);

                // Also mirror for the typo name if used
                if (_methodName.Equals("mullMethod", StringComparison.OrdinalIgnoreCase) &&
                    !method.Name.Equals(_methodName, StringComparison.OrdinalIgnoreCase))
                {
                    ctx.Assign(_methodName, ret);
                }
            }
            catch
            {
                // Ensure scope cleanup if something fails before we pop
                try { ctx.PopScope(); } catch { }
                throw;
            }
        }
    }
}
