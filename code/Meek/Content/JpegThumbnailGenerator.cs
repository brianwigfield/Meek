using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;

namespace Meek.Content
{
    public class JpegThumbnailGenerator : ThumbnailGenerator
    {
        private readonly ImageResizer _resizer;
        private readonly string[] _supportedContentTypes;

        public JpegThumbnailGenerator(ImageResizer resizer)
        {
            _resizer = resizer;
            _supportedContentTypes = new[] {"image/jpeg", "image/jpg", "image/gif", "image/png", "image/pjepg"};
        }

        public ThumbnailGenerationPriority? WillProcess(string requestedType)
        {
            return _supportedContentTypes.Any(_ => _ == requestedType) ? ThumbnailGenerationPriority.High : (ThumbnailGenerationPriority?)null;
        }

        public Thumbnail MakeThumbnail(string contentType, byte[] file, string fileName, int width)
        {
            var resized = _resizer.Resize(new Bitmap(new MemoryStream(file)), width);
            using(var output = new MemoryStream())
            {
                resized.Save(output, ImageFormat.Jpeg);
                return new Thumbnail
                {
                    File = output.ToArray(),
                    ContentType = System.Net.Mime.MediaTypeNames.Image.Jpeg,
                    Name = Path.GetFileNameWithoutExtension(fileName) + ".jpg"
                };   
            }
        }
    }
}
