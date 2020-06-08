using Newtonsoft.Json;
using RestauracjaWebAPP.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Management;
using System.Web.Mvc;

namespace RestauracjaWebAPP.Controllers
{
    public class _TableViewController : Controller
    {
        [HttpPost]
        [Route("Table/UpdateDish")]
        public ActionResult updateQuantity()
        {
            var inputJson = new StreamReader(Request.InputStream).ReadToEnd();
            var inputContainer = new {
                orderId = -1,
                dishId = -1,
                tableId = -1,
                quantity = -1
            };

            inputContainer = JsonConvert.DeserializeAnonymousType(inputJson, inputContainer);
            if (inputContainer.tableId.Equals(-1) || inputContainer.quantity.Equals(-1) || inputContainer.dishId.Equals(-1))
                return Json(new { success = false, message = "Error parsing input body" });

            try
            {
                DataAccess.Instance.UpdateDish(
                    inputContainer.tableId, 
                    inputContainer.orderId, 
                    inputContainer.dishId, 
                    inputContainer.quantity);

            } catch(Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

            double price = DataAccess.Instance.Dishes.Find(x=> x.Id == inputContainer.dishId).Price;
            return Json(new { success = true, message = Math.Round(price * inputContainer.quantity, 2) });
        }
    }
}