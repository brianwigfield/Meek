using System.Collections.Generic;

namespace Meek.Content
{
    public class BrowseFiles
    {
        public string Callback { get; set; }
        public string Message { get; set; }
        public IDictionary<string,string> Files { get; set; }
    }
}
