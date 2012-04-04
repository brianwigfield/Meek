using System;

namespace Meek.Storage
{
    public class ResourceChangedArgs : EventArgs
    {
        public string Path { get; set; }
    }
}