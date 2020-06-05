using Model.Object;
using System.Collections.Generic;
using System.Linq;

namespace Model.DAO
{
    public class SlideDAO
    {
        OnlineShop db = null;

        public SlideDAO()
        {
            db = new OnlineShop();
        }

        public List<Slide> ListAll()
        {
            return db.Slides.Where(x => x.Status == true).OrderBy(y => y.DisplayOrder).ToList();
        }
    }
}
