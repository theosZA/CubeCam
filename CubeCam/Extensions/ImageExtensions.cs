using System;
using System.Drawing;

namespace CubeCam.Extensions
{
    internal static class ImageExtensions
    {
        public enum XPosition
        {
            Left,
            Right
        }
        public enum YPosition
        {
            Top,
            Bottom
        }

        /// <summary>
        /// Writes text on to an image using some default options.
        /// </summary>
        /// <param name="image">The image to be written on.</param>
        /// <param name="text">The text to be written.</param>
        /// <param name="xPosition">The vertical side of the image relative to which the text is written.</param>
        /// <param name="yPosition">The horizontal side of the image relative to which the text is written.</param>
        /// <param name="xOffset">The x offset relative to the vertical side.</param>
        /// <param name="yOffset">The y offset relative to the horizontal side.</param>
        /// <param name="emSize">The em size of the text to be written.</param>
        /// <param name="brush">A brush to be used for writing to specify the colour for example.</param>
        public static void WriteString(this Image image, string text, XPosition xPosition, YPosition yPosition, float xOffset, float yOffset, float emSize, Brush brush)
        {
            var graphic = Graphics.FromImage(image);
            graphic.WriteString(text, 
                                OffsetToCoordinate(xOffset, xPosition, image.Width),
                                OffsetToCoordinate(yOffset, yPosition, image.Height), 
                                emSize,
                                brush,
                                PositionToAlignment(xPosition),
                                PositionToAlignment(yPosition));
            graphic.Flush();
        }

        private static float OffsetToCoordinate(float xOffset, XPosition xPosition, int width)
        {
            switch (xPosition)
            {
                case XPosition.Left:    return xOffset;
                case XPosition.Right:   return width - xOffset;
                default: throw new ArgumentException($"Unknown XPosition {(int)xPosition}");
            }
        }

        private static float OffsetToCoordinate(float yOffset, YPosition yPosition, int height)
        {
            switch (yPosition)
            {
                case YPosition.Top:     return yOffset;
                case YPosition.Bottom:  return height - yOffset;
                default: throw new ArgumentException($"Unknown YPosition {(int)yPosition}");
            }
        }

        private static StringAlignment PositionToAlignment(XPosition xPosition)
        {
            switch (xPosition)
            {
                case XPosition.Left:    return StringAlignment.Near;
                case XPosition.Right:   return StringAlignment.Far;
                default:    throw new ArgumentException($"Unknown XPosition {(int)xPosition}");
            }
        }

        private static StringAlignment PositionToAlignment(YPosition yPosition)
        {
            switch (yPosition)
            {
                case YPosition.Top:     return StringAlignment.Near;
                case YPosition.Bottom:  return StringAlignment.Far;
                default: throw new ArgumentException($"Unknown YPosition {(int)yPosition}");
            }
        }
    }
}
