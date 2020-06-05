using Model.Object;
using System;

namespace OnlineShop.Models
{
    [Serializable]
    public class CartItem
    {
        public Product Product { set; get; }

        public int Quantity { set; get; }
    }
}