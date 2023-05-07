using System;
using VT100.Attributes;
using VT100.FullScreen.ControlBehaviour;

/// <summary>
/// A specification for a border
/// </summary>
[AttributeUsage(AttributeTargets.Class|AttributeTargets.Property|AttributeTargets.Method)]
[PropertyAttribute]
public class BorderAttribute : Attribute
{
    public IBorderStyle Border { get; }

    public BorderAttribute(BorderType borderType)
    {
        Border = new BorderBorderStyle
        {
            TopBorder = borderType,
            RightBorder = borderType,
            BottomBorder = borderType,
            LeftBorder = borderType
        };
    }
    
    public BorderAttribute(BorderType topBorder, BorderType rightBorder, BorderType bottomBorder, BorderType leftBorder)
    {
        Border = new BorderBorderStyle
        {
            TopBorder = topBorder,
            RightBorder = rightBorder,
            BottomBorder = bottomBorder,
            LeftBorder = leftBorder
        };
    }
}