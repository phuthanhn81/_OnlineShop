using Model.Object;
using System.Linq;

namespace Model.DAO
{
    public class FooterDAO
    {
        OnlineShop db = null;

        public FooterDAO()
        {
            db = new OnlineShop();
        }

        public Footer GetFooter()
        {
            return db.Footers.SingleOrDefault(x => x.Status == true);
        }
    }
}
