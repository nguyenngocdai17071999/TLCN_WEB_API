using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TLCN_WEB_API.Models
{
    public class Dish
    {
        public string Dish_ID { get; set; }
        public string DishName { get; set; }
        public string DishPrice { get; set; }
        public string DishPicture { get; set; }
        public string DishType_ID { get; set; }
        public string Menu_ID { get; set; }
    }
}
