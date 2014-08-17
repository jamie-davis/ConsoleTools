using System;
using System.Collections.Generic;
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
        private readonly List<OptionDefinition> _options = new List<OptionDefinition>();
        private bool _showAsMenu;
        private string _menuHeading;
        internal Read() { }

        /// <summary>
        /// Specify the prompt to display to the user.
        /// </summary>
        /// <param name="prompt">The prompt text.</param>
        public Read<T> Prompt(string prompt)
        {
            _prompt = prompt;
            return this;
        }

        /// <summary>
        /// Specify an option 
        /// </summary>
        /// <param name="selectedValue"></param>
        /// <param name="requiredValue"></param>
        /// <param name="prompt"></param>
        public Read<T> Option(T selectedValue, string requiredValue, string prompt)
        {
            _options.Add(new OptionDefinition
            {
                Prompt = prompt,
                SelectedValue = selectedValue,
                RequiredValue = requiredValue
            });
            return this;
        }

        public Read<T> AsMenu(string heading)
        {
            _showAsMenu = true;
            _menuHeading = heading;
            return this;
        }

        public static implicit operator T(Read<T> item)
        {
            return item.Value;
        }

        public T Value { get; set; }

        string IReadInfo.Prompt { get { return _prompt; } }

        IEnumerable<OptionDefinition> IReadInfo.Options { get {return _options; } }

        Type IReadInfo.ValueType { get { return typeof (T); } }

        bool IReadInfo.ShowAsMenu { get { return _showAsMenu; } }

        string IReadInfo.MenuHeading { get { return _menuHeading; } }

        object IReadInfo.MakeValueInstance(object value)
        {
            var newItem = new Read<T>();
            newItem.Value = (T)value;
            return newItem;
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
