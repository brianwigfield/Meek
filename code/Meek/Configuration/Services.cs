using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Meek.Storage;

namespace Meek.Configuration
{
    public interface Services
    {
        Repository GetRepository();
        Authorization GetAuthorization();
        ImageResizer GetImageResizer();
    }
}
