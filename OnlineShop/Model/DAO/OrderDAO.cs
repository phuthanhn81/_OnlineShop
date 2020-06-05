using Model.Object;

namespace Model.DAO
{
    public class OrderDAO
    {
        OnlineShop db = null;

        public OrderDAO()
        {
            db = new OnlineShop();
        }

        public long Insert(Order order)
        {
            db.Orders.Add(order);
            db.SaveChanges();
            return order.ID;
        }
    }
}
