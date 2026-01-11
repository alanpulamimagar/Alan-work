using System;
using System.Collections.Generic;
using BOOSE.Ast;
using BOOSE.Parsing;

namespace BOOSE.Runtime
{
    /// <summary>
    /// Factory used for mapping BOOSE command keywords (moveto, pen, circle, ...)
    /// to ICommand objects.
    ///
    /// Factory pattern requirement: commands are created here rather than scattered across the codebase.
    /// </summary>
    public sealed class CommandFactory
    {
        private readonly Dictionary<string, Func<string, ICommand>> _builders;

        public CommandFactory(ExpressionParser exprParser)
        {
            _builders = new Dictionary<string, Func<string, ICommand>>(StringComparer.OrdinalIgnoreCase)
            {
                { "moveto", args => BuildTwo(args, (a,b) => new MoveToCommand(a,b), exprParser) },
                { "drawto", args => BuildTwo(args, (a,b) => new DrawToCommand(a,b), exprParser) },
                { "circle", args => new CircleCommand(exprParser.Parse(args)) },
                { "rect", args => BuildTwo(args, (a,b) => new RectCommand(a,b), exprParser) },
                { "tri", args => BuildTwo(args, (a,b) => new TriCommand(a,b), exprParser) },
                { "pen", args => BuildThree(args, (a,b,c) => new PenCommand(a,b,c), exprParser) },
                { "pencolour", args => BuildThree(args, (a,b,c) => new PenCommand(a,b,c), exprParser) },
                { "pencolor", args => BuildThree(args, (a,b,c) => new PenCommand(a,b,c), exprParser) },
                { "clear", _ => new ClearCommand() },
                { "reset", _ => new ResetCommand() },
                { "set", args => BuildTwo(args, (a,b) => new SetCommand(a,b), exprParser) },

                // Many templates/programs use 'text' rather than 'write'.
                // We support both raw text and expression-based text.
                { "text", args => BuildText(args, exprParser) },
            };
        }

        public bool TryCreate(string keyword, string args, out ICommand command)
        {
            if (_builders.TryGetValue(keyword, out var builder))
            {
                command = builder(args);
                return true;
            }
            command = null!;
            return false;
        }

        private static ICommand BuildTwo(string args, Func<IExpression, IExpression, ICommand> ctor, ExpressionParser exprParser)
        {
            var parts = TextUtils.SplitArgs(args);
            if (parts.Count != 2)
                throw new FormatException("Expected 2 arguments.");
            return ctor(exprParser.Parse(parts[0]), exprParser.Parse(parts[1]));
        }

        private static ICommand BuildThree(string args, Func<IExpression, IExpression, IExpression, ICommand> ctor, ExpressionParser exprParser)
        {
            var parts = TextUtils.SplitArgs(args);
            if (parts.Count != 3)
                throw new FormatException("Expected 3 arguments.");
            return ctor(exprParser.Parse(parts[0]), exprParser.Parse(parts[1]), exprParser.Parse(parts[2]));
        }

        private static ICommand BuildText(string args, ExpressionParser exprParser)
        {
            // If the user provided quotes or an expression-like token, treat it as an expression.
            // Otherwise treat the whole argument string as raw text.
            var trimmed = (args ?? string.Empty).Trim();
            if (trimmed.Length == 0)
                return new TextCommand(string.Empty, null);

            bool hasQuotes = trimmed.Contains('"');
            bool looksLikeExpr = hasQuotes || trimmed.IndexOfAny(new[] { '+', '-', '*', '/', '(', ')', '<', '>', '!', '&', '|', '=' }) >= 0;

            // A single identifier should also be treated as an expression so: text myVar works.
            if (!looksLikeExpr)
            {
                bool singleToken = TextUtils.SplitArgs(trimmed).Count == 1;
                if (singleToken)
                {
                    var t = trimmed;
                    bool isIdentifier = char.IsLetter(t[0]) || t[0] == '_';
                    if (isIdentifier)
                        looksLikeExpr = true;
                }
            }

            return looksLikeExpr
                ? new TextCommand(string.Empty, exprParser.Parse(trimmed))
                : new TextCommand(trimmed, null);
        }
    }

    /// <summary>
    /// Singleton registry to provide a single CommandFactory instance.
    /// </summary>
    public sealed class CommandFactorySingleton
    {
        private static CommandFactorySingleton? _instance;
        public static CommandFactorySingleton Instance => _instance ??= new CommandFactorySingleton();

        public CommandFactory? Factory { get; private set; }

        private CommandFactorySingleton() { }

        public void Initialise(ExpressionParser parser)
        {
            Factory ??= new CommandFactory(parser);
        }
    }
}
