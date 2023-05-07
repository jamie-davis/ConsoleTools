using System;

namespace VT100.Attributes
{
    /// <summary>
    /// This indicates that the attributed class contains a screen definition. Individual properties can be
    /// given attributes such as <see cref="ButtonAttribute"/> or <see cref="TextBoxAttribute"/> to populate the
    /// screen with controls.
    /// </summary>
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class ScreenAttribute : Attribute
    {
        public bool GenerateExitButton { get; }
        public string ExitButtonCaption { get; }

        public ScreenAttribute(bool generateExitButton = true, string exitButtonCaption = null)
        {
            GenerateExitButton = generateExitButton;
            ExitButtonCaption = exitButtonCaption;
        }
    }
}