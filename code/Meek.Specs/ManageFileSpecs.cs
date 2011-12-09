using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using System.Web.Mvc;
using Meek.Configuration;
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
    public class When_uploading_an_image : WithSubject<MeekTestController>
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

    public class When_asking_to_browse_images : WithSubject<MeekTestController>
    {

        Establish that = () =>
            {
                The<Configuration.Configuration>()
                    .WhenToldTo(x => x.ViewEngineOptions)
                    .Return(new ViewEngineOptions {Type = ViewEngineType.Razor});

                The<Configuration.Configuration>()
                    .WhenToldTo(x => x.GetRepository().GetFiles())
                    .Return(new Dictionary<string, string>
                                {
                                    {"one","one.name"},
                                    {"two", "two.name"},
                                    {"three", "three.name"}
                                });
            };

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

    public class When_asking_for_an_image : WithSubject<MeekTestController>
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
                _result.AssertResultIs<FileResult>().FileDownloadName.ShouldEqual("blah.jpg");
            };

        static ActionResult _result;
    }

    public class When_asking_for_a_thumbnail_it_can_generate : WithSubject<MeekTestController>
    {
        Establish that = () =>
        {
            The<Configuration.Configuration>()
                .WhenToldTo(x => x.GetRepository().GetFile("1"))
                .Return(new MeekFile("TestFile.pdf", "application/pdf", GivenIt.IsAny<byte[]>()));

            _correctGenerator = An<ThumbnailGenerator>();
            _correctGenerator
                .WhenToldTo(_ => _.WillProcess("TestFile.pdf", "application/pdf"))
                .Return(ThumbnailGenerationPriority.High);

            _correctGenerator
                .WhenToldTo(_ => _.MakeThumbnail("application/pdf", Param<byte[]>.IsAnything, "TestFile.pdf", 125))
                .Return(new Thumbnail { Name = "TestFile.jpg", File = new byte[0], ContentType = System.Net.Mime.MediaTypeNames.Image.Jpeg });

            _incorrectGenerator = An<ThumbnailGenerator>();
            _incorrectGenerator
                .WhenToldTo(_ => _.WillProcess("TestFile.pdf", "application/pdf"))
                .Return(ThumbnailGenerationPriority.Low);

            The<Configuration.Configuration>()
                .WhenToldTo(x => x.GetThumbnailGenerators())
                .Return(new[] { _correctGenerator, _incorrectGenerator });
        };

        Because of = () =>
            _result = Subject.GetFileThumbnail("1");

        It Should_select_a_generator = () =>
            _correctGenerator
                .WasToldTo(_ => _.WillProcess("TestFile.pdf", "application/pdf"))
                .OnlyOnce();

        It Should_use_the_correct_generator = () =>
            _correctGenerator
                .WasToldTo(_ => _.MakeThumbnail("application/pdf", Param<byte[]>.IsAnything, "TestFile.pdf", 125))
                .OnlyOnce();

        It Should_return_a_thumbnail = () =>
        {
            _result.AssertResultIs<FileContentResult>();
            ((FileContentResult)_result).ContentType.ShouldEqual(System.Net.Mime.MediaTypeNames.Image.Jpeg);
            ((FileContentResult)_result).FileDownloadName.ShouldEqual("TestFile.jpg");
        };

        static ActionResult _result;
        static ThumbnailGenerator _incorrectGenerator;
        static ThumbnailGenerator _correctGenerator;
    }

    public class when_asking_for_a_thumbnail_it_cant_generate : WithSubject<MeekTestController>
    {
        Establish that = () =>
        {
            The<Configuration.Configuration>()
                .WhenToldTo(x => x.GetRepository().GetFile("1"))
                .Return(new MeekFile("TestFile.pdf", "application/pdf", GivenIt.IsAny<byte[]>()));

            The<ThumbnailGenerator>()
                .WhenToldTo(_ => _.WillProcess("test.file", "application/pdf"))
                .Return((ThumbnailGenerationPriority?)null);
        };

        Because of = () =>
            _result = Subject.GetFileThumbnail("1");

        It Should_return_a_500_error = () =>
            _result.AssertResultIs<HttpStatusCodeResult>().StatusCode.Equals(500);

        static ActionResult _result;
    }

    public class When_removing_a_file : WithSubject<MeekTestController>
    {
        Establish that = () =>
            The<Configuration.Configuration>()
                .WhenToldTo(_ => _.GetRepository())
                .Return(The<Repository>());

        Because of = () =>
            _result = Subject.RemoveFile("new.file");

        It Should_of_told_the_repository_to_remove_the_file = () =>
            The<Repository>()
                .WasToldTo(_ => _.RemoveFile("new.file"))
                .OnlyOnce();

        It Should_return_to_the_browse_page = () =>
            _result.AssertHttpRedirect().Url.ShouldEqual("/Meek/BrowseFiles");

        static ActionResult _result;
    }

}
