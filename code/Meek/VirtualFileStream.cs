using System.Web.Hosting;

namespace Meek
{
    public class VirtualFileStream : VirtualFile
    {
        private readonly System.IO.Stream _stream;

        public VirtualFileStream(System.IO.Stream stream, string virtualPath)
            : base(virtualPath)
        {
            _stream = stream;
        }

        public override System.IO.Stream Open()
        {
            return _stream;
        }

    }
}
