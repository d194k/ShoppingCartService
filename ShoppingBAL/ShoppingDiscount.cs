using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ShoppingDAL;

namespace ShoppingBAL
{
    public class ShoppingDiscount : IDisposable
    {
        public decimal TotalAfterDiscount(int orderid)
        {
            List<Cart> myOrderCart;
            List<Products> allProducts;
            List<Discount> allDiscounts;

            decimal totalAmount = 0;
            decimal itemCost = 0;

            myOrderCart = ShoppingData.OrderCart(orderid).ToList();
            allDiscounts = ShoppingData.Discounts().ToList();
            allProducts = ShoppingData.ProductList().ToList();

            foreach (var item in myOrderCart)
            {
                itemCost = 0;

                if (CartDataValidation.ValidateEmptyCartProduct(item))
                {
                    itemCost = 0;
                }
                else
                {
                    itemCost = itemCost + ItemCost(item, allProducts.First(p => p.P_ID == item.C_ProductID));

                    if (allDiscounts.Any(d => d.D_ProductID == item.C_ProductID))
                    {
                        foreach (var discount in allDiscounts.Where(d => d.D_ProductID == item.C_ProductID))
                        {
                            switch (discount.D_DiscountCode.ToUpper())
                            {
                                case "3FOR2":
                                    itemCost = itemCost - ThreeForTwoDiscount(item, allProducts.First(p => p.P_ID == item.C_ProductID), discount);
                                    continue;
                                default:
                                    itemCost = itemCost - ProductDiscount(item, allProducts.First(p => p.P_ID == item.C_ProductID), discount);
                                    continue;
                            }
                        }
                    }
                }

                totalAmount = totalAmount + itemCost;
            }

            if (allDiscounts.Any(d => d.D_ProductID == 0))
            {
                foreach (var discount in allDiscounts.Where(d => d.D_ProductID == 0))
                {
                    switch (discount.D_DiscountCode.ToUpper())
                    {
                        case "ALL10":
                            totalAmount = totalAmount - CommonDiscount(totalAmount, discount);
                            continue;
                        default:
                            totalAmount = totalAmount - CommonDiscount(totalAmount, discount);
                            continue;
                    }
                }
            }

            return totalAmount;
        }

        private decimal ItemCost(Cart cart, Products product)
        {
            decimal itemCost = 0;

            itemCost = cart.C_Count * product.P_Price;

            return itemCost;
        }

        private decimal ThreeForTwoDiscount(Cart cart, Products product, Discount discount)
        {
            decimal itemDiscount = 0;
            int quetient = cart.C_Count / 3;

            itemDiscount = (quetient * product.P_Price);

            return itemDiscount;
        }

        private decimal ProductDiscount(Cart cart, Products product, Discount discount)
        {
            decimal itemDiscount = 0;

            itemDiscount = (cart.C_Count * product.P_Price) * discount.D_Multiplier;

            return itemDiscount;
        }

        private decimal CommonDiscount(decimal total, Discount discount)
        {
            decimal itemDiscount = 0;

            itemDiscount = total * discount.D_Multiplier;

            return itemDiscount;
        }

        #region IDisposable Support
        private bool disposedValue = false; // To detect redundant calls

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                // TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
                // TODO: set large fields to null.

                disposedValue = true;
            }
        }

        // TODO: override a finalizer only if Dispose(bool disposing) above has code to free unmanaged resources.
        ~ShoppingDiscount()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(false);
        }

        // This code added to correctly implement the disposable pattern.
        public void Dispose()
        {
            // Do not change this code. Put cleanup code in Dispose(bool disposing) above.
            Dispose(true);
            // TODO: uncomment the following line if the finalizer is overridden above.
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
