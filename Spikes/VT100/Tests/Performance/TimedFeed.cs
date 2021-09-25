using System.Collections.Generic;
using System.Diagnostics;
// ReSharper disable UnusedType.Global
// ReSharper disable UnusedMember.Global

namespace VT100.Tests.Performance
{
    /// <summary>
    /// This is useful if you need to profile console input handling.
    /// <example>
    ///   <code>
    ///    [Fact]
    ///    public void PerformTestForTenSeconds()
    ///    {
    ///        var textBox = new TextBox();
    ///        textBox.Position(0,0,5,1);
    ///        var valueWrapper = new ValueWrapper&lt;string&gt;("Test long string");
    ///        var data = new object[] { ResolvedCode.End, 'A', 'B', 'C', ResolvedCode.Home, ResolvedCode.End, ResolvedCode.Backspace, ResolvedCode.Backspace, ResolvedCode.Backspace };
    ///        var testRig = new ControlTestRig&lt;string&gt;(textBox, valueWrapper, data);
    ///                
    ///        //Act
    ///        var timedFeed = new TimedFeed(data);
    ///        ControlInputFeeder.Process(textBox, timedFeed.Run(10000), null);
    ///        
    ///        _testOutputHelper.WriteLine($"Total characters fed: {timedFeed.NumCharactersFed:N}");
    ///    }
    ///   </code>
    /// </example>
    /// </summary>
    public class TimedFeed
    {
        private readonly object[] _data;
        private int _feedCount;
        public int NumCharactersFed => _feedCount;

        public TimedFeed(object[] data)
        {
            _data = data;
        }
        public IEnumerable<object> Run(int msToRun)
        {
            _feedCount = 0;
            var stopWatch = new Stopwatch();
            stopWatch.Start();
            
            bool NotDone()
            {
                return stopWatch.ElapsedMilliseconds < msToRun;
            }

            while (NotDone())
            {
                foreach (var value in _data)
                {
                    if (NotDone())
                        yield return value;
                    else
                        break;

                    ++_feedCount;
                }
            }
        }
    }
}