using System;
using VT100.Attributes;

/// <summary>
/// A specification for a fixed width. The width will be interpreted by a control. For example, if you set the width
/// on a <see cref="TextBoxAttribute"/>, the width will be used to size the input area of the text box, but the label
/// will be sized by the layout container and will not be governed by the width. A button on the other hand, will use
/// the width to size the whole button width including the border, truncating its caption if needed.
/// <para/>
/// <remarks>Some controls cannot function below a certain width, and will not attempt to be smaller than their
/// functional minimum. For example, a textbox of width zero would not exist.</remarks>
/// </summary>
[AttributeUsage(AttributeTargets.Class|AttributeTargets.Property|AttributeTargets.Method)]
[PropertyAttribute]
public class WidthAttribute : Attribute
{
    public int Width { get; }

    public WidthAttribute(int width)
    {
        Width = width;
    }
}