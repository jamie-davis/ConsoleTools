using System;

namespace VT100.FullScreen
{
    [AttributeUsage(AttributeTargets.Class)]
    internal class ControlAttribute : Attribute
    {
        public Type IntroducingAttribute { get; }
        public bool BindsToProperty { get; }

        public ControlAttribute(Type introducingAttribute, bool bindsToProperty = true)
        {
            IntroducingAttribute = introducingAttribute;
            BindsToProperty = bindsToProperty;
            if (IntroducingAttribute == null || !typeof(Attribute).IsAssignableFrom(IntroducingAttribute))
                throw new Exception("Control attribute requires attribute parameter");
        }
    }
}