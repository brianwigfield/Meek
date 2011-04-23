using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using Meek.Content;
using MvcContrib.TestHelper;
using Machine.Fakes;
using Machine.Specifications;
using Meek.Storage;
using Moq;
using It = Machine.Specifications.It;
using GivenIt = Moq.It;


namespace Meek.Specs
{
    public class When_uploading_an_image : WithSubject<MeekController>
    {
        Establish that = () =>
            {
                _file = new Mock<HttpPostedFileBase>();
                _file.Setup(x => x.FileName).Returns("Test.jpg");
                _file.Setup(x => x.ContentType).Returns("image/jpeg");
                _file.Setup(x => x.InputStream).Returns(Assembly.GetExecutingAssembly().GetManifestResourceStream("Meek.Specs.UploadFile.jpg"));

                The<Configuration.Configuration>()
                    .WhenToldTo(x => x.GetRepository().SaveFile(GivenIt.IsAny<MeekFile>()))
                    .Return("qwerty");
            };


        Because of = () =>
            _result = Subject.UploadFile(_file.Object, "callback");

        It Should_save_the_image_to_storage = () =>
            The<Configuration.Configuration>().WasToldTo(x => x.GetRepository().SaveFile(GivenIt.IsAny<MeekFile>()));

        It Should_return_a_ckeditor_callback = () =>
            {
                _result.AssertViewRendered().ForView("UploadFileSuccess");
                _result.AssertViewRendered().Model.ShouldBeOfType<UploadFileSuccess>();
                var model = _result.AssertViewRendered().Model as UploadFileSuccess;
                model.Callback.ShouldNotBeEmpty();
                model.Url.ShouldNotBeEmpty();
            };

        static ActionResult _result;
        static Mock<HttpPostedFileBase> _file;
    }

    public class When_asking_to_browse_images : WithSubject<MeekController>
    {

        Establish that = () =>
            The<Configuration.Configuration>()
                .WhenToldTo(x => x.GetRepository().GetFiles())
                .Return(new List<string>()
                            {
                                "one",
                                "two",
                                "three"
                            });

        Because of = () =>
            _result = Subject.BrowseFiles("image", "default", "callback");

        It Should_return_a_list_of_images = () =>
            {
                _result.AssertViewRendered().ForView("BrowseFiles");
                _result.AssertViewRendered().Model.ShouldBeOfType<BrowseFiles>();
                (_result.AssertViewRendered().Model as BrowseFiles).Files.Count().ShouldEqual(3);
            };

        It Should_return_a_callback = () =>
            (_result.AssertViewRendered().Model as BrowseFiles).Callback.ShouldNotBeEmpty();

        static ActionResult _result;
    }

    public class When_asking_for_an_image : WithSubject<MeekController>
    {

        Establish that = () =>
            The<Configuration.Configuration>()
                .WhenToldTo(x => x.GetRepository().GetFile("test"))
                .Return(new MeekFile("blah.jpg", "image/jpeg", new byte[0]));

        Because of = () =>
            _result = Subject.GetFile("test");

        It Should_return_the_file = () =>
            {
                _result.AssertResultIs<FileResult>();
                _result.AssertResultIs<FileResult>().FileDownloadName.ShouldEqual<string>("blah.jpg");
            };

        static ActionResult _result;
    }

    public class When_asking_for_an_image_thumbnail : WithSubject<MeekController>
    {

        Establish that = () =>
            {
                The<Configuration.Configuration>()
                    .WhenToldTo(x => x.GetRepository().GetFile("test"))
                    .Return(new MeekFile("blah.jpg", "image/jpeg", Assembly.GetExecutingAssembly().GetManifestResourceStream("Meek.Specs.UploadFile.jpg").ReadFully()));

                The<Configuration.Configuration>()
                    .WhenToldTo(x => x.GetImageResizer().Resize(GivenIt.IsAny<Image>(), 125))
                    .Return(new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Meek.Specs.UploadFile.jpg")));

            };


        Because of = () =>
            _result = Subject.GetFileThumbnail("test", 125);

        It Should_use_the_resizer_for_the_image = () =>
            The<Configuration.Configuration>()
                .WasToldTo(x => x.GetImageResizer().Resize(GivenIt.IsAny<Image>(), 125));

        It Should_return_the_file = () =>
        {
            _result.AssertResultIs<FileResult>();
            _result.AssertResultIs<FileResult>().FileDownloadName.ShouldEqual<string>("blah.jpg");
        };

        static ActionResult _result;
    }

}
