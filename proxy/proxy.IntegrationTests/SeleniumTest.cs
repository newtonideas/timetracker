using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System.IO;

namespace proxy.IntegrationTests
{
    [TestClass]
    public class SeleniumTest
    {
        //reference for the browser
        private IWebDriver driver;

        [TestInitialize]
        public void Initialize() {
            driver = new ChromeDriver(Directory.GetCurrentDirectory());
            
        }

        [TestMethod]
        public void GoogleSearchTest() {
            //navigation to the webpage
            driver.Navigate().GoToUrl(@"https://www.google.com.ua/");
            //element selection
            IWebElement searchField = driver.FindElement(By.Name("q"));
            //typing in the field
            searchField.SendKeys("newton");

            IWebElement searchBtn = driver.FindElement(By.Name("btnK"));
            searchBtn.Submit();
            
        }

        [TestCleanup]
        public void Cleanup() {
            driver.Close();
        }
    }
}
