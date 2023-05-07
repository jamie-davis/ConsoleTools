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
        public Positioner Positioner { get; private set; }

        public FakeFullScreenApplication(ILayout layout, int columns = 80, int rows = 25)
        {
            _layout = layout;
            Console = new FakeFullScreenConsole(columns, rows);
        }
        
        #region Implementation of IFullScreenApplication

        public void Start()
        {
            var layoutControls = LayoutControls.Extract(this, _layout).ToList();
            var props = ControlPropertyExtractor.Extract(_layout?.GetType());
            var screenProps = new ScreenProps();
            var plateInterface = new PlateFullScreenConsole(Console.WindowWidth, Console.WindowHeight, screenProps.MakeBaseFormat());
            Positioner = new Positioner(Console.WindowWidth, Console.WindowHeight, CaptionAlignment.Left, layoutControls, plateInterface);
            Positioner.Render(props);
            PlateStack = new PlateStack(plateInterface.Plate);
            PlateStack.Render(Console);
            Positioner.SetFocus(Console);
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