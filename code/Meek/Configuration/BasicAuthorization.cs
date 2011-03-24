using System;
using System.Web;

namespace Meek.Configuration
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
