using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;

namespace CubeCam.Extensions
{
    internal static class BitmapExtensions
    {
        [DllImport("kernel32.dll", EntryPoint = "CopyMemory", SetLastError = false)]
        private static extern void CopyMemory(IntPtr dest, IntPtr src, int count);

        /// <summary>
        /// Creates a new bitmap that is a deep copy of the given bitmap with a fast memory copy.
        /// </summary>
        /// <param name="src">The bitmap to copy.</param>
        /// <returns>A new bitmap that is a copy of the original.</returns>
        public static Bitmap FastCopy(this Bitmap src)
        {
            var dest = new Bitmap(src.Width, src.Height, src.PixelFormat);
            src.UncheckedFastCopyTo(dest);
            return dest;
        }

        /// <summary>
        /// Copies one bitmap to another with a fast memory copy. The two bitmaps must have the exact
        /// same dimensions (including pixel format).
        /// </summary>
        /// <param name="src"></param>
        /// <param name="dest"></param>
        public static void FastCopyTo(this Bitmap src, Bitmap dest)
        {
            if (src.Width != dest.Width || src.Height != dest.Height || src.PixelFormat != dest.PixelFormat)
            {
                throw new Exception("Can't copy bitmaps because the dimensions of src and dest are different");
            }
            UncheckedFastCopyTo(src, dest);
        }

        private static void UncheckedFastCopyTo(this Bitmap src, Bitmap dest)
        {
            var srcData = src.LockBits(new Rectangle(0, 0, src.Width, src.Height), ImageLockMode.ReadOnly, src.PixelFormat);
            var destData = dest.LockBits(new Rectangle(0, 0, dest.Width, dest.Height), ImageLockMode.WriteOnly, dest.PixelFormat);
            var srcPtr = srcData.Scan0;
            var destPtr = destData.Scan0;
            var size = srcData.Stride * srcData.Height;
            CopyMemory(destPtr, srcPtr, size);
            src.UnlockBits(srcData);
            dest.UnlockBits(destData);
        }
    }
}
