using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestauracjaWebAPP.Models
{
    public class Order
    {
        public List<DishContainer> Dishes { get; set; } = new List<DishContainer>();

        public bool Payed { get; set; }
    }
}