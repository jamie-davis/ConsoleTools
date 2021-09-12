namespace VT100.Tests.TestUtilities
{
    internal class ValueWrapper<T>
    {
        private T _value;
        public T Value => _value;

        public ValueWrapper(T initialValue)
        {
            _value = initialValue;
        }


        public object GetValue(object _)
        {
            return _value;
        }

        public void SetValue(object _, object value)
        {
            _value = (T)value;
        }
    }
}