using BOOSE.Runtime;

namespace BOOSE.Ast
{
    /// <summary>
    /// Executable program statement.
    /// </summary>
    public interface IStatement
    {
        void Execute(ExecutionContext ctx);
    }
}
