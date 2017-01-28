using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CubeCam.Extensions
{
    internal static class IEnumerableExtensions
    {
        /// <summary>
        /// Returns the index of the minimum element in the given sequence. If two or more are tied, then the index of the
        /// first tied element is returned.
        /// </summary>
        public static int MinIndex<T>(this IEnumerable<T> sequence) where T: IComparable<T>
        {
            int minIndex = 0;
            var min = default(T);
            int index = 0;
            foreach (var i in sequence)
            {
                if (index == 0 || i.CompareTo(min) < 0)
                {
                    min = i;
                    minIndex = index;
                }
                ++index;
            }
            return minIndex;
        }

        /// <summary>
        /// Returns the index of the maximum element in the given sequence. If two or more are tied, then the index of the
        /// last tied element is returned.
        /// </summary>
        public static int MaxIndex<T>(this IEnumerable<T> sequence) where T : IComparable<T>
        {
            int maxIndex = 0;
            var max = default(T);
            int index = 0;
            foreach (var i in sequence)
            {
                if (index == 0 || i.CompareTo(max) >= 0)
                {
                    max = i;
                    maxIndex = index;
                }
                ++index;
            }
            return maxIndex;
        }

        /// <summary>
        /// Drops the lowest and highest values from the given sequence and returns the average of the remaining elements.
        /// </summary>
        public static double AverageDropHighLow(this IEnumerable<double> sequence)
        {
            var count = sequence.Count();
            if (count < 3)
            {
                return 0.0;
            }
            var sum = sequence.Sum();
            var high = sequence.Max();
            var low = sequence.Min();
            return (sum - high - low) / (count - 2);
        }
    }
}
