using System;

namespace BOOSE.Runtime
{
    /// <summary>
    /// Typed array value for BOOSE.
    /// </summary>
    public sealed class ArrayValue
    {
        public ValueKind ElementKind { get; }
        private readonly Value[] _items;

        public int Length => _items.Length;

        public ArrayValue(ValueKind elementKind, int length)
        {
            if (length < 0) throw new ArgumentOutOfRangeException(nameof(length));
            ElementKind = elementKind;
            _items = new Value[length];
            for (int i = 0; i < length; i++)
            {
                _items[i] = elementKind switch
                {
                    ValueKind.Int => Value.FromInt(0),
                    ValueKind.Real => Value.FromReal(0.0),
                    ValueKind.Boolean => Value.FromBoolean(false),
                    ValueKind.String => Value.FromString(string.Empty),
                    _ => Value.Null
                };
            }
        }

        public Value Get(int index)
        {
            if (index < 0 || index >= _items.Length)
                throw new IndexOutOfRangeException($"Array index {index} out of range (0..{_items.Length - 1})");
            return _items[index];
        }

        public void Set(int index, Value value)
        {
            if (index < 0 || index >= _items.Length)
                throw new IndexOutOfRangeException($"Array index {index} out of range (0..{_items.Length - 1})");

            // Basic type enforcement (numeric promotion allowed Int -> Real)
            if (ElementKind == ValueKind.Real && value.Kind == ValueKind.Int)
            {
                _items[index] = Value.FromReal(value.AsReal());
                return;
            }

            if (value.Kind != ElementKind)
            {
                // Allow assigning numeric to numeric
                if ((ElementKind == ValueKind.Int || ElementKind == ValueKind.Real) &&
                    (value.Kind == ValueKind.Int || value.Kind == ValueKind.Real))
                {
                    _items[index] = ElementKind == ValueKind.Int ? Value.FromInt(value.AsInt()) : Value.FromReal(value.AsReal());
                    return;
                }

                throw new InvalidOperationException($"Cannot assign {value.Kind} to array of {ElementKind}");
            }

            _items[index] = value;
        }
    }
}
