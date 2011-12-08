namespace Meek.Content
{
    public interface ThumbnailGenerator
    {
        ThumbnailGenerationPriority? WillProcess(string fileName, string contentType);
        Thumbnail MakeThumbnail(string contentType, byte[] file, string fileName, int width);
    }
}