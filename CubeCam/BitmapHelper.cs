using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace CubeCam
{
    static internal class BitmapHelper
    {
        [DllImport("kernel32.dll", EntryPoint = "CopyMemory", SetLastError = false)]
        private static extern void CopyMemory(IntPtr dest, IntPtr src, int count);

        static internal Bitmap FastCopy(this Bitmap src)
        {
            var dest = new Bitmap(src.Width, src.Height, src.PixelFormat);
            src.UncheckedFastCopyTo(dest);
            return dest;
        }

        static internal void FastCopyTo(this Bitmap src, Bitmap dest)
        {
            if (src.Width != dest.Width || src.Height != dest.Height || src.PixelFormat != dest.PixelFormat)
            {
                throw new Exception("Can't copy bitmaps because the dimensions of src and dest are different");
            }
            UncheckedFastCopyTo(src, dest);
        }

        static private void UncheckedFastCopyTo(this Bitmap src, Bitmap dest)
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
