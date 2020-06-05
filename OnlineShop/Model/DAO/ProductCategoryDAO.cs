using Model.Object;
using System.Collections.Generic;
using System.Linq;

namespace Model.DAO
{
    public class ProductCategoryDAO
    {
        OnlineShop db = null;

        public ProductCategoryDAO()
        {
            db = new OnlineShop();
        }

        public List<ProductCategory> ListAll()
        {
            return db.ProductCategories.Where(x => x.Status == true).OrderBy(x => x.DisplayOrder).ToList();
        }

        public ProductCategory ViewDetail(long ID)
        {
            return db.ProductCategories.Find(ID);
        }
    }
}
