using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using System.Text;
using Machine.Fakes;
using Machine.Specifications;

namespace Meek.Specs
{
    public class When_resizing_an_image : WithSubject<DefaultImageResizer>
    {

        Establish that = () =>
           _original = new Bitmap(Assembly.GetExecutingAssembly().GetManifestResourceStream("Meek.Specs.UploadFile.jpg"));

        Because of = () =>
            _new = Subject.Resize(_original, 125);

        It should_be_the_correct_size = () =>
            {
                _new.Width.ShouldEqual(125);
                _new.Height.ShouldEqual(94);
            };

        static Image _original;
        static Image _new;
    }
}
