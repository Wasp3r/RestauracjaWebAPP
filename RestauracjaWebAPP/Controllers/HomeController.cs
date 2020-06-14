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

        #region Getting Information Routes

        /// <summary>
        /// Main veiw. Initiatest room.
        /// </summary>
        /// <param name="tableId">Optional parameter. Defines witch table to load in main view</param>
        /// <returns>Main view with tableId information in ViewBag</returns>
        [Route("{tableId?}")]
        public ActionResult Index(int tableId = 0)
        {
            /// Initializes room
            Initialize();
            Room room = GetRoom();
            /// Check if given table id exists in room. If it doesn't, set id to 0
            if (room.Tables.Find(x => x.Id == tableId) == null)
                tableId = 0;
            ViewBag.Table = tableId;
            return View(room);
        }

        /// <summary>
        /// Get Table
        /// </summary>
        /// <param name="id">Table ID</param>
        /// <returns>Partial view with table viwe model</returns>
        [HttpGet]
        [Route("Home/GetTable/{id}")]
        public ActionResult GetTableView(int id)
        {
            return PartialView("_TableView", GetTable(id));
        }

        /// <summary>
        /// Gets all available dishes in list
        /// </summary>
        /// <returns>List of dishes</returns>
        [HttpGet]
        [Route("GetDishes")]
        public ActionResult GetDishes()
        {
            List<Dish> dishes = GetAllDishes();
            return Json(new { success = true, message = dishes }, JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets dish price
        /// </summary>
        /// <returns>Dish price rounded to 2 decimals</returns>
        [HttpGet]
        [Route("GetDishPrice/{dishId}")]
        public ActionResult GetDishPriceRoute(int dishId)
        {
            return Json(Math.Round(GetDish(dishId).Price, 2), JsonRequestBehavior.AllowGet);
        }

        /// <summary>
        /// Gets dish quantity. DishID and TableID are given in post body
        /// </summary>
        /// <returns>Dish quantity</returns>
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

        /// <summary>
        /// Get tip from order.
        /// </summary>
        /// <param name="tableId">Table id</param>
        /// <returns>Tip for order</returns>
        [HttpGet]
        [Route("Table/GetTip/{tableId}")]
        public ActionResult GetTip(int tableId)
        {
            return Json(GetOrder(tableId).GetTip(5), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [Route("Table/GetDishPrice/{tableId}")]
        public ActionResult GetTableDishPrice(int tableId)
        {
            return Json(GetOrder(tableId).GetSum(), JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Update Routes

        /// <summary>
        /// Updates dish quantity with given dishId, tableId and quantity
        /// </summary>
        /// <returns>Json result of new sum</returns>
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

                UpdateDish(
                    inputContainer.tableId,
                    inputContainer.dishId,
                    inputContainer.quantity);

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

            /// Loads order and calculates dish sum
            double price = GetDish(inputContainer.dishId).Price;
            DishContainer dishContainer = GetTableDish(inputContainer.dishId, inputContainer.tableId);
            return Json(new { success = true, message = Math.Round(price * dishContainer.Quantity, 2) });
        }

        /// <summary>
        /// Removes order from table. TableId is given in post body
        /// </summary>
        [HttpPost]
        [Route("Table/RemoveOrder")]
        public ActionResult RemoveOrder()
        {
            var inputJson = new StreamReader(Request.InputStream).ReadToEnd();
            var inputContainer = new { tableId = -1};

            inputContainer = JsonConvert.DeserializeAnonymousType(inputJson, inputContainer);
            RemoveOrder(inputContainer.tableId);

            return Json("");
        }

        /// <summary>
        /// Renives dish from order. OrderId is given in post body
        /// </summary>
        [HttpPost]
        [Route("Table/RemoveDish")]
        public ActionResult RemoveDish()
        {
            var inputJson = new StreamReader(Request.InputStream).ReadToEnd();
            var inputContainer = new
            {
                tableId = -1,
                dishId = -1
            };

            inputContainer = JsonConvert.DeserializeAnonymousType(inputJson, inputContainer);
            RemoveDish(inputContainer.tableId, inputContainer.dishId);

            if (GetOrder(inputContainer.tableId).Dishes.Count == 0)
                RemoveOrder(inputContainer.tableId);
            return Json(new { success = true, message = "OK" });
        }

        /// <summary>
        /// Creates new order. TableId is given in post body.
        /// </summary>
        [HttpPost]
        [Route("Table/CreateOrder")]
        public ActionResult CreateNewOrder()
        {
            var inputJson = new StreamReader(Request.InputStream).ReadToEnd();
            var inputContainer = new { tableId = -1};

            inputContainer = JsonConvert.DeserializeAnonymousType(inputJson, inputContainer);
            CreateOrder(inputContainer.tableId);

            return Json(new { success = true, message = "OK" });
        }

        /// <summary>
        /// Close order. TableId is given in post body.
        /// </summary>
        [HttpPost]
        [Route("Table/CloseOrder")]
        public ActionResult CloseOrder()
        {
            var inputJson = new StreamReader(Request.InputStream).ReadToEnd();
            var inputContainer = new
            {
                tableId = -1,
            };

            inputContainer = JsonConvert.DeserializeAnonymousType(inputJson, inputContainer);
            RemoveOrder(inputContainer.tableId);

            return Json(new { success = true, message = "OK" });
        }

        #endregion


        #region Session Controller

        /// <summary>
        /// Initializes new room
        /// </summary>
        private void Initialize()
        {
            Room room = GetRoom();
            /// If room already exists, return
            if (room != null)
                return;

            room = new Room();
            room.Tables = new List<Table>();
            /// Generates available dishes
            GenerateDishes();

            /// Adding default tables
            for (int a = 0; a < 5; a++)
                room.Tables.Add(CreateTable(a));

            UpdateRoom(room);
        }

        #region Dish Methods

        /// <summary>
        /// Gets dish from dish list
        /// </summary>
        /// <param name="dishId">Dish ID</param>
        /// <returns>Dish from dish list</returns>
        private Dish GetDish(int dishId)
        {
            List<Dish> dishes = GetAllDishes();

            /// Check if dish with given ID exists
            if (dishes.Find(x => x.Id == dishId) == null)
                throw new Exception($"Danie o id {dishId} nie isntieje!");

            return dishes.Find(x => x.Id == dishId);
        }

        /// <summary>
        /// Gets list of all available dishes
        /// </summary>
        /// <returns>List of dishes</returns>
        private List<Dish> GetAllDishes()
        {
            return (List< Dish>)Session["Dishes"];
        }

        /// <summary>
        /// Gets dish from order
        /// </summary>
        /// <param name="dishId">ID of the dish</param>
        /// <param name="tableId">ID of the table</param>
        /// <returns>Dish container</returns>
        private DishContainer GetTableDish(int dishId, int tableId)
        {
            Order order = GetOrder(tableId);
            DishContainer dishContainer = order.Dishes.Find(x => x.DishObject.Id == dishId);
            /// If this dish already exists in orded, return it
            if (dishContainer != null)
                return dishContainer;
            /// When dish isn't in order, ceate blank dish, and return it
            else
                return new DishContainer() { DishObject = GetDish(dishId), Quantity = 0 };
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

        /// <summary>
        /// Add new dish to table order.
        /// </summary>
        /// <param name="dishId">ID of the dish</param>
        /// <param name="quantity">Quantity of the dish to add</param>
        /// <param name="tableId">ID of the table</param>
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

        /// <summary>
        /// Update dish in order.
        /// </summary>
        /// <param name="tableId">ID of the table</param>
        /// <param name="dishId">ID of the dish</param>
        /// <param name="quantity">Quantity of the dish to add</param>
        private void UpdateDish(int tableId, int dishId, int quantity)
        {
            Order order = GetOrder(tableId);

            if (order.Closed)
                throw new Exception("Nie można edytować opłaconego zamówienia!");

            AddDish(dishId, quantity, tableId);
        }

        /// <summary>
        /// Remove dish from table.
        /// </summary>
        /// <param name="tableId">ID of the table</param>
        /// <param name="dishId">ID of the dish</param>
        private void RemoveDish(int tableId, int dishId)
        {
            Room room = GetRoom();
            room.Tables[tableId].CurrentOrder.Dishes.Remove(GetTableDish(dishId, tableId));
            UpdateRoom(room);
        }

        #endregion

        #region Order Methods

        /// <summary>
        /// Get order from session storage
        /// </summary>
        /// <param name="tableId">ID of the table</param>
        /// <returns>Order object</returns>
        private Order GetOrder(int tableId)
        {
            Table table = GetTable(tableId);

            if (table.CurrentOrder == null)
                throw new Exception($"Zamówienie nie istnieje");

            return table.CurrentOrder;
        }

        /// <summary>
        /// Remove order from storage memory.
        /// </summary>
        /// <param name="tableId">ID of the table</param>
        private void RemoveOrder(int tableId)
        {
            Room room = GetRoom();
            room.Tables[tableId].CurrentOrder = new Order() { Closed = true };
            UpdateRoom(room);
        }

        /// <summary>
        /// Create new order in storage memory.
        /// </summary>
        /// <param name="tableId">ID of the table</param>
        private void CreateOrder(int tableId)
        {
            Room room = GetRoom();
            room.Tables[tableId].CurrentOrder = new Order() { Closed = false };
            UpdateRoom(room);
        }

        #endregion

        #region Table Methods

        /// <summary>
        /// Get table from session storage.
        /// </summary>
        /// <param name="tableId">ID of the table</param>
        /// <returns>Table object</returns>
        private Table GetTable(int tableId)
        {
            Room room = GetRoom();

            try
            {
                if (room.Tables.Find(x => x.Id.Equals(tableId)) == null)
                    throw new Exception($"Stolik o numerze {tableId} nie istnieje");

                return room.Tables[tableId];
            } catch (Exception ex)
            {
                throw new Exception("Błąd poczas pobierania stolika! Treść błędu: " + ex.Message);
            }

            
        }

        /// <summary>
        /// Create table in session storage.
        /// </summary>
        /// <param name="tableId">ID of the table</param>
        /// <returns>Created table</returns>
        private Table CreateTable(int tableId)
        {
            Table table = new Table()
            {
                Id = tableId,
                Name = $"Stolik {tableId+1}"
            };

            return table;
        }

        #endregion

        #region Room Methods

        /// <summary>
        /// Get room from session storage
        /// </summary>
        /// <returns>Room object</returns>
        private Room GetRoom()
        {
            return (Room)Session["RoomContainer"];
        } 

        /// <summary>
        /// Update room in session storage
        /// </summary>
        /// <param name="room">Room object</param>
        private void UpdateRoom(Room room)
        {
            Session["RoomContainer"] = room;
        }
        #endregion

        #endregion
    }
}