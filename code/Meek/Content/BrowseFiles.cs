using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Meek.Storage;

namespace Meek.Content
{
    public class BrowseFiles
    {
        public string Callback { get; set; }
        public string Message { get; set; }
        public IEnumerable<string> Files { get; set; }
    }
}
