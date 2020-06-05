using Model.Object;
using Model.ViewModel;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Model.DAO
{
    public class ProductDAO
    {
        OnlineShop db = null;

        public ProductDAO()
        {
            db = new OnlineShop();
        }

        public IEnumerable<Product> ListAllPaging(string searchString, int page, int pageSize)
        {
            IQueryable<Product> list = db.Products;
            if (!string.IsNullOrEmpty(searchString))
            {
                list = list.Where(x => x.Name.Contains(searchString) || x.CreatedDate.ToString().Contains(searchString));
            }
            return list.OrderByDescending(x => x.ID).ToPagedList(page, pageSize);
        }

        public bool ChangeStatus(long ID)
        {
            Product product = db.Products.Find(ID);
            product.Status = !product.Status;
            db.SaveChanges();
            return product.Status;
        }

        public bool Delete(long ID)
        {
            try
            {
                db.Products.Remove(GetByID(ID));
                db.SaveChanges();
                return true;
            }
            catch (Exception) { return false; }
        }

        public Product GetByID(long ID)
        {
            return db.Products.Find(ID);
        }

        public bool Create(Product product)
        {
            try
            {
                db.Products.Add(product);
                db.SaveChanges();
                return true;
            }
            catch (Exception) { return false; }
        }

        public long Edit(Product product)
        {
            try
            {
                Product check = db.Products.Find(product.ID);
                check.Name = product.Name;
                check.MetaTitle = product.MetaTitle;
                check.Description = product.Description;
                check.Image = product.Image;
                check.CategoryID = product.CategoryID;
                check.Detail = product.Detail;
                check.Warranty = product.Warranty;
                check.MetaKeywords = product.MetaKeywords;
                check.MetaDescriptions = product.MetaDescriptions;
                check.Status = product.Status;
                db.SaveChanges();
                return 1;
            }
            catch (Exception) { return -1; }
        }

        // param show max bao nhiêu cái lên trang
        public List<Product> ListNewProduct(int top)
        {
            return db.Products.OrderByDescending(x => x.CreatedDate).Take(top).ToList();
        }
        public List<Product> ListFeatureProduct(int top)
        {
            return db.Products.Where(x => x.TopHot != null && x.TopHot > DateTime.Now).OrderByDescending(x => x.CreatedDate).Take(top).ToList();
        }

        public Product ViewDetail(long id)
        {
            return db.Products.Find(id);
        }

        public List<Product> ListRelatedProducts(long productId)
        {
            Product product = db.Products.Find(productId);
            return db.Products.Where(x => x.ID != productId && x.CategoryID == product.CategoryID).ToList();
        }

        public List<ProductViewModel> ListByCategoryId(long categoryID, ref int totalRecord, int pageIndex = 1, int pageSize = 0)
        {
            totalRecord = db.Products.Where(x => x.CategoryID == categoryID).Count();
            // join : FK -> PK bao nhiêu thằng sài bấy nhiêu record
            var model = (from a in db.Products
                         join b in db.ProductCategories
                         on a.CategoryID equals b.ID
                         where a.CategoryID == categoryID
                         select new // linq ko cho tạo 1 object đã biết (property tự đặt)
                         {
                             CateMetaTitle = b.MetaTitle,
                             CateName = b.Name,
                             CreatedDate = a.CreatedDate,
                             ID = a.ID,
                             Images = a.Image,
                             Name = a.Name,
                             MetaTitle = a.MetaTitle,
                             Price = a.Price
                         }).AsEnumerable().Select(x => new ProductViewModel() // gán object vừa tạo vào object đã biết
                         {
                             CateMetaTitle = x.CateMetaTitle,
                             CateName = x.CateName,
                             CreatedDate = x.CreatedDate,
                             ID = x.ID,
                             Images = x.Images,
                             Name = x.Name,
                             MetaTitle = x.MetaTitle,
                             Price = x.Price
                         });

            // (1-1)*5 = 0 -> 0 1 2 3 4
            // (2-1)*5 = 5 -> 5 6 7 8 9
            model = model.OrderBy(x => x.ID).Skip((pageIndex - 1) * pageSize).Take(pageSize);
            return model.ToList();
        }

        public List<string> ListName(string keyword)
        {
            return db.Products.Where(x => x.Name.Contains(keyword)).Select(x => x.Name).ToList();
        } // Autocomplete

        public List<ProductViewModel> Search(string keyword, ref int totalRecord, int pageIndex = 1, int pageSize = 2)
        {
            totalRecord = db.Products.Where(x => x.Name.Contains(keyword)).Count();
            var model = (from a in db.Products
                         join b in db.ProductCategories
                         on a.CategoryID equals b.ID
                         where a.Name.Contains(keyword)
                         select new
                         {
                             CateMetaTitle = b.MetaTitle,
                             CateName = b.Name,
                             CreatedDate = a.CreatedDate,
                             ID = a.ID,
                             Images = a.Image,
                             Name = a.Name,
                             MetaTitle = a.MetaTitle,
                             Price = a.Price
                         }).AsEnumerable().Select(x => new ProductViewModel()
                         {
                             CateMetaTitle = x.MetaTitle,
                             CateName = x.Name,
                             CreatedDate = x.CreatedDate,
                             ID = x.ID,
                             Images = x.Images,
                             Name = x.Name,
                             MetaTitle = x.MetaTitle,
                             Price = x.Price
                         });
            model.OrderByDescending(x => x.CreatedDate).Skip((pageIndex - 1) * pageSize).Take(pageSize);
            return model.ToList();
        }

        public void UpdateImages(long productId, string images)
        {
            Product product = db.Products.Find(productId);
            product.MoreImages = images; // ghi đè
            db.SaveChanges();
        }
    }
}
