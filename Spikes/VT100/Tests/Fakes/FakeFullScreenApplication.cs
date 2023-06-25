using System.Collections.Generic;
using System.Linq;
using VT100.ControlPropertyAnalysis;
using VT100.FullScreen;
using VT100.FullScreen.ScreenLayers;

namespace VT100.Tests.Fakes
{
    internal class FakeFullScreenApplication : IFullScreenApplication
    {
        private readonly ILayout _layout;
        private ILayoutControl _focus;
        public PlateStack PlateStack { get; private set; }
        public ControlSet Controls { get; private set; }
        
        public List<LayedOutControl> LayedOutControls { get; private set; }
        public LayoutProperties LayoutProperties { get; private set; }
        public ScreenProps ScreenProps { get; private set; }
        public List<PropertySetting>Props { get; private set; }
        public PlateFullScreenConsole PlateInterface { get; private set; }

        public FakeFullScreenApplication(ILayout layout, int columns = 80, int rows = 25)
        {
            _layout = layout;
            Console = new FakeFullScreenConsole(columns, rows);
        }
        
        #region Implementation of IFullScreenApplication

        /// <summary>
        /// Configure and render the user interface. For the purposes of testing you can call this method and the
        /// application will be ready for input, or you can call the components of this call one at a time to
        /// examine state at intermediate stages and arrive at the same result. The stages are:
        /// <para/>
        /// <see cref="PrepareForLayout"/> - extracts the controls and properties from the layout, creates the types
        /// needed for positioning.
        /// <para/>
        /// <see cref="PositionOnPlate"/> - uses <see cref="Positioner"/> to determine positions for all of the controls
        /// based on the layout. After this call, <see cref="PlateInterface"/> will contain the rendered UI.
        /// <para/>
        /// <see cref="RenderToConsole"/> - Renders the UI previously set up in the <see cref="PlateInterface"/> to
        /// the <see cref="PlateFullScreenConsole"/> instance.
        /// </summary>
        public void Start()
        {
            PrepareForLayout();
            PositionOnPlate();
            RenderToConsole();
        }

        /// <summary>
        /// Position the controls and render them to the plate interface
        /// <remarks>After this call, <see cref="Controls"/> will have been initialised using the
        /// <see cref="Positioner"/>, and then rendered to the <see cref="PlateInterface"/>.</remarks> 
        /// </summary>
        public void PositionOnPlate()
        {
            Controls = Positioner.Position(0, 0, Console.WindowWidth, Console.WindowHeight, CaptionAlignment.Left,
                LayedOutControls, LayoutProperties);
            Controls.Render(Props, PlateInterface);
        }

        /// <summary>
        /// Render the contents of the plate interface to the console
        /// <remarks>After this call <see cref="PlateInterface"/> will have been initialised, the UI will have been
        /// rendered and focus will have been set via the <see cref="FocusController"/> held by the
        /// <see cref="Controls"/> <see cref="ControlSet"/>.</remarks>
        /// </summary>
        public void RenderToConsole()
        {
            PlateStack = new PlateStack(PlateInterface.Plate);
            PlateStack.Render(Console);
            Controls.FocusController.SetFocus(Console);
        }

        /// <summary>
        /// Extracts data from the <see cref="_layout"/> and prepares for positioning.
        /// <remarks>After this call, <see cref="LayedOutControls"/>, <see cref="Props"/>, <see cref="ScreenProps"/>,
        /// <see cref="PlateInterface"/> and <see cref="LayoutProperties"/> will have been initialised.</remarks> 
        /// </summary>
        public void PrepareForLayout()
        {
            LayedOutControls = LayoutControls.Extract(this, _layout).ToList();
            Props = ControlPropertyExtractor.Extract(_layout?.GetType());
            ScreenProps = new ScreenProps();
            PlateInterface =
                new PlateFullScreenConsole(Console.WindowWidth, Console.WindowHeight, ScreenProps.MakeBaseFormat());
            LayoutProperties = new LayoutProperties();
            ControlPropertySetter.Set(LayoutProperties, Props);
        }

        public void GotFocus(ILayoutControl focusControl)
        {
            _focus = focusControl;
        }

        public bool IsCursorModeInsert()
        {
            return InsertModeOn;
        }

        public void CloseScreen()
        {
            
        }

        public void ReRender()
        {
            
        }

        public FakeFullScreenConsole Console { get; }

        public bool InsertModeOn { get; set; } = true;

        #endregion
    }
}