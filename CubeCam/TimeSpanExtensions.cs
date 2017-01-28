using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeCam
{
    static class TimeSpanExtensions
    {
        public static string ToSecondsString(this TimeSpan time)
        {
            return Math.Floor(time.TotalSeconds).ToString() + time.ToString("\\.ff");
        }
    }
}
