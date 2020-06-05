using Model.Object;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Model.DAO
{
    public class CategoryDAO
    {
        OnlineShop db = null;

        public CategoryDAO()
        {
            db = new OnlineShop();
        }

        public IEnumerable<Category> GetList(string SearchString, int Page, int Size)
        {
            IQueryable<Category> list = db.Categories;
            if (!string.IsNullOrEmpty(SearchString)) //có keyword
            {
                list = list.Where(x => x.Name.Contains(SearchString) || x.CreatedDate.ToString().Contains(SearchString));
            }
            return list.OrderBy(x => x.ID).ToPagedList(Page, Size);
        }

        public bool ChangeStatus(long ID)
        {
            Category category = db.Categories.Find(ID);
            category.Status = !category.Status;
            db.SaveChanges();
            return category.Status;
        }

        public bool Delete(long ID)
        {
            try
            {
                Category category = db.Categories.Find(ID);
                db.Categories.Remove(category);
                db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public long Insert(Category category)
        {
            db.Categories.Add(category);
            db.SaveChanges();
            return category.ID;
        }

        public bool Update(Category category)
        {
            try
            {
                Category check = Get(category.ID);
                check.Name = category.Name;
                check.MetaTitle = category.MetaTitle;
                check.ParentID = category.ParentID;
                check.DisplayOrder = category.DisplayOrder;
                check.SeoTitle = category.SeoTitle;
                check.CreatedDate = category.CreatedDate;
                check.CreatedBy = category.CreatedBy;
                check.ModifiedDate = category.ModifiedDate;
                check.ModifiedBy = category.ModifiedBy;
                check.MetaKeywords = category.MetaKeywords;
                check.MetaDescriptions = category.MetaDescriptions;
                check.Status = category.Status;
                check.ShowOnHome = category.ShowOnHome;
                db.SaveChanges();
                return true;
            }
            catch (Exception) { return false; }
        }

        public Category Get(long ID)
        {
            return db.Categories.Find(ID);
        }

        public List<Category> ListAll()
        {
            return db.Categories.Where(x => x.Status == true).ToList();
        }

        public ProductCategory ViewDetail(long ID)
        {
            return db.ProductCategories.Find(ID);
        }
    }
}
