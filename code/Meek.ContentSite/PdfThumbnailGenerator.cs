using System.Drawing.Imaging;
using System.IO;
using Meek.Content;

namespace Meek.ContentSite
{
    public class PdfThumbnailGenerator : ThumbnailGenerator
    {
        public ThumbnailGenerationPriority? WillProcess(string fileName, string contentType)
        {
            if (contentType == "application/pdf")
                return ThumbnailGenerationPriority.High;
            else
                return ThumbnailGenerationPriority.Low;
        }

        public Thumbnail MakeThumbnail(string contentType, byte[] file, string fileName, int width)
        {
            var ll = new MemoryStream();
            Resource1.PdfIcon.Save(ll, ImageFormat.Jpeg);
            return new Thumbnail
                       {
                           Name = Path.GetFileNameWithoutExtension(fileName) + ".png",
                           ContentType = "image/png",
                           File = ll.ToArray()
                       };
        }
    }
}