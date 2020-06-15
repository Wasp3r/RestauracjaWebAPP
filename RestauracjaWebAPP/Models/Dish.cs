using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestauracjaWebAPP.Models
{
    /// <summary>
    /// Dish object
    /// </summary>
    public class Dish
    {
        /// <summary>
        /// ID of the dish
        /// </summary>
        public int Id { get; set; }
        /// <summary>
        /// Name of the dish
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        /// Price of the dish
        /// </summary>
        public double Price { get; set; }
    }
}