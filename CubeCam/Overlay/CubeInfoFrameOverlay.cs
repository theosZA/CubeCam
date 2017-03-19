using AForge;
using AForge.Imaging.Filters;
using CubeCam.Extensions;
using System;
using System.Drawing;
using System.Linq;
using static CubeCam.Extensions.ImageExtensions;

namespace CubeCam.Overlay
{
    /// <summary>
    /// Writes cube information to each frame that comes in.
    /// </summary>
    internal class CubeInfoFrameOverlay
    {
        public void WriteSolveTime(Bitmap frame, TimeSpan solveTimeElapsed, bool solved)
        {
            (solved ? solvedTimeElement : solvingTimeElement).Write(frame, solveTimeElapsed);
        }

        public void WriteInspectionTime(Bitmap frame, TimeSpan inspectionTimeElapsed)
        {
            var remainingSeconds = (int)Math.Ceiling(15.0 - inspectionTimeElapsed.TotalSeconds);
            if (remainingSeconds < 0)
            {
                remainingSeconds = 0;
            }
            inspectionTimeElement.Write(frame, remainingSeconds.ToString());
        }

        public void WriteSolveNumber(Bitmap frame, int solveNumber)
        {
            solveNumberElement.Write(frame, $"Solve #{solveNumber}");
        }

        public void WriteScramble(Bitmap frame, string scramble)
        {
            scrambleElement.Write(frame, scramble);
        }

        public void WriteResults(Bitmap frame, TimeAggregator timeAggregator)
        {
            // Reduce brightness of incoming image to approximately 20% before we write on it.
            LevelsLinear filter = new LevelsLinear();
            filter.OutRed = new IntRange(0, 50);
            filter.OutGreen = new IntRange(0, 50);
            filter.OutBlue = new IntRange(0, 50);
            filter.ApplyInPlace(frame);

            var maxRows = Math.Min(20, timeAggregator.Times.Count()) + 3;
            frame.WriteLine($"Average (drop high & low): {timeAggregator.AverageDropHighLow.ToSecondsString()}", 0, maxRows, resultsDefaultBrush);
            frame.WriteLine($"Mean: {timeAggregator.Mean.ToSecondsString()}", 1, maxRows, resultsDefaultBrush);
            int currentIndex = 0;
            int minIndex = timeAggregator.Times.MinIndex();
            int maxIndex = timeAggregator.Times.MaxIndex();
            foreach (var time in timeAggregator.Times)
            {
                var brush = currentIndex == minIndex ? resultsBestTimeBrush :
                            currentIndex == maxIndex ? resultsWorstTimeBrush : resultsDefaultBrush;
                frame.WriteLine($"{currentIndex + 1}. {time.ToSecondsString()}", 3 + currentIndex, maxRows, brush);
                ++currentIndex;
            }
        }

        private OverlayElement solvingTimeElement = new OverlayElement
        {
            Brush = new SolidBrush(Color.Yellow),
            TextSizeFraction = 1.0f / 15,
            XPosition = XPosition.Right,
            XOffsetFraction = 1.0f / 48,
            YPosition = YPosition.Bottom,
            YOffsetFraction = 1.0f / 48
        };

        private OverlayElement solvedTimeElement = new OverlayElement
        {
            Brush = new SolidBrush(Color.LimeGreen),
            TextSizeFraction = 1.0f / 15,
            XPosition = XPosition.Right,
            XOffsetFraction = 1.0f / 48,
            YPosition = YPosition.Bottom,
            YOffsetFraction = 1.0f / 48
        };

        private OverlayElement inspectionTimeElement = new OverlayElement
        {
            Brush = new SolidBrush(Color.OrangeRed),
            TextSizeFraction = 1.0f / 30,
            XPosition = XPosition.Right,
            XOffsetFraction = 1.0f / 48,
            YPosition = YPosition.Bottom,
            YOffsetFraction = 13.0f / 120
        };

        private OverlayElement solveNumberElement = new OverlayElement
        {
            Brush = new SolidBrush(Color.Yellow),
            TextSizeFraction = 1.0f / 30,
            XPosition = XPosition.Left,
            XOffsetFraction = 1.0f / 48,
            YPosition = YPosition.Top,
            YOffsetFraction = 1.0f / 48
        };

        private OverlayElement scrambleElement = new OverlayElement
        {
            Brush = new SolidBrush(Color.Yellow),
            TextSizeFraction = 1.0f / 30,
            XPosition = XPosition.Left,
            XOffsetFraction = 1.0f / 48,
            YPosition = YPosition.Bottom,
            YOffsetFraction = 1.0f / 48
        };

        private Brush resultsDefaultBrush = new SolidBrush(Color.Yellow);
        private Brush resultsBestTimeBrush = new SolidBrush(Color.LimeGreen);
        private Brush resultsWorstTimeBrush = new SolidBrush(Color.OrangeRed);
    }
}
