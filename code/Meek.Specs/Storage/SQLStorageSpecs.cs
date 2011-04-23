using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using Machine.Fakes;
using Machine.Specifications;
using Machine.Specifications.Model;
using Meek.Storage;

namespace Meek.Specs.Storage.SQL
{

    public class When_checking_the_schema_of_a_database_that_does_not_have_meek_setup : WithSubject<SqlRepository>
    {

        Establish that = () =>
            Subject = new SqlRepository("EmptyDatabase");

        Because of = () =>
            Subject.EnsureSchema();

        It Should_create_the_tables_for_use = () =>
            {
                var ex = Catch.Exception(() => Subject.AvailableRoutes(null));
                ex.ShouldBeNull();
            };

    }

    public class When_saving_content_to_storage : WithSubject<SqlRepository>
    {
        Establish that = () =>
            Subject = new SqlRepository("TestDatabase");

        Because of = () =>
        {
            Subject.Save("MADE/up/route", new MeekContent("title", "my testing content here", false));
            Subject.Save("made/up/ROUTE", new MeekContent("title", "my testing content here", false));
            Subject.Save("MADE/up/partial", new MeekContent(null, "my testing content here", true));
            Subject.Save("made/up/Partial", new MeekContent(null, "my testing content here", true));
        };

        It Should_allow_it_to_be_retrived_case_insensitive = () =>
        {
            Subject.Get("made/up/route").Contents.Length.ShouldEqual<int>(23);
            Subject.Get("made/up/partial").Contents.Length.ShouldEqual<int>(23);
        };

        It Should_report_it_as_existing_case_insensitive = () =>
        {
            Subject.Exists("made/up/route").ShouldBeTrue();
            Subject.Exists("made/up/partial").ShouldBeTrue();
        };

        It Should_save_keys_case_insensitive = () =>
            Subject.AvailableRoutes(null).Count().ShouldEqual(2);

        It Should_return_it_in_route_list = () =>
            Subject.AvailableRoutes(null).FirstOrDefault(x => x == "MADE/up/route").
                ShouldNotBeNull();

        Cleanup The_records_from_storage = () =>
        {
            Subject.Remove("made/up/route");
            Subject.Remove("made/up/partial");
        };
        
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

    public class With_Test_Data : WithSubject<SqlRepository>
    {

        Establish that = () =>
            {
                Subject = new SqlRepository("TestDatabase");
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

    public class When_saving_a_file_to_storage : WithSubject<SqlRepository>
    {

        Establish that = () =>
           Subject = new SqlRepository("TestDatabase");

        Because of = () =>
            _fileId = Subject.SaveFile(new MeekFile("Test.jpg", "image/jpeg", _fileData));

        It Should_allow_it_to_be_retrived_case_insensitive = () =>
        {
            Subject.GetFile(_fileId.ToLower()).ShouldNotBeNull();
            var file = Subject.GetFile(_fileId.ToUpper());
            file.ShouldNotBeNull();
            file.Contents.ShouldEqual<byte[]>(_fileData);
            file.FileName.ShouldNotBeNull();
            file.Contents.ShouldNotBeNull();
        };

        It Should_generate_a_file_id = () =>
            _fileId.ShouldNotBeEmpty();

        Cleanup The_records_from_storage = () => 
            Subject.RemoveFile(_fileId);

        static byte[] _fileData = Assembly.GetExecutingAssembly().GetManifestResourceStream("Meek.Specs.UploadFile.jpg").ReadFully();
        static string _fileId;
    }

    public class When_asking_to_browse_image_files : WithSubject<SqlRepository>
    {

        Establish that = () =>
            {
                Subject = new SqlRepository("TestDatabase");

                Subject.SaveFile(new MeekFile("Test.jpg", "image/jpeg",
                                              Assembly.GetExecutingAssembly().GetManifestResourceStream(
                                                  "Meek.Specs.UploadFile.jpg").ReadFully()));
                Subject.SaveFile(new MeekFile("Test.jpg", "image/jpeg",
                                              Assembly.GetExecutingAssembly().GetManifestResourceStream(
                                                  "Meek.Specs.UploadFile.jpg").ReadFully()));
                Subject.SaveFile(new MeekFile("Test.jpg", "image/jpeg",
                                              Assembly.GetExecutingAssembly().GetManifestResourceStream(
                                                  "Meek.Specs.UploadFile.jpg").ReadFully()));
            };

        Because of = () =>
            _result = Subject.GetFiles();

        It Should_return_the_list_of_images_in_storage = () =>
            _result.Count().ShouldEqual<int>(3);

        Cleanup The_records_from_storage = () =>
            {
                foreach (var fileID in _result)
                {
                    Subject.RemoveFile(fileID);
                }
            };

        static IEnumerable<string> _result;
    }

}
