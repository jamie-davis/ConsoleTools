using System;
using VT100.FullScreen;

namespace VT100.Attributes
{
    /// <summary>
    /// Base class for all simple colour attributes. This simplifies their definitions.
    /// </summary>
    [AttributeUsage(AttributeTargets.Property | AttributeTargets.Class, AllowMultiple = false)]
    [PropertyAttribute]
    public abstract class BaseColourAttribute : Attribute
    {
        public VtColour Colour { get; }

        public BaseColourAttribute(VtColour colour)
        {
            Colour = colour;
        }
    }
}