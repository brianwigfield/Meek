using System.Web;

namespace Meek.Configuration
{

    public interface Authorization
    {
        bool IsContentAdmin(HttpContextBase context);
    }

}
