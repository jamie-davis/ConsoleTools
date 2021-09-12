using System;

namespace Vt100.FullScreen
{
    [AttributeUsage(AttributeTargets.Class)]
    internal class ControlAttribute : Attribute
    {
        public Type IntroducingAttribute { get; }

        public ControlAttribute(Type introducingAttribute)
        {
            IntroducingAttribute = introducingAttribute;
            IntroducingAttribute = introducingAttribute;
            if (IntroducingAttribute == null || !typeof(Attribute).IsAssignableFrom(IntroducingAttribute))
                throw new Exception("Control attribute requires attribute parameter");

        }
    }
}