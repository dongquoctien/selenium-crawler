using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Support.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Crawler.AddExtentions
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = new ChromeOptions();
            options.AddExtension("Extentions/Touch-VPN.crx");

            options.AddArguments("--ignore-certificate-errors");
            options.AddArguments("--allow-running-insecure-content");
            options.PageLoadStrategy = PageLoadStrategy.Normal;


            var driver = new ChromeDriver(options);
            driver.Manage().Timeouts().ImplicitWait = TimeSpan.FromSeconds(10);
            Thread.Sleep(8000);

            //Switch To tab 1

            //if (tabs.Count > 1)
            //{
            //    driver.SwitchTo().Window(tabs[0]);
            //}

            for (int i = 0; i < 2; i++)
            {
                //Use touch VPN
                connectVPN(driver);

                driver.SwitchTo().Window(driver.WindowHandles[0]);
                driver.Navigate().GoToUrl("https://httpbin.org/get");
                driver.GetScreenshot().SaveAsFile(Guid.NewGuid().ToString() + ".png");
                Thread.Sleep(2000);
            }
           

        }

        private static void connectVPN(IWebDriver driver)
        {
            var keyTouchVPN = "bihmplhobchoageeokmgbdihknkjbknd";
            var urlExtention = $"chrome-extension://{keyTouchVPN}/panel/index.html";

            var tabs = driver.WindowHandles;
            driver.SwitchTo().Window(tabs[1]);
            driver.Navigate().GoToUrl(urlExtention);

            var className = driver.FindElement(By.Id("ConnectionButton")).GetAttribute("class");

            driver.FindElement(By.Id("ConnectionButton")).Click();
            Thread.Sleep(5000);
            if (className == "connected")
            {
                driver.FindElement(By.Id("ConnectionButton")).Click();
                Thread.Sleep(5000);
            }
        }
    }
}
