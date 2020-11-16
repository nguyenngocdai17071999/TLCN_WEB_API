using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TLCN_WEB_API.Models
{
    public class Store
    {
        public int StoreID { get; set; }
        public string StoreAddress { get; set; }
        public string StoreName { get; set; }
        public string StorePicture { get; set; }
        public string OpenTime { get; set; }
        public string CLoseTime { get; set; }
        public int UserID { get; set; }
        public int ProvinceID { get; set; }
        public int MenuID { get; set; }
        public int BusinessTypeID { get; set; }
        public int RatePoint { get; set; }
    }
}
