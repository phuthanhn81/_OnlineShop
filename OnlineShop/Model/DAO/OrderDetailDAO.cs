using Model.Object;

namespace Model.DAO
{
    public class OrderDetailDAO
    {
        OnlineShop db = null;

        public OrderDetailDAO()
        {
            db = new OnlineShop();
        }

        public bool Insert(OrderDetail detail)
        {
            try
            {
                db.OrderDetails.Add(detail);
                db.SaveChanges();
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}
