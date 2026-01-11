using System.Collections.Generic;
using BOOSE.Ast;

namespace BOOSE.Runtime
{
    /// <summary>
    /// Represents a BOOSE method definition.
    /// </summary>
    public sealed class MethodDef
    {
        public string Name { get; }
        public ValueKind ReturnKind { get; }
        public IReadOnlyList<ParameterDef> Parameters { get; }
        public IReadOnlyList<IStatement> Body { get; }

        public MethodDef(string name, ValueKind returnKind, IReadOnlyList<ParameterDef> parameters, IReadOnlyList<IStatement> body)
        {
            Name = name;
            ReturnKind = returnKind;
            Parameters = parameters;
            Body = body;
        }
    }

    public sealed record ParameterDef(ValueKind Kind, string Name);
}
