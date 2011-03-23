using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using TechTalk.SpecFlow;

namespace Meek.ContentSite.Features.Setups
{
    [Binding]
    public class WebServer
    {
        private static Process _iisProcess;
        private const int Port = 2222;

        public static string Site
        {
            get { return string.Format("http://localhost:{0}", Port); }
        }

        public static string GetUrl(string path, string query = null)
        {
            return (new UriBuilder(Site) { Path = path, Query = query }).Uri.AbsoluteUri;
        }

        [BeforeTestRun]
        public static void Setup()
        {
            var thread = new Thread(StartIisExpress) { IsBackground = true };
            thread.Start();
        }

        [AfterTestRun]
        public static void ShutDown()
        {
            _iisProcess.CloseMainWindow();
        }

        private static string GetSiteRunningPath
        {
            get
            {
                return
                    Path.Combine(
                        Path.GetDirectoryName(
                            Path.GetDirectoryName(Path.GetDirectoryName(Directory.GetCurrentDirectory()))),
                        "Meek.ContentSite");
            }
        }

        private static void StartIisExpress()
        {
            var startInfo = new ProcessStartInfo
            {
                WindowStyle = ProcessWindowStyle.Normal,
                ErrorDialog = true,
                LoadUserProfile = true,
                CreateNoWindow = false,
                UseShellExecute = false,
                RedirectStandardInput = true,
                Arguments = string.Format("/path:\"{0}\" /port:{1}", GetSiteRunningPath, Port)
            };

            var programfiles = string.IsNullOrEmpty(startInfo.EnvironmentVariables["programfiles(x86)"])
                                ? startInfo.EnvironmentVariables["programfiles"]
                                : startInfo.EnvironmentVariables["programfiles(x86)"];

            startInfo.FileName = programfiles + "\\IIS Express\\iisexpress.exe";

            try
            {
                _iisProcess = new Process { StartInfo = startInfo };
                _iisProcess.Start();
            }
            catch
            {
                _iisProcess.CloseMainWindow();
                _iisProcess.Dispose();
                throw;
            }
        }

    }
}
