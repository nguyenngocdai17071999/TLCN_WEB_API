using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TLCN_WEB_API.Models
{
    public class Comment
    {
        public string CommentID { get; set; }
        public string Content { get; set; }
        public string Date { get; set; }
        public string Image { get; set; }
        public string UserID { get; set; }
        public string StoreID { get; set; }
        public string ParentComment_ID { get; set; }
    }
}
