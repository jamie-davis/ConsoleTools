using System;
using System.Collections.Generic;
using System.Linq;
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
        private readonly List<ValidationFunc> _validators = new List<ValidationFunc>();

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

        /// <summary>
        /// Display the options in menu format.
        /// </summary>
        /// <param name="heading">The heading for the menu. If null, no heading will be displayed.</param>
        public Read<T> AsMenu(string heading)
        {
            _showAsMenu = true;
            _menuHeading = heading;
            return this;
        }

        /// <summary>
        /// Specifies a validation that input values must pass.<para/>
        /// Multiple validations may be specified, each with an appropriate error message. All validations must
        /// be passed for a value to be accepted.<para/>
        /// Validations will be applied in the order in which they are specified.
        /// </summary>
        /// <param name="validator">The validation that must return true for the value to be acceptable.</param>
        /// <param name="errorMessage">The error message to display if the validation fails.</param>
        public Read<T> Validate(Func<T, bool> validator, string errorMessage)
        {
            _validators.Add(new ValidationFunc(validator, errorMessage));
            return this;
        }

        public class ValidationFunc
        {
            public Func<T, bool> Validator { get; private set; }
            public string ErrorMessage { get; private set; }

            public ValidationFunc(Func<T, bool> validator, string errorMessage)
            {
                Validator = validator;
                ErrorMessage = errorMessage;
            }
        }

        public static implicit operator T(Read<T> item)
        {
            return item.Value;
        }

        /// <summary>
        /// The value received.
        /// </summary>
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

        string IReadInfo.GetValidationError(object value)
        {
            var failedValidation = _validators.FirstOrDefault(v => !v.Validator((T) value));
            if (failedValidation != null)
                return failedValidation.ErrorMessage;

            return null;
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
