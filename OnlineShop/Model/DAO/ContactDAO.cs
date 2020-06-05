using Model.Object;
using System;
using System.Linq;

namespace Model.DAO
{
    public class ContactDAO
    {
        OnlineShop db = null;

        public ContactDAO()
        {
            db = new OnlineShop();
        }

        public Contact GetActiveContact()
        {
            return db.Contacts.Single(x => x.Status == true);
        }

        public int InsertFeedBack(Feedback fb)
        {
            try
            {
                db.Feedbacks.Add(fb);
                db.SaveChanges();
                return fb.ID;
            }
            catch (Exception) { return -1; }
        }
    }
}
