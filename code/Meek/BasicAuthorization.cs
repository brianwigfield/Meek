using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;

namespace Meek
{
    public class BasicAuthorization : Authorization
    {
        static Func<HttpContextBase, bool> _contentAdmin;

        public BasicAuthorization(Func<HttpContextBase, bool> contentAdmin)
        {
            _contentAdmin = contentAdmin;
        }

        public bool IsContentAdmin(HttpContextBase context)
        {
            if (context == null)
                return false;

            return _contentAdmin.Invoke(context);
        }
    }
}
