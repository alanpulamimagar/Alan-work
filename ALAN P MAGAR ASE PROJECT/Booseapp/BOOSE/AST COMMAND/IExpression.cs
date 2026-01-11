using BOOSE.Runtime;

namespace BOOSE.Ast
{
    /// <summary>
    /// An evaluatable expression.
    /// </summary>
    public interface IExpression
    {
        Value Eval(ExecutionContext ctx);
    }
}
