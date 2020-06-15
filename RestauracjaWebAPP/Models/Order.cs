using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestauracjaWebAPP.Models
{
    /// <summary>
    /// Table order object
    /// </summary>
    public class Order
    {
        /// <summary>
        /// List of ordered dishes
        /// </summary>
        public List<DishContainer> Dishes { get; set; } = new List<DishContainer>();

        /// <summary>
        /// Hold information if order is closed or not
        /// </summary>
        public bool Closed { get; set; } = true;

        /// <summary>
        /// Gets tip for this order
        /// </summary>
        /// <param name="tipPercentage">Tip percentage</param>
        /// <returns>Percentage value of order summed dishes</returns>
        public double GetTip(double tipPercentage)
        {
            double result = GetSum();
            return Math.Round((result/100)* tipPercentage, 2);
        }

        /// <summary>
        /// Gets sum price of the dishes
        /// </summary>
        /// <returns></returns>
        public double GetSum()
        {
            double result = 0;

            foreach (var dish in Dishes)
                result += dish.DishObject.Price * dish.Quantity;

            return result;
        }
    }
}