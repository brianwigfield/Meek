namespace Meek.Storage
{
    public class MeekFile
    {
        public MeekFile() {}

        public MeekFile(string fileId,string fileName, string contentType, byte[] contents)
        {
            FileId = fileId;
            FileName = fileName;
            ContentType = contentType;
            Contents = contents;
        }

        public string FileId { get; set; }
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public byte[] Contents { get; set; }
    }
}
