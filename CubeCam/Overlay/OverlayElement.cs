using CubeCam.Extensions;
using System;
using System.Drawing;
using static CubeCam.Extensions.ImageExtensions;

namespace CubeCam.Overlay
{
    /// <summary>
    /// Holds information about how to present an element as an overlay on a frame.
    /// </summary>
    class OverlayElement
    {
        public Brush Brush { get; set; }
        public float TextSizeFraction { get; set; }

        public XPosition XPosition { get; set; }
        public float XOffsetFraction { get; set; }
        public YPosition YPosition { get; set; }
        public float YOffsetFraction { get; set; }

        public void Write(Bitmap frame, string text)
        {
            frame.WriteString(text, XPosition, YPosition, frame.Height * XOffsetFraction, frame.Height * YOffsetFraction, frame.Height * TextSizeFraction, Brush);
        }

        public void Write(Bitmap frame, TimeSpan timeSpan)
        {
            Write(frame, timeSpan.ToSecondsString());
        }

        public void Write(Bitmap frame, int value)
        {
            Write(frame, value.ToString());
        }
    }
}
