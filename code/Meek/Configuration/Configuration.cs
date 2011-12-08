using System.Collections.Generic;
using Meek.Content;
using Meek.Storage;

namespace Meek.Configuration
{
    public interface Configuration
    {

        string CkEditorPath { get; set; }
        string AltManageContentRoute { get; set; }
        string NotFoundView { get; set; }
        ViewEngineOptions ViewEngineOptions { get; set; }

        Repository GetRepository();
        Authorization GetAuthorization();
        ImageResizer GetImageResizer();
        IEnumerable<ThumbnailGenerator> GetThumbnailGenerators();
    }
}