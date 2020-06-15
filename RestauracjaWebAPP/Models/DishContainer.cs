using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestauracjaWebAPP.Models
{
    /// <summary>
    /// Dusg container - it is used in table <see cref="Order"/> to hold information about ordered dish
    /// </summary>
    public class DishContainer
    {
        /// <summary>
        /// Actual dish object
        /// </summary>
        public Dish DishObject { get; set; }
        /// <summary>
        /// Ordered quantity of <see cref="DishObject"/>
        /// </summary>
        public int Quantity { get; set; }
    }
}