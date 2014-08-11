using System;
using System.Linq.Expressions;

namespace ConsoleToolkit.CommandLineInterpretation
{
    /// <summary>
    /// A positional parameter. The framework must be able to convert the user's input string from the command line into 
    /// the specific type that this parameter accepts (<see cref="TParameter"/>)
    /// </summary>
    /// <typeparam name="TParameter">The parameter type. The text supplied by the user will be converted 
    /// to this type by the framework.</typeparam>
    /// <typeparam name="T">The command object type, an instance of which will be passed to the update routine (see <see cref="_positionalInitialiser"/>).</typeparam>
    public class CommandPositional<T, TParameter> : BasePositional where T :class
    {
        private readonly Action<T, TParameter> _positionalInitialiser;
        private Func<string, object> _converter;

        public override Type ParameterType { get { return typeof (TParameter); } }

        public CommandPositional(string parameterName, Action<T, TParameter> positionalInitialiser)
            : base(parameterName)
        {
            _positionalInitialiser = positionalInitialiser;
            if (!CommandLineInterpreterConfiguration.Converters.TryGetValue(typeof(TParameter), out _converter))
                throw new InvalidParameterType(typeof(TParameter));
        }

        public override string Accept(object command, string value)
        {
            var typeCastCommand = command as T;

            var parameter = _converter(value);
            if (parameter == null)
                return String.Format("The {0} parameter value \"{1}\" is invalid.", ParameterName, value);
            _positionalInitialiser(typeCastCommand, (TParameter)parameter);
            return null;
        }
    }
}