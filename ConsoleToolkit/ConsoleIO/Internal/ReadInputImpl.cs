using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Runtime.InteropServices;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.ConsoleIO.Internal;
using ConsoleToolkit.Exceptions;

namespace ConsoleToolkit.ConsoleIO.Internal
{
    /// <summary>
    /// This class prompts the user for the properties of a type. The types processed should be custom
    /// defined for use as input classes as there is no mechanism for skipping properties and not all
    /// property types will be appropriate for user input.
    /// <para/>
    /// The handling of string conversion errors depends on whether the console input stream has been
    /// redirected.<para/>
    /// If the stream has not been redirected, an error message will be displayed and the
    /// user will be prompted for the value again.
    /// <para/>
    /// If the input stream has been redirected, an error message will be displayed and the input process
    /// will proceed with the next item. When all of the data items have been attempted in this way, a 
    /// null will be returned.
    /// <para/>
    /// If all of the data items receive valid values, a populated instance of <see cref="T"/> will be returned.
    /// </summary>
    /// <typeparam name="T">The type being populated.</typeparam>
    internal class ReadInputImpl<T> where T : class
    {
        private interface ITargetTypeHandler
        {
            IEnumerable<InputItem<T>> Properties { get; }
            T MakeResult();
        }

        private class DefaultHandler : ITargetTypeHandler
        {
            private List<InputItem<T>> _properties;
            private T _template;

            public IEnumerable<InputItem<T>> Properties { get { return _properties; } }

            public DefaultHandler(T template = null)
            {
                _template = template;
                _properties = typeof(T).GetProperties().Select(MakeProperty).ToList();
            }

            public T MakeResult()
            {
                return InstanceConstructor<T>.MakeInstance(Properties);
            }

            private InputItem<T> MakeProperty(PropertyInfo prop)
            {
                IReadInfo inputInfo = null;
                var type = prop.PropertyType;
                if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Read<>))
                {
                    type = type.GetGenericArguments()[0];
                    if (_template == null)
                        MakeTemplateInstance();

                    inputInfo = prop.GetValue(_template, null) as IReadInfo;
                    if (inputInfo == null)
                        throw new ReadPropertyMustBeInitialised(prop.Name);
                }

                return new InputItem<T>
                {
                    Name = prop.Name,
                    Type = type,
                    Property = prop,
                    ReadInfo = inputInfo
                };
            }

            private void MakeTemplateInstance()
            {
                if (HasDefaultConstructor())
                    _template = Activator.CreateInstance<T>();
                else
                    throw new ReadPropertyInvalidWithoutTemplate();
            }

            private bool HasDefaultConstructor()
            {
                var constructor = typeof(T).GetConstructor(new Type[] { });
                return constructor != null;
            }
        }

        private class ReaderHandler : ITargetTypeHandler
        {
            private InputItem<T> _property;

            public ReaderHandler(T template)
            {
                _property = new InputItem<T>
                {
                    ReadInfo = (IReadInfo)template,
                    Type = ((IReadInfo)template).ValueType
                };
            }

            public IEnumerable<InputItem<T>> Properties
            {
                get { return new[] {_property}; }
            }

            public T MakeResult()
            {
                return (T)_property.Value;
            }
        }

        private readonly IConsoleInInterface _consoleIn;
        private IConsoleAdapter _consoleOut;
        private T _result;
        private ITargetTypeHandler _handler;

        public bool Successful { get; private set; }

        public T Result
        {
            get { return _result; }
        }

        public IEnumerable<InputItem<T>> Properties { get { return _handler.Properties; } }

        public ReadInputImpl(IConsoleInInterface consoleInInterface, IConsoleAdapter consoleOutInterface)
        {
            _consoleIn = consoleInInterface;
            _consoleOut = consoleOutInterface;
            
            GetHandler(null);

            SetResult();
        }

        public ReadInputImpl(IConsoleInInterface consoleInInterface, IConsoleAdapter consoleOutInterface, T template)
        {
            _consoleIn = consoleInInterface;
            _consoleOut = consoleOutInterface;

            GetHandler(template);

            SetResult();
        }

        private void GetHandler(T template)
        {
            if (ItemIsReadDerived())
                _handler = new ReaderHandler(template);
            else
                _handler = new DefaultHandler(template);
        }

        private bool ItemIsReadDerived()
        {
            var type = typeof (T);
            if (type.IsGenericType)
            {
                if (type.GetGenericTypeDefinition() == typeof (Read<>))
                    return true;
            }

            return false;
        }

        private void SetResult()
        {
            Successful = true;

            GetValues();

            if (Successful)
                _result = _handler.MakeResult();
        }

        private void GetValues()
        {
            foreach (var item in _handler.Properties)
            {
                if (!ReadInputItem.GetValue(item, _consoleIn, _consoleOut))
                {
                    Successful = false;

                    if (!_consoleIn.InputIsRedirected)
                        break;
                }
            }
        }
    }

    internal class InputItem<T> : IPropertySource where T : class
    {
        private object _value;
        public Type Type { get; internal set; }
        public string Name { get; internal set; }
        public PropertyInfo Property { get; internal set; }

        public object Value
        {
            get
            {
                return ReadInfo != null ? ReadInfo.MakeValueInstance(_value) : _value;
            }

            internal set
            {
                _value = value;
            }
        }

        public IReadInfo ReadInfo { get; set; }

        public string TypeDescription()
        {
            return String.Format("{0} {1}", Type.Name, Name);
        }
    }
}

/// <summary>
/// Read a value from console in to an <see cref="InputItem{T}"/>.
/// </summary>
internal static class ReadInputItem
{
    public static bool GetValue<T>(InputItem<T> item, IConsoleInInterface consoleIn, IConsoleAdapter consoleOut) where T : class
    {
        var redirected = consoleIn.InputIsRedirected;
        object value;

        string prompt = null;
        string optionString = null;
        if (item.ReadInfo != null)
        {
            if (item.ReadInfo.Prompt != null)
                prompt = item.ReadInfo.Prompt;

            if (item.ReadInfo.Options.Any())
            {
                var options = item.ReadInfo.Options.Select(o => string.Format("{0}-{1}", o.RequiredValue, o.Prompt));
                optionString = string.Format("[{0}]", string.Join(", ", options));
            }
        }

        if (prompt == null)
            prompt = PropertyNameConverter.ToPrompt(item.Property);

        var displayPrompt = string.Format("{0}{1}: ", prompt, optionString == null ? string.Empty : " " + optionString);

        do
        {
            consoleOut.Wrap(displayPrompt);
            if (ReadValue(item, consoleIn, consoleOut, out value))
            {
                item.Value = value;
                return true;
            }
        } while (!redirected);

        return false;
    }

    private static bool ReadValue<T>(InputItem<T> item, IConsoleInInterface consoleIn, IConsoleAdapter consoleOut, out object value) where T : class
    {
        var input = consoleIn.ReadLine();
        if (item.ReadInfo != null && item.ReadInfo.Options.Any())
            return SelectOption(input, item.ReadInfo.Options, consoleOut, out value);
        
        return ConvertString(input, item.Type, consoleOut, out value);
    }

    private static bool SelectOption(string input, IEnumerable<OptionDefinition> options, IConsoleAdapter consoleOut, out object result)
    {
        var hit = options.FirstOrDefault(o => o.RequiredValue == input);
        if (hit == null)
        {
            consoleOut.WrapLine(@"""{0}"" is not a valid selection.", input);
            result = null;
            return false;
        }

        result = hit.SelectedValue;
        return true;
    }

    private static bool ConvertString(string input, Type type, IConsoleAdapter consoleOut, out object result)
    {
        try
        {
            var conversion = typeof(Convert).GetMethods()
                .FirstOrDefault(m => m.ReturnType == type
                                     && m.GetParameters().Length == 1
                                     && m.GetParameters()[0].ParameterType == typeof(string));
            if (conversion != null)
            {
                result = conversion.Invoke(null, new object[] { input });
                return true;
            }

            result = null;
        }
        catch (TargetInvocationException e)
        {
            result = null;
            consoleOut.WrapLine(e.InnerException.Message);
        }
        return false;
    }

}