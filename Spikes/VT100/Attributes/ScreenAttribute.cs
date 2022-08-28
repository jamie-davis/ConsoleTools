using System;

namespace VT100.Attributes
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    internal class ScreenAttribute : Attribute
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