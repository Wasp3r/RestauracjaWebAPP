using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestauracjaWebAPP.Models
{
    /// <summary>
    /// Room object
    /// </summary>
    public class Room
    {
        /// <summary>
        /// List of the tables
        /// </summary>
        public List<Table> Tables { get; set; } = new List<Table>();
    }
}