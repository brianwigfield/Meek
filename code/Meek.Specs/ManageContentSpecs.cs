using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using Machine.Fakes;
using Machine.Specifications;
using Meek.Configuration;
using Meek.Storage;
using MvcContrib.TestHelper;
using It = Machine.Specifications.It;
using GivenIt = Moq.It;

namespace Meek.Specs
{

    public class When_a_content_admin_asks_for_a_non_existent_page : WithSubject<MeekTestController>
    {
        Establish that = () =>
            {
                The<Configuration.Configuration>()
                    .WhenToldTo(x => x.GetAuthorization().IsContentAdmin(GivenIt.IsAny<HttpContextBase>()))
                    .Return(true);

                The<Configuration.Configuration>()
                    .WhenToldTo(x => x.GetRepository().Exists("a/bogus/url"))
                    .Return(false);

                The<Configuration.Configuration>()
                    .WhenToldTo(x => x.ViewEngineOptions)
                    .Return(new ViewEngineOptions{ Type = ViewEngineType.Razor });

            };

        Because of = () =>
            _result = Subject.Manage("/a/bogus/url");

        It Should_check_the_repository = () =>
            The<Configuration.Configuration>().WasToldTo(x => x.GetRepository().Exists("a/bogus/url")).OnlyOnce();

        It Should_present_a_blank_add_content_screen = () =>
            _result.AssertViewRendered().ForView("Manage");

        It Should_auto_populate_the_url_field = () =>
            (_result.AssertViewRendered().Model as Content.Manage).ManageUrl.ShouldNotBeEmpty();

        static ActionResult _result;
    }

    public class When_a_content_admin_asks_to_edit_an_existing_page : WithSubject<MeekTestController>
    {
        Establish that = () =>
        {
            The<Configuration.Configuration>()
                .WhenToldTo(x => x.GetAuthorization().IsContentAdmin(GivenIt.IsAny<HttpContextBase>()))
                .Return(true);

            The<Configuration.Configuration>()
                .WhenToldTo(x => x.GetRepository().Exists("existing/route"))
                .Return(true);

            The<Configuration.Configuration>()
                .WhenToldTo(x => x.ViewEngineOptions)
                .Return(new ViewEngineOptions { Type = ViewEngineType.Razor });

            The<Configuration.Configuration>()
                .WhenToldTo(x => x.GetRepository().Get("existing/route"))
                .Return(new MeekContent("a title", "some content", true));

        };

        Because of = () =>
            _result = Subject.Manage("/existing/route");

        It Should_present_a_edit_content_screen = () =>
            _result.AssertViewRendered().ForView("Manage");

        It Should_populate_the_form_with_the_existing_data = () =>
            {
                var model = (_result.AssertViewRendered().Model as Content.Manage);
                model.ManageUrl.ShouldEqual("existing/route");
                model.Partial.ShouldBeTrue();
                model.ContentTitle.ShouldEqual("a title");
                model.EditorContents.ShouldEqual("some content");
            };

        static ActionResult _result;
    }

    public class When_a_content_admin_asks_to_delete_an_existing_page : WithSubject<MeekTestController>
    {
        Establish that = () =>
        {
            The<Configuration.Configuration>()
                .WhenToldTo(x => x.GetAuthorization().IsContentAdmin(GivenIt.IsAny<HttpContextBase>()))
                .Return(true);

            The<Configuration.Configuration>()
                .WhenToldTo(x => x.GetRepository().Remove("existing/route"));

            RouteTable.Routes.Add(new MeekRoute("existing/route"));
        };

        Because of = () =>
            _result = Subject.Delete("existing/route");

        It Should_delete_from_storage = () =>
            The<Configuration.Configuration>()
                .WasToldTo(x => x.GetRepository().Remove("existing/route"));

        It Should_remove_the_route = () =>
            RouteTable.Routes.Cast<MeekRoute>().FirstOrDefault(x => x.Url == "existing/route").ShouldBeNull();

        It Should_redirect_to_the_homepage = () => 
            _result.AssertHttpRedirect().ToUrl("/");

        static ActionResult _result;
    }


    public class When_a_content_admin_enters_page_content : WithSubject<MeekTestController>
    {
        Establish that = () =>
            {
                The<Configuration.Configuration>()
                    .WhenToldTo(x => x.GetAuthorization().IsContentAdmin(GivenIt.IsAny<HttpContextBase>()))
                    .Return(true);

                The<Configuration.Configuration>()
                    .WhenToldTo(x => x.GetRepository().Save(GivenIt.IsAny<string>(), GivenIt.IsAny<MeekContent>()));
            };

        Because of = () =>
            _result =
            Subject.Manage(new Content.Manage()
                            {
                                ManageUrl = "a/bogus/url",
                                ContentTitle = "a title",
                                Partial = false,
                                EditorContents = "some content"
                            });


        It Should_save_the_content = () =>
            The<Configuration.Configuration>().WasToldTo(x => x.GetRepository().Save(GivenIt.IsAny<string>(), GivenIt.IsAny<MeekContent>())).OnlyOnce();

        It Should_redirect_them_to_the_new_content = () =>
            _result.AssertHttpRedirect().ToUrl("/a/bogus/url");

        It Should_have_the_route_registered = () =>
            RouteTable.Routes.Cast<MeekRoute>().FirstOrDefault(x => x.Url == "a/bogus/url").ShouldNotBeNull();

        static ActionResult _result;
    }

    public class When_a_non_content_admin_enters_page_content : WithSubject<MeekTestController>
    {
        Establish that = () =>
            The<Configuration.Configuration>()
                .WhenToldTo(x => x.GetAuthorization().IsContentAdmin(GivenIt.IsAny<HttpContextBase>()))
                .Return(false);

        Because of = () =>
            _result =
            Subject.Manage(new Content.Manage()
                            {
                                ManageUrl = "a/bogus/url",
                                ContentTitle = "a title",
                                Partial = false,
                                EditorContents = "some content"
                            });


        It Should_return_a_403_result = () =>
            (_result as HttpStatusCodeResult).StatusCode.ShouldEqual(403);

        static ActionResult _result;
    }

    public class When_a_content_admin_enters_page_content_without_a_url : WithSubject<MeekTestController>
    {
        Establish that = () =>
            {
                The<Configuration.Configuration>()
                    .WhenToldTo(x => x.GetAuthorization().IsContentAdmin(GivenIt.IsAny<HttpContextBase>()))
                    .Return(true);

                The<Configuration.Configuration>()
                    .WhenToldTo(x => x.ViewEngineOptions)
                    .Return(new ViewEngineOptions {Type = ViewEngineType.Razor});
            };

        Because of = () =>
            _result =
            Subject.Manage(new Content.Manage()
            {
                ManageUrl = string.Empty,
                ContentTitle = "a title",
                Partial = false,
                EditorContents = "some content"
            });

        It Should_return_the_form_with_an_error = () =>
            _result.AssertViewRendered().ForView("Manage");

        It Should_return_an_error = () =>
            {
                Subject.ModelState.IsValid.ShouldBeFalse();
                Subject.ModelState.Count().ShouldBeGreaterThan(0);
            };

        static ActionResult _result;
    }

    public class When_a_content_admin_enters_partial_page_content : WithSubject<MeekTestController>
    {
        Establish that = () =>
            {
                The<Configuration.Configuration>()
                    .WhenToldTo(x => x.GetAuthorization().IsContentAdmin(GivenIt.IsAny<HttpContextBase>()))
                    .Return(true);

                The<Configuration.Configuration>()
                    .WhenToldTo(x => x.GetRepository().Save(GivenIt.IsAny<string>(), GivenIt.IsAny<MeekContent>()));
            };

        Because of = () =>
            _result =
            Subject.Manage(new Content.Manage()
                            {
                                ManageUrl = "a/bogus/partial/url",
                                ContentTitle = string.Empty,
                                Partial = true,
                                EditorContents = "some content"
                            });

        It Should_save_the_content = () =>
            The<Configuration.Configuration>().WasToldTo(
                x =>
                x.GetRepository().Save(GivenIt.IsAny<string>(), GivenIt.IsAny<MeekContent>())).
                OnlyOnce();

        It Should_redirect_them_to_the_homepage = () =>
            _result.AssertHttpRedirect().ToUrl("/");

        It Should_not_have_the_route_registered = () =>
            RouteTable.Routes.Cast<MeekRoute>().FirstOrDefault(x => x.Url == "a/bogus/partial/url").ShouldBeNull();

        static ActionResult _result;
    }

    public class When_a_non_content_admin_asks_to_manage_pages : WithSubject<MeekTestController>
    {

        Establish that = () =>
            {
                The<Configuration.Configuration>()
                    .WhenToldTo(x => x.GetAuthorization().IsContentAdmin(GivenIt.IsAny<HttpContextBase>()))
                    .Return(false);

                The<Configuration.Configuration>()
                    .WhenToldTo(x => x.NotFoundView)
                    .Return("NotFound");
            };

        Because of = () =>
            _result = Subject.Manage("/a/bogus/url");

        It Should_return_a_not_found_view = () =>
            _result.AssertViewRendered().ForView("NotFound");

        It Should_return_a_404_status = () =>
            _result.AssertResultIs<HttpNotFoundViewResult>();

        private static ActionResult _result;

    }

    public class When_a_content_admin_enters_page_content_with_empty_title_and_content : WithSubject<MeekTestController>
    {
        Establish that = () =>
            {
                The<Configuration.Configuration>()
                    .WhenToldTo(x => x.GetAuthorization().IsContentAdmin(GivenIt.IsAny<HttpContextBase>()))
                    .Return(true);

                The<Configuration.Configuration>()
                    .WhenToldTo(x => x.GetRepository().Save(GivenIt.IsAny<string>(), GivenIt.IsAny<MeekContent>()));
            };

        Because of = () =>
            _result =
            Subject.Manage(new Content.Manage()
            {
                ManageUrl = "a/bogus/url",
                ContentTitle = null,
                Partial = false,
                EditorContents = null
            });


        It Should_save_the_content = () =>
            The<Configuration.Configuration>().WasToldTo(
                x => x.GetRepository().Save(GivenIt.IsAny<string>(), GivenIt.IsAny<MeekContent>())).
                OnlyOnce();

        It Should_redirect_them_to_the_new_content = () =>
             _result.AssertHttpRedirect().ToUrl("/a/bogus/url");

        It Should_have_the_route_registered = () =>
             RouteTable.Routes.Cast<MeekRoute>().FirstOrDefault(x => x.Url == "a/bogus/url").ShouldNotBeNull();

        static ActionResult _result;
    }

}
