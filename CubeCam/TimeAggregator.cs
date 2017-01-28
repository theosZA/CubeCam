using CubeCam.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace CubeCam
{
    class TimeAggregator
    {
        public IEnumerable<TimeSpan> Times => times;

        public TimeSpan Mean => new TimeSpan((int)TimeTicks.Average());

        public TimeSpan AverageDropHighLow => new TimeSpan((int)TimeTicks.AverageDropHighLow());

        public string TextListDropHighLow
        {
            get
            {
                if (times.Count == 0)
                {
                    return "";
                }
                var minTimeIndex = times.MinIndex();
                var maxTimeIndex = times.MaxIndex();
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

        private IEnumerable<double> TimeTicks => times.Select(time => (double)time.Ticks);

        private IList<TimeSpan> times = new List<TimeSpan>();
    }
}
