using System;
using ConsoleToolkit.ConsoleIO.Internal;

namespace ConsoleToolkit.ConsoleIO
{
    /// <summary>
    /// Object that allows parameters to be specified that influence the way in which
    /// data is captured from standard input.
    /// </summary>
    /// <typeparam name="T">The type to be read.</typeparam>
    public class Read<T> : IReadInfo
    {
        private string _prompt;

        internal Read() { }

        public Read<T> Prompt(string prompt)
        {
            _prompt = prompt;
            return this;
        }

        string IReadInfo.Prompt { get { return _prompt; } }
        public Type ValueType { get { return typeof (T); } }

        object IReadInfo.MakeValueInstance(object value)
        {
            var newItem = new Read<T>();
            newItem.Value = (T)value;
            return newItem;
        }

        public T Value { get; set; }

        public static implicit operator T(Read<T> item)
        {
            return item.Value;
        }
    }

    /// <summary>
    /// Static class allowing creation of <see cref="Read{T}"/> instances.
    /// </summary>
    public static class Read
    {
        public static Read<bool> Boolean() { return new Read<bool>(); }
        public static Read<int> Int() { return new Read<int>(); }
        public static Read<long> Long() { return new Read<long>(); }
        public static Read<double> Double(){ return new Read<double>();}
        public static Read<decimal> Decimal(){ return new Read<decimal>();}
        public static Read<DateTime> DateTime(){ return new Read<DateTime>();}
        public static Read<string> String(){ return new Read<string>();}
    }
}
