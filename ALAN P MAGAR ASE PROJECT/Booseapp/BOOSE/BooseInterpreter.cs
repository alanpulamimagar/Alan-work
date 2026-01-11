using System;
using System.Text;
using BOOSE.Parsing;
using BOOSE.Runtime;

namespace BOOSE
{
    /// <summary>
    /// Executes BOOSE programs against an <see cref="ICanvas"/>.
    /// </summary>
    public sealed class BooseInterpreter
    {
        private readonly ICanvas _canvas;
        private readonly Action<string> _output;

        public BooseInterpreter(ICanvas canvas, Action<string>? output = null)
        {
            _canvas = canvas ?? throw new ArgumentNullException(nameof(canvas));
            _output = output ?? (_ => { });
        }

        /// <summary>
        /// Parse and execute a BOOSE program.
        /// </summary>
        public ExecutionResult Execute(string programText)
        {
            var log = new StringBuilder();
            void Sink(string msg)
            {
                log.AppendLine(msg);
                _output(msg);
            }

            var expr = new ExpressionParser();
            var parser = new BooseProgramParser(expr);
            var parsed = parser.Parse(programText);

            var ctx = new ExecutionContext(_canvas, Sink);
            foreach (var kv in parsed.Methods)
                ctx.Methods[kv.Key] = kv.Value;

            int executed = 0;
            foreach (var st in parsed.MainStatements)
            {
                st.Execute(ctx);
                executed++;
            }

            return new ExecutionResult(executed, log.ToString());
        }
    }

    public sealed record ExecutionResult(int StatementsExecuted, string Log);
}
