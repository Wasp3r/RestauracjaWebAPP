using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestauracjaWebAPP.Models
{
    /// <summary>
    /// Table object
    /// </summary>
    public class Table
    {
        /// <summary>
        /// ID of the table
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Current table order
        /// </summary>
        public Order CurrentOrder { get; set; } = new Order();

        /// <summary>
        /// Name of the table
        /// </summary>
        public string Name { get; set; }
    }
}