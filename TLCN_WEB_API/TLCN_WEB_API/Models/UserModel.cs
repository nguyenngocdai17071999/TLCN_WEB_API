using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TLCN_WEB_API.Models
{
    public class UserModel
    {
        public string UserName { get; set; }
        public string PassWord { get; set; }
        public string EmailAddress { get; set; }
        public string Status { get; set; }
    }
}
