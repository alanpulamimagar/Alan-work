using System;
using System.Globalization;

namespace BOOSE.Runtime
{
    /// <summary>
    /// Discriminated union used to represent BOOSE values.
    /// </summary>
    public sealed class Value
    {
        public ValueKind Kind { get; }
        public object? Raw { get; }

        private Value(ValueKind kind, object? raw)
        {
            Kind = kind;
            Raw = raw;
        }

        public static readonly Value Null = new(ValueKind.Null, null);

        public static Value FromInt(int v) => new(ValueKind.Int, v);
        public static Value FromReal(double v) => new(ValueKind.Real, v);
        public static Value FromBoolean(bool v) => new(ValueKind.Boolean, v);
        public static Value FromString(string v) => new(ValueKind.String, v);
        public static Value FromArray(ArrayValue arr) => new(ValueKind.Array, arr);

        public int AsInt()
        {
            return Kind switch
            {
                ValueKind.Int => (int)Raw!,
                ValueKind.Real => (int)Math.Round((double)Raw!, MidpointRounding.AwayFromZero),
                ValueKind.Boolean => (bool)Raw! ? 1 : 0,
                ValueKind.String => int.Parse((string)Raw!, CultureInfo.InvariantCulture),
                _ => throw new InvalidOperationException($"Cannot convert {Kind} to int")
            };
        }

        public double AsReal()
        {
            return Kind switch
            {
                ValueKind.Real => (double)Raw!,
                ValueKind.Int => (int)Raw!,
                ValueKind.Boolean => (bool)Raw! ? 1.0 : 0.0,
                ValueKind.String => double.Parse((string)Raw!, CultureInfo.InvariantCulture),
                _ => throw new InvalidOperationException($"Cannot convert {Kind} to real")
            };
        }

        public bool AsBoolean()
        {
            return Kind switch
            {
                ValueKind.Boolean => (bool)Raw!,
                ValueKind.Int => (int)Raw! != 0,
                ValueKind.Real => Math.Abs((double)Raw!) > double.Epsilon,
                ValueKind.String => bool.Parse((string)Raw!),
                _ => throw new InvalidOperationException($"Cannot convert {Kind} to boolean")
            };
        }

        public string AsString()
        {
            return Kind switch
            {
                ValueKind.String => (string)Raw!,
                ValueKind.Int => ((int)Raw!).ToString(CultureInfo.InvariantCulture),
                ValueKind.Real => ((double)Raw!).ToString(CultureInfo.InvariantCulture),
                ValueKind.Boolean => ((bool)Raw!).ToString(),
                ValueKind.Null => "null",
                ValueKind.Array => "[array]",
                _ => Raw?.ToString() ?? string.Empty
            };
        }

        public ArrayValue AsArray()
        {
            if (Kind != ValueKind.Array || Raw is not ArrayValue arr)
                throw new InvalidOperationException($"Value is not an array (was {Kind})");
            return arr;
        }

        public override string ToString() => AsString();
    }
}
