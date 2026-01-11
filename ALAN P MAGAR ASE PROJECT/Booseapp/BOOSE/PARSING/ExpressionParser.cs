using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using BOOSE.Ast;
using BOOSE.Runtime;

namespace BOOSE.Parsing
{
    /// <summary>
    /// Parses BOOSE expressions into an expression tree.
    /// Supports numeric, boolean, and string expressions.
    /// </summary>
    public sealed class ExpressionParser
    {
        private static readonly Dictionary<string, int> Precedence = new(StringComparer.Ordinal)
        {
            { "!", 7 },
            { "u-", 7 },
            { "u+", 7 },
            { "*", 6 },
            { "/", 6 },
            { "+", 5 },
            { "-", 5 },
            { "<", 4 },
            { ">", 4 },
            { "<=", 4 },
            { ">=", 4 },
            { "==", 3 },
            { "!=", 3 },
            { "&&", 2 },
            { "||", 1 },
        };

        public IExpression Parse(string expr)
        {
            expr = (expr ?? string.Empty).Trim();
            if (expr.Length == 0) return new LiteralExpression(Value.Null);

            var tokens = Tokenize(expr);
            var output = new Stack<IExpression>();
            var ops = new Stack<string>();

            string? prevTokenType = null; // "op", "value", "("

            for (int i = 0; i < tokens.Count; i++)
            {
                var t = tokens[i];

                if (t.Kind == TokenKind.Number)
                {
                    output.Push(t.IsReal
                        ? new LiteralExpression(Value.FromReal(t.RealValue))
                        : new LiteralExpression(Value.FromInt(t.IntValue)));
                    prevTokenType = "value";
                    continue;
                }

                if (t.Kind == TokenKind.String)
                {
                    output.Push(new LiteralExpression(Value.FromString(t.Text)));
                    prevTokenType = "value";
                    continue;
                }

                if (t.Kind == TokenKind.Identifier)
                {
                    if (t.Text.Equals("true", StringComparison.OrdinalIgnoreCase))
                    {
                        output.Push(new LiteralExpression(Value.FromBoolean(true)));
                    }
                    else if (t.Text.Equals("false", StringComparison.OrdinalIgnoreCase))
                    {
                        output.Push(new LiteralExpression(Value.FromBoolean(false)));
                    }
                    else if (TryColourConstant(t.Text, out var colour))
                    {
                        output.Push(new LiteralExpression(Value.FromInt(colour)));
                    }
                    else
                    {
                        output.Push(new IdentifierExpression(t.Text));
                    }

                    prevTokenType = "value";
                    continue;
                }

                if (t.Kind == TokenKind.LParen)
                {
                    ops.Push("(");
                    prevTokenType = "(";
                    continue;
                }

                if (t.Kind == TokenKind.RParen)
                {
                    while (ops.Count > 0 && ops.Peek() != "(")
                        PopOperator(ops.Pop(), output);

                    if (ops.Count == 0 || ops.Pop() != "(")
                        throw new FormatException("Mismatched parentheses");

                    prevTokenType = "value";
                    continue;
                }

                if (t.Kind == TokenKind.Operator)
                {
                    string op = t.Text;

                    // Unary handling (+/-)
                    if (op == "+" || op == "-")
                    {
                        if (prevTokenType is null || prevTokenType == "op" || prevTokenType == "(")
                            op = op == "+" ? "u+" : "u-";
                    }

                    // Unary NOT is always unary
                    // (no extra detection needed)

                    while (ops.Count > 0 && ops.Peek() != "(" &&
                           Precedence.TryGetValue(ops.Peek(), out var p2) &&
                           Precedence.TryGetValue(op, out var p1) &&
                           p2 >= p1)
                    {
                        PopOperator(ops.Pop(), output);
                    }

                    ops.Push(op);
                    prevTokenType = "op";
                    continue;
                }

                throw new FormatException($"Unexpected token: {t.Text}");
            }

            while (ops.Count > 0)
            {
                var op = ops.Pop();
                if (op == "(") throw new FormatException("Mismatched parentheses");
                PopOperator(op, output);
            }

            if (output.Count != 1)
                throw new FormatException("Invalid expression");

            return output.Pop();
        }

        private static void PopOperator(string op, Stack<IExpression> output)
        {
            if (op == "u-" || op == "u+" || op == "!")
            {
                if (output.Count < 1) throw new FormatException("Missing operand");
                var a = output.Pop();
                output.Push(new UnaryExpression(op == "u-" ? "-" : op == "u+" ? "+" : "!", a));
                return;
            }

            if (output.Count < 2) throw new FormatException("Missing operand");
            var right = output.Pop();
            var left = output.Pop();
            output.Push(new BinaryExpression(op, left, right));
        }

        private static bool TryColourConstant(string name, out int value)
        {
            // Allows: pen red,0,0  -> red becomes 255
            // (If you never use this feature, it won’t affect anything.)
            switch (name.ToLowerInvariant())
            {
                case "red": value = 255; return true;
                case "green": value = 255; return true;
                case "blue": value = 255; return true;
                case "black": value = 0; return true;
                case "white": value = 255; return true;
                default: value = 0; return false;
            }
        }

        private static List<Token> Tokenize(string expr)
        {
            // Normalize copy/paste dashes to normal '-'
            expr = expr
                .Replace('\u2212', '-') // minus sign (−)
                .Replace('\u2013', '-') // en dash (–)
                .Replace('\u2014', '-') // em dash (—)
                .Replace('\u2012', '-') // figure dash (‒)
                .Replace('\uFE63', '-') // small hyphen-minus (﹣)
                .Replace('\uFF0D', '-'); // fullwidth hyphen-minus (－)

            var tokens = new List<Token>();
            int i = 0;

            while (i < expr.Length)
            {
                char c = expr[i];

                if (char.IsWhiteSpace(c)) { i++; continue; }

                if (c == '(') { tokens.Add(Token.LParen()); i++; continue; }
                if (c == ')') { tokens.Add(Token.RParen()); i++; continue; }

                if (c == '"')
                {
                    i++;
                    var sb = new StringBuilder();
                    while (i < expr.Length && expr[i] != '"')
                    {
                        sb.Append(expr[i]);
                        i++;
                    }
                    if (i >= expr.Length) throw new FormatException("Unterminated string literal");
                    i++; // closing quote
                    tokens.Add(Token.String(sb.ToString()));
                    continue;
                }

                // Operators (multi-char first)
                string? op = null;
                if (i + 1 < expr.Length)
                {
                    var two = expr.Substring(i, 2);
                    if (two is "<=" or ">=" or "==" or "!=" or "&&" or "||")
                        op = two;
                }
                if (op is not null)
                {
                    tokens.Add(Token.Op(op));
                    i += 2;
                    continue;
                }

                // ✅ IMPORTANT: single '=' in BOOSE IF conditions means equality
                if (c == '=')
                {
                    tokens.Add(Token.Op("=="));
                    i++;
                    continue;
                }

                // Single-char operators (includes unary '!')
                if (c is '+' or '-' or '*' or '/' or '<' or '>' or '!')
                {
                    tokens.Add(Token.Op(c.ToString()));
                    i++;
                    continue;
                }

                // Number
                if (char.IsDigit(c) || (c == '.' && i + 1 < expr.Length && char.IsDigit(expr[i + 1])))
                {
                    int start = i;
                    bool hasDot = false;
                    while (i < expr.Length && (char.IsDigit(expr[i]) || expr[i] == '.'))
                    {
                        if (expr[i] == '.') hasDot = true;
                        i++;
                    }
                    var txt = expr.Substring(start, i - start);
                    if (hasDot)
                    {
                        double v = double.Parse(txt, CultureInfo.InvariantCulture);
                        tokens.Add(Token.Real(v));
                    }
                    else
                    {
                        int v = int.Parse(txt, CultureInfo.InvariantCulture);
                        tokens.Add(Token.Int(v));
                    }
                    continue;
                }

                // Identifier
                if (char.IsLetter(c) || c == '_')
                {
                    int start = i;
                    while (i < expr.Length && (char.IsLetterOrDigit(expr[i]) || expr[i] == '_')) i++;
                    var name = expr.Substring(start, i - start);
                    tokens.Add(Token.Id(name));
                    continue;
                }

                throw new FormatException($"Unexpected character '{c}' in expression.");
            }

            return tokens;
        }

        private enum TokenKind { Number, String, Identifier, Operator, LParen, RParen }

        private sealed record Token(TokenKind Kind, string Text, int IntValue = 0, double RealValue = 0.0, bool IsReal = false)
        {
            public static Token Int(int v) => new(TokenKind.Number, v.ToString(CultureInfo.InvariantCulture), IntValue: v, IsReal: false);
            public static Token Real(double v) => new(TokenKind.Number, v.ToString(CultureInfo.InvariantCulture), RealValue: v, IsReal: true);
            public static Token String(string s) => new(TokenKind.String, s);
            public static Token Id(string s) => new(TokenKind.Identifier, s);
            public static Token Op(string s) => new(TokenKind.Operator, s);
            public static Token LParen() => new(TokenKind.LParen, "(");
            public static Token RParen() => new(TokenKind.RParen, ")");
        }
    }
}
