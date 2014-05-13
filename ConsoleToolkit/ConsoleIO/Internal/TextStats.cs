using System.Collections.Generic;
using System.Linq;

namespace ConsoleToolkit.ConsoleIO.Internal
{
    /// <summary>
    /// This class maintains statistics about text values it has been passed.
    /// 
    /// This class is not thread safe.
    /// </summary>
    public class TextStats : ITextStats
    {
        private bool _minWidthSet;
        private double _totalWidth;
        private int _numValues;

        private Dictionary<int, long> _lengthTallies = new Dictionary<int, long>(); 

        public void Add(string value)
        {
            var length = value.Length;

            if (length > MaxWidth)
                MaxWidth = length;

            if (!_minWidthSet || length < MinWidth)
            {
                MinWidth = length;
                _minWidthSet = true;
            }

            if (_lengthTallies.ContainsKey(length))
                _lengthTallies[length]++;
            else
                _lengthTallies[length] = 1;

            _totalWidth += length;
            _numValues++;
        }

        public int MaxWidth { get; private set; }
        public int MinWidth { get; private set; }
        public double AvgWidth { get { return _totalWidth/_numValues; }
        }

        public IEnumerable<LengthTally> LengthTallies
        {
            get { return _lengthTallies.Select(kv => new LengthTally(kv.Key, kv.Value)); }
        }

        public class LengthTally
        {
            public LengthTally(int length, long count)
            {
                Length = length;
                Count = count;
            }

            public int Length { get; private set; }
            public long Count { get; private set; }
        }
    }
}
