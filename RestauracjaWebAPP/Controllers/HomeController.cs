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
            return View(DataAccess.Instance.GetRoom);
        }

        [HttpGet]
        [Route("Home/UpdateTable/{id}")]
        public ActionResult UpdateTable(int id)
        {
            return PartialView("_TableView", GetTable(id));
        }

        [HttpPost]
        [Route("Table/UpdateDish")]
        public ActionResult updateQuantity()
        {
            var inputJson = new StreamReader(Request.InputStream).ReadToEnd();
            var inputContainer = new
            {
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
                UpdateDish(
                    inputContainer.tableId,
                    inputContainer.orderId,
                    inputContainer.dishId,
                    inputContainer.quantity);

            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }

            double price = GetDishPrice(inputContainer.dishId);
            return Json(new { success = true, message = Math.Round(price * inputContainer.quantity, 2) });
        }

        #region Session Controller

        private void Initialize()
        {
            Room room = GetRoom();
            if (room != null)
                return;

            room = new Room();
            room.Tables = new List<Table>();
            UpdateRoom(room);

            GenerateDishes();
            for (int a = 0; a < 5; a++)
            {
                CreateTable();
                room.Tables.Last().AddNewOrder();
            }

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

        private void AddDish(int dishId, int quantity, int tableId, int orderId)
        {
            Order order = GetOrder(tableId, orderId);
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
            room.Tables[tableId].Orders[orderId] = order;
            UpdateRoom(room);
        }

        private void UpdateDish(int tableId, int ordedId, int dishId, int quantity)
        {
            Order order = GetOrder(tableId, ordedId);

            if (order.Payed)
                throw new Exception("Nie można edytować opłaconego zamówienia!");

            AddDish(dishId, quantity, tableId, ordedId);
        }

        #endregion

        #region Order Methods

        private Order GetOrder(int tableId, int ordedId)
        {
            Table table = GetTable(tableId);

            if (table.Orders[ordedId] == null)
                throw new Exception($"Zamówienei o numerze {ordedId} nie istnieje");

            return table.Orders[ordedId];
        }

        public Order GenerateBlankOrder(int orderId, int tableId)
        {
            Order order = new Order();

            order.Dishes.Add(new DishContainer()
            {
                DishObject = GetDish(0),
                Quantity = 1
            });

            return order;
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

        private void CreateTable()
        {
            Room room = GetRoom();
            room.Tables.Add(new Table()
            {
                Id = room.Tables.Count,
                Name = $"Stolik {room.Tables.Count}"
            });
            room.Tables.Last().Orders.Add(GenerateBlankOrder(0, room.Tables.Count));

            UpdateRoom(room);
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