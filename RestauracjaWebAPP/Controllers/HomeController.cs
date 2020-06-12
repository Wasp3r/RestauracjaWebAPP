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
            Initialize();
            return View(GetRoom());
        }

        [HttpGet]
        [Route("Home/UpdateTable/{id}")]
        public ActionResult UpdateTable(int id)
        {
            return PartialView("_TableView", GetTable(id));
        }

        [HttpPost]
        [Route("Table/UpdateDish")]
        public ActionResult UpdateQuantity()
        {
            var inputJson = new StreamReader(Request.InputStream).ReadToEnd();
            var inputContainer = new
            {
                dishId = -1,
                tableId = -1,
                quantity = -1
            };

            try
            {
                inputContainer = JsonConvert.DeserializeAnonymousType(inputJson, inputContainer);
                if (inputContainer.tableId.Equals(-1) || inputContainer.quantity.Equals(-1) || inputContainer.dishId.Equals(-1))
                    return Json(new { success = false, message = "Error parsing input body" });

                UpdateDish(
                    inputContainer.tableId,
                    inputContainer.dishId,
                    inputContainer.quantity);

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

            double price = GetDishPrice(inputContainer.dishId);
            DishContainer dishContainer = GetTableDish(inputContainer.dishId, inputContainer.tableId);
            return Json(new { success = true, message = Math.Round(price * dishContainer.Quantity, 2) });
        }

        [HttpPost]
        [Route("Table/RemoveOrder")]
        public ActionResult RemoveOrder()
        {
            var inputJson = new StreamReader(Request.InputStream).ReadToEnd();
            var inputContainer = new
            {
                tableId = -1,
            };

            try
            {
                inputContainer = JsonConvert.DeserializeAnonymousType(inputJson, inputContainer);
                RemoveOrder(inputContainer.tableId);
            } catch
            {
                return Json(new { success = false, message = "Nie udało się usunąć zamówienia!" });
            }

            return Json(new { success = true, message = "OK" });
        }

        [HttpPost]
        [Route("Table/CreateOrder")]
        public ActionResult CreateNewOrder()
        {
            var inputJson = new StreamReader(Request.InputStream).ReadToEnd();
            var inputContainer = new
            {
                tableId = -1,
            };

            try
            {
                inputContainer = JsonConvert.DeserializeAnonymousType(inputJson, inputContainer);
                CreateOrder(inputContainer.tableId);
            }
            catch
            {
                return Json(new { success = false, message = "Nie udało się stworzyć zamówienia!" });
            }

            return Json(new { success = true, message = "OK" });
        }

        [HttpGet]
        [Route("GetDishes")]
        public ActionResult GetDishes()
        {
            List<Dish> dishes = GetAllDishes();
            return Json(new { success = true, message = dishes }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [Route("GetDishPrice")]
        public ActionResult GetDishPrice()
        {
            var inputJson = new StreamReader(Request.InputStream).ReadToEnd();
            double price = 0;
            var inputContainer = new
            {
                dishId = -1,
            };

            try
            {
                inputContainer = JsonConvert.DeserializeAnonymousType(inputJson, inputContainer);
                price = GetDish(inputContainer.dishId).Price;
            }
            catch
            {
                return Json(new { success = false, message = "Błąd pobierania ceny dania" });
            }

            return Json(Math.Round(price, 2));
        }

        [HttpPost]
        [Route("GetTableDishQuantity")]
        public ActionResult GetTableDishQuantity()
        {
            var inputJson = new StreamReader(Request.InputStream).ReadToEnd();
            double quantity = 0;
            var inputContainer = new
            {
                dishId = -1,
                tableId = -1
            };

            try
            {
                inputContainer = JsonConvert.DeserializeAnonymousType(inputJson, inputContainer);
                quantity = GetTableDish(inputContainer.dishId, inputContainer.tableId).Quantity;
            }
            catch
            {
                return Json(new { success = false, message = "Błąd pobierania zamówionej ilości dania" });
            }

            return Json(quantity);
        }


        #region Session Controller

        private void Initialize()
        {
            Room room = GetRoom();
            if (room != null)
                return;

            room = new Room();
            room.Tables = new List<Table>();
            GenerateDishes();

            for (int a = 0; a < 5; a++)
                room.Tables.Add(CreateTable(a));

            UpdateRoom(room);
        }

        #region Dish Methods

        private Dish GetDish(int dishId)
        {
            List<Dish> dishes = GetAllDishes();

            if (dishes.Find(x => x.Id == dishId) == null)
                throw new Exception($"Danie o id {dishId} nie isntieje!");

            return dishes.Find(x => x.Id == dishId);
        }

        private List<Dish> GetAllDishes()
        {
            return (List< Dish>)Session["Dishes"];
        }

        private double GetDishPrice(int dishId)
        {
            return GetDish(dishId).Price;
        }

        private DishContainer GetTableDish(int dishId, int tableId)
        {
            return GetOrder(tableId).Dishes[dishId];
        }

        /// <summary>
        /// Generate dummy data to work with
        /// </summary>
        private void GenerateDishes()
        {
            List<Dish> dishes = new List<Dish>();
            dishes.AddRange(new List<Dish>() {
                new Dish() {
                    Id = 0,
                    Name = "Jajecznica",
                    Price = 2
                },
                new Dish() {
                    Id = 1,
                    Name = "Schabowy",
                    Price = 10
                },
                new Dish() {
                    Id = 2,
                    Name = "Filet z kurczaka",
                    Price = 8
                },
                new Dish() {
                    Id = 3,
                    Name = "Kawa",
                    Price = 5
                }
            });
            Session["Dishes"] = dishes;
        }

        private void AddDish(int dishId, int quantity, int tableId)
        {
            Order order = GetOrder(tableId);
            DishContainer dishFromExisting = order.Dishes.Find(x => x.DishObject.Id == dishId);

            if (dishFromExisting == null)
            {
                DishContainer newDishContainer = new DishContainer()
                {
                    DishObject = GetDish(dishId),
                    Quantity = quantity
                };
                order.Dishes.Add(newDishContainer);
            }
            else
                order.Dishes.Find(x => x.DishObject.Id == dishId).Quantity = quantity;

            Room room = GetRoom();
            room.Tables[tableId].CurrentOrder = order;
            UpdateRoom(room);
        }

        private void UpdateDish(int tableId, int dishId, int quantity)
        {
            Order order = GetOrder(tableId);

            if (order.Payed)
                throw new Exception("Nie można edytować opłaconego zamówienia!");

            AddDish(dishId, quantity, tableId);
        }

        #endregion

        #region Order Methods

        private Order GetOrder(int tableId)
        {
            Table table = GetTable(tableId);

            if (table.CurrentOrder == null)
                throw new Exception($"Zamówienie nie istnieje");

            return table.CurrentOrder;
        }

        private Order GenerateBlankOrder()
        {
            Order order = new Order();
            order.Dishes.Add(new DishContainer()
            {
                DishObject = GetDish(0),
                Quantity = 1
            });

            return order;
        }

        private void RemoveOrder(int tableId)
        {
            Room room = GetRoom();
            room.Tables[tableId].CurrentOrder = null;
            UpdateRoom(room);
        }

        private void CreateOrder(int tableId)
        {
            Room room = GetRoom();
            room.Tables[tableId].CurrentOrder = new Order();
            UpdateRoom(room);
        }

        #endregion

        #region Table Methods
        private Table GetTable(int id)
        {
            Room room = GetRoom();
            if (room.Tables.Find(x => x.Id.Equals(id)) == null)
                throw new Exception($"Stolik o numerze {id} nie istnieje");

            return room.Tables[id];
        }

        private Table CreateTable(int tableId)
        {
            Table table = new Table()
            {
                Id = tableId,
                Name = $"Stolik {tableId+1}"
            };
            table.CurrentOrder = GenerateBlankOrder();

            return table;
        }

        #endregion

        #region Room Methods
        private Room GetRoom()
        {
            return (Room)Session["RoomContainer"];
        } 

        private void UpdateRoom(Room room)
        {
            Session["RoomContainer"] = room;
        }
        #endregion

        #endregion
    }
}