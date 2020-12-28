using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TLCN_WEB_API.Models
{
    public class UserCommentInfo
    {
        public UserCommentInfo()
        {
        }

        public UserCommentInfo(string userName, string picture, string email)
        {
            UserName = userName;
            Picture = picture;
            Email = email;
        }

        public string UserName { get; set; }
        public string Email { get; set; }
        public string Picture { get; set; }
    }
}
