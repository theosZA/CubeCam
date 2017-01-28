using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeCam
{
    class TimeAggregator
    {
        public IEnumerable<TimeSpan> Times
        {
            get
            {
                return times;
            }
        }

        public TimeSpan Mean
        {
            get
            {
                return new TimeSpan((int)times.Select(time => time.Ticks).Average());
            }
        }

        public TimeSpan AverageDropHighLow
        {
            get
            {
                if (times.Count < 3)
                {
                    return new TimeSpan(0); 
                }
                var ticksSequence = times.Select(time => time.Ticks);
                var sum = ticksSequence.Sum();
                var high = ticksSequence.Max();
                var low = ticksSequence.Min();
                var average = (sum - high - low) / (times.Count - 2);
                return new TimeSpan(average);
            }
        }

        public string TextListDropHighLow
        {
            get
            {
                if (times.Count == 0)
                {
                    return "";
                }
                var minTimeIndex = 0;
                var maxTimeIndex = 0;
                for (int i = 1; i < times.Count; ++i)
                {
                    if (times[i] < times[minTimeIndex])
                    {
                        minTimeIndex = i;
                    }
                    if (times[i] > times[maxTimeIndex])
                    {
                        maxTimeIndex = i;
                    }
                }
                var text = new StringBuilder();
                for (int i = 0; i < times.Count; ++i)
                {
                    if (i > 0)
                    {
                        text.Append(", ");
                    }
                    if (i == minTimeIndex || i == maxTimeIndex)
                    {
                        text.Append('(');
                    }
                    text.Append(times[i].ToSecondsString());
                    if (i == minTimeIndex || i == maxTimeIndex)
                    {
                        text.Append(')');
                    }
                }
                return text.ToString();
            }
        }

        public void AddTime(TimeSpan newTime)
        {
            times.Add(newTime);
        }

        private IList<TimeSpan> times = new List<TimeSpan>();
    }
}
