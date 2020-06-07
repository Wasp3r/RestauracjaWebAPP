using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestauracjaWebAPP.Models
{
    public class Order
    {
        public List<Dish> Dishes { get; set; }

        public Order()
        {
            Dishes = new List<Dish>() { new Dish() { Name = "Jajo", Price = 5, Quantity = 1 } };

        }
    }
}