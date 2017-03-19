using CubeCam.Extensions;
using CubeCam.Overlay;
using CubeCam.Video;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

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
            scrambles = ScrambleList.ReadFromFile(fileName);
        }

        public void SetScrambles(IEnumerable<string> scrambles)
        {
            this.scrambles = new ScrambleList(scrambles);
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
                    cubeInfoFrameOverlay.WriteResults(frame, timeAggregator);
                    mustSaveFrame = true;
                    return;
                }
                currentState = State.None;
            }

            mustSaveFrame = (currentState == State.Inspection || currentState == State.Solving ||
                            (currentState == State.Solved && cubeTimer.TimeSinceSolved.TotalSeconds <= 4.0));

            if (currentState != State.Scrambling)
            {
                cubeInfoFrameOverlay.WriteSolveTime(frame, cubeTimer.SolveTime, currentState == State.Solved);
            }
            if (currentState == State.Inspection)
            {
                cubeInfoFrameOverlay.WriteInspectionTime(frame, cubeTimer.InspectionTime);
            }
            cubeInfoFrameOverlay.WriteSolveNumber(frame, solveNumber);
            cubeInfoFrameOverlay.WriteScramble(frame, scramble);
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
        private CubeInfoFrameOverlay cubeInfoFrameOverlay = new CubeInfoFrameOverlay();
    }
}
