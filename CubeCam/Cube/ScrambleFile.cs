using System.Linq;

namespace CubeCam.Cube
{
    /// <summary>
    /// A source of scrambles read from a text file. The file should consist of a new scramble every line.
    /// The scrambles can begin with any characters other than the standard faces (D, U, F, L, B, R) and
    /// everything from the first face character is considered part of the scramble.
    /// </summary>
    internal class ScrambleFile : IScrambleSource
    {
        public ScrambleFile(string fileName)
        {
            string[] lines = System.IO.File.ReadAllLines(fileName);
            // Each line can begin with a preamble to distinguish each scramble (e.g. "5." to indicate scramble 5).
            // We skip ahead to the first instance of a face character.
            scrambles = lines.Select(line => line.Substring(line.IndexOfAny("DUFLBR".ToCharArray()))).ToArray();
        }

        public string GetNextScramble()
        {
            return (index < scrambles.Length) ? scrambles[index++] : null;
        }

        private string[] scrambles;
        private int index = 0;
    }
}
