namespace Meek.Storage
{
    public class MeekFile
    {
        public MeekFile() {}

        public MeekFile(string fileName, string contentType, byte[] contents)
        {
            FileName = fileName;
            ContentType = contentType;
            Contents = contents;
        }

        public string FileName { get; set; }
        public string ContentType { get; set; }
        public byte[] Contents { get; set; }
    }
}
