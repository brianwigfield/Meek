using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Machine.Fakes;
using Machine.Specifications;
using Machine.Specifications.Model;
using Meek.Storage;

namespace Meek.Specs.Storage.SQL
{
    public class When_saving_to_storage : WithSubject<SQLRepository>
    {
        Establish that = () =>
            Subject = new SQLRepository("TestDatabase");

        Because of = () =>
            {
                Subject.Save("MADE/up/route", new MeekContent("title", "content", false));
                Subject.Save("made/up/ROUTE", new MeekContent("title", "content", false));
            };

        It Should_allow_it_to_be_retrived_case_insensitive = () =>
            Subject.Get("made/up/route").ShouldNotBeNull();

        It Should_report_it_as_existing_case_insensitive = () =>
            Subject.Exists("made/up/route").ShouldBeTrue();

        It Should_save_keys_case_insensitive = () =>
            Subject.AvailableRoutes(null).Count().ShouldEqual(1);

        It Should_return_it_in_route_list = () =>
            Subject.AvailableRoutes(null).FirstOrDefault(x => x == "MADE/up/route").
                ShouldNotBeNull();

        Cleanup The_records_from_storage = () =>
            Subject.Remove("made/up/route");
        
    }

    public class When_asking_for_full_routes : With_Test_Data
    {
        Because of = () =>
            _results = Subject.AvailableRoutes(ContentTypes.Full);

        It Should_return_only_full_routes = () =>
            _results.Count().ShouldEqual(2);

        static IEnumerable<string> _results;
    }

    public class When_asking_for_partial_routes : With_Test_Data
    {
        Because of = () =>
            _results = Subject.AvailableRoutes(ContentTypes.Partial);

        It Should_return_only_full_routes = () =>
            _results.Count().ShouldEqual(1);

        static IEnumerable<string> _results;
    }

    public class When_asking_for_all_routes : With_Test_Data
    {
        Because of = () =>
            _results = Subject.AvailableRoutes(null);

        It Should_return_only_full_routes = () =>
            _results.Count().ShouldEqual(3);

        static IEnumerable<string> _results;
    }

    public class When_deleting_content : With_Test_Data
    {
        Because of = () =>
            Subject.Remove("route/number/one");

        It Should_not_contain_the_route_anymore = () =>
            Subject.Exists("route/number/one").ShouldBeFalse();
    }

    public class With_Test_Data : WithSubject<SQLRepository>
    {

        Establish that = () =>
            {
                Subject = new SQLRepository("TestDatabase");
                Subject.Save("route/number/one", new MeekContent("one", "first", false));
                Subject.Save("route/number/two", new MeekContent("two", "second", true));
                Subject.Save("route/number/three", new MeekContent("three", "third", false));
            };

        Cleanup The_records_from_storage = () =>
            {
                Subject.Remove("route/number/one");
                Subject.Remove("route/number/two");
                Subject.Remove("route/number/three");
            };
    }

}
