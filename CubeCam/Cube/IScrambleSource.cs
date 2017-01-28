namespace CubeCam.Cube
{
    internal interface IScrambleSource
    {
        /// <summary>
        /// Returns the next scramble from the source. 
        /// </summary>
        /// <returns>The next scramble or null if there are no more scrambles available.</returns>
        string GetNextScramble();
    }
}
