using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TLCN_WEB_API.Models
{
    public class GanToi
    {
        private string v;

        public GanToi()
        {
        }

        public GanToi(string storeID, string storeAddress, string storeName, string storePicture, string openTime, string cLoseTime, string userID, string provinceID, string menuID, string businessTypeID, string ratePoint, string v)
        {
            StoreID = storeID;
            StoreAddress = storeAddress;
            StoreName = storeName;
            StorePicture = storePicture;
            OpenTime = openTime;
            CLoseTime = cLoseTime;
            UserID = userID;
            ProvinceID = provinceID;
            MenuID = menuID;
            BusinessTypeID = businessTypeID;
            RatePoint = ratePoint;
            khoangcach = v;
        }

        public GanToi(string storeID, string storeAddress, string storeName, string storePicture, string openTime, string cLoseTime, string userID, string provinceID, string menuID, string businessTypeID, string ratePoint,string khoangCach, string v)
        {
            StoreID = storeID;
            StoreAddress = storeAddress;
            StoreName = storeName;
            StorePicture = storePicture;
            OpenTime = openTime;
            CLoseTime = cLoseTime;
            UserID = userID;
            ProvinceID = provinceID;
            MenuID = menuID;
            BusinessTypeID = businessTypeID;
            RatePoint = ratePoint;
            khoangcach = v;
        }

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
        public string khoangcach { get; set; }
    }
}
