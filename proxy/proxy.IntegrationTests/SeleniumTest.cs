using Microsoft.VisualStudio.TestTools.UnitTesting;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using System;
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

            IWebElement firstLink = driver.FindElement(By.XPath("(//h3)[1]/a"));
            firstLink.Click();

            IWebElement theoryOfColour = driver.FindElement(By.LinkText("theory of colour"));
            theoryOfColour.Click();

            driver.Navigate().Back();
            
            IWebElement createAcc = driver.FindElement(By.Id("pt-createaccount"));
            createAcc.Click();

            driver.FindElement(By.Id("wpName2")).SendKeys("tti_newton");
            driver.FindElement(By.Id("wpPassword2")).SendKeys("tti_newton");
            driver.FindElement(By.Id("wpRetype")).SendKeys("tti_newton");
            driver.FindElement(By.Id("wpEmail")).SendKeys("tti@newton.ideas");
            
            driver.FindElement(By.Id("pt-login")).Click();

            driver.FindElement(By.Id("wpName1")).SendKeys("tti_newton");
            driver.FindElement(By.Id("wpPassword1")).SendKeys("tti_newton");

            driver.FindElement(By.Id("wpRemember")).Click();
            driver.FindElement(By.Id("wpLoginAttempt")).Click();

            driver.FindElement(By.LinkText("Log out")).Click();

            driver.Navigate().GoToUrl(@"https://www.google.com.ua/search?q=newton");
        }

        [TestCleanup]
        public void Cleanup() {
            driver.Close();
        }
    }
}
