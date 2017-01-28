using AForge.Video;
using Accord.Video.FFMPEG;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;

namespace CubeCam
{
    class CubeVideo
    {
        public delegate void FrameHandler(Bitmap newFrame);
        public event FrameHandler OnNewFrame;

        public delegate void TimeHandler(TimeSpan newTime);
        public event TimeHandler OnNewTime;

        public int Width { get; set; }
        public int Height { get; set; }
        public string VideoFileName { get; set; }

        public enum State
        {
            Scrambling,
            Inspection,
            Solving,
            Solved
        }
        public State CurrentState
        {
            get
            {
                return currentState;
            }
        }

        public CubeVideo()
        {
            VideoFileName = "cube.avi";
        }

        ~CubeVideo()
        {
            StopVideoSource();
        }

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
            string[] lines = System.IO.File.ReadAllLines(fileName);
            // Each line can begin with a preamble to distinguish each scramble (e.g. "5." to indicate scramble 5).
            // We skip ahead to the first instance of a face character.
            scrambles = lines.Select(line => line.Substring(line.IndexOfAny("DUFLBR".ToCharArray())));
        }

        public void StartVideoSource(IVideoSource newVideoSource)
        {
            StopVideoSource();
            for (int i = 0; i < frames.Length; ++i)
            {
                frames[i]?.Dispose();
                frames[i] = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);
            }
            videoSource = newVideoSource;
            videoSource.NewFrame += OnNewSourceFrame;
            videoSource.Start();
        }

        public void StopVideoSource()
        {
            if (videoFileWriter.IsOpen)
            {
                videoFileWriter.Close();
            }

            if (videoSource != null)
            {
                videoSource.SignalToStop();

                // wait ~ 3 seconds
                for (int i = 0; i < 30; i++)
                {
                    if (!videoSource.IsRunning)
                        break;
                    System.Threading.Thread.Sleep(100);
                }

                if (videoSource.IsRunning)
                {
                    videoSource.Stop();
                }

                videoSource = null;
            }
        }

        private void OnNewSourceFrame(object sender, NewFrameEventArgs e)
        {
            frameIndex = 1 - frameIndex;
            var frame = frames[frameIndex];
            e.Frame.FastCopyTo(frame);
            
            var defaultTextBrush = Brushes.Yellow;
            var textSize = Height / 30;
            var largeTextSize = Height / 15;
            var edgeOffset = Height / 48;
            if (CurrentState != State.Scrambling)
            {
                // Write time.
                var elapsedTime = stopwatch.Elapsed;
                var brush = CurrentState == State.Solved ? Brushes.LimeGreen : defaultTextBrush;
                ImageText.WriteStringBottomRight(frame, elapsedTime.ToSecondsString(), edgeOffset, edgeOffset, largeTextSize, brush);
            }
            if (CurrentState == State.Inspection)
            {
                // Write inspection time remaining.
                var elapsedTime = inspectionStopwatch.Elapsed;
                var remainingSeconds = (int)Math.Ceiling(15.0 - elapsedTime.TotalSeconds);
                if (remainingSeconds < 0)
                {
                    remainingSeconds = 0;
                }
                ImageText.WriteStringBottomRight(frame, remainingSeconds.ToString(), edgeOffset, 2 * edgeOffset + largeTextSize, textSize, Brushes.OrangeRed);
            }
            // Write solve number.
            ImageText.WriteStringTopLeft(frame, $"Solve #{solveNumber}", edgeOffset, edgeOffset, textSize, defaultTextBrush);
            // Write scramble.
            ImageText.WriteStringBottomLeft(frame, scramble, edgeOffset, edgeOffset, textSize, defaultTextBrush);

            // Save to file.
            if (videoFileWriter.IsOpen &&
                (CurrentState == State.Inspection || CurrentState == State.Solving ||
                (CurrentState == State.Solved && afterSolveStopwatch.Elapsed.TotalSeconds <= 4.0)))
            {
                videoFileWriter.WriteVideoFrame(frame);
            }

            OnNewFrame?.Invoke(frame);
        }

        private void StartScramble()
        {
            if (scrambles != null)
            {
                if (solveNumber >= scrambles.Count())
                {
                    // Out of scrambles.
                    return;
                }
                scramble = scrambles.ElementAtOrDefault(solveNumber);
            }
            else
            {
                scramble = scrambler.GetNextScramble();
            }
            currentState = State.Scrambling;
            ++solveNumber;
            stopwatch.Reset();
        }

        private void StartInspection()
        {
            currentState = State.Inspection;
            inspectionStopwatch.Restart();
            if (!videoFileWriter.IsOpen)
            {
                videoFileWriter.Open(VideoFileName, Width, Height, 25, VideoCodec.MPEG4, 8*1024*1024);
            }
        }

        private void StartSolve()
        {
            currentState = State.Solving;
            stopwatch.Restart();
        }

        private void EndSolve()
        {
            currentState = State.Solved;
            stopwatch.Stop();
            OnNewTime?.Invoke(stopwatch.Elapsed);
            afterSolveStopwatch.Restart();
        }

        private State currentState = State.Solved;
        private int solveNumber = 0;

        private Random random = new Random();
        private RandomStateScrambler scrambler = new RandomStateScrambler();
        private string scramble = null;
        private IEnumerable<string> scrambles = null;

        private IVideoSource videoSource = null;
        private VideoFileWriter videoFileWriter = new VideoFileWriter();

        private Stopwatch stopwatch = new Stopwatch();
        private Stopwatch inspectionStopwatch = new Stopwatch();
        private Stopwatch afterSolveStopwatch = new Stopwatch();

        private Bitmap[] frames = new Bitmap[2];
        private int frameIndex = 0;
    }
}
