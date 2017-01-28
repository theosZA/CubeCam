using Accord.Video.FFMPEG;
using AForge.Video;
using System.Drawing;

namespace CubeCam.Video
{
    /// <summary>
    /// Class for streaming from a video input to file with the possibility to update the content of the frame from another class.
    /// </summary>
    internal class VideoStreaming
    {
        public delegate void FrameHandler(Bitmap newFrame);
        public event FrameHandler OnNewFrame;

        public IVideoInjector VideoInjector { get; set; }
        public string VideoOutputFileName { get; set; }

        public VideoStreaming()
        {
            VideoOutputFileName = "default.avi";
            videoInput.OnNewFrame += OnNewInputFrame;
        }

        ~VideoStreaming()
        {
            StopVideoSource();
        }

        public void StartVideoSource(IVideoSource newVideoSource, int width, int height)
        {
            StopVideoSource();
            this.width = width;
            this.height = height;
            videoInput.StartRead(newVideoSource, width, height);
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
            bool mustSaveFrame = true;
            if (VideoInjector != null)
            {
                VideoInjector.UpdateFrame(frame, out mustSaveFrame);
            }

            if (mustSaveFrame)
            {
                if (!videoFileWriter.IsOpen)
                {
                    videoFileWriter.Open(VideoOutputFileName, width, height, 25, VideoCodec.MPEG4, 8 * 1024 * 1024);
                }
                videoFileWriter.WriteVideoFrame(frame);
            }

            OnNewFrame?.Invoke(frame);
        }

        private VideoInput videoInput = new VideoInput();
        private VideoFileWriter videoFileWriter = new VideoFileWriter();

        private int width;
        private int height;
    }
}
