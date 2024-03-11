using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Crawler
{
    public class HotelCrawlingModel
    {
        public HotelCrawlingModel()
        {
            RoomCrawlings = new List<RoomCrawlingModel>();
        }
        public string HotelCode { get; set; }
        public List<RoomCrawlingModel> RoomCrawlings { get; set; }
    }

    public class RoomCrawlingModel
    {
        public RoomCrawlingModel()
        {
            this.Price = new List<string>();
        }
        public string RoomType { get; set; }
        public List<string> Price { get; set; }
    }
}
