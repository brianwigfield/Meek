using System;
using Meek.Configuration;

namespace Meek.Configuration
{
    public class StandardSettings : Settings
    {
        public string CkEditorPath { get; set; }
        public string AltManageContentRoute { get; set; }
        public string NotFoundView { get; set; }
    }
}