using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using ConsoleToolkit.Annotations;

namespace ConsoleToolkit.ConsoleIO.Internal
{
    public interface IReadInfo
    {
        string Prompt { get; }
        object MakeValueInstance(object value);
    }

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
        private readonly IConsoleInInterface _consoleIn;
        private IConsoleAdapter _consoleOut;
        private T _template;
        private T _result;

        public bool Successful { get; private set; }

        public T Result
        {
            get { return _result; }
        }

        public List<InputItem> Properties { get; set; }

        internal class InputItem
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
                return string.Format("{0} {1}", Type.Name, Name);
            }
        }

        public ReadInputImpl(IConsoleInInterface consoleInInterface, IConsoleAdapter consoleOutInterface)
        {
            _consoleIn = consoleInInterface;
            _consoleOut = consoleOutInterface;
            SetResult();
        }

        public ReadInputImpl(IConsoleInInterface consoleInInterface, IConsoleAdapter consoleOutInterface, T template)
        {
            _consoleIn = consoleInInterface;
            _consoleOut = consoleOutInterface;
            _template = template;
            SetResult();
        }

        private void SetResult()
        {
            Successful = true;
            Properties = typeof (T).GetProperties().Select(MakeProperty).ToList();

            GetValues();

            if (Successful)
                _result = MakeResult();
        }

        private T MakeResult()
        {
            if (HasConstructor())
                return MakeInstanceUsingConstructor();

            return MakeInstanceUsingMemberSetters();
        }

        private bool HasConstructor()
        {
            var constructors = typeof (T).GetConstructors()
                .Where(ConstructorHasMatchingParameters);
            var num = constructors.Count();
            return num == 1;
        }

        private bool HasDefaultConstructor()
        {
            var constructor = typeof (T).GetConstructor(new Type[] {});
            return constructor != null;
        }

        private bool ConstructorHasMatchingParameters(ConstructorInfo ctor)
        {
            var props = Properties
                .Select((p, i) => new {Index = i, Type = p.Property.PropertyType})
                .ToList();
            var ctorParams = ctor.GetParameters()
                .Select((p, i) => new {Index = i, Type = p.ParameterType});

            return props.Intersect(ctorParams).Count() == props.Count();
        }

        private T MakeInstanceUsingConstructor()
        {
            var ctor = typeof (T).GetConstructors().FirstOrDefault(ConstructorHasMatchingParameters);
            Debug.Assert(ctor != null);

            var args = Properties.Select(item => item.Value).ToArray();
            return (T) ctor.Invoke(args);
        }

        private T MakeInstanceUsingMemberSetters()
        {
            var instance = Activator.CreateInstance(typeof (T));
            foreach (var item in Properties)
            {
                item.Property.SetValue(instance, item.Value, null);
            }
            return (T) instance;
        }

        private void GetValues()
        {
            foreach (var item in Properties)
            {
                GetValue(item);

                if (!Successful && !_consoleIn.InputIsRedirected)
                    break;
            }
        }

        private void GetValue(InputItem item)
        {
            var redirected = _consoleIn.InputIsRedirected;
            object value;

            string prompt;
            if (item.ReadInfo != null && item.ReadInfo.Prompt != null)
                prompt = item.ReadInfo.Prompt;
            else
                prompt = PropertyNameConverter.ToPrompt(item.Property);

            do
            {
                _consoleOut.Wrap(prompt);
                if (ReadValue(item.Type, out value))
                {
                    item.Value = value;
                    return;
                }
            } while (!redirected);

            Successful = false;
        }

        private bool ReadValue(Type type, out object value)
        {
            var input = _consoleIn.ReadLine();
            if (!ConvertString(input, type, out value))
            {
                return false;
            }

            return true;
        }

        private bool ConvertString(string input, Type type, out object result)
        {
            try
            {
                var conversion = typeof (Convert).GetMethods()
                    .FirstOrDefault(m => m.ReturnType == type
                                         && m.GetParameters().Length == 1
                                         && m.GetParameters()[0].ParameterType == typeof (string));
                if (conversion != null)
                {
                    result = conversion.Invoke(null, new object[] {input});
                    return true;
                }

                result = null;
            }
            catch (TargetInvocationException e)
            {
                result = null;
                _consoleOut.WrapLine(e.InnerException.Message);
            }
            return false;
        }

        private InputItem MakeProperty(PropertyInfo prop)
        {
            IReadInfo inputInfo = null;
            var type = prop.PropertyType;
            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof (Read<>))
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

    }

    public class ReadPropertyInvalidWithoutTemplate : Exception
    {
        public ReadPropertyInvalidWithoutTemplate() : base("Objects with Read properties cannot be used for input without a template instance.")
        {
            
        }
    }

    public class ReadPropertyMustBeInitialised : Exception
    {
        public string PropertyName { [UsedImplicitly] get; private set; }

        public ReadPropertyMustBeInitialised(string propertyName) : base(string.Format("The {0} property must have a templated value.", propertyName))
        {
            PropertyName = propertyName;
        }
    }
}
