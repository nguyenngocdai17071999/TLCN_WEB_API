using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TLCN_WEB_API.Models
{
    public class BienThongKeBusinessType
    {
        public string BusinessTypeID { get; set; }
        public string BusinessTypetName { get; set; }
        public int SoQuan { get; set; }
        public BienThongKeBusinessType()
        {
            BusinessTypeID = "";
            BusinessTypetName = "";
            SoQuan = 0;

        }
    }
}
