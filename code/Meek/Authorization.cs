using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Meek
{

    public interface Authorization
    {
        bool IsContentAdmin(HttpContextBase context);
    }

}
