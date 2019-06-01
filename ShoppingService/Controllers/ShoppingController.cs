using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web.Http;
using ShoppingDAL;
using ShoppingBAL;


namespace ShoppingService.Controllers
{
    //[Authorize]
    [RoutePrefix("api/shopping")]
    public class ShoppingController : ApiController
    {
        #region Get Data 
        [HttpGet, Route("products")]
        public IHttpActionResult GetProducts()
        {
            if (ShoppingData.ProductList().Any())
            {
                return Ok(ShoppingData.ProductList());
            }
            else
            {
                return NotFound();
            }

        }

        [HttpGet, Route("users")]
        public IHttpActionResult GetUsers()
        {

            if (ShoppingData.UsersList().Any())
            {
                return Ok(ShoppingData.UsersList());
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet, Route("orders")]
        public IHttpActionResult GetOrders()
        {
            if (ShoppingData.OrdersList().Any())
            {
                return Ok(ShoppingData.OrdersList());
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet, Route("carts")]
        public IHttpActionResult GetCartDetail()
        {
            if (ShoppingData.CartsList().Any())
            {
                return Ok(ShoppingData.CartsList());
            }
            else
            {
                return NotFound();
            }
        }
        #endregion

        [HttpPost, Route("createorder")]
        public IHttpActionResult CreateOrder([FromBody]Orders order)
        {
            using (ShoppingEntities entities = new ShoppingEntities())
            {
                if (order == null)
                {
                    return BadRequest("Order details missing");
                }
                else
                {
                    if (OrderDataValidation.ValidOrderStatus(order) && order.O_Status.ToUpper() == ShoppingData.OpenOrderStatus)
                    {
                        if (entities.Users.Any(u => u.U_ID == order.O_UserID))
                        {
                            if (entities.Orders.Any(o => o.O_UserID == order.O_UserID && o.O_Status.ToUpper() == order.O_Status.ToUpper()))
                            {
                                return Ok("There is already open order for user");
                            }
                            else
                            {
                                order.O_TotalAmount = 0;
                                order.O_Status = ShoppingData.OpenOrderStatus;
                                entities.Orders.Add(order);
                                entities.SaveChanges();
                                return Ok("Order Created wirh OrderID = " + entities.Orders.Where(o => o.O_UserID == order.O_UserID && o.O_Status.ToUpper() == order.O_Status.ToUpper()).First().O_ID.ToString());
                            }

                        }
                        else
                        {
                            return BadRequest("Invalid User ID");
                        }
                    }
                    else
                    {
                        return BadRequest("Invalid Order Status");
                    }
                }
            }
        }

        [HttpGet, Route("orders/{OrderID:int:min(1)}")]
        public IHttpActionResult GetOrderDetails(int OrderID)
        {
            if (ShoppingData.CartsList().Any(c => c.C_OrderID == OrderID))
            {
                return Ok(ShoppingData.CartsList().Where(c => c.C_OrderID == OrderID).ToList());
            }
            else
            {
                return NotFound();
            }
        }


        #region Add or Remove items
        [HttpPut, Route("orders/{OrderID:int:min(1)}/carts/{ProductID:int:min(1)}")]
        public IHttpActionResult AddProductToCart(int OrderID, int ProductID)
        {

            if (OrderDataValidation.ValidateOpenOrder(OrderID))
            {
                if (ShoppingData.ProductList().Any(p => p.P_ID == ProductID))
                {
                    using (ShoppingEntities entities = new ShoppingEntities())
                    {
                        if (entities.Cart.Any(c => c.C_OrderID == OrderID && c.C_ProductID == ProductID))
                        {
                            int NoOfItems = entities.Cart.Where(c => c.C_OrderID == OrderID && c.C_ProductID == ProductID).First().C_Count;
                            NoOfItems = NoOfItems + 1;

                            entities.Cart.First(c => c.C_OrderID == OrderID && c.C_ProductID == ProductID).C_Count = NoOfItems;
                            entities.SaveChanges();
                        }
                        else
                        {
                            entities.Cart.Add(new Cart { C_OrderID = OrderID, C_ProductID = ProductID, C_Count = 1 });
                            entities.SaveChanges();
                        }
                        return Ok("Item added in Cart");
                    }
                }
                else
                {
                    return BadRequest("Invalid Product ID");
                }
            }
            else
            {
                return BadRequest("Invalid OrderID");
            }

        }

        [HttpDelete, Route("orders/{OrderID:int:min(1)}/carts/{ProductID:int:min(1)}")]
        public IHttpActionResult RemoveProductFromCart(int OrderID, int ProductID)
        {

            if (OrderDataValidation.ValidateOpenOrder(OrderID))
            {
                if (ShoppingData.ProductList().Any(p => p.P_ID == ProductID))
                {
                    using (ShoppingEntities entities = new ShoppingEntities())
                    {
                        if (entities.Cart.Any(c => c.C_OrderID == OrderID && c.C_ProductID == ProductID))
                        {
                            int NoOfItems = entities.Cart.Where(c => c.C_OrderID == OrderID && c.C_ProductID == ProductID).First().C_Count;

                            if (NoOfItems > 0)
                            {
                                NoOfItems = NoOfItems - 1;

                                entities.Cart.First(c => c.C_OrderID == OrderID && c.C_ProductID == ProductID).C_Count = NoOfItems;
                                entities.SaveChanges();
                                return Ok("1 Item removed from Cart");
                            }
                            else
                            {
                                return BadRequest("0 items in Cart");
                            }
                        }
                        else
                        {
                            return BadRequest("Items not present in Cart");
                        }
                    }
                }
                else
                {
                    return BadRequest("Invalid Product ID");
                }
            }
            else
            {
                return BadRequest("Invalid OrderID");
            }

        }
        #endregion


        [HttpGet, Route("orders/{OrderID:int:min(1)}/checkout")]
        public IHttpActionResult CheckOutOrder(int OrderID)
        {
            if (OrderDataValidation.ValidateOpenOrder(OrderID))
            {
                if (CartDataValidation.ValidateEmptyCart(OrderID))
                {
                    return BadRequest("There is no item in Cart");
                }
                else
                {
                    using (ShoppingDiscount total = new ShoppingDiscount())
                    {
                        return Ok("Total Amount after Discount = " + total.TotalAfterDiscount(OrderID));
                    }
                }
            }
            else
            {
                return BadRequest("Invalid OrderID");
            }
        }

        [HttpPut, Route("orders/{OrderID:int:min(1)}/payment")]
        public IHttpActionResult CompleteOrder(int OrderID)
        {
            if (OrderDataValidation.ValidateOpenOrder(OrderID))
            {
                if (!CartDataValidation.ValidateEmptyCart(OrderID))
                {
                    using (ShoppingEntities entity = new ShoppingEntities())
                    {
                        using (ShoppingDiscount total = new ShoppingDiscount())
                        {
                            entity.Orders.First(o => o.O_ID == OrderID).O_TotalAmount = total.TotalAfterDiscount(OrderID);
                            entity.Orders.First(o => o.O_ID == OrderID).O_Status = ShoppingData.CloseOrderStatus;
                            entity.SaveChanges();
                            return Ok("Total Amount after Discount = " + total.TotalAfterDiscount(OrderID));
                        }
                    }
                }
                else
                {
                    return BadRequest("There is no item in cart");
                }
            }
            else
            {
                return BadRequest("Invalid OrderID");
            }
        }


    }
}
