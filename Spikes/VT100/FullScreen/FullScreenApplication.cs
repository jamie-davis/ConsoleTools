using System;
using System.Linq;
using System.Threading.Tasks;
using VT100.FullScreen.ScreenLayers;
using VT100.Utilities;
using VT100.Utilities.ReadConsole;

namespace VT100.FullScreen
{
    internal class FullScreenApplication : IDisposable, IFullScreenApplication
    {
        private readonly ILayout _layout;
        private readonly IVTModeControl _vtModeControl;
        private ScreenCapture _screenCapture;
        private ILayoutControl _focus;
        private bool _exit;

        public FullScreenApplication(ILayout layout, IVTModeControl vtModeControl, IFullScreenConsole console = null)
        {
            _layout = layout;
            _vtModeControl = vtModeControl;
            _screenCapture = new ScreenCapture();

            _layout.LayoutUpdated += LayoutUpdated;

            Console = console ?? new DefaultFullScreenConsole();
        }

        private void LayoutUpdated(object sender, LayoutUpdatedArgs args)
        {
            
        }

        ~FullScreenApplication()
        {
            var sc = _screenCapture;
            _screenCapture = null;
            sc?.Dispose();
        }

        public void GotFocus(ILayoutControl focusControl)
        {
            _focus = focusControl;
        }

        public bool IsCursorModeInsert()
        {
            return _vtModeControl.InsertModeOn;
        }

        public void CloseScreen()
        {
            _exit = true;
        }

        public void ReRender()
        {
            
        }

        public IFullScreenConsole Console { get; }

        #region IDisposable

        public void Dispose()
        {
            GC.SuppressFinalize(this);
            _screenCapture.Dispose();
        }

        #endregion

        public void Run()
        {
            var layoutControls = LayoutControls.Extract(this, _layout).ToList();
            var plateInterface = new PlateFullScreenConsole(Console.WindowWidth, Console.WindowHeight);
            var positioner = new Positioner(Console.WindowWidth, Console.WindowHeight, CaptionAlignment.Left, layoutControls, plateInterface);
            positioner.Render();
            var plateStack = new PlateStack(plateInterface.Plate);
            plateStack.Render(Console);
            positioner.SetFocus(Console);
            _exit = false;

            var reader = new ConsoleInputReader(CodeAnalyserSettings.PreferPF3Modifiers);
            var monitor = new Monitor();
            reader.KeyMonitor = monitor;
            Task.Factory.StartNew(reader.Read, TaskCreationOptions.LongRunning);
            foreach (var item in reader.Items.GetConsumingEnumerable())
            {
                if (ExitKey(item))
                {
                    reader.Stop();
                    continue;
                }
                
                if (ModeSwitchKey(item))
                {
                    _vtModeControl.ToggleInsertMode();
                    continue;
                }
                
                if (NextFocusKey(item))
                {
                    positioner.NextFocus(Console, _focus);
                    continue;
                }
                _focus?.Accept(Console, item);

                if (_exit)
                {
                    reader.Stop();
                }
            }
        }

        private bool ExitKey(ControlSequence next)
        {
            return next.ResolvedCode == ResolvedCode.Escape;
        }

        private bool NextFocusKey(ControlSequence next)
        {
            return next.ResolvedCode == ResolvedCode.Tab;
        }

        private bool ModeSwitchKey(ControlSequence next)
        {
            return next.ResolvedCode == ResolvedCode.Insert;
        }
    }
}
