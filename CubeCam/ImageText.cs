using System.Drawing;

namespace CubeCam
{
    static class ImageText
    {
        static public void WriteString(Graphics graphic, string text, float x, float y, float emSize, Brush brush, StringFormat stringFormat)
        {
            graphic.SmoothingMode = System.Drawing.Drawing2D.SmoothingMode.AntiAlias;
            graphic.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            graphic.PixelOffsetMode = System.Drawing.Drawing2D.PixelOffsetMode.HighQuality;
            graphic.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAliasGridFit;
            graphic.DrawString(text, new Font("Tahoma", emSize), brush, x, y, stringFormat);
        }

        static public void WriteStringTopLeft(Image image, string text, float x, float y, float emSize, Brush brush)
        {
            var stringFormat = new StringFormat()
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Near
            };
            var graphic = Graphics.FromImage(image);
            WriteString(graphic, text, x, y, emSize, brush, stringFormat);
            graphic.Flush();
        }

        static public void WriteStringBottomLeft(Image image, string text, float x, float y, float emSize, Brush brush)
        {
            var stringFormat = new StringFormat()
            {
                Alignment = StringAlignment.Near,
                LineAlignment = StringAlignment.Far
            };
            var graphic = Graphics.FromImage(image);
            WriteString(graphic, text, x, image.Height - y, emSize, brush, stringFormat);
            graphic.Flush();
        }

        static public void WriteStringTopRight(Image image, string text, float x, float y, float emSize, Brush brush)
        {
            var stringFormat = new StringFormat()
            {
                Alignment = StringAlignment.Far,
                LineAlignment = StringAlignment.Near
            };
            var graphic = Graphics.FromImage(image);
            WriteString(graphic, text, image.Width - x, y, emSize, brush, stringFormat);
            graphic.Flush();
        }

        static public void WriteStringBottomRight(Image image, string text, float x, float y, float emSize, Brush brush)
        {
            var stringFormat = new StringFormat()
            {
                Alignment = StringAlignment.Far,
                LineAlignment = StringAlignment.Far
            };
            var graphic = Graphics.FromImage(image);
            WriteString(graphic, text, image.Width - x, image.Height - y, emSize, brush, stringFormat);
            graphic.Flush();
        }
    }
}
