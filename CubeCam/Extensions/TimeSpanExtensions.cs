using System;

namespace CubeCam.Extensions
{
    internal static class TimeSpanExtensions
    {
        /// <summary>
        /// Formats a time-span as seconds with 2 decimal places, e.g. "126.99".
        /// </summary>
        /// <param name="time">A time-span to format.</param>
        /// <returns>The formatted time-span as text.</returns>
        public static string ToSecondsString(this TimeSpan time)
        {
            return Math.Floor(time.TotalSeconds).ToString() + time.ToString("\\.ff");
        }
    }
}
