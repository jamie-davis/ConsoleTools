using System;
using System.Collections.Generic;
using VT100.ControlPropertyAnalysis;
using VT100.FullScreen.ControlBehaviour;
using VT100.FullScreen.ScreenLayers;
using VT100.Utilities.ReadConsole;

namespace VT100.FullScreen
{
    internal interface ILayoutControl
    {
        int Column { get; }
        int Row { get; }
        int Width { get; }
        int Height { get; }
        
        IEnumerable<BoxRegion> BoxRegions { get; }
        
        void PropertyBind(IFullScreenApplication app, object layout, Func<object, object> getter,
            Action<object, object> setter);
        void MethodBind(IFullScreenApplication app, object layout, Func<object, bool> method);
        string Caption { get; }
        void Render(IFullScreenConsole console);

        (int Width, int Height) GetRequestedSize()
        {
            return (1, 1);
        }
        
        void Position(int column, int row, int width, int height);

        bool CanAcceptFocus => true;
        void SetFocus(IFullScreenConsole console);
        void Accept(IFullScreenConsole console, ControlSequence next);
        
        BorderBorderStyle BorderBorderStyle { get; }
        void Refresh(IFullScreenConsole console);
    }

    /// <summary>
    /// This is used to indicate the property definitions used by a control. The control will receive property values via instance of the <see cref="TFormat"/> parameter
    /// referenced through <see cref="Format"/>.
    /// </summary>
    /// <typeparam name="TFormat">The type defining property settings through members. The properties will be given values from the properties visible to the implementing control.
    /// See <see cref="ControlPropertySetter"/>.</typeparam>
    interface IFormattedLayoutControl<TFormat> : ILayoutControl 
        where TFormat : class, new()
    {
        /// <summary>
        /// The instance of the <see cref="TFormat"/> type that will receive property settings.
        /// The control can initialise this but it will normally be instantiated by the framework. 
        /// </summary>
        public TFormat Format { get; set; }

        /// <summary>
        /// Return the minimum size for this control. If the control has settings that limit the size, these should
        /// be taken into account. 
        /// </summary>
        /// <returns>A width and height</returns>
        (int Width, int Height) GetMinSize();

        /// <summary>
        /// Return the maximum size for this control. If the control has settings that limit the size, these should
        /// be taken into account. 
        /// </summary>
        /// <param name="visibleWidth">The constraining screen area available for the control. This can be ignored,
        /// but is used to provide reference so that if the control can occupy unlimited space, </param>
        /// <returns>A width and height</returns>
        (int Width, int Height) GetMaxSize(int visibleWidth, int visibleHeight);

    }
}