using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TLCN_WEB_API.Models
{
    public class BienThongKeQuan
    {
        public string DistrictID { get; set; }
        public string DistrictName { get; set; }
        public int SoQuan { get; set; }
        public BienThongKeQuan()
        {
            DistrictID = "";
            DistrictName = "";
            SoQuan = 0;

        }
    }
}
