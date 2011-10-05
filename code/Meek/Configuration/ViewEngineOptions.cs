namespace Meek.Configuration
{
    public class ViewEngineOptions
    {
        public ViewEngineType Type { get; set; }
        public string Layout { get; set; }
        public string PlaceHolder { get; set; }
    }

    public enum ViewEngineType
    {
        Razor = 1,
        ASPX = 2
    }
}