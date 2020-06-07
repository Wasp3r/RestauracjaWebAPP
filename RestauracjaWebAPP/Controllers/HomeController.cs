using RestauracjaWebAPP.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RestauracjaWebAPP.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            Room room = new Room();
            room.Tables = new List<Table>();
            for (int a = 0; a < 5; a++)
                room.Tables.Add(
                    new Table() 
                    { 
                        Name = $"Stolik {a+1}", Orders = new List<Order>() { new Order() }
                    });
            return View(room);
        }
    }
}