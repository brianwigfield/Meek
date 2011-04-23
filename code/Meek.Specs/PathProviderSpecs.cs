using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Hosting;
using Machine.Fakes;
using Machine.Specifications;
using Meek.Configuration;
using Meek.Storage;
using Moq;
using It = Machine.Specifications.It;

namespace Meek.Specs
{
    public class When_checking_for_meek_content : With_path_provider_context
    {
        Establish that = () =>
           _contentPath = "/Views/Meek/made/up/route.cshtml";

        Because of = () =>
           _checkResult = _provider.FileExists(_contentPath);

        It Should_use_the_meek_storage = () => 
            _repo.Verify(x => x.Exists(Moq.It.IsAny<string>()));

        It Should_say_it_exists = () =>
            _checkResult.ShouldBeTrue();

        static bool _checkResult;
        static string _contentPath;
    }

    public class When_retrieving_meek_content : With_path_provider_context
    {
        Establish that = () =>
            _contentPath = "/Views/Meek/made/up/route.cshtml";

        Because of = () =>
        {
            _contentResult = _provider.GetFile(_contentPath);
            _contentResult.Open();
        };

        It Should_use_the_meek_storage = () =>
        {
            _repo.Verify(x => x.Exists(Moq.It.IsAny<string>()));
            _repo.Verify(x => x.Get(Moq.It.IsAny<string>()));
        };

        It Should_return_the_content = () =>
            _contentResult.ShouldNotBeNull();

        static VirtualFile _contentResult;
        static string _contentPath;
    }

    public class When_retrieving_meek_manager_page : With_path_provider_context
    {
        Establish that = () =>
            _contentPath = "/Views/Meek/Manage.cshtml";

        Because of = () =>
        {
            _contentResult = _provider.GetFile(_contentPath);
            _contentResult.Open();
        };

        It Should_not_use_the_meek_storage = () =>
            _repo.Verify(x => x.Exists(Moq.It.IsAny<string>()), Times.Never());

        It Should_return_the_content = () =>
            _contentResult.ShouldNotBeNull();

        static VirtualFile _contentResult;
        static string _contentPath;
    }

    public class When_checking_for_non_meek_content : With_path_provider_context
    {
        Establish that = () =>
            _contentPath = "/PathProviderSpecs.cs";

        Because of = () =>
           _provider.FileExists(_contentPath);

        It Should_not_use_the_meek_storage = () =>
            _repo.Verify(x => x.Exists(Moq.It.IsAny<string>()), Times.Never());

        static string _contentPath;
    }

    public class When_retrieving_non_meek_content : With_path_provider_context
    {
        Establish that = () =>
            _contentPath = "/PathProviderSpecs.cs";

        Because of = () =>
            _provider.GetFile(_contentPath);

        It Should_not_use_the_meek_storage = () =>
        {
            _repo.Verify(x => x.Exists(Moq.It.IsAny<string>()), Times.Never());
            _repo.Verify(x => x.Get(Moq.It.IsAny<string>()), Times.Never());
        };

        static string _contentPath;
    }


    public class With_path_provider_context
    {

        Establish that = () =>
        {
            _repo = new Mock<Repository>();
            _repo.Setup(x => x.Exists(Moq.It.IsAny<string>())).Returns(true);
            _repo.Setup(x => x.Get(Moq.It.IsAny<string>())).Returns(new MeekContent("title", "contents", false));
            var auth = new Mock<Authorization>();
            var services = new Mock<Configuration.Configuration>();
            services.Setup(x => x.GetRepository()).Returns(_repo.Object);
            services.Setup(x => x.GetAuthorization()).Returns(auth.Object);
            auth.Setup(x => x.IsContentAdmin(Moq.It.IsAny<HttpContextBase>())).Returns(true);
            _provider = new ContentPathProvider(new BasicVirtualPathProvider(), services.Object);
        };


        protected static Mock<Repository> _repo;
        protected static ContentPathProvider _provider;

    }

}
