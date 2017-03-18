using System.Collections.Generic;
using System.Linq;

namespace CubeCam.Cube
{
    /// <summary>
    /// A source of scrambles from a text list. The text should consist of a new scramble every line.
    /// The scrambles can begin with any characters other than the standard faces (D, U, F, L, B, R) and
    /// everything from the first face character is considered part of the scramble.
    /// </summary>
    internal class ScrambleList : IScrambleSource
    {
        public static ScrambleList ReadFromFile(string fileName)
        {
            return new ScrambleList(System.IO.File.ReadAllLines(fileName));
        }

        public ScrambleList(IEnumerable<string> lines)
        {
            // Each line can begin with a preamble to distinguish each scramble (e.g. "5." to indicate scramble 5).
            // We skip ahead to the first instance of a face letter. If there are no face letters then skip that line.
            var faceLetters = "DUFLBR".ToCharArray();
            scrambles = lines.Where(line => line.IndexOfAny(faceLetters) != -1)
                             .Select(line => line.Substring(line.IndexOfAny(faceLetters)))
                             .ToArray();
        }

        public string GetNextScramble()
        {
            return (index < scrambles.Length) ? scrambles[index++] : null;
        }

        private string[] scrambles;
        private int index = 0;
    }
}
