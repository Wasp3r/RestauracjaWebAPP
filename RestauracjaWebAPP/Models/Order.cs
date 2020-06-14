using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestauracjaWebAPP.Models
{
    public class Order
    {
        public List<DishContainer> Dishes { get; set; } = new List<DishContainer>();

        public bool Closed { get; set; } = true;

        public double GetTip(double tipPercentage)
        {
            double result = GetSum();
            return Math.Round((result/100)* tipPercentage, 2);
        }

        public double GetSum()
        {
            double result = 0;

            foreach (var dish in Dishes)
                result += dish.DishObject.Price * dish.Quantity;

            return result;
        }
    }
}