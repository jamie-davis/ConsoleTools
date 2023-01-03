using System;
using System.Configuration;
using VT100.Attributes;

namespace VT100.ControlPropertyAnalysis
{
    public class PropertySetting
    {
        public PropertySetting(string property)
        {
            Property = property;
        }

        public string Property { get; }

        public virtual object GetValue()
        {
            return null;
        }

        public virtual Type GetValueType()
        {
            return null;
        }
        
        public T GetTypedValue<T>()
        {
            var value = GetValue();
            if (value is T typedValue)
                return typedValue;
                
            return default;
        }
    }
    
    public class PropertySetting<T> :  PropertySetting
    {
        public PropertySetting(string property, T value) : base(property)
        {
            Value = value;
        }

        public T Value { get; }

        #region Overrides of PropertySetting

        public override object GetValue() => Value;

        public override Type GetValueType() => typeof(T);

        #endregion
    }

}