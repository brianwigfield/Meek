using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Web;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Meek.Content;
using Meek.Storage;

namespace Meek
{

    public class MeekController : Controller
    {

        readonly Repository _repository;
        readonly Authorization _auth;

        protected override void OnException(ExceptionContext filterContext)
        {
            base.OnException(filterContext);
        }

        public MeekController()
        {
            _repository = Configuration.Configuration.Services.GetRepository();
            _auth = Configuration.Configuration.Services.GetAuthorization();
        }

        public MeekController(Repository repository, Authorization auth)
        {
            _repository = repository;
            _auth = auth;
        }

        public ActionResult Manage(string aspxerrorpath, bool partial = false)
        {
            if (!_auth.IsContentAdmin(HttpContext))
            {
                return new HttpNotFoundViewResult(Configuration.Configuration.Config.NotFoundView);
            }

            ViewBag.CkEditorPath = Configuration.Configuration.Config.CkEditorPath + "/ckeditor.js";

            var model = new Manage { ManageUrl = aspxerrorpath };
            if (model.ManageUrl.StartsWith("/"))
                model.ManageUrl = model.ManageUrl.Substring(1);

            if (_repository.Exists(model.ManageUrl))
            {
                var existingContent = _repository.Get(model.ManageUrl);
                model.ContentTitle = existingContent.Title;
                model.EditorContents = Encoding.UTF8.GetString(existingContent.Contents);
                model.Partial = existingContent.Partial;
            }
            else
            {
                model.Partial = partial;
            }

            return View("Manage", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        [AcceptParameter(Name = "submit", Value = "save")]
        public ActionResult Manage(Content.Manage model)
        {
            if (!_auth.IsContentAdmin(HttpContext))
                return new HttpStatusCodeResult(403);

            ViewBag.CkEditorPath = Configuration.Configuration.Config.CkEditorPath + "/ckeditor.js";

            if (string.IsNullOrEmpty(model.ManageUrl))
                ModelState.AddModelError("ManageUrl", "Url is required.");

            if (model.Partial && !string.IsNullOrEmpty(model.ContentTitle))
                ModelState.AddModelError("ContentTitle", "Title can not be applied to partial content.");

            if (!ModelState.IsValid)
                return View("Manage", model);

            //Since the route table does not accept routes starting with / then trim it off
            if (model.ManageUrl.StartsWith("/"))
                model.ManageUrl = model.ManageUrl.Substring(1);

            _repository.Save(model.ManageUrl, new MeekContent(model.ContentTitle, model.EditorContents, model.Partial));
            if (!model.Partial && !RouteTable.Routes.Cast<Route>().Any(x => x.Url == model.ManageUrl))
                RouteTable.Routes.Insert(0, new MeekRoute(model.ManageUrl));

            if (model.Partial)
                return Redirect("/");

            if (!model.ManageUrl.StartsWith("/"))
                model.ManageUrl = "/" + model.ManageUrl;

            return Redirect(model.ManageUrl);
        }

        [HttpPost]
        [ValidateInput(false)]
        [ActionName("Manage")]
        [AcceptParameter(Name="submit", Value="delete")]
        public ActionResult Delete(string manageUrl)
        {
            if (!_auth.IsContentAdmin(HttpContext))
                return new HttpStatusCodeResult(403);

            _repository.Remove(manageUrl);
            RouteTable.Routes.Remove(RouteTable.Routes.Cast<Route>().Single(x => x.Url == manageUrl));

            return Redirect("/");
        }

        public ActionResult GetContent()
        {
            return View(((Route)Url.RequestContext.RouteData.Route).Url);
        }

        [ChildActionOnly]
        public ActionResult GetPartialResource(string content)
        {
            if (content.StartsWith("/"))
                content = content.Substring(1);

            if (_repository.Exists(content))
                return View(content);

            var model = new CreatePartial()
                            {
                                CreateLink = @"/" + Configuration.Configuration.Config.AltManageContentRoute + "?aspxerrorpath=" + content + "&partial=true",
                                IsContentAdmin = _auth.IsContentAdmin(HttpContext)
                            };

            return View("CreatePartial", model);
        }


        public ActionResult BrowseFiles(string type, string ckEditor, string ckEditorFuncName)
        {
            return View("BrowseFiles");
        }

    }

}