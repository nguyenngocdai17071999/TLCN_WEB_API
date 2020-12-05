using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TLCN_WEB_API.Models
{
    public class Dish
    {
        public int Dish_ID { get; set; }
        public string DishName { get; set; }
        public string DishPrice { get; set; }
        public string DishPicture { get; set; }
        public int DishType_ID { get; set; }
        public int Menu_ID { get; set; }
    }
}
