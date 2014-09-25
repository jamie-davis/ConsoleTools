using System;
using System.Collections.Generic;
using System.Linq;
using ApprovalTests;
using ApprovalTests.Reporters;
using ApprovalUtilities.Utilities;
using ConsoleToolkit.ConsoleIO;
using ConsoleToolkit.ConsoleIO.Internal;
using ConsoleToolkitTests.TestingUtilities;
using NUnit.Framework;

namespace ConsoleToolkitTests.ConsoleIO.Internal
{
    [TestFixture]
    [UseReporter(typeof (CustomReporter))]
    public class TestColourWriter
    {
        private ConsoleInterfaceForTesting _consoleOut;
        private ColourWriter _writer;
        private static readonly Dictionary<string, ColourControlItem.ControlCode> Codes
            = new Dictionary<string, ColourControlItem.ControlCode>
                {
                    {"push", ColourControlItem.ControlCode.Push},
                    {"pop", ColourControlItem.ControlCode.Pop},
                    {"fore", ColourControlItem.ControlCode.SetForeground},
                    {"back", ColourControlItem.ControlCode.SetBackground},
                    {"newline", ColourControlItem.ControlCode.NewLine},
                };

        [SetUp]
        public void SetUp()
        {
            _consoleOut = new ConsoleInterfaceForTesting();
            _writer = new ColourWriter(_consoleOut);
        }

        [Test]
        public void TextIsOutputToConsole()
        {
            var components = new List<ColourControlItem>
            {
                new ColourControlItem("text")
            };
            _writer.Write(components);    
        
            Assert.That(_consoleOut.GetBuffer().TrimEnd(), Is.EqualTo("text"));
        }

        [Test]
        public void TextIsOutputInCorrectColour()
        {
            var components = new List<ColourControlItem>
            {
                new ColourControlItem(instructions: Instructions("fore,r", "back,b")),
                new ColourControlItem("text")
            };
            _writer.Write(components);    
        
            Approvals.Verify(_consoleOut.GetBuffer(ConsoleBufferFormat.Interleaved));
        }

        [Test]
        public void ColoursArePushedAndPopped()
        {
            var components = new List<ColourControlItem>
            {
                new ColourControlItem(instructions: Instructions("push")),
                new ColourControlItem(instructions: Instructions("fore,r", "back,b")),
                new ColourControlItem("text"),
                new ColourControlItem(instructions: Instructions("pop")),
                new ColourControlItem("more text"),
            };
            _writer.Write(components);    
        
            Approvals.Verify(_consoleOut.GetBuffer(ConsoleBufferFormat.Interleaved));
        }

        [Test]
        public void NewLineStartsANewLine()
        {
            var components = new List<ColourControlItem>
            {
                new ColourControlItem(instructions: Instructions("push")),
                new ColourControlItem(instructions: Instructions("fore,r", "back,b")),
                new ColourControlItem("text"),
                new ColourControlItem(instructions: Instructions("pop")),
                new ColourControlItem("more text"),
            };
            _writer.Write(components);    
            _writer.NewLine();
        
            Approvals.Verify(_consoleOut.GetBuffer(ConsoleBufferFormat.Interleaved));
        }

        [Test]
        public void NewLineInstructionStartsANewLine()
        {
            var components = new List<ColourControlItem>
            {
                new ColourControlItem("text"),
                new ColourControlItem(instructions: Instructions("newline")),
                new ColourControlItem("more text"),
            };
            _writer.Write(components);    
        
            Approvals.Verify(_consoleOut.GetBuffer(ConsoleBufferFormat.Interleaved));
        }

        [Test]
        public void NewLineIsSuppressedIfThePrecedingLineEndedAtRightEdge()
        {
            //move cursor to 4 from edge.
            _writer.Write(new List<ColourControlItem> { new ColourControlItem(new string('z', _consoleOut.BufferWidth - 4)) });

            var components = new List<ColourControlItem>
            {
                new ColourControlItem("text"),
                new ColourControlItem(instructions: Instructions("newline")),
                new ColourControlItem("more text"),
            };
            _writer.Write(components);    
        
            Approvals.Verify(_consoleOut.GetBuffer(ConsoleBufferFormat.Interleaved));
        }

        [Test]
        public void OnlyOneNewLineIsSuppressedIfThePrecedingLineEndedAtRightEdge()
        {
            //move cursor to 4 from edge.
            _writer.Write(new List<ColourControlItem> { new ColourControlItem(new string('z', _consoleOut.BufferWidth - 4)) });

            var components = new List<ColourControlItem>
            {
                new ColourControlItem("text"),
                new ColourControlItem(instructions: Instructions("newline", "newline")),
                new ColourControlItem("more text"),
            };
            _writer.Write(components);    
        
            Approvals.Verify(_consoleOut.GetBuffer(ConsoleBufferFormat.Interleaved));
        }

        [Test]
        public void NewLineIsOnlySuppressedIfThePrecedingLineEndedAtRightEdge()
        {
            //move cursor to 4 from edge.
            _writer.Write(new List<ColourControlItem> { new ColourControlItem(new string('z', _consoleOut.BufferWidth - 4)) });

            var components = new List<ColourControlItem>
            {
                new ColourControlItem("55555"),
                new ColourControlItem(instructions: Instructions("newline", "newline")),
                new ColourControlItem("more text"),
            };
            _writer.Write(components);    
        
            Approvals.Verify(_consoleOut.GetBuffer(ConsoleBufferFormat.Interleaved));
        }

        [Test]
        public void PrefixTextIsPrintedBeforeContent()
        {
            _writer.PrefixText = "prefix: ";
            _writer.Write(new List<ColourControlItem> { new ColourControlItem("data") });

            Approvals.Verify(_consoleOut.GetBuffer(ConsoleBufferFormat.Interleaved));
        }

        [Test]
        public void PrefixTextIsAddedWhenContentWraps()
        {
            _writer.PrefixText = "prefix: ";
            var text = Enumerable.Range(0, 20).Select(i => string.Format("text{0}", i)).JoinWith(" ");
            _writer.Write(new List<ColourControlItem> { new ColourControlItem(text) });

            Approvals.Verify(_consoleOut.GetBuffer(ConsoleBufferFormat.Interleaved));
        }

        [Test]
        public void TextColourDoesNotBleedIntoPrefix()
        {
            _writer.PrefixText = "prefix: ";
            var text = Enumerable.Range(0, 20).Select(i => string.Format("text{0}", i)).JoinWith(" ");
            var components = new List<ColourControlItem>
            {
                new ColourControlItem("first text "),
                new ColourControlItem(instructions: Instructions("push")),
                new ColourControlItem(instructions: Instructions("fore,r", "back,b")),
                new ColourControlItem(text),
                new ColourControlItem(instructions: Instructions("pop")),
                new ColourControlItem(" more text"),
            };
            _writer.Write(components);

            Approvals.Verify(_consoleOut.GetBuffer(ConsoleBufferFormat.Interleaved));
        }


        private IEnumerable<ColourControlItem.ControlInstruction> Instructions(params string[] instructions)
        {
            foreach (var instruction in instructions)
            {
                var arg = ConsoleColor.Black;
                ColourControlItem.ControlCode code;

                var parts = instruction.Split(',');
                if (!Codes.TryGetValue(parts[0], out code))
                    throw new Exception(string.Format("Bad instruction code: {0}", parts[0]));

                if (code == ColourControlItem.ControlCode.SetForeground 
                    || code == ColourControlItem.ControlCode.SetBackground)
                    arg = ColourConverter.Convert(parts[1]);

                yield return new ColourControlItem.ControlInstruction(code, arg);
            }
        }
    }
}