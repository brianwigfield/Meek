using System.Drawing;

namespace Meek
{
    public interface ImageResizer
    {
        Image Resize(Image original, int width);
    }
}
