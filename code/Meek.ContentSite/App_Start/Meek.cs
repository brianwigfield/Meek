using System.Reflection;
using System.Web.Mvc;
using Meek.Content;
using Meek.Storage;
using Microsoft.Practices.Unity;

[assembly: WebActivator.PostApplicationStartMethod(typeof(Meek.ContentSite.App_Start.Meek), "Start")]
namespace Meek.ContentSite.App_Start

{
    public static class Meek
    {

        public static void Start()
        {

            var container = new UnityContainer();
            container.RegisterType<Repository, InMemoryRepository>(new ContainerControlledLifetimeManager());
            container.RegisterType<ThumbnailGenerator, PdfThumbnailGenerator>("PDF", new ContainerControlledLifetimeManager());
            DependencyResolver.SetResolver(new UnityResolver(container));

            //Do this to setup the test data before Initialize() for automated tests using InMemoryRepository
            SetupRepository(container.Resolve<Repository>());

            BootStrapper.Initialize();
            //Configuration.Configuration.Initialize(repository, new BasicAuthorization(x => x.User.IsInRole("Content Admin")), "Missing");
            
        }

        private static void SetupRepository(Repository repository)
        {
            repository.Save("A/Junk/Route", new MeekContent("Junk", "route table padding", false));
            repository.Save("some/existing/content", new MeekContent("An existing title", "Existing HTML content", false));
            repository.Save("content/for/edit", new MeekContent("An existing title", "Existing HTML content", false));
            repository.Save("content/for/delete", new MeekContent("An existing title", "Existing HTML content to delete", false));
            repository.Save("A/Partial/Page", new MeekContent(null, "Existing partial content", true));
            repository.Save("Partial/For/Edit", new MeekContent(null, "Existing partial content to edit", true));
            repository.Save("Another/Junk/Route", new MeekContent(null, "route table padding", true));

            repository.SaveFile(new MeekFile("PreLoaded", "image/jpeg",
                                             Assembly.GetExecutingAssembly().GetManifestResourceStream(
                                                 "Meek.ContentSite.Content.PreLoad.jpg").ReadFully()));
        }

    }
}