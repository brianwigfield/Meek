using System;
using Meek.ContentSite.Features.Setups;
using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Support.UI;
using TechTalk.SpecFlow;

namespace Meek.ContentSite.Features.Definitions
{
    [Binding]
    public class ContentDefinitions
    {

        [Given(@"I have logged in as a content admin")]
        public void GivenIHaveLoggedInAsAContentAdmin()
        {
            UxSession.Driver.Navigate().GoToUrl(WebServer.GetUrl("/Account/LogOn"));
            var wait = new WebDriverWait(UxSession.Driver, TimeSpan.FromSeconds(10));
            wait.Until(x => x.FindElement(By.Id("UserName")) != null);

            UxSession.Driver.FindElement(By.Id("UserName")).SendKeys("admin");
            UxSession.Driver.FindElement(By.Id("Password")).SendKeys("password");
            UxSession.Driver.FindElement(By.XPath("//input[@value='Log On']")).SendKeys(Keys.Enter);

            wait.Until(x => x.Title == "Home Page");
        }

        [When(@"I ask for a non existent page")]
        public void WhenIAskForANonExistentPage()
        {
            UxSession.Driver.Navigate().GoToUrl(WebServer.GetUrl("/NoPage/" + DateTime.Now.ToString("yyyyMMddhhmmss")));
        }

        [When(@"I ask to edit an existing page")]
        public void WhenIAskToEditAnExistingPage()
        {
            UxSession.Driver.Navigate().GoToUrl(WebServer.GetUrl("/Meek/Manage", "aspxerrorpath=content/for/edit"));
        }

        [When(@"I ask to edit an un-needed page")]
        public void WhenIAskToEditAnUnNeededPage()
        {
            UxSession.Driver.Navigate().GoToUrl(WebServer.GetUrl("/Meek/Manage", "aspxerrorpath=content/for/delete"));
        }

        [When(@"I ask to edit an existing partial content")]
        public void WhenIAskToEditAnExistingPartialContent()
        {
            UxSession.Driver.Navigate().GoToUrl(WebServer.GetUrl("/Meek/Manage", "aspxerrorpath=partial/for/edit"));
        }

        [When(@"I ask to view an existing page with partial content")]
        public void WhenIAskToViewAnExistingPageWithPartialContent()
        {
            UxSession.Driver.Navigate().GoToUrl(WebServer.GetUrl("/"));
        }

        [When(@"I ask for a page with non existent partial content")]
        public void WhenIAskForAPageWithNonExistentPartialContent()
        {
            UxSession.Driver.Navigate().GoToUrl(WebServer.GetUrl("/Home/News"));
        }

        [When(@"I navigate to a partial content resource")]
        public void WhenINavigateToAPartialContentResource()
        {
            UxSession.Driver.Navigate().GoToUrl(WebServer.GetUrl("/A/Partial/Page"));
        }

        [When(@"I navigate to a content page")]
        public void WhenINavigateToAContentPage()
        {
            UxSession.Driver.Navigate().GoToUrl(WebServer.GetUrl("/some/existing/content"));            
        }

        [When(@"I choose a file & press upload")]
        public void WhenIChooseAFilePressUpload()
        {
            var wait = new WebDriverWait(UxSession.Driver, TimeSpan.FromSeconds(5));
            wait.Until(x => UxSession.Driver.FindElement(By.Id("cke_67")) != null);
            //for some reason the webdriver click won't work correctly
            (UxSession.Driver as IJavaScriptExecutor).ExecuteScript("$(\"#cke_67\").click();");

            wait.Until(x => UxSession.Driver.FindElement(By.LinkText("Upload")) != null);
            UxSession.Driver.FindElement(By.LinkText("Upload")).Click();

            wait.Until(x => UxSession.Driver.FindElement(By.Name("upload")) != null);
            UxSession.Driver.FindElement(By.Name("upload")).Click();


            ScenarioContext.Current.Pending();
        }

        [When(@"I press navigate to the list page")]
        public void WhenIPressNavigateToTheListPage()
        {
            UxSession.Driver.Navigate().GoToUrl(WebServer.GetUrl("/Meek/List"));
        }

        [Then(@"it should present me with a create content link")]
        public void ThenItShouldPresentMeWithACreateContentLink()
        {
            var wait = new WebDriverWait(UxSession.Driver, TimeSpan.FromSeconds(5));
            wait.Until(x => UxSession.Driver.FindElement(By.ClassName("MeekCreateLink")) != null);
        }

        [Then(@"it should show an empty section")]
        public void ThenItShouldShowAnEmptySection()
        {
            var wait = new WebDriverWait(UxSession.Driver, TimeSpan.FromSeconds(5));
            wait.Until(x => x.PageSource.Contains("Daily"));

            try
            {
                UxSession.Driver.FindElement(By.ClassName("MeekCreateLink"));
                Assert.Fail("Create link exists but shouldn't.");
            }
            catch (NoSuchElementException)
            {
                Assert.Pass();
            }
        }

        [Then(@"It should present me with an edit link")]
        public void ThenItShouldPresentMeWithAnEditLink()
        {
            var wait = new WebDriverWait(UxSession.Driver, TimeSpan.FromSeconds(5));
            wait.Until(x => UxSession.Driver.FindElement(By.ClassName("MeekEditLink")) != null);
        }

        [Then(@"it should provide me with a blank content editor")]
        public void ThenItShouldProvideMeWithABlankContentEditor()
        {
            var wait = new WebDriverWait(UxSession.Driver, TimeSpan.FromSeconds(5));
            wait.Until(x => UxSession.Driver.FindElement(By.Id("EditorContents")) != null);
            Assert.IsNullOrEmpty(UxSession.Driver.FindElement(By.Id("EditorContents")).Value.Trim());
        }

        [Then(@"It should not present me with an edit link")]
        public void ThenItShouldNotPresentMeWithAnEditLink()
        {
            var wait = new WebDriverWait(UxSession.Driver, TimeSpan.FromSeconds(5));
            wait.Until(x => x.PageSource.Contains("Existing HTML content") || x.PageSource.Contains("Existing partial content"));

            try
            {
                UxSession.Driver.FindElement(By.ClassName("MeekEditLink"));
                Assert.Fail("Edit link exists but shouldn't.");
            }
            catch (NoSuchElementException)
            {
                Assert.Pass();
            }
        }

        [Then(@"Allow me to create my content")]
        public void ThenAllowMeToCreateMyContent()
        {
            var browserJs = (IJavaScriptExecutor)UxSession.Driver;
            browserJs.ExecuteScript("CKEDITOR.instances['EditorContents'].setData('My custom content!');");

            UxSession.Driver.FindElement(By.Id("ContentTitle")).SendKeys("A new title");
            UxSession.Driver.FindElement(By.Id("SaveContent")).SendKeys(Keys.Enter);

            var wait = new WebDriverWait(UxSession.Driver, TimeSpan.FromSeconds(5));
            wait.Until(x => x.Title == "A new title");
            Assert.IsTrue(UxSession.Driver.PageSource.Contains("My custom content!"));
        }

        [Then(@"allow me to edit my existing partial content")]
        public void ThenAllowMeToEditMyExistingPartialContent()
        {
            var browserJs = (IJavaScriptExecutor)UxSession.Driver;
            browserJs.ExecuteScript("CKEDITOR.instances['EditorContents'].setData('My new partial content');");

            UxSession.Driver.FindElement(By.Id("SaveContent")).SendKeys(Keys.Enter);
        }

        [Then(@"return me to the home page")]
        public void ThenReturnMeToTheHomePage()
        {
            var wait = new WebDriverWait(UxSession.Driver, TimeSpan.FromSeconds(5));
            wait.Until(x => x.Url == WebServer.GetUrl(null));
        }

        [Then(@"let me navigate to view my partial content")]
        public void ThenLetMeNavigateToViewMyPartialContent()
        {
            UxSession.Driver.Navigate().GoToUrl(WebServer.GetUrl("/Home/About"));

            var wait = new WebDriverWait(UxSession.Driver, TimeSpan.FromSeconds(5));
            wait.Until(x => x.PageSource.Contains("My new partial content"));
        }


        [Then(@"it should provide me with a not found page")]
        public void ThenItShouldProvideMeWithANotFoundPage()
        {
            var wait = new WebDriverWait(UxSession.Driver, TimeSpan.FromSeconds(5));
            wait.Until(x => x.PageSource.Contains("Page Not Found"));
            Assert.True(true);
        }

        [Then(@"it should provide me with a populated content editor")]
        public void ThenItShouldProvideMeWithAPopulatedContentEditor()
        {
            var wait = new WebDriverWait(UxSession.Driver, TimeSpan.FromSeconds(5));
            wait.Until(x => UxSession.Driver.FindElement(By.Id("EditorContents")) != null);
            Assert.IsTrue(UxSession.Driver.FindElement(By.Id("EditorContents")).Value.Contains("Existing HTML content"));
            Assert.AreEqual(UxSession.Driver.FindElement(By.Id("ContentTitle")).Value, "An existing title");
        }

        [Then(@"it should provide me with a populated partial content editor")]
        public void ThenItShouldProvideMeWithAPopulatedPartialContentEditor()
        {
            var wait = new WebDriverWait(UxSession.Driver, TimeSpan.FromSeconds(5));
            wait.Until(x => UxSession.Driver.FindElement(By.Id("EditorContents")) != null);
            Assert.IsTrue(UxSession.Driver.FindElement(By.Id("EditorContents")).Value.Contains("Existing partial content to edit"));
            Assert.IsEmpty(UxSession.Driver.FindElement(By.Id("ContentTitle")).Value);
        }

        [Then(@"Allow me to edit my existing content")]
        public void ThenAllowMeToEditMyExistingContent()
        {
            var browserJs = (IJavaScriptExecutor)UxSession.Driver;
            browserJs.ExecuteScript("CKEDITOR.instances['EditorContents'].setData('My new HTML content');");

            UxSession.Driver.FindElement(By.Id("ContentTitle")).SendKeys("Changed ");
            UxSession.Driver.FindElement(By.Id("SaveContent")).SendKeys(Keys.Enter);

            var wait = new WebDriverWait(UxSession.Driver, TimeSpan.FromSeconds(5));
            wait.Until(x => x.Title == "Changed An existing title");
            Assert.IsTrue(UxSession.Driver.PageSource.Contains("My new HTML content"));
        }

        [Then(@"allow me to delete the content and return me to the home page")]
        public void ThenAllowMeToDeleteTheContentAndReturnMeToTHeHomePage()
        {
            UxSession.Driver.FindElement(By.Id("DeleteContent")).SendKeys(Keys.Enter);

            var wait = new WebDriverWait(UxSession.Driver, TimeSpan.FromSeconds(5));
            wait.Until(x => x.Url == WebServer.GetUrl(null));

            //Verify if you try to view the page it goes to the page to create it
            UxSession.Driver.Navigate().GoToUrl(WebServer.GetUrl("/content/for/delete"));
            wait.Until(x => x.Url == WebServer.GetUrl("/Missing", "aspxerrorpath=/content/for/delete"));

        }

        [Then(@"the it should show a list of links")]
        public void ThenTheItShouldShowAListOfLinks()
        {
            var wait = new WebDriverWait(UxSession.Driver, TimeSpan.FromSeconds(5));
            wait.Until(x => UxSession.Driver.FindElement(By.LinkText("/content/for/edit")) != null);

            UxSession.Driver.FindElement(By.LinkText("/content/for/edit")).Click();
            wait.Until(x => UxSession.Driver.Title == "Manage content");
        }

    }
}
