using System;
using System.Reflection;

namespace ConsoleToolkit.ConsoleIO.Internal
{
    internal class InputItem : IPropertySource
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