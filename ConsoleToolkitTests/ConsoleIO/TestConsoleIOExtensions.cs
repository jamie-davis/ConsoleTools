using ApprovalTests.Reporters;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.ConsoleIO.Internal;
using ConsoleToolkitTests.TestingUtilities;
using Xunit;

namespace ConsoleToolkitTests.ConsoleIO
{
    [UseReporter(typeof (CustomReporter))]
    public class TestConsoleIOExtensions
    {
        public static readonly object[][] FGColourExtensionTestCases =
        {
            new object[] {"Black".Black(), "{>PSk}Black{<p}"},
            new object[] {"DarkBlue".DarkBlue(), "{>PSB}DarkBlue{<p}"},
            new object[] {"DarkGreen".DarkGreen(), "{>PSG}DarkGreen{<p}"},
            new object[] {"DarkCyan".DarkCyan(), "{>PSC}DarkCyan{<p}"},
            new object[] {"DarkRed".DarkRed(), "{>PSR}DarkRed{<p}"},
            new object[] {"DarkMagenta".DarkMagenta(), "{>PSM}DarkMagenta{<p}"},
            new object[] {"DarkYellow".DarkYellow(), "{>PSY}DarkYellow{<p}"},
            new object[] {"Gray".Gray(), "{>PSx}Gray{<p}"},
            new object[] {"DarkGray".DarkGray(), "{>PSX}DarkGray{<p}"}, 
            new object[] {"Blue".Blue(), "{>PSb}Blue{<p}"},
            new object[] {"Green".Green(), "{>PSg}Green{<p}"},
            new object[] {"Cyan".Cyan(), "{>PSc}Cyan{<p}"},
            new object[] {"Red".Red(), "{>PSr}Red{<p}"},
            new object[] {"Magenta".Magenta(), "{>PSm}Magenta{<p}"},
            new object[] {"Yellow".Yellow(), "{>PSy}Yellow{<p}"},
            new object[] {"White".White(), "{>PSw}White{<p}"}
        };

        public static readonly object[][] BGColourExtensionTestCases =
        {
            new object[] {"Black".BGBlack(), "{>Psk}Black{<p}"},
            new object[] {"DarkBlue".BGDarkBlue(), "{>PsB}DarkBlue{<p}"},
            new object[] {"DarkGreen".BGDarkGreen(), "{>PsG}DarkGreen{<p}"},
            new object[] {"DarkCyan".BGDarkCyan(), "{>PsC}DarkCyan{<p}"},
            new object[] {"DarkRed".BGDarkRed(), "{>PsR}DarkRed{<p}"},
            new object[] {"DarkMagenta".BGDarkMagenta(), "{>PsM}DarkMagenta{<p}"},
            new object[] {"DarkYellow".BGDarkYellow(), "{>PsY}DarkYellow{<p}"},
            new object[] {"Gray".BGGray(), "{>Psx}Gray{<p}"},
            new object[] {"DarkGray".BGDarkGray(), "{>PsX}DarkGray{<p}"}, 
            new object[] {"Blue".BGBlue(), "{>Psb}Blue{<p}"},
            new object[] {"Green".BGGreen(), "{>Psg}Green{<p}"},
            new object[] {"Cyan".BGCyan(), "{>Psc}Cyan{<p}"},
            new object[] {"Red".BGRed(), "{>Psr}Red{<p}"},
            new object[] {"Magenta".BGMagenta(), "{>Psm}Magenta{<p}"},
            new object[] {"Yellow".BGYellow(), "{>Psy}Yellow{<p}"},
            new object[] {"White".BGWhite(), "{>Psw}White{<p}"}
        };

        [Theory]
        [MemberData("FGColourExtensionTestCases")]
        public void FGColourExtensionAddsControlSequenceToString(string testCase, string expected)
        {
            var text = SequenceVisualizer(testCase);
            Assert.Equal(expected, text);
        }

        [Theory, MemberData("BGColourExtensionTestCases")]
        public void BGColourExtensionAddsControlSequenceToString(string testCase, string expected)
        {
            var text = SequenceVisualizer(testCase);
            Assert.Equal(expected, text);
        }

        private string SequenceVisualizer(string data)
        {
            return data
                    .Replace(AdapterConfiguration.ControlSequenceIntroducer, '{')
                    .Replace(AdapterConfiguration.ControlSequenceTerminator, '}');
        }
    }
}
