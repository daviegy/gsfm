using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
namespace iHealth2.CustomClasses
{
    public class ImgResize
    {
        /// <summary>
        /// Resize the image to the specified width and height.
        /// </summary>
        /// <param name="image">The image to resize.</param>
        /// <param name="width">The width to resize to.</param>
        /// <param name="height">The height to resize to.</param>
        /// <returns>The resized image.</returns>
        public Bitmap ResizeImage(Image image, int width, int height)
        {
            var destRect = new Rectangle(0, 0, width, height);
            var destImage = new Bitmap(width, height);

            destImage.SetResolution(image.HorizontalResolution, image.VerticalResolution);

            using (var graphics = Graphics.FromImage(destImage))
            {
                graphics.CompositingMode = CompositingMode.SourceCopy;
                graphics.CompositingQuality = CompositingQuality.HighQuality;
                graphics.InterpolationMode = InterpolationMode.HighQualityBicubic;
                graphics.SmoothingMode = SmoothingMode.HighQuality;
                graphics.PixelOffsetMode = PixelOffsetMode.HighQuality;

                using (var wrapMode = new ImageAttributes())
                {
                    wrapMode.SetWrapMode(WrapMode.TileFlipXY);
                    graphics.DrawImage(image, destRect, 0, 0, image.Width, image.Height, GraphicsUnit.Pixel, wrapMode);
                }
            }

            return destImage;
        }
        /// <summary>
        /// Covert Image to Byte Array
        /// </summary>
        /// <param name="imageIn">Image to convert to byte</param>
        /// <param name="formatOfImg">Format of Image to convert</param>
        /// <param name="width">New Width of Image to convert</param>
        /// <param name="height">New Height of Image to Convert</param>
        /// <returns>Byte Array of Image</returns>
        public byte[] imageToByteArray(System.Drawing.Image imageIn, System.Drawing.Imaging.ImageFormat formatOfImg, int width, int height)
        {
           
            using (var ms = new MemoryStream())
            {
                ResizeImage(imageIn, width, height).Save(ms, formatOfImg);
                return ms.ToArray();
            }
        }
    }

}