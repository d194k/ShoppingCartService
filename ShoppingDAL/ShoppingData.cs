using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShoppingDAL
{
    public static class ShoppingData
    {
        public const string OpenOrderStatus = "O";
        public const string CloseOrderStatus = "C";

        public static IEnumerable<Products> ProductList()
        {
            using (ShoppingEntities entities = new ShoppingEntities())
            {
                return entities.Products.ToList();
            }
        }

        public static IEnumerable<Users> UsersList()
        {
            using (ShoppingEntities entities = new ShoppingEntities())
            {
                return entities.Users.ToList();
            }
        }

        public static IEnumerable<Orders> OrdersList()
        {
            using (ShoppingEntities entities = new ShoppingEntities())
            {
                return entities.Orders.ToList();
            }
        }

        public static IEnumerable<Cart> CartsList()
        {
            using (ShoppingEntities entities = new ShoppingEntities())
            {
                return entities.Cart.ToList();
            }
        }

        public static IEnumerable<Discount> Discounts()
        {
            using (ShoppingEntities shop = new ShoppingEntities())
            {
                return shop.Discount.ToList();
            }
        }

        public static IEnumerable<Cart> OrderCart(int orderid)
        {
            return CartsList().Where(o => o.C_OrderID == orderid).ToList();
        }
    }
}
