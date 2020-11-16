using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TLCN_WEB_API.Models
{
    public class Comment
    {
        public int CommentID { get; set; }
        public string Content { get; set; }
        public string Date { get; set; }
        public string Image { get; set; }
        public int UserID { get; set; }
        public int StoreID { get; set; }
        public int ParentComment_ID { get; set; }
    }
}
