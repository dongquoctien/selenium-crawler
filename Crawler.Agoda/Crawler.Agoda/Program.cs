using System;
using System.Linq;
using System.Collections.Generic;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using Newtonsoft.Json;
using OpenQA.Selenium.DevTools;
using System.Threading.Tasks;

namespace Crawler
{

    class Program
    {
        static void Main()
        {
            using (var driver = new ChromeDriver())
            {
               _= GetReponseFromByNetworkAsync(driver);

                driver.Navigate().GoToUrl("https://www.agoda.com/ko-kr/seoul-olympic-parktel/hotel/seoul-kr.html?finalPriceView=1&isShowMobileAppPrice=false&cid=-1&numberOfBedrooms=&familyMode=false&adults=2&children=0&rooms=1&maxRooms=0&checkIn=2024-04-24&isCalendarCallout=false&childAges=&numberOfGuest=0&missingChildAges=false&travellerType=1&showReviewSubmissionEntry=false&currencyCode=KRW&isFreeOccSearch=false&isCityHaveAsq=false&tspTypes=7%2C8&los=1&searchrequestid=a255e89b-1ab0-4931-b82a-6c6eef132580&ds=Zzdt8f7kSBVbwXRp");

                //demo get room by element
                var json = JsonConvert.SerializeObject(GetFromElementByAttribute(driver));
                Console.WriteLine("GetFromElementByAttribute", json);


                driver.Quit();
            }
        }

        private static void ResponseReceived(object sender, OpenQA.Selenium.DevTools.V122.Network.ResponseReceivedEventArgs e)
        {
            Console.WriteLine(sender);
        }


        //by element
        public static HotelCrawlingModel GetFromElementByAttribute(IWebDriver driver)
        {
            var model = new HotelCrawlingModel();
            model.HotelCode = driver.FindElement(By.CssSelector("[data-selenium='hotel-header-name']")).Text;

            var roomgrid = driver.FindElement(By.CssSelector("[data-selenium='roomgrid-container']"));
            var rooms = roomgrid.FindElements(By.CssSelector("[data-selenium='MasterRoom']"));

            foreach (var room in rooms)
            {
                var elementID = room.GetAttribute("id");

                var roomCrawl = new RoomCrawlingModel();
                roomCrawl.RoomType = room.FindElement(By.CssSelector("[data-selenium='masterroom-title-name']")).Text;
                //price list
                var roomPrices = room.FindElements(By.CssSelector("[data-selenium='ChildRoomsList-room']"));
                foreach (var price in roomPrices)
                {
                    roomCrawl.Price.Add(price.FindElement(By.CssSelector("[data-ppapi='room-price']")).Text);
                }
                model.RoomCrawlings.Add(roomCrawl);
            }

            return model;
        }

        public class ResponeModel
        {
            public string Url { get; set; }
            public string Body { get; set; }
        }

        public static async Task<List<ResponeModel>> GetReponseFromByNetworkAsync(IWebDriver driver)
        {
            var model = new HotelCrawlingModel();
            var session = ((IDevTools)driver).GetDevToolsSession();
            var domains = session.GetVersionSpecificDomains<OpenQA.Selenium.DevTools.V122.DevToolsSessionDomains>();

            var res = new List<ResponeModel>();
            domains.Network.ResponseReceived += (_, response) =>
            {
                if (response.Response.Url.Contains("api/personalization/PersonalizeRecommendedProperties/v1"))
                {
                    Console.WriteLine($"Intercepted response for URL: {response.Response.Url}");

                    // Access the response body
                    var responseBody = domains.Network.GetResponseBody(new OpenQA.Selenium.DevTools.V122.Network.GetResponseBodyCommandSettings
                    {
                        RequestId = response.RequestId
                    });

                    Console.WriteLine($"Response Data: {responseBody.Result.Body}");
                    res.Add(new ResponeModel()
                    {
                        Url = response.Response.Url,
                        Body = responseBody.Result.Body
                    });
                }
            };
            _ = await domains.Network.Enable(new OpenQA.Selenium.DevTools.V122.Network.EnableCommandSettings());

            return res;
        }
    }
}
