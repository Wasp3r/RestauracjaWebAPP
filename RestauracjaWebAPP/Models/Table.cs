using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestauracjaWebAPP.Models
{
    public class Table
    {
        public int Id { get; set; }
        public int CurrentOrder { get; set; }
        public List<Order> Orders { get; set; } = new List<Order>();
        public string Name { get; set; }
    }
}