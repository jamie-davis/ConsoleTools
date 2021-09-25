using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace VT100.Utilities.ReadConsole
{
    internal class ConsoleInputReader
    {
        private readonly CodeAnalyserSettings _analyserSettings;
        private bool _stop;
        public BlockingCollection<ControlSequence> Items { get; } = new BlockingCollection<ControlSequence>();

        internal IInputMonitor KeyMonitor { get; set; }

        public ConsoleInputReader(CodeAnalyserSettings analyserSettings = 0)
        {
            _analyserSettings = analyserSettings;
        }

        public void Stop()
        {
            _stop = true;
        }

        public async Task Read()
        {
            var seqAvailable = false;
            List<ControlElement> keys;

            void StartSequence()
            {
                keys = new List<ControlElement>();
            }

            void Switch()
            {
                if (keys.Count == 0)
                    return;

                KeyMonitor?.SequenceCaptured(keys);

                var controlSequence = AnsiRecognition.Split(keys, _analyserSettings);
                foreach (var sequence in controlSequence)
                {
                    Items.Add(sequence);
                }
                StartSequence();
            }

            bool CheckForNextKeyAvailable()
            {
                if (Console.KeyAvailable) return true;
                Thread.Yield();
                return Console.KeyAvailable;
            }
        
            StartSequence();
            while (!_stop)
            {
                var keyAvailable = CheckForNextKeyAvailable();
                if (seqAvailable && !keyAvailable)
                    Switch();

                if (_stop) continue;

                if (keyAvailable)
                {
                    var key = Console.ReadKey(true);
                    keys.Add(new ControlElement(key));
                    seqAvailable = true;
                }
            }

            Switch();

            if (_stop)
            {
                Items.CompleteAdding();
            }
        }
    }
}