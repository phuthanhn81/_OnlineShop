using Model.Object;
using System.Collections.Generic;
using System.Linq;

namespace Model.DAO
{
    public class MenuDAO
    {
        OnlineShop db = null;

        public MenuDAO()
        {
            db = new OnlineShop();
        }

        public List<Menu> ListByGroupId(int groupId)
        {
            return db.Menus.Where(x => x.TypeID == groupId && x.Status == true).OrderBy(x => x.DisplayOrder).ToList();
        }
    }
}
