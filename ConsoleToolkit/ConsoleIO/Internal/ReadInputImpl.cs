using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
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
            IEnumerable<InputItem> Properties { get; }
            T MakeResult();
        }

        private class DefaultHandler : ITargetTypeHandler
        {
            private List<InputItem> _properties;
            private T _template;

            public IEnumerable<InputItem> Properties { get { return _properties; } }

            public DefaultHandler(T template = null)
            {
                _template = template;
                _properties = typeof(T).GetProperties().Select(MakeProperty).ToList();
            }

            public T MakeResult()
            {
                return InstanceConstructor<T>.MakeInstance(Properties);
            }

            private InputItem MakeProperty(PropertyInfo prop)
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

                return new InputItem
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
            private InputItem _property;

            public ReaderHandler(T template)
            {
                _property = new InputItem
                {
                    ReadInfo = (IReadInfo)template,
                    Type = ((IReadInfo)template).ValueType
                };
            }

            public IEnumerable<InputItem> Properties
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

        public IEnumerable<InputItem> Properties { get { return _handler.Properties; } }

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
}