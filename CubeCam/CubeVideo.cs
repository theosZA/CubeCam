using Accord.Video.FFMPEG;
using AForge.Video;
using CubeCam.Cube;
using CubeCam.Extensions;
using System;
using System.Drawing;
using static CubeCam.Extensions.ImageExtensions;

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
            videoInput.OnNewFrame += OnNewInputFrame;
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
            scrambles = new ScrambleFile(fileName);
        }

        public void StartVideoSource(IVideoSource newVideoSource)
        {
            StopVideoSource();
            videoInput.StartRead(newVideoSource, Width, Height);
        }

        public void StopVideoSource()
        {
            if (videoFileWriter.IsOpen)
            {
                videoFileWriter.Close();
            }
            videoInput.StopRead();
        }

        private void OnNewInputFrame(Bitmap frame)
        {
            var defaultTextBrush = Brushes.Yellow;
            var textSize = Height / 30;
            var largeTextSize = Height / 15;
            var edgeOffset = Height / 48;
            if (CurrentState != State.Scrambling)
            {
                // Write time.
                var elapsedTime = cubeTimer.SolveTime;
                var brush = CurrentState == State.Solved ? Brushes.LimeGreen : defaultTextBrush;
                frame.WriteString(elapsedTime.ToSecondsString(), XPosition.Right, YPosition.Bottom, edgeOffset, edgeOffset, largeTextSize, brush);
            }
            if (CurrentState == State.Inspection)
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

            // Save to file.
            if (videoFileWriter.IsOpen &&
                (CurrentState == State.Inspection || CurrentState == State.Solving ||
                (CurrentState == State.Solved && cubeTimer.TimeSinceSolved.TotalSeconds <= 4.0)))
            {
                videoFileWriter.WriteVideoFrame(frame);
            }

            OnNewFrame?.Invoke(frame);
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
            if (!videoFileWriter.IsOpen)
            {
                videoFileWriter.Open(VideoFileName, Width, Height, 25, VideoCodec.MPEG4, 8*1024*1024);
            }
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

        private State currentState = State.Solved;
        private int solveNumber = 0;

        private IScrambleSource scrambles = new RandomStateScrambler();
        private string scramble;

        private CubeTimer cubeTimer = new CubeTimer();

        private VideoInput videoInput = new VideoInput();
        private VideoFileWriter videoFileWriter = new VideoFileWriter();
    }
}
