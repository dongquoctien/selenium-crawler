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
            var options = new ChromeOptions();

            options.AddExcludedArgument("disable-popup-blocking");


            using (var driver = new ChromeDriver(options))
            {

                // _ =   GetReponseFromByNetworkAsync(driver);
                _ = GetReponseFromByNetworkAsync(driver);

                driver.Navigate().GoToUrl("https://vn.trip.com/hotels/detail/?cityId=359&hotelId=996749&checkIn=2024-03-12&checkOut=2024-03-13&adult=2&children=0&subStamp=624&crn=1&travelpurpose=0&curr=VND&link=title&hoteluniquekey=H4sIAAAAAAAAAONqY-JikmASYuJglPrNyNG89cQWVovfgo7fNGL6D7lGOniCGUdjHQJ4ChnAQMNhEmMGp8g694dVFi0OgmAxg0YHJW2O2x-6xAW0JO48vG-pwKoJUc3gYAhjWFwWCGLh4JdgiWJQUlBg0YQZZwhjWDzjDGLlmM8IUuHEyrGQT4JlBmPva5uNjBAVRg47GJlOMPItYLp1j20XE0TFISD9G6jlFBPY7EtMDLeYGB4xQcx5xcTwiYnhF1RpEzNYSRczwyRmiKZZzBB1i5gZpHgtDYyNkgzMjBKTk40TFYQ0zj7ft57NSGkSI1No8ClGKUNzQwMjA3NLEyNjAyO9VKM0Q8-AiJSK7JQAK2YpRjcPxiA2Q0cjMzfDKC0u5hAPJ0jIMHywl2IODXZRnLr7_MeLj-bYa4HkDEu2iv4-_e6kfRJrap5uaHDGS74Cxi5GDgFGD8YIxgrGV4wgPT9AngcAbJWpJaEBAAA&subChannel=&masterhotelid_tracelogid=9032b062acc3a&NewTaxDescForAmountshowtype0=T&detailFilters=17%7C1~17~1&hotelType=normal&barcurr=USD&locale=vi_vn");

                new Actions(driver).ScrollByAmount(0, 300).Perform();

                driver.Quit();
            }
        }

        public static async Task<ResponeModel> GetReponseFromByNetworkAsync(IWebDriver driver)
        {
            var session = ((IDevTools)driver).GetDevToolsSession();
            var domains = session.GetVersionSpecificDomains<OpenQA.Selenium.DevTools.V122.DevToolsSessionDomains>();

            var res = new ResponeModel();
            var getHotelRoomListRequestId = "";
            var url = "";
            // Request ID get Data
            domains.Network.ResponseReceived += (_, response) =>
            {
                Console.WriteLine($"ResponseReceived {response.RequestId} => {response.Response.Url} ");
                if (response.Response.Url.Contains("getHotelRoomList"))
                {
                    getHotelRoomListRequestId = response.RequestId;
                    url = response.Response.Url;
                }
            };

            domains.Network.LoadingFinished += async (_, response) =>
            {
                var requestID = response.RequestId;
                Console.WriteLine($"LoadingFinished => {requestID} ");
                if (getHotelRoomListRequestId == requestID)
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
