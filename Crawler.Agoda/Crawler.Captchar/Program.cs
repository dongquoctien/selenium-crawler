using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools;
using OpenQA.Selenium.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crawler.Captchar
{
    class Program
    {
        static async Task Main()
        {
            var options = new ChromeOptions();
            options.AddArgument("start-maximized");
            options.AddArguments("disable-infobars"); // disabling infobars
            options.AddArguments("--disable-extensions"); // disabling extensions
            options.AddArguments("--disable-gpu"); // applicable to windows os only
            options.AddArguments("--disable-dev-shm-usage"); // overcome limited resource problems
            options.AddArguments("--no-sandbox"); // Bypass OS security model
            options.AddArguments("--incognito");
            options.AddArguments("--disable-blink-features=AutomationControlled");
            options.AddArguments("disable-infobars");

            var driver = new ChromeDriver(options);
            _ = await GetReponseFromByNetworkAsync(driver);
            driver.Navigate().GoToUrl("https://www.traveloka.com/en-th/hotel/detail?spec=29-04-2024.30-04-2024.1.1.HOTEL.1000000324420..2");
            new Actions(driver)
                .ScrollByAmount(0, 300)
                .Perform();
          

            //  driver.GetScreenshot().SaveAsFile(Guid.NewGuid().ToString() + ".png");
            //  driver.Quit();
        }

        public class ResponeModel
        {
            public string Url { get; set; }
            public string Body { get; set; }
        }

        public static async Task<ResponeModel> GetReponseFromByNetworkAsync(IWebDriver driver)
        {
           
            var session = ((IDevTools)driver).GetDevToolsSession();
            var domains = session.GetVersionSpecificDomains<OpenQA.Selenium.DevTools.V122.DevToolsSessionDomains>();

            var res = new ResponeModel();
            var dataRequestId = "";
            var url = "";
            // Request ID get Data
            domains.Network.ResponseReceived += (_, response) =>
            {
                try
                {
                    Console.WriteLine($"ResponseReceived {response.RequestId} => {response.Response.Url} ");
                    if (response.Response.Url.Contains("searchRooms"))
                    {
                        dataRequestId = response.RequestId;
                        url = response.Response.Url;
                    }
                }
                catch (Exception ex)
                {

                    Console.WriteLine($"ResponseReceived {response.RequestId} => {response.Response.Url} ");
                }

            };

            domains.Network.LoadingFinished += async (_, response) =>
            {
                var requestID = response.RequestId;
                Console.WriteLine($"LoadingFinished => {requestID} ");
                if (dataRequestId == requestID)
                {
                    try
                    {
                        var responseBody = await domains.Network.GetResponseBody(new OpenQA.Selenium.DevTools.V122.Network.GetResponseBodyCommandSettings
                        {
                            RequestId = requestID
                        });
                        res.Body = responseBody.Body;
                        res.Url = url;

                        Console.WriteLine($"Loading Finished Body => {responseBody.Body} ");
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Loading Finished({requestID}) fail: {ex.Message}");
                    }
                }

            };
            _ = await domains.Network.Enable(new OpenQA.Selenium.DevTools.V122.Network.EnableCommandSettings() { });

            return res;
        }

    }
}
