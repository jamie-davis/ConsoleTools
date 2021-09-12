using System;
using System.ComponentModel.Design;
using System.Data;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using VT100;
using VT100.Utilities;
using VT100.Utilities.ReadConsole;

namespace Vt100.FullScreen
{
    internal class FullScreenApplication : IDisposable, IFullScreenApplication
    {
        private readonly ILayout _layout;
        private readonly IVTModeControl _vtModeControl;
        private ScreenCapture _screenCapture;
        private ILayoutControl _focus;
        private IFullScreenConsole _console;

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
            var positioner = new Positioner(Console.WindowWidth, Console.WindowHeight, CaptionAlignment.Left, layoutControls);
            positioner.Render();
            positioner.SetFocus();

            var reader = new ConsoleInputReader(CodeAnalyserSettings.PreferPF3Modifiers);
            var monitor = new Monitor();
            reader.KeyMonitor = monitor;
            Task.Factory.StartNew(reader.Read, TaskCreationOptions.LongRunning);
            var goUI = false;
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
                _focus?.Accept(item);
            }
        }

        private bool ExitKey(ControlSequence next)
        {
            return next.ResolvedCode == ResolvedCode.Escape;
        }

        private bool ModeSwitchKey(ControlSequence next)
        {
            return next.ResolvedCode == ResolvedCode.Insert;
        }
    }

    internal class DefaultFullScreenConsole : IFullScreenConsole
    {
        #region Implementation of IFullScreenConsole

        public void Write(string text)
        {
            Console.Write(text);
        }

        public void Write(char? character)
        {
            Console.Write(character ?? ' ');
        }

        public void SetCursorPosition(int column, int row)
        {
            Console.SetCursorPosition(column, row);
        }

        public int WindowWidth => Console.WindowWidth;
        public int WindowHeight => Console.WindowHeight;

        #endregion
    }
}
