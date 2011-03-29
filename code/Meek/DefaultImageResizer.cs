using System;
using System.Drawing;

namespace Meek
{
    public class DefaultImageResizer : ImageResizer
    {

        public Image Resize(Image original, int width)
        {

            if (original.Width < width)
                return original;

            var ratio = (decimal)width / original.Width;
            var newWidth = width;
            var newHeight = (int)Math.Round(original.Height * ratio);

            var resizedImage = new Bitmap(newWidth, newHeight);
            var graphic = Graphics.FromImage(resizedImage);
            graphic.InterpolationMode = System.Drawing.Drawing2D.InterpolationMode.HighQualityBicubic;
            graphic.FillRectangle(Brushes.White, 0, 0, newWidth, newHeight);
            graphic.DrawImage(original, 0, 0, newWidth, newHeight);

            original.Dispose();

            return resizedImage;
        }

    }
}
