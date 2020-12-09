using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TLCN_WEB_API.Models
{
    public class Store
    {
        public string StoreID { get; set; }
        public string StoreAddress { get; set; }
        public string StoreName { get; set; }
        public string StorePicture { get; set; }
        public string OpenTime { get; set; }
        public string CLoseTime { get; set; }
        public string UserID { get; set; }
        public string ProvinceID { get; set; }
        public string MenuID { get; set; }
        public string BusinessTypeID { get; set; }
        public string RatePoint { get; set; }
    }
}
