using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestauracjaWebAPP.Models
{
    public sealed class DataAccess
    {
        #region Singleton

        private static DataAccess instance = null;

        public static DataAccess Instance
        {
            get
            {
                if (instance == null)
                    instance = new DataAccess();

                return instance;
            }
        }

        #endregion

        public Room GetRoom { get; private set; } = new Room();

        public List<Dish> Dishes { get ; set; } = new List<Dish>();



        public void Initialize()
        {
            if (GetRoom.Tables.Count != 0)
                return;

            GetDishes();
            for (int a = 0; a < 5; a++)
                GetRoom.Tables.Add(new Table() { Name = $"Stolik {a + 1}"});
            AddOrder(GetRoom.Tables.First());
        }

        public double GetDishPrice(int dishId)
        {
            if (Dishes.Find(x => x.Id == dishId) == null)
                throw new Exception($"Dish with id {dishId} doesn't exist!");

            return Dishes.Find(x => x.Id == dishId).Price;
        }

        public Dish GetDish(int dishId)
        {
            if (Dishes.Find(x => x.Id == dishId) == null)
                throw new Exception($"Dish with id {dishId} doesn't exist!");

            return Dishes.Find(x => x.Id == dishId);
        }

        #region Table Methds

        public Table GetTable(int tableId)
        {
            if (GetRoom.Tables.Find(x => x.Id.Equals(tableId)) == null)
                throw new Exception($"Stolik o numerze {tableId} nie istnieje");

            return GetRoom.Tables[tableId];
        }
        
        #endregion

        #region Order Methods

        private void AddOrder(Table table)
        {
            table.Orders.Add(new Order() { Id = table.Orders.Count });
            table.CurrentOrder = table.Orders.Last().Id;

        }

        private Order GetOrder(int tableId, int ordedId)
        {
            Table table = GetTable(tableId);

            if (table.Orders[ordedId] == null)
                throw new Exception($"Zamówienei o numerze {ordedId} nie istnieje");

            return table.Orders[ordedId];
        }

        public void UpdateDish(int tableId, int ordedId, int dishId, int quantity)
        {
            Order order = GetOrder(tableId, ordedId);

            if (order.Payed)
                throw new Exception("Nie można edytować opłaconego zamówienia!");

            GetRoom.Tables[tableId].Orders[ordedId].AddDish(dishId, quantity);
        }

        #endregion

        #region Helper Methods
        /// <summary>
        /// Generate dummy data to work with
        /// </summary>
        private void GetDishes()
        {
            Dishes.AddRange(new List<Dish>() {
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
        } 
        #endregion
    }
}