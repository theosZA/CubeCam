using System.Drawing;

namespace CubeCam.Video
{
    /// <summary>
    /// A class which injects its own content onto incoming video frames as well as determining whether the frames should be saved or not.
    /// </summary>
    internal interface IVideoInjector
    {
        /// <summary>
        /// Updates the incoming video frame with whatever content it wants before it is displayed and saved.
        /// </summary>
        /// <param name="frame">The incoming frame which may be updated.</param>
        /// <param name="mustSaveFrame">Set to true when the frame must be saved in addition to being displayed.</param>
        void UpdateFrame(Bitmap frame, out bool mustSaveFrame);
    }
}
