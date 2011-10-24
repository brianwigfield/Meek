using System.Collections.Generic;

namespace Meek.Content
{
    public class BrowseFiles
    {
        public string Callback { get; set; }
        public string Message { get; set; }
        public IEnumerable<string> Files { get; set; }
    }
}
