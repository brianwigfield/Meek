using Meek.Storage;

namespace Meek.Configuration
{
    public interface Configuration
    {

        string CkEditorPath { get; set; }
        string AltManageContentRoute { get; set; }
        string NotFoundView { get; set; }

        Repository GetRepository();
        Authorization GetAuthorization();
        ImageResizer GetImageResizer();

    }
}