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
            _controls.FirstOrDefault(c => c.CanAcceptFocus)?.SetFocus(console);
        }

        private IEnumerable<ILayoutControl> AllFocusControls()
        {
            IEnumerable<ILayoutControl> Enumerate(IEnumerable<ILayoutControl> controls)
            {
                foreach (var layoutControl in controls)
                {
                    if (layoutControl is IRegionControl region)
                    {
                        foreach (var child in Enumerate(region.GetLayoutControls()))
                        {
                            yield return child;
                        }
                    }
                    else
                        yield return layoutControl;
                }
            }

            foreach (var focusControl in Enumerate(_controls)) yield return focusControl;
        } 
        
        public void NextFocus(IFullScreenConsole console, ILayoutControl layoutControl)
        {
            var allControls = AllFocusControls().ToList();
            var focusContainer = allControls.FirstOrDefault(c => ReferenceEquals(c, layoutControl));
            if (focusContainer == null)
            {
                SetFocus(console);
                return;
            }

            ILayoutControl control;
            var index = allControls.IndexOf(focusContainer);
            do
            {
                if (++index >= allControls.Count)
                {
                    SetFocus(console);
                    return;
                }

                control = allControls[index];
            } while (!control.CanAcceptFocus);

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