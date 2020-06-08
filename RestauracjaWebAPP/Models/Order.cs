using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RestauracjaWebAPP.Models
{
    public class Order
    {
        public int Id { get; set; }
        public List<DishContainer> Dishes { get; private set; } = new List<DishContainer>();

        public bool Payed { get; private set; }

        public Order()
        {
            Dishes.Add(new DishContainer()
            {
                DishObject = DataAccess.Instance.GetDish(0),
                Quantity = 1
            });
        }

        public void AddDish(int dishId, int quantity)
        {
            DishContainer dishFromExisting = Dishes.Find(x => x.DishObject.Id == dishId);

            if (dishFromExisting == null)
            {
                DishContainer newDishContainer = new DishContainer()
                {
                    DishObject = DataAccess.Instance.GetDish(dishId),
                    Quantity = quantity
                };
                Dishes.Add(newDishContainer);
            }
            else
                Dishes.Find(x => x.DishObject.Id == dishId).Quantity = quantity;
        }
    }
}