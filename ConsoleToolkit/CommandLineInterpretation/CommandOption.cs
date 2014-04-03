using System;
using System.Collections.Generic;
using System.Linq;

namespace ConsoleToolkit.CommandLineInterpretation
{
    /// <summary>
    /// A command option. The framework must be able to convert the user's input strings from the command 
    /// line into the specific types that this option accepts.  The types are extracted from the 
    /// specification of the provided action (<see cref="TAction"/>).
    /// </summary>
    /// <typeparam name="TAction">The action that will be executed when the option is supplied with parameters that can be converted.</typeparam>
    public class CommandOption<TAction> : BaseOption
    {
        private readonly TAction _optionInitialiser;
        private List<Type> _paramTypes;
        private Type _actionType;

        public CommandOption(string optionName, TAction optionInitialiser)
        {
            _optionInitialiser = optionInitialiser;
            Name = optionName;

            AnalyseAction();
        }

        private void AnalyseAction()
        {
            _actionType = _optionInitialiser.GetType();
            if (_actionType.IsGenericType)
            {
                var genericArguments = _actionType.GetGenericArguments();
                _paramTypes = genericArguments.Skip(1).ToList();
                ParameterCount = _paramTypes.Count();

                var invalidParamType = _paramTypes.FirstOrDefault(t => t != typeof(string) && !CommandLineInterpreterConfiguration.Converters.ContainsKey(t));
                if (invalidParamType != null)
                    throw new InvalidParameterType(invalidParamType);
            }
        }

        public override void Apply(object command, IEnumerable<string> parameters, out string error)
        {
            object[] callParameters;
            if (CreateCallParameters(command, parameters, out callParameters, out error))
                _actionType.GetMethod("Invoke").Invoke(_optionInitialiser, callParameters);
            else if (error == null)
                error = "Internal error parsing command.";
        }

        private bool CreateCallParameters(object command, IEnumerable<string> parameters, out object[] callParameters, out string error)
        {
            error = null;

            var convertedParameters = new List<object> {command};

            using (var paramEnumerator = parameters.GetEnumerator())
            {
                var index = 0;
                while (paramEnumerator.MoveNext())
                {
                    var parameter = paramEnumerator.Current;

                    if (index >= _paramTypes.Count)
                    {
                        callParameters = null;
                        error = String.Format("The {0} option has too many parameters.", Name);
                        return false;
                    }


                    if (!AddParameter(_paramTypes[index++], ref convertedParameters, parameter))
                    {
                        callParameters = null;
                        error = String.Format("The parameter \"{0}\" of the {1} option has an invalid value.", parameter, Name);
                        return false;
                    }
                }

                if (IsBoolean && index == 0)
                {
                    callParameters = new[] { command, true };
                    return true;
                }

                if (ParameterCount == 0)
                {
                    callParameters = new [] { command };
                    return true;
                }

                if (index < ParameterCount)
                {
                    callParameters = null;
                    error = String.Format("Not enough parameters for the {0} option.", Name);
                    return false;
                }

                callParameters = convertedParameters.ToArray();
                return true;
                        
            }
        }

        private bool AddParameter(Type paramType, ref List<object> parameterList, string parameterText)
        {
            object value = null;
            if (typeof (string) == paramType)
                value = parameterText;
            else
            {
                Func<string, object> converter;
                if (CommandLineInterpreterConfiguration.Converters.TryGetValue(paramType, out converter))
                    value = converter(parameterText);
            }
            parameterList.Add(value);
            return value != null;
        }
    }
}