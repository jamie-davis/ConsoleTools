using System;
using System.Collections.Generic;
using System.Text;
using VT100.FullScreen;
using VT100.Tests.Fakes;

namespace VT100.Tests.TestUtilities
{
    internal class ControlTestRig<T>
    {
        private readonly ILayoutControl _control;
        private readonly ValueWrapper<T> _valueWrapper;
        private readonly object[] _input;
        private readonly FakeFullScreenApplication _app;
        private readonly List<string> _keyReports = new List<string>();

        public ControlTestRig(ILayoutControl control, ValueWrapper<T> valueWrapper, object[] input)
        {
            _control = control;
            _valueWrapper = valueWrapper;
            _input = input;
            var fakeLayout = new FakeLayout();
            _app = new FakeFullScreenApplication(fakeLayout, 20, 1);
            _control.PropertyBind(_app, fakeLayout, _valueWrapper.GetValue, _valueWrapper.SetValue);
            _control.Render(_app.Console);
            _control.SetFocus(_app.Console);

            MakeInitialReport();
        }

        public FakeFullScreenConsole Console => _app.Console;
        
        private void MakeInitialReport()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Initial display:");
            sb.Append(GetDisplayReport());
            _keyReports.Add(sb.ToString());
        }

        public void RunTest()
        {
            void PostProcess(string keyRequested)
            {
                var sb = new StringBuilder();
                sb.AppendLine($"Processed: {keyRequested}");
                sb.Append(GetDisplayReport());
                _keyReports.Add(sb.ToString());
            }
            
            ControlInputFeeder.Process(_app.Console, _control, _input, PostProcess);
        }

        public string GetDisplayReport()
        {
            var sb = new StringBuilder();
            sb.Append(_app.Console.GetDisplayReport());
            sb.AppendLine();
            sb.AppendLine($"Value is: -->{_valueWrapper.Value}<--");
            return sb.ToString();
        }

        public string GetReport()
        {
            var sb = new StringBuilder();
            foreach (var keyReport in _keyReports)
            {
                sb.AppendLine(keyReport);
            }

            return sb.ToString();
        }
        
    }
}