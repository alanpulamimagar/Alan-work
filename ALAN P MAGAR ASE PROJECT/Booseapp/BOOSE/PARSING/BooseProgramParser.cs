using System;
using System.Collections.Generic;
using System.Linq;
using BOOSE.Ast;
using BOOSE.Runtime;

namespace BOOSE.Parsing 
{
    /// <summary>
    /// Line-oriented parser for the BOOSE language as used in the supplied example programs.
    /// </summary>
    public sealed class BooseProgramParser
    {
        private readonly ExpressionParser _expr;  
        private readonly CommandFactorySingleton _factorySingleton;

        public BooseProgramParser(ExpressionParser expr)
        {
            _expr = expr;
            _factorySingleton = CommandFactorySingleton.Instance;
            _factorySingleton.Initialise(_expr);
        }

        public ParsedProgram Parse(string programText)
        {
            var lines = SplitLines(programText);
            int idx = 0;

            var methods = new Dictionary<string, MethodDef>(StringComparer.OrdinalIgnoreCase);
            var main = ParseBlock(lines, ref idx, endKeywords: null, methods);

            return new ParsedProgram(main, methods);
        }

        private IReadOnlyList<IStatement> ParseBlock(List<string> lines, ref int idx, HashSet<string>? endKeywords, Dictionary<string, MethodDef> methods)
        {
            var statements = new List<IStatement>();

            while (idx < lines.Count)
            {
                string raw = lines[idx];
                string line = raw.Trim();

                if (line.Length == 0 || line.StartsWith("*") || line.StartsWith("//"))
                {
                    idx++;
                    continue;
                }

                string lower = line.ToLowerInvariant();

                if (endKeywords is not null && endKeywords.Contains(lower))
                {
                    break;
                }

                // Method declaration
                if (lower.StartsWith("method "))
                {
                    var m = ParseMethod(lines, ref idx, methods);
                    methods[m.Name] = m;
                    // not part of main execution
                    continue;
                }

                // If
                if (lower.StartsWith("if "))
                {
                    statements.Add(ParseIf(lines, ref idx, methods));
                    continue;
                }

                // While
                if (lower.StartsWith("while "))
                {
                    statements.Add(ParseWhile(lines, ref idx, methods));
                    continue;
                }

                // For
                if (lower.StartsWith("for "))
                {
                    statements.Add(ParseFor(lines, ref idx, methods)); 
                    continue;
                }

                // Variable declarations
                if (lower.StartsWith("int ") || lower.StartsWith("real ") || lower.StartsWith("boolean "))
                {
                    statements.Add(ParseVarDecl(line));
                    idx++;
                    continue;
                }

                // Array declaration
                if (lower.StartsWith("array "))
                {
                    statements.Add(ParseArrayDecl(line));
                    idx++;
                    continue;
                }

                // Poke / Peek
                if (lower.StartsWith("poke "))
                {
                    statements.Add(ParsePoke(line));
                    idx++;
                    continue;
                }

                if (lower.StartsWith("peek "))
                {
                    statements.Add(ParsePeek(line));
                    idx++;
                    continue;
                }

                // Call
                if (lower.StartsWith("call "))
                {
                    statements.Add(ParseCall(line));
                    idx++;
                    continue;
                }

                // Write
                if (lower.StartsWith("write "))
                {
                    var exprStr = line[5..].Trim();
                    statements.Add(new WriteStatement(_expr.Parse(exprStr)));
                    idx++;
                    continue;
                }

                // Assignment (must come after other statements that include '=')
                int assignIdx = FindAssignmentOperator(line);
                if (assignIdx > 0)
                {
                    string left = line[..assignIdx].Trim();
                    string right = line[(assignIdx + 1)..].Trim();
                    statements.Add(new AssignStatement(left, _expr.Parse(right)));
                    idx++;
                    continue;
                }

                // Drawing/utility command
                var firstSpace = line.IndexOf(' ');
                string keyword = firstSpace < 0 ? line : line[..firstSpace];
                string args = firstSpace < 0 ? string.Empty : line[(firstSpace + 1)..];

                if (_factorySingleton.Factory!.TryCreate(keyword, args, out var cmd))
                {
                    statements.Add(new CommandStatement(cmd));
                    idx++;
                    continue;
                }

                throw new FormatException($"Unknown statement: '{line}'");
            }

            return statements;
        }

        private static List<string> SplitLines(string programText)
        {
            // Trim BOM and normalise newlines
            programText ??= string.Empty;
            programText = programText.TrimStart('\uFEFF');
            return programText
                .Replace("\r\n", "\n")
                .Replace("\r", "\n")
                .Split('\n')
                .Select(l => l.TrimEnd())
                .ToList();
        }

        private static int FindAssignmentOperator(string line)
        {
            // find '=' that is not part of '==', '!=', '<=', '>='
            for (int i = 0; i < line.Length; i++)
            {
                if (line[i] != '=') continue;

                char prev = i > 0 ? line[i - 1] : '\0';
                char next = i + 1 < line.Length ? line[i + 1] : '\0';

                if (prev == '=' || prev == '!' || prev == '<' || prev == '>') continue;
                if (next == '=') continue;

                return i;
            }
            return -1;
        }

        private VarDeclStatement ParseVarDecl(string line)
        {
            var parts = line.Split([' '], 3, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 2) throw new FormatException("Invalid variable declaration");

            ValueKind kind = parts[0].ToLowerInvariant() switch
            { 
                "int" => ValueKind.Int,
                "real" => ValueKind.Real,
                "boolean" => ValueKind.Boolean,
                _ => throw new FormatException("Unknown type")
            };

            string rest = line[parts[0].Length..].Trim();
            // rest is 'name' or 'name = expr'
            var eq = FindAssignmentOperator(rest);
            if (eq < 0)
            {
                string name = rest.Trim();
                return new VarDeclStatement(kind, name, null);
            }
            else
            {
                string name = rest[..eq].Trim();
                string expr = rest[(eq + 1)..].Trim();
                return new VarDeclStatement(kind, name, _expr.Parse(expr));
            }
        }

        private ArrayDeclStatement ParseArrayDecl(string line)
        {
            // array int nums 10
            var parts = line.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 4) throw new FormatException("Invalid array declaration. Expected: array <type> <name> <length>");

            ValueKind elementKind = parts[1].ToLowerInvariant() switch
            {
                "int" => ValueKind.Int,
                "real" => ValueKind.Real,
                "boolean" => ValueKind.Boolean,
                _ => throw new FormatException("Unsupported array element type")
            };

            string name = parts[2];
            string lenExpr = string.Join(' ', parts.Skip(3));
            return new ArrayDeclStatement(elementKind, name, _expr.Parse(lenExpr));
        }

        private PokeStatement ParsePoke(string line)
        {
            // poke nums 5 = 99
            // poke prices 5 = 99.99
            var rest = line[4..].Trim();
            var eq = FindAssignmentOperator(rest);
            if (eq < 0) throw new FormatException("Invalid poke. Expected: poke <array> <index> = <expr>");

            var left = rest[..eq].Trim();
            var right = rest[(eq + 1)..].Trim();

            var leftParts = left.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (leftParts.Length < 2) throw new FormatException("Invalid poke target");

            string arrayName = leftParts[0];
            string indexExpr = string.Join(' ', leftParts.Skip(1));

            return new PokeStatement(arrayName, _expr.Parse(indexExpr), _expr.Parse(right));
        }

        private PeekStatement ParsePeek(string line)
        {
            // peek x = nums 5
            var rest = line[4..].Trim();
            var eq = FindAssignmentOperator(rest);
            if (eq < 0) throw new FormatException("Invalid peek. Expected: peek <dest> = <array> <index>");

            string dest = rest[..eq].Trim();
            string right = rest[(eq + 1)..].Trim();

            var rightParts = right.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
            if (rightParts.Length < 2) throw new FormatException("Invalid peek source");

            string arrayName = rightParts[0];
            string indexExpr = string.Join(' ', rightParts.Skip(1));

            return new PeekStatement(dest, arrayName, _expr.Parse(indexExpr));
        }

        private CallStatement ParseCall(string line)
        {
            // call <method> [args...]
            // The example programs often use spaces between call arguments (e.g. call test 10 15).
            // For resilience, we also accept comma-separated call arguments.

            var rest = line.Substring(4).Trim();
            if (rest.Length == 0)
                throw new FormatException("Invalid call. Expected: call <method> [args...]" );

            int firstSpace = rest.IndexOf(' ');
            string name = firstSpace < 0 ? rest : rest.Substring(0, firstSpace).Trim();
            string argsText = firstSpace < 0 ? string.Empty : rest.Substring(firstSpace + 1).Trim();

            if (name.Length == 0)
                throw new FormatException("Invalid call. Expected: call <method> [args...]" );

            var args = new List<IExpression>();
            foreach (var p in TextUtils.SplitArgs(argsText))
            {
                args.Add(_expr.Parse(p));
            }

            return new CallStatement(name, args);
        }

        private IfStatement ParseIf(List<string> lines, ref int idx, Dictionary<string, MethodDef> methods)
        {
            // if condition
            string header = lines[idx].Trim();
            var cond = _expr.Parse(header.Substring(2).Trim());
            idx++;

            var thenEnd = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "else", "end if", "endif" };
            var thenBlock = ParseBlock(lines, ref idx, thenEnd, methods);

            IReadOnlyList<IStatement>? elseBlock = null;
            if (idx < lines.Count && lines[idx].Trim().Equals("else", StringComparison.OrdinalIgnoreCase))
            {
                idx++; // consume else
                var elseEnd = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "end if", "endif" };
                elseBlock = ParseBlock(lines, ref idx, elseEnd, methods);
            }

            var endIfLine = idx < lines.Count ? lines[idx].Trim() : string.Empty;
            if (!endIfLine.Equals("end if", StringComparison.OrdinalIgnoreCase) && !endIfLine.Equals("endif", StringComparison.OrdinalIgnoreCase))
                throw new FormatException("Missing 'end if'");

            idx++; // consume end if
            return new IfStatement(cond, thenBlock, elseBlock);
        }

        private WhileStatement ParseWhile(List<string> lines, ref int idx, Dictionary<string, MethodDef> methods)
        {
            string header = lines[idx].Trim();
            var cond = _expr.Parse(header.Substring(5).Trim());
            idx++;

            var end = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "end while", "endwhile" };
            var body = ParseBlock(lines, ref idx, end, methods);

            var endWhileLine = idx < lines.Count ? lines[idx].Trim() : string.Empty;
            if (!endWhileLine.Equals("end while", StringComparison.OrdinalIgnoreCase) && !endWhileLine.Equals("endwhile", StringComparison.OrdinalIgnoreCase))
                throw new FormatException("Missing 'end while'");
            idx++;

            return new WhileStatement(cond, body);
        }

        private ForStatement ParseFor(List<string> lines, ref int idx, Dictionary<string, MethodDef> methods)
        {
            // for count = 1 to 20 step 2
            string header = lines[idx].Trim();
            string rest = header.Substring(3).Trim();

            int eq = rest.IndexOf('=');
            if (eq < 0) throw new FormatException("Invalid for loop. Expected '='");

            string varName = rest.Substring(0, eq).Trim();
            string afterEq = rest.Substring(eq + 1).Trim();

            int toIdx = afterEq.IndexOf(" to ", StringComparison.OrdinalIgnoreCase);
            if (toIdx < 0) throw new FormatException("Invalid for loop. Expected 'to'");
            string startExpr = afterEq.Substring(0, toIdx).Trim();
            string afterTo = afterEq.Substring(toIdx + 4).Trim();

            int stepIdx = afterTo.IndexOf(" step ", StringComparison.OrdinalIgnoreCase);
            if (stepIdx < 0) throw new FormatException("Invalid for loop. Expected 'step'");
            string endExpr = afterTo.Substring(0, stepIdx).Trim();
            string stepExpr = afterTo.Substring(stepIdx + 6).Trim();

            idx++;

            var end = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "end for", "endfor" };
            var body = ParseBlock(lines, ref idx, end, methods);

            var endForLine = idx < lines.Count ? lines[idx].Trim() : string.Empty;
            if (!endForLine.Equals("end for", StringComparison.OrdinalIgnoreCase) && !endForLine.Equals("endfor", StringComparison.OrdinalIgnoreCase))
                throw new FormatException("Missing 'end for'");
            idx++;

            return new ForStatement(varName, _expr.Parse(startExpr), _expr.Parse(endExpr), _expr.Parse(stepExpr), body);
        }

        private MethodDef ParseMethod(List<string> lines, ref int idx, Dictionary<string, MethodDef> methods)
        {
            // method int testMethod int one, int two
            string header = lines[idx].Trim();
            string rest = header.Substring(6).Trim();

            var parts = rest.Split(new[] { ' ' }, 3, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 2) throw new FormatException("Invalid method header");

            ValueKind returnKind = parts[0].ToLowerInvariant() switch
            {
                "int" => ValueKind.Int,
                "real" => ValueKind.Real,
                "boolean" => ValueKind.Boolean,
                _ => throw new FormatException("Unsupported return type")
            };

            string name = parts[1];
            string paramText = parts.Length >= 3 ? parts[2] : string.Empty;

            var parameters = new List<ParameterDef>();
            if (!string.IsNullOrWhiteSpace(paramText))
            {
                // split by commas first
                var paramParts = paramText.Split(',');
                foreach (var p in paramParts)
                {
                    var pp = p.Trim();
                    if (pp.Length == 0) continue;
                    var bits = pp.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                    if (bits.Length != 2) throw new FormatException($"Invalid parameter '{pp}'");

                    ValueKind pk = bits[0].ToLowerInvariant() switch
                    {
                        "int" => ValueKind.Int,
                        "real" => ValueKind.Real,
                        "boolean" => ValueKind.Boolean,
                        _ => throw new FormatException($"Unsupported parameter type '{bits[0]}'")
                    };

                    parameters.Add(new ParameterDef(pk, bits[1]));
                }
            }

            idx++;

            var end = new HashSet<string>(StringComparer.OrdinalIgnoreCase) { "end method", "endmethod" };
            var body = ParseBlock(lines, ref idx, end, methods);

            var endMethodLine = idx < lines.Count ? lines[idx].Trim() : string.Empty;
            if (!endMethodLine.Equals("end method", StringComparison.OrdinalIgnoreCase) && !endMethodLine.Equals("endmethod", StringComparison.OrdinalIgnoreCase))
                throw new FormatException("Missing 'end method'");
            idx++;

            return new MethodDef(name, returnKind, parameters, body);
        }
    }

    public sealed class ParsedProgram
    {
        public IReadOnlyList<IStatement> MainStatements { get; }
        public IReadOnlyDictionary<string, MethodDef> Methods { get; }

        public ParsedProgram(IReadOnlyList<IStatement> main, IReadOnlyDictionary<string, MethodDef> methods)
        {
            MainStatements = main;
            Methods = methods;
        }
    }
}
