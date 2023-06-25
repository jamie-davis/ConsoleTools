using System.Collections.Generic;
using System.Linq;

namespace VT100.FullScreen
{
    internal class FocusController
    {
        private readonly List<ILayoutControl> _controls;

        public FocusController(IEnumerable<ILayoutControl> controls)
        {
            _controls = controls.ToList();
        }
        public void SetFocus(IFullScreenConsole console)
        {
            _controls.FirstOrDefault()?.SetFocus(console);
        }

        public void NextFocus(IFullScreenConsole console, ILayoutControl layoutControl)
        {
            var focusContainer = _controls.FirstOrDefault(c => ReferenceEquals(c, layoutControl));
            if (focusContainer == null)
            {
                SetFocus(console);
                return;
            }
            var index = _controls.IndexOf(focusContainer);
            if (index + 1 >= _controls.Count)
            {
                SetFocus(console);
                return;
            }

            var control = _controls[index + 1];
            control?.SetFocus(console);
        }

        public void PrevFocus(IFullScreenConsole console, ILayoutControl layoutControl)
        {
            var focusContainer = _controls.FirstOrDefault(c => ReferenceEquals(c, layoutControl));
            if (focusContainer == null)
            {
                SetFocus(console);
                return;
            }
            var index = _controls.IndexOf(focusContainer);
            var newIndex = index - 1;
            if (newIndex < 0 && _controls.Count > 0) newIndex = _controls.Count - 1;
            if (newIndex < 0) return;

            var control = _controls[newIndex];
            control?.SetFocus(console);
        }
        
    }
}