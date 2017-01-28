using CubeCam.Extensions;
using CubeCam.Video;
using System;
using System.Drawing;
using static CubeCam.Extensions.ImageExtensions;

namespace CubeCam.Cube
{
    internal class CubeStateMachine : IVideoInjector
    {
        public delegate void TimeHandler(TimeSpan newTime);
        public event TimeHandler OnNewTime;

        public void AdvanceState()
        {
            switch (currentState)
            {
                case State.Scrambling:
                    StartInspection();
                    break;

                case State.Inspection:
                    StartSolve();
                    break;

                case State.Solving:
                    EndSolve();
                    break;

                default:
                    StartScramble();
                    break;
            }
        }

        public void LoadScrambles(string fileName)
        {
            scrambles = new ScrambleFile(fileName);
        }

        public void UpdateFrame(Bitmap frame, out bool mustSaveFrame)
        {
            mustSaveFrame = (currentState == State.Inspection || currentState == State.Solving ||
                            (currentState == State.Solved && cubeTimer.TimeSinceSolved.TotalSeconds <= 4.0));

            var defaultTextBrush = Brushes.Yellow;
            var textSize = frame.Height / 30;
            var largeTextSize = frame.Height / 15;
            var edgeOffset = frame.Height / 48;
            if (currentState != State.Scrambling)
            {
                // Write time.
                var elapsedTime = cubeTimer.SolveTime;
                var brush = currentState == State.Solved ? Brushes.LimeGreen : defaultTextBrush;
                frame.WriteString(elapsedTime.ToSecondsString(), XPosition.Right, YPosition.Bottom, edgeOffset, edgeOffset, largeTextSize, brush);
            }
            if (currentState == State.Inspection)
            {
                // Write inspection time remaining.
                var elapsedTime = cubeTimer.InspectionTime;
                var remainingSeconds = (int)Math.Ceiling(15.0 - elapsedTime.TotalSeconds);
                if (remainingSeconds < 0)
                {
                    remainingSeconds = 0;
                }
                frame.WriteString(remainingSeconds.ToString(), XPosition.Right, YPosition.Bottom, edgeOffset, 2 * edgeOffset + largeTextSize, textSize, Brushes.OrangeRed);
            }
            // Write solve number.
            frame.WriteString($"Solve #{solveNumber}", XPosition.Left, YPosition.Top, edgeOffset, edgeOffset, textSize, defaultTextBrush);
            // Write scramble.
            frame.WriteString(scramble, XPosition.Left, YPosition.Bottom, edgeOffset, edgeOffset, textSize, defaultTextBrush);
        }

        private void StartScramble()
        {
            scramble = scrambles.GetNextScramble();
            currentState = State.Scrambling;
            ++solveNumber;
            cubeTimer.Reset();
        }

        private void StartInspection()
        {
            currentState = State.Inspection;
            cubeTimer.StartInspection();
        }

        private void StartSolve()
        {
            currentState = State.Solving;
            cubeTimer.StartSolve();
        }

        private void EndSolve()
        {
            currentState = State.Solved;
            cubeTimer.EndSolve();
            OnNewTime?.Invoke(cubeTimer.SolveTime);
        }

        private enum State
        {
            None,
            Scrambling,
            Inspection,
            Solving,
            Solved
        }
        private State currentState = State.Solved;

        private IScrambleSource scrambles = new RandomStateScrambler();
        private string scramble;
        private int solveNumber = 0;

        private CubeTimer cubeTimer = new CubeTimer();
    }
}
