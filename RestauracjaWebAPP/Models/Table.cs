using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestauracjaWebAPP.Models
{
    public class Table
    {
        public List<Order> Orders { get; set; }
        public string Name { get; set; }
    }
}