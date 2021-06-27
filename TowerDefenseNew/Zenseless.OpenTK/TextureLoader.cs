using ImageMagick;
using OpenTK.Graphics.OpenGL4;
using System.IO;
using Zenseless.Patterns;

namespace Zenseless.OpenTK
{
    /// <summary>
    /// Class for loading textures from images
    /// </summary>
    public static class TextureLoader
    {
        /// <summary>
        /// Load a texture out of the given stream.
        /// </summary>
        /// <param name="stream">A stream containing an image.</param>
        /// <returns>A Texture.</returns>
        public static Texture Load(Stream stream)
        {
            using var image = new MagickImage(stream);
            var format = PixelFormat.Rgb;
            var internalFormat = (SizedInternalFormat)All.Rgb8; //till OpenTK issue resolved on github https://github.com/opentk/opentk/issues/752
            switch (image.ChannelCount)
            {
                case 2: format = PixelFormat.LuminanceAlpha; internalFormat = SizedInternalFormat.Rg8; break;
                case 3: break;
                case 4: format = PixelFormat.Rgba; internalFormat = SizedInternalFormat.Rgba8; break;
                default: throw new InvalidDataException("Unexpected image format");
            }
            image.Flip();
            var bytes = image.GetPixelsUnsafe().ToArray();
            var texture = new Texture(image.Width, image.Height, internalFormat)
            {
                Function = TextureWrapMode.ClampToEdge,
                MagFilter = TextureMagFilter.Linear,
                MinFilter = TextureMinFilter.LinearMipmapLinear
            };

            GL.TextureSubImage2D(texture.Handle, 0, 0, 0, image.Width, image.Height, format, PixelType.UnsignedByte, bytes);
            GL.GenerateTextureMipmap(texture.Handle);
            return texture;
        }

        /// <summary>
        /// Load a texture out of the given embedded resource.
        /// </summary>
        /// <param name="name">The name of the resource that contains an image.</param>
        /// <returns>A Texture.</returns>
        public static Texture LoadFromResource(string name)
        {
            using var stream = Resource.LoadStream(name);
            return Load(stream);
        }
    }
}
