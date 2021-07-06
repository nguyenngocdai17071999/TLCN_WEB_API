using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TLCN_WEB_API.Models
{
    public class BienThongKeTinh
    {
        public string ProvinceID { get; set; }
        public string ProvinceName { get; set; }
        public int SoQuan { get; set; }
        public BienThongKeTinh(){
            ProvinceID = "";
            ProvinceName = "";
            SoQuan = 0;

        }
    }
}
