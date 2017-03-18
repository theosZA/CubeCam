using AForge;
using AForge.Imaging.Filters;
using CubeCam.Extensions;
using CubeCam.Video;
using System;
using System.Drawing;
using System.Linq;
using System.Text;
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

        public void StartWriteResults()
        {
            currentState = State.Results;
            cubeTimer.StartResults();
        }

        public string GetTextSummary()
        {
            var text = new StringBuilder();
            int count = 0;
            foreach (var time in timeAggregator.Times)
            {
                ++count;
                text.Append($"{count}. {time.ToSecondsString()}\n");
            }
            text.Append($"\nMean: {timeAggregator.Mean.ToSecondsString()}");
            text.Append($"\nAverage (drop high & low): {timeAggregator.AverageDropHighLow.ToSecondsString()}");
            text.Append($"\n\n{timeAggregator.TextListDropHighLow}");
            return text.ToString();
        }

        public void UpdateFrame(Bitmap frame, out bool mustSaveFrame)
        {
            if (currentState == State.Results)
            {
                if (cubeTimer.ResultsTime.TotalSeconds < 10)
                {
                    WriteResults(frame);
                    mustSaveFrame = true;
                    return;
                }
                currentState = State.None;
            }

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

        private void WriteResults(Bitmap frame)
        {
            // Reduce brightness of incoming image to approximately 20% before we write on it.
            LevelsLinear filter = new LevelsLinear();
            filter.OutRed = new IntRange(0, 50);
            filter.OutGreen = new IntRange(0, 50);
            filter.OutBlue = new IntRange(0, 50);
            filter.ApplyInPlace(frame);

            var defaultBrush = Brushes.Yellow;
            var maxRows = Math.Min(20, timeAggregator.Times.Count()) + 3;
            frame.WriteLine($"Average (drop high & low): {timeAggregator.AverageDropHighLow.ToSecondsString()}", 0, maxRows, defaultBrush);
            frame.WriteLine($"Mean: {timeAggregator.Mean.ToSecondsString()}", 1, maxRows, defaultBrush);
            int currentIndex = 0;
            int minIndex = timeAggregator.Times.MinIndex();
            int maxIndex = timeAggregator.Times.MaxIndex();
            foreach (var time in timeAggregator.Times)
            {
                var brush = currentIndex == minIndex ? Brushes.LimeGreen :
                            currentIndex == maxIndex ? Brushes.OrangeRed : defaultBrush;
                frame.WriteLine($"{currentIndex + 1}. {time.ToSecondsString()}", 3 + currentIndex, maxRows, brush);
                ++currentIndex;
            }
        }

        private void StartScramble()
        {
            scramble = scrambles.GetNextScramble();
            if (scramble != null)
            {
                currentState = State.Scrambling;
                ++solveNumber;
                cubeTimer.Reset();
            }
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
            timeAggregator.AddTime(cubeTimer.SolveTime);
            OnNewTime?.Invoke(cubeTimer.SolveTime);
        }

        private enum State
        {
            None,
            Scrambling,
            Inspection,
            Solving,
            Solved,
            Results
        }
        private State currentState = State.None;

        private IScrambleSource scrambles = new RandomStateScrambler();
        private string scramble;
        private int solveNumber = 0;

        private CubeTimer cubeTimer = new CubeTimer();
        private TimeAggregator timeAggregator = new TimeAggregator();
    }
}
