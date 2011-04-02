using System.Text;

namespace Meek.Storage
{
    public class MeekContent
    {
        public MeekContent() {}
        
        public MeekContent(string title, string contents, bool partial)
        {
            Title = title;
            Partial = partial;
            Contents = contents;
        }

        public string Title { get; set; }
        public bool Partial { get; set; }
        public string Contents { get; set; }

    }
}