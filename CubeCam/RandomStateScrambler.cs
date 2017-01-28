using min2phase;
using System;
using System.Text.RegularExpressions;

namespace CubeCam
{
    public class RandomStateScrambler
    {
        public RandomStateScrambler(Random random = null)
        {
            Search.init();
            Tools.setRandomSource(random ?? new Random());
        }

        public string GetNextScramble()
        {
            var scramble = solver.solution(Tools.randomCube(), 20, 100000, 0, Search.INVERSE_SOLUTION);
            return Regex.Replace(scramble, @"\s\s+", " ");  // strip out extra spaces
        }

        private Search solver = new Search();
    }
}
