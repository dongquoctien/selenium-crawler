using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.DevTools;
using OpenQA.Selenium.Interactions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Crawler.TripCom
{
    class Program
    {
        public class ResponeModel
        {
            public string Url { get; set; }
            public string Body { get; set; }
        }

        static void Main(string[] args)
        {
            // Create a ManualResetEventSlim object
            ManualResetEventSlim manualEvent = new ManualResetEventSlim();

            // Start a new thread to do some work
            ThreadPool.QueueUserWorkItem(state =>
            {
                // Simulate some work
                Thread.Sleep(5000);

                // Signal that the work is done
                manualEvent.Set();
            });

            // Wait for the event to be set or 30 seconds to pass
            if (!manualEvent.Wait(30000))
            {
                // If 30 seconds have passed and the event is still not set, do something
                Console.WriteLine("Timed out waiting for the event to be set.");
            }
            else
            {
                // If the event is set before the timeout, do something else
                Console.WriteLine("Event was set.");
            }
        }


        //static void Main(string[] args)
        //{
        //    var options = new ChromeOptions();

        //    options.AddExcludedArgument("disable-popup-blocking");


        //    using (var driver = new ChromeDriver(options))
        //    {

        //        // _ =   GetReponseFromByNetworkAsync(driver);
        //        _ = GetReponseFromByNetworkAsync(driver);

        //        driver.Navigate().GoToUrl("https://vn.trip.com/hotels/detail/?cityId=359&hotelId=996749&checkIn=2024-03-12&checkOut=2024-03-13&adult=2&children=0&subStamp=624&crn=1&travelpurpose=0&curr=VND&link=title&hoteluniquekey=H4sIAAAAAAAAAONqY-JikmASYuJglPrNyNG89cQWVovfgo7fNGL6D7lGOniCGUdjHQJ4ChnAQMNhEmMGp8g694dVFi0OgmAxg0YHJW2O2x-6xAW0JO48vG-pwKoJUc3gYAhjWFwWCGLh4JdgiWJQUlBg0YQZZwhjWDzjDGLlmM8IUuHEyrGQT4JlBmPva5uNjBAVRg47GJlOMPItYLp1j20XE0TFISD9G6jlFBPY7EtMDLeYGB4xQcx5xcTwiYnhF1RpEzNYSRczwyRmiKZZzBB1i5gZpHgtDYyNkgzMjBKTk40TFYQ0zj7ft57NSGkSI1No8ClGKUNzQwMjA3NLEyNjAyO9VKM0Q8-AiJSK7JQAK2YpRjcPxiA2Q0cjMzfDKC0u5hAPJ0jIMHywl2IODXZRnLr7_MeLj-bYa4HkDEu2iv4-_e6kfRJrap5uaHDGS74Cxi5GDgFGD8YIxgrGV4wgPT9AngcAbJWpJaEBAAA&subChannel=&masterhotelid_tracelogid=9032b062acc3a&NewTaxDescForAmountshowtype0=T&detailFilters=17%7C1~17~1&hotelType=normal&barcurr=USD&locale=vi_vn");

        //        IWebElement footer = driver.FindElement(By.CssSelector("div.mc-ft.mc-ft_line"));
        //        int deltaY = footer.Location.Y;
        //        new Actions(driver, TimeSpan.FromSeconds(1))
        //            .ScrollByAmount(0, deltaY)
        //            .Perform();


        //       // var hotelName = driver.FindElement(By.XPath("/html/body/div[2]/div[3]/div[1]/div[1]/div[1]/div[1]/h1")).Text;

        //        driver.Quit();
        //    }
        //}

        public static async Task<List<ResponeModel>> GetReponseFromByNetworkAsync(IWebDriver driver)
        {
            var session = ((IDevTools)driver).GetDevToolsSession();
            var domains = session.GetVersionSpecificDomains<OpenQA.Selenium.DevTools.V122.DevToolsSessionDomains>();

            var res = new List<ResponeModel>();
            domains.Network.ResponseReceived += async (_, response) =>
            {
                if (response.Response.Url.Contains("getHotelRoomList"))
                {
                    Console.WriteLine($"Intercepted response for URL: {response.Response.Url}");

                    // Access the response body
                    var responseBody = await domains.Network.GetResponseBody(new OpenQA.Selenium.DevTools.V122.Network.GetResponseBodyCommandSettings
                    {
                        RequestId = response.RequestId
                    });

                    Console.WriteLine($"Response Data: {responseBody.Body}");
                    res.Add(new ResponeModel()
                    {
                        Url = response.Response.Url,
                        Body = responseBody.Body
                    });
                }
            };
            _ = await domains.Network.Enable(new OpenQA.Selenium.DevTools.V122.Network.EnableCommandSettings());

            return res;
        }

        private bool IsInViewport(IWebElement element, IWebDriver driver)
        {
            String script =
                "for(var e=arguments[0],f=e.offsetTop,t=e.offsetLeft,o=e.offsetWidth,n=e.offsetHeight;\n"
                + "e.offsetParent;)f+=(e=e.offsetParent).offsetTop,t+=e.offsetLeft;\n"
                + "return f<window.pageYOffset+window.innerHeight&&t<window.pageXOffset+window.innerWidth&&f+n>\n"
                + "window.pageYOffset&&t+o>window.pageXOffset";
            IJavaScriptExecutor javascriptDriver = driver as IJavaScriptExecutor;

            return (bool)javascriptDriver.ExecuteScript(script, element);
        }
    }
}
