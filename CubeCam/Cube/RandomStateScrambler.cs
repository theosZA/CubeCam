using min2phase;
using System;
using System.Text.RegularExpressions;

namespace CubeCam.Cube
{
    /// <summary>
    /// A source of scrambles using a random state scrambler, that is a solvable cube is randomly chosen with equal probability
    /// to all other solvalble cubes, and a scramble is returned that yields that cube.
    /// </summary>
    internal class RandomStateScrambler : IScrambleSource
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
