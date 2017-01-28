using AForge.Video;
using System.Drawing;
using System.Drawing.Imaging;

namespace CubeCam
{
    /// <summary>
    /// Video input from an AForge.Video.IVideoSource. Internally the frames are copied from
    /// the source video and so may be altered in the OnNewFrame event.
    /// </summary>
    internal class VideoInput
    {
        public delegate void FrameHandler(Bitmap newFrame);
        /// <summary>
        /// Event for when a new frame is available from the video source. A deep copy of the
        /// frame is provided in the event and so may be altered by the event handler.
        /// However the frame will be overwritten for the frame after the next one.
        /// </summary>
        public event FrameHandler OnNewFrame;

        /// <summary>
        /// Start reading video from the given video source. New frames from that source
        /// will be passed to the OnNewFrame event.
        /// </summary>
        /// <param name="newVideoSource">Any AForge video source.</param>
        /// <param name="width">The width of the source video.</param>
        /// <param name="height">The height of the source video.</param>
        public void StartRead(IVideoSource newVideoSource, int width, int height)
        {
            StopRead();
            for (int i = 0; i < frames.Length; ++i)
            {
                frames[i]?.Dispose();
                frames[i] = new Bitmap(width, height, PixelFormat.Format24bppRgb);
            }
            source = newVideoSource;
            source.NewFrame += OnNewSourceFrame;
            source.Start();
        }

        /// <summary>
        /// Stop reading video from the current video source.
        /// </summary>
        public void StopRead()
        {
            if (source != null)
            {
                source.SignalToStop();

                // wait ~ 3 seconds
                for (int i = 0; i < 30; i++)
                {
                    if (!source.IsRunning)
                        break;
                    System.Threading.Thread.Sleep(100);
                }

                if (source.IsRunning)
                {
                    source.Stop();
                }

                source = null;
            }
        }

        private void OnNewSourceFrame(object sender, NewFrameEventArgs e)
        {
            frameIndex = 1 - frameIndex;
            var frame = frames[frameIndex];
            e.Frame.FastCopyTo(frame);

            OnNewFrame?.Invoke(frame);
        }

        private IVideoSource source = null;
        private Bitmap[] frames = new Bitmap[2];
        private int frameIndex = 0;
    }
}
