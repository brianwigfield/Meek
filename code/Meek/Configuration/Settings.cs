using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Meek.Configuration
{
    public interface Settings
    {

        string CkEditorPath { get; set; }
        string AltManageContentRoute { get; set; }
        string NotFoundView { get; set; }

    }
}
