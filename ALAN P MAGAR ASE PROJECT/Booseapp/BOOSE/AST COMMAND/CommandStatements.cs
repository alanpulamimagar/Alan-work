using System;
using System.Collections.Generic;
using BOOSE.Runtime;

namespace BOOSE.Ast
{
    /// <summary>
    /// Statement wrapper for executable commands.
    /// </summary>
    public sealed class CommandStatement : IStatement
    {
        private readonly ICommand _command;
        public CommandStatement(ICommand command) => _command = command;
        public void Execute(ExecutionContext ctx) => _command.Execute(ctx);
    }

    public interface ICommand
    {
        void Execute(ExecutionContext ctx);
    }

    /// <summary>
    /// Simple output + draw-text statement.
    /// </summary>
    public sealed class WriteStatement : IStatement
    {
        private readonly IExpression _expr;
        public WriteStatement(IExpression expr) => _expr = expr;

        public void Execute(ExecutionContext ctx)
        {
            var v = _expr.Eval(ctx);
            var s = v.AsString();
            ctx.Output(s);
            ctx.Canvas.WriteText(s);
        }
    }

    /// <summary>
    /// Draws text at the current pen position.
    ///
    /// Many BOOSE example programs (and some UI templates) use a <c>text</c>
    /// command rather than a <c>write</c> statement. This command supports both:
    ///
    /// - Raw text: <c>text hello world</c>
    /// - Expression text: <c>text "hello"</c> or <c>text someVar</c>
    /// </summary>
    public sealed class TextCommand : ICommand
    {
        private readonly string _raw;
        private readonly IExpression? _expr;

        public TextCommand(string raw, IExpression? expr)
        {
            _raw = raw ?? string.Empty;
            _expr = expr;
        }

        public void Execute(ExecutionContext ctx)
        {
            var text = _expr is null ? _raw : _expr.Eval(ctx).AsString();
            ctx.Canvas.WriteText(text);
            ctx.Output(text);
        }
    }

    public sealed class MoveToCommand : ICommand
    {
        private readonly IExpression _x;
        private readonly IExpression _y;
        public MoveToCommand(IExpression x, IExpression y) { _x = x; _y = y; }
        public void Execute(ExecutionContext ctx) => ctx.Canvas.MoveTo(_x.Eval(ctx).AsInt(), _y.Eval(ctx).AsInt());
    }

    public sealed class DrawToCommand : ICommand
    {
        private readonly IExpression _x;
        private readonly IExpression _y;
        public DrawToCommand(IExpression x, IExpression y) { _x = x; _y = y; }
        public void Execute(ExecutionContext ctx) => ctx.Canvas.DrawTo(_x.Eval(ctx).AsInt(), _y.Eval(ctx).AsInt());
    }

    public sealed class CircleCommand : ICommand
    {
        private readonly IExpression _r;
        private readonly bool _filled;
        public CircleCommand(IExpression r, bool filled = false) { _r = r; _filled = filled; }
        public void Execute(ExecutionContext ctx) => ctx.Canvas.Circle(_r.Eval(ctx).AsInt(), _filled);
    }

    public sealed class RectCommand : ICommand
    {
        private readonly IExpression _w;
        private readonly IExpression _h;
        private readonly bool _filled;
        public RectCommand(IExpression w, IExpression h, bool filled = false) { _w = w; _h = h; _filled = filled; }
        public void Execute(ExecutionContext ctx) => ctx.Canvas.Rect(_w.Eval(ctx).AsInt(), _h.Eval(ctx).AsInt(), _filled);
    }

    public sealed class TriCommand : ICommand
    {
        private readonly IExpression _w;
        private readonly IExpression _h;
        public TriCommand(IExpression w, IExpression h) { _w = w; _h = h; }
        public void Execute(ExecutionContext ctx) => ctx.Canvas.Tri(_w.Eval(ctx).AsInt(), _h.Eval(ctx).AsInt());
    }

    public sealed class PenCommand : ICommand
    {
        private readonly IExpression _r;
        private readonly IExpression _g;
        private readonly IExpression _b;
        public PenCommand(IExpression r, IExpression g, IExpression b) { _r = r; _g = g; _b = b; }
        public void Execute(ExecutionContext ctx)
        {
            ctx.Canvas.SetColour(_r.Eval(ctx).AsInt(), _g.Eval(ctx).AsInt(), _b.Eval(ctx).AsInt());
        }
    }

    public sealed class ClearCommand : ICommand
    {
        public void Execute(ExecutionContext ctx) => ctx.Canvas.Clear();
    }

    public sealed class ResetCommand : ICommand
    {
        public void Execute(ExecutionContext ctx) => ctx.Canvas.Reset();
    }

    public sealed class SetCommand : ICommand
    {
        private readonly IExpression _w;
        private readonly IExpression _h;
        public SetCommand(IExpression w, IExpression h) { _w = w; _h = h; }
        public void Execute(ExecutionContext ctx) => ctx.Canvas.Set(_w.Eval(ctx).AsInt(), _h.Eval(ctx).AsInt());
    }
}
