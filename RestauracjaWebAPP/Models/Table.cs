﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestauracjaWebAPP.Models
{
    public class Table
    {
        public int Id { get; set; }
        public Order CurrentOrder { get; set; } = new Order();
        public string Name { get; set; }
    }
}