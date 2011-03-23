using System.Web.Mvc;

namespace Meek
{
    public class HttpNotFoundViewResult : ViewResult
    {
        public HttpNotFoundViewResult(string viewName)
        {
            ViewName = viewName;
        }

        public override void ExecuteResult(ControllerContext context)
        {
            context.HttpContext.Response.StatusCode = 404;
            context.HttpContext.Response.TrySkipIisCustomErrors = true;
            base.ExecuteResult(context);
        }

    }
}
