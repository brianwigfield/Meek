using System.Text;

namespace Meek.Storage
{
    public class MeekContent
    {
        public MeekContent() {}

        public MeekContent(string title, byte[] contents, bool partial)
        {
            Title = title;
            Partial = partial;
            Contents = contents;
        }

        public MeekContent(string title, string contents, bool partial)
        {
            Title = title;
            Partial = partial;
            Contents = new UTF8Encoding().GetBytes(contents);
        }

        public string Title { get; set; }
        public bool Partial { get; set; }
        public byte[] Contents { get; set; }

    }
}