using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShoppingDAL;

namespace ShoppingBAL
{
    public static class OrderDataValidation
    {
        public static bool ValidOrderStatus(Orders order)
        {
            if (order.O_Status.ToUpper() == ShoppingData.OpenOrderStatus || order.O_Status.ToUpper() == ShoppingData.CloseOrderStatus)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool ValidateOpenOrder(int orderid)
        {
            using (ShoppingEntities entities = new ShoppingEntities())
            {
                if (entities.Orders.Any(o => o.O_ID == orderid && o.O_Status.ToUpper() == ShoppingData.OpenOrderStatus))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public static bool ValidateOrderExists(int orderid)
        {
            using (ShoppingEntities entities = new ShoppingEntities())
            {
                if (entities.Orders.Any(o => o.O_ID == orderid))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }

    public static class UserDataValidation
    {
        public static bool ValidateUserExists(int userid)
        {
            using (ShoppingEntities entities = new ShoppingEntities())
            {
                if (entities.Users.Any(u => u.U_ID == userid))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }
    }


    public static class CartDataValidation
    {
        public static bool ValidateEmptyCart(int orderid)
        {

            if (OrderDataValidation.ValidateOrderExists(orderid))
            {
                if (ShoppingData.OrderCart(orderid).Count() == 0)
                {
                    return true;
                }
                else
                {
                    if (ShoppingData.OrderCart(orderid).Where(c => c.C_Count != 0).Count() == 0)
                    {
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
            }
            else
            {
                return true;
            }
        }

        public static bool ValidateEmptyCartProduct(Cart cart)
        {
            if (cart.C_Count == 0)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

    }

}
