using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using Machine.Fakes;
using Machine.Specifications;
using Meek.Content;
using It = Machine.Specifications.It;
using GivenIt = Moq.It;

namespace Meek.Specs
{
    public class When_asking_for_an_image_thumbnail : WithSubject<JpegThumbnailGenerator>
    {
        Establish that = () =>
            The<ImageResizer>()
                .WhenToldTo(_ => _.Resize(GivenIt.IsAny<Image>(), 125))
                .Return(new Bitmap(200,200));

        private Because of = () =>
            {
                var image = new Bitmap(200, 200);
                var stream = new MemoryStream();
                image.Save(stream, ImageFormat.Gif);
                thumbnail = Subject.MakeThumbnail("image/gif", stream.ToArray(), "testing.gif", 125);
            };

        It should_of_used_the_resizer = () =>
            The<ImageResizer>()
                .WasToldTo(_ => _.Resize(GivenIt.IsAny<Image>(), 125))
                .OnlyOnce();

        It should_return_the_thumbnail = () =>
            thumbnail.File.ShouldNotBeNull();

        It should_name_the_file_properly = () =>
            thumbnail.Name.ShouldEqual("testing.jpg");

        It should_set_a_proper_content_type = () =>
            thumbnail.ContentType.ShouldEqual("image/jpeg");

        private static Thumbnail thumbnail;
    }
}
