using System;
using System.Collections.Generic;

namespace BOOSE.Runtime
{
    /// <summary>
    /// Holds runtime state for program execution: variable scopes, method registry, canvas, and output.
    /// </summary>
    public sealed class ExecutionContext
    {
        private readonly Stack<Dictionary<string, Value>> _scopes = new();

        public ICanvas Canvas { get; }
        public Action<string> Output { get; }

        /// <summary>All methods defined in the program.</summary>
        public Dictionary<string, MethodDef> Methods { get; } = new(StringComparer.OrdinalIgnoreCase);

        public ExecutionContext(ICanvas canvas, Action<string>? output)
        {
            Canvas = canvas ?? throw new ArgumentNullException(nameof(canvas));
            Output = output ?? (_ => { });
            _scopes.Push(new Dictionary<string, Value>(StringComparer.OrdinalIgnoreCase)); // global scope
        }

        public void PushScope() => _scopes.Push(new Dictionary<string, Value>(StringComparer.OrdinalIgnoreCase));
        public void PopScope()
        {
            if (_scopes.Count <= 1)
                throw new InvalidOperationException("Cannot pop global scope");
            _scopes.Pop();
        }

        public bool TryGet(string name, out Value value)
        {
            foreach (var scope in _scopes)
            {
                if (scope.TryGetValue(name, out value!))
                    return true;
            }
            value = Value.Null;
            return false;
        }

        public Value Get(string name)
        {
            return TryGet(name, out var v)
                ? v
                : throw new KeyNotFoundException($"Variable '{name}' not defined.");
        }

        public void Declare(string name, Value value)
        {
            _scopes.Peek()[name] = value;
        }

        /// <summary>
        /// Assigns to the nearest existing scope; if not found, creates in current scope.
        /// </summary>
        public void Assign(string name, Value value)
        {
            foreach (var scope in _scopes)
            {
                if (scope.ContainsKey(name))
                {
                    scope[name] = value;
                    return;
                }
            }
            _scopes.Peek()[name] = value;
        }

        public void EnsureDeclared(string name, Value defaultValue)
        {
            if (!TryGet(name, out _))
                Declare(name, defaultValue);
        }
    }
}
