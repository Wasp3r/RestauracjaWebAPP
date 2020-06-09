using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

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
    }
}