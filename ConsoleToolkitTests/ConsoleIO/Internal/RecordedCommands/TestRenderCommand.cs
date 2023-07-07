using System;
using ApprovalTests;
using ApprovalTests.Reporters;
using ApprovalUtilities.Utilities;
using ConsoleToolkit.ConsoleIO.Internal;
using ConsoleToolkit.ConsoleIO.Internal.RecordedCommands;
using ConsoleToolkitTests.ConsoleIO.UnitTestUtilities;
using ConsoleToolkitTests.TestingUtilities;
using Xunit;

namespace ConsoleToolkitTests.ConsoleIO.Internal.RecordedCommands
{
    [UseReporter(typeof (CustomReporter))]
    public class TestRenderCommand
    {
        private ReplayBuffer _buffer;
        private const int TestBufferWidth = 20;
        public TestRenderCommand()
        {
            _buffer = new ReplayBuffer(TestBufferWidth);
        }

        [Fact]
        public void RenderCommandReplaysRenderableData()
        {
            var renderer = new RecordingConsoleAdapter();
            renderer.Write("Rendering test");
            renderer.WriteLine();
            renderer.WriteLine(RulerFormatter.MakeRuler(20));
            renderer.Wrap("XXXXX XXXX XXXX XXXX XXXX XXXX XXXX XXXX XXXX");

            var command = new RenderCommand(renderer);

            _buffer.Write("===");
            command.Replay(_buffer);
            _buffer.Write("YYY");

            Approvals.Verify(_buffer.ToLines().JoinWith(Environment.NewLine));
        }
    }
}