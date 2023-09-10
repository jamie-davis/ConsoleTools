using System;

namespace VT100.Attributes
{
    public enum StackDirection
    {
        Vertical,
        Horizontal
    }
    
    public class StackLayoutAttribute : Attribute
    {
        public StackDirection StackDirection { get; set; }    
    }
}