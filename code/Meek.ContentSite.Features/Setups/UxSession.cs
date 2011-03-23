using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using OpenQA.Selenium;
using OpenQA.Selenium.IE;
using TechTalk.SpecFlow;

namespace Meek.ContentSite.Features.Setups
{
    [Binding]
    public class UxSession
    {
        public static IWebDriver Driver;

        [BeforeScenario("UXSession")]
        [BeforeFeature("UXSession")]
        public static void SetupWebDriver()
        {
            Driver = new InternetExplorerDriver();
        }

        [AfterScenario("UXSession")]
        [AfterFeature("UXSession")]
        public static void ShutdownWebDriver()
        {
            Driver.Quit();
        }

    }
}
