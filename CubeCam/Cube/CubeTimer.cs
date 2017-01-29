using System;
using System.Diagnostics;

namespace CubeCam.Cube
{
    internal class CubeTimer
    {
        public TimeSpan InspectionTime => inspection.Elapsed;
        public TimeSpan SolveTime => solving.Elapsed;
        public TimeSpan TimeSinceSolved => afterSolve.Elapsed;
        public TimeSpan ResultsTime => results.Elapsed;

        public void Reset()
        {
            inspection.Reset();
            solving.Reset();
            afterSolve.Reset();
            results.Reset();
        }

        public void StartInspection()
        {
            inspection.Restart();
        }

        public void StartSolve()
        {
            inspection.Stop();
            solving.Restart();
        }

        public void EndSolve()
        {
            solving.Stop();
            afterSolve.Restart();
        }

        public void StartResults()
        {
            results.Restart();
        }

        private Stopwatch inspection = new Stopwatch();
        private Stopwatch solving = new Stopwatch();
        private Stopwatch afterSolve = new Stopwatch();
        private Stopwatch results = new Stopwatch();
    }
}
