using System;
using VT100.Attributes;
using VT100.FullScreen;

/// <summary>
/// A specification for caption alignment.
/// <para/>
/// <remarks>This property applies to layout across screens and regions and is not intended to control caption
/// positioning on a single control.</remarks>
/// </summary>
[AttributeUsage(AttributeTargets.Class)]
[PropertyAttribute]
public class CaptionPositionAttribute : Attribute
{
    public CaptionPosition CaptionPosition { get; }

    public CaptionPositionAttribute(CaptionPosition captionPosition)
    {
        CaptionPosition = captionPosition;
    }
}