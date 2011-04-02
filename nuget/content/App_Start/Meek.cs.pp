using Meek;

[assembly: WebActivator.PostApplicationStartMethod(typeof($rootnamespace$.App_Start.Meek), "Start")]
namespace $rootnamespace$.App_Start

{
    public static class Meek
    {

        public static void Start()
        {
            BootStrapper.Initialize();
        }

    }
}