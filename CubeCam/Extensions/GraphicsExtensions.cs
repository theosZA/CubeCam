using System.Drawing;

namespace CubeCam.Extensions
{
    internal static class GraphicsExtensions
    {
        /// <summary>
        /// Writes text on to a graphic using some default options.
        /// </summary>
        /// <param name="graphic">The graphic to be written on.</param>
        /// <param name="text">The text to be written.</param>
        /// <param name="x">The x position to start writing the text.</param>
        /// <param name="y">The y position to start writing the text.</param>
        /// <param name="emSize">The em size of the text to be written.</param>
        /// <param name="brush">A brush to be used for writing to specify the colour for example.</param>
        /// <param name="xAlignment">StringAlignment.Near to have the x position to the left of the text; StringAlignment.Far to have the x position to the right of the text.</param>
        /// <param name="yAlignment">StringAlignment.Near to have the y position above the text; StringAlignment.Far to have the y position below the text.</param>
        public static void WriteString(this Graphics graphic, string text, float x, float y, float emSize, Brush brush, StringAlignment xAlignment, StringAlignment yAlignment)
        {
            graphic.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            graphic.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            graphic.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            graphic.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
            using (var stringFormat = new StringFormat()
            {
                Alignment = xAlignment,
                LineAlignment = yAlignment
            })
            {
                graphic.DrawString(text, new Font("Tahoma", emSize), brush, x, y, stringFormat);
            }
        }
    }
}
