using Newtonsoft.Json;
using RestauracjaWebAPP.Models;
using System;
using System.Collections.Generic;
using System.IO;
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

        [HttpGet]
        [Route("Home/UpdateTable/{id}")]
        public ActionResult UpdateTable(int id)
        {
            /*var inputJson = new StreamReader(Request.InputStream).ReadToEnd();
            var tableNumber = 0;
            tableNumber = JsonConvert.DeserializeObject<int>(inputJson);*/

            return PartialView("_TableView",DataAccess.Instance.GetTable(id));

        }
    }
}