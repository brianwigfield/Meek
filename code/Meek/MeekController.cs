using System.Collections.Generic;
using System.Web;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Meek.Configuration;
using Meek.Content;
using Meek.Storage;

namespace Meek
{

    public class MeekController : Controller
    {

        readonly Repository _repository;
        readonly Authorization _auth;
        readonly IEnumerable<ThumbnailGenerator> _thumbnailGenerators;
        readonly Configuration.Configuration _config;

        public MeekController()
        {
            _config = BootStrapper.Configuration;
            _repository = BootStrapper.Configuration.GetRepository();
            _auth = BootStrapper.Configuration.GetAuthorization();
            _thumbnailGenerators = BootStrapper.Configuration.GetThumbnailGenerators();
        }

        protected MeekController(Configuration.Configuration config)
        {
            _config = config;
            _repository = config.GetRepository();
            _auth = config.GetAuthorization();
            _thumbnailGenerators = config.GetThumbnailGenerators();
        }

        private ViewResult MeekResult(string view, object model)
        {
            if (!string.IsNullOrWhiteSpace(_config.ViewEngineOptions.Layout))
            {
                ViewBag.PlaceHolder = _config.ViewEngineOptions.PlaceHolder;
                return View(view, _config.ViewEngineOptions.Layout, model);
            }

            return View(view, model);
        }

        public ActionResult List()
        {
            if (!_auth.IsContentAdmin(HttpContext))
            {
                if (string.IsNullOrEmpty(_config.NotFoundView))
                    return new HttpNotFoundResult();
                else
                    return new HttpNotFoundViewResult(_config.NotFoundView);
            }

            return MeekResult("List", _repository.AvailableRoutes(null).Select(x => "/" + x).OrderBy(x => x));
        }

        public ActionResult Manage(string aspxerrorpath, bool partial = false)
        {
            if (!_auth.IsContentAdmin(HttpContext))
            {
                if (string.IsNullOrEmpty(_config.NotFoundView))
                    return new HttpNotFoundResult();
                else
                    return new HttpNotFoundViewResult(_config.NotFoundView);
            }

            ViewBag.CkEditorPath = _config.CkEditorPath + "/ckeditor.js";

            var model = new Manage
                            {
                                ManageUrl = aspxerrorpath,
                                IncludeFormTag = _config.ViewEngineOptions.IncludeFormTag
                            };

            if (model.ManageUrl.StartsWith("/"))
                model.ManageUrl = model.ManageUrl.Substring(1);

            if (_repository.Exists(model.ManageUrl))
            {
                var existingContent = _repository.Get(model.ManageUrl);
                model.ContentTitle = existingContent.Title;
                model.EditorContents = existingContent.Contents;
                model.Partial = existingContent.Partial;
            }
            else
            {
                model.Partial = partial;
            }

            return MeekResult("Manage", model);
        }

        [HttpPost]
        [ValidateInput(false)]
        [AcceptParameter(Name = "submit", Value = "save")]
        public ActionResult Manage(Manage model)
        {
            if (!_auth.IsContentAdmin(HttpContext))
                return new HttpStatusCodeResult(403);

            ViewBag.CkEditorPath = _config.CkEditorPath + "/ckeditor.js";

            if (string.IsNullOrEmpty(model.ManageUrl))
                ModelState.AddModelError("ManageUrl", "Url is required.");

            if (model.Partial && !string.IsNullOrEmpty(model.ContentTitle))
                ModelState.AddModelError("ContentTitle", "Title can not be applied to partial content.");

            if (!ModelState.IsValid)
                return MeekResult("Manage", model);

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

            var existingContent = _repository.Get(manageUrl);

            _repository.Remove(manageUrl);

            if (existingContent.Partial == false)
                RouteTable.Routes.Remove(RouteTable.Routes.Cast<Route>().Single(x => x.Url == manageUrl));

            return Redirect("/");
        }

        public ActionResult GetContent()
        {
            ViewBag.IsContentAdmin = _auth.IsContentAdmin(HttpContext);
            return View(((Route)Url.RequestContext.RouteData.Route).Url);
        }

        [ChildActionOnly]
        public ActionResult GetPartialResource(string content)
        {
            if (content.StartsWith("/"))
                content = content.Substring(1);

            if (_repository.Exists(content))
            {
                ViewBag.IsContentAdmin = _auth.IsContentAdmin(HttpContext);
                return View(content);
            }

            var model = new CreatePartial
                            {
                                CreateLink = @"/" + _config.AltManageContentRoute + "?aspxerrorpath=" + content + "&partial=true",
                                IsContentAdmin = _auth.IsContentAdmin(HttpContext)
                            };

            return View("CreatePartial", model);
        }

        public ActionResult GetFile(string id)
        {
            var file = _repository.GetFile(id);
            if (file == null)
                return new HttpNotFoundResult();

            return File(file.Contents, file.ContentType, file.FileName);
        }

        public ActionResult GetFileThumbnail(string id, int width = 125)
        {
            var file = _repository.GetFile(id);
            if (file == null)
                return new HttpNotFoundResult();

            var generatorChoice = _thumbnailGenerators.Select(_ => new {Result = _.WillProcess(file.FileName, file.ContentType), Generator = _ })
                .Where(_ => _.Result != null)
                .OrderBy(_ => _.Result)
                .FirstOrDefault();

            if (generatorChoice == null)
                return new HttpStatusCodeResult(500, "No thumbnail can be generated.");

            var thumbnail = generatorChoice.Generator.MakeThumbnail(file.ContentType, file.Contents, file.FileName, width);
            return File(thumbnail.File, thumbnail.ContentType, thumbnail.Name);
        }

        public ActionResult BrowseFiles(string type, string ckEditor, string ckEditorFuncNum)
        {

            return MeekResult("BrowseFiles", new BrowseFiles { Files = _repository.GetFiles(), Callback = ckEditorFuncNum });
        }

        public ActionResult UploadFile(HttpPostedFileBase upload, string ckEditorFuncNum)
        {
            var fileId = _repository.SaveFile(new MeekFile(upload.FileName, upload.ContentType, upload.InputStream.ReadFully()));
            var model = new UploadFileSuccess
                            {
                                Callback = ckEditorFuncNum,
                                Url = "/Meek/GetFile/" + fileId
                            };
            return View("UploadFileSuccess", model);
        }

        public ActionResult RemoveFile(string id)
        {
            _repository.RemoveFile(id);
            return Redirect("/Meek/BrowseFiles");
        }



    }

}