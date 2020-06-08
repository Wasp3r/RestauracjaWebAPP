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
            DataAccess.Instance.Initialize();
            return View(DataAccess.Instance.GetRoom);
        }


    }
}