using OpenTK.Graphics.OpenGL4;

namespace Zenseless.OpenTK
{
    /// <summary>
    /// Contains methods for saving the frame buffer into an image
    /// </summary>
    public static class FrameBufferHelper
    {
        /// <summary>
        /// Saves a rectangular area of the current frame buffer into an array of bytes
        /// </summary>
        /// <param name="x">start position in x-direction</param>
        /// <param name="y">start position in y-direction</param>
        /// <param name="width">size in x-direction</param>
        /// <param name="height">size in y-direction</param>
        /// <param name="alpha">If <code>true</code> the alpha channel is stored.
        /// <code>false</code> by default.</param>
        /// <returns>byte[]</returns>
        public static byte[] ToByteArray(int x, int y, int width, int height, bool alpha = false)
        {
            var format = alpha ? PixelFormat.Rgba : PixelFormat.Rgb;
            var channelCount = alpha ? 4 : 3;
            var data = new byte[width * height * channelCount];
            GL.ReadPixels(x, y, width, height, format, PixelType.UnsignedByte, data);
            return data;
        }
    }
}
