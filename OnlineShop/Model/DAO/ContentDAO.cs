using Common;
using Model.Object;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Model.DAO
{
    public class ContentDAO
    {
        OnlineShop db = null;

        public ContentDAO()
        {
            db = new OnlineShop();
        }

        public IEnumerable<Content> ListAllPaging(string searchString, int page, int pageSize)
        {
            IQueryable<Content> list = db.Contents;
            if (!string.IsNullOrEmpty(searchString))
            {
                list = list.Where(x => x.Name.Contains(searchString) || x.CreatedDate.ToString().Contains(searchString));
            }
            return list.OrderBy(x => x.ID).ToPagedList(page, pageSize);
        }
        public IEnumerable<Content> ListAllPaging(int page, int pageSize, ref int totalRecord)
        {
            totalRecord = db.Contents.ToList().Count();
            IQueryable<Content> list = db.Contents;
            return list.OrderBy(x => x.ID).ToPagedList(page, pageSize);
        }
        public IEnumerable<Content> ListAllByTag(string tagId, int page, int pageSize, ref int totalRecord)
        {
            totalRecord = db.ContentTags.Where(n => n.TagID == tagId).ToList().Count();
            IEnumerable<Content> list =
                        (from a in db.Contents
                         join b in db.ContentTags
                         on a.ID equals b.ContentID
                         where b.TagID == tagId
                         select new
                         {
                             Name = a.Name,
                             MetaTitle = a.MetaTitle,
                             Image = a.Image,
                             Description = a.Description,
                             CreatedDate = a.CreatedDate,
                             CreatedBy = a.CreatedBy,
                             ID = a.ID

                         }).AsEnumerable().Select(x => new Content()
                         {
                             Name = x.Name,
                             MetaTitle = x.MetaTitle,
                             Image = x.Image,
                             Description = x.Description,
                             CreatedDate = x.CreatedDate,
                             CreatedBy = x.CreatedBy,
                             ID = x.ID
                         });
            return list.OrderBy(x => x.ID).ToPagedList(page, pageSize);
        }

        public bool ChangeStatus(long ID)
        {
            Content content = db.Contents.Find(ID);
            content.Status = !content.Status;
            db.SaveChanges();
            return content.Status;
        }

        public Content GetByID(long ID)
        {
            return db.Contents.Find(ID);
        }

        public bool Delete(long ID)
        {
            try
            {
                db.Contents.Remove(GetByID(ID));
                db.SaveChanges();
                return true;
            }
            catch (Exception) { return false; }
        }

        public long Create(Content content)
        {
            // chuyển Name -> MetaTitle (ko cho nhập MetaTitle)
            if (string.IsNullOrEmpty(content.MetaTitle))
            {
                content.MetaTitle = StringHelper.ToUnsignString(content.Name);
            }
            content.CreatedDate = DateTime.Now;
            content.ViewCount = 0;
            db.Contents.Add(content);
            db.SaveChanges();
            // Xử lý tag
            if (!string.IsNullOrEmpty(content.Tags)) // có tags
            {
                string[] tags = content.Tags.Split(',');
                foreach (string tag in tags)
                {
                    string tagId = StringHelper.ToUnsignString(tag);
                    bool existedTag = CheckTag(tagId);
                    // insert to to tag table
                    if (!existedTag)
                    {
                        InsertTag(tagId, tag);
                    }
                    // insert to content tag
                    InsertContentTag(content.ID, tagId);
                }
            }
            return content.ID;
        }

        public bool CheckTag(string id)
        {
            return db.Tags.Count(x => x.ID == id) > 0;
        }
        public void InsertTag(string id, string name)
        {
            Tag tag = new Tag();
            tag.ID = id;
            tag.Name = name;
            db.Tags.Add(tag);
            db.SaveChanges();
        }
        public void InsertContentTag(long contentId, string tagId)
        {
            ContentTag contentTag = new ContentTag();
            contentTag.ContentID = contentId;
            contentTag.TagID = tagId;
            db.ContentTags.Add(contentTag);
            db.SaveChanges();
        }

        public long Edit(Content content)
        {
            // Xử lý alias
            if (string.IsNullOrEmpty(content.MetaTitle))
            {
                content.MetaTitle = StringHelper.ToUnsignString(content.Name);
            }
            content.CreatedDate = DateTime.Now;
            Content check = GetByID(content.ID);
            check.Name = content.Name;
            check.MetaTitle = content.MetaTitle;
            check.Description = content.Description;
            check.Image = content.Image;
            check.CategoryID = content.CategoryID;
            check.Detail = content.Detail;
            check.Warranty = content.Warranty;
            check.MetaKeywords = content.MetaKeywords;
            check.MetaDescriptions = content.MetaDescriptions;
            check.Status = content.Status;
            check.Tags = content.Tags;
            db.SaveChanges();
            // Xử lý tag
            if (!string.IsNullOrEmpty(content.Tags))
            {
                RemoveAllContentTag(content.ID); // vẫn là ContentID đó but TagID(s) could khác (keep or change) -> xóa thì mới re_add PK
                string[] tags = content.Tags.Split(',');
                foreach (string tag in tags) // dù có thay đổi or không thì vẫn sẽ split
                {
                    string tagId = StringHelper.ToUnsignString(tag);
                    bool existedTag = CheckTag(tagId);
                    // insert to to tag table
                    if (!existedTag)
                    {
                        InsertTag(tagId, tag); // chưa có tagId đó thì add
                    }
                    // insert to content tag
                    InsertContentTag(content.ID, tagId); // giữ nguyên hay ko thì vẫn sẽ add lại lần nữa
                }
            }
            return content.ID;
        }

        public void RemoveAllContentTag(long contentId) // n-n
        {
            db.ContentTags.RemoveRange(db.ContentTags.Where(x => x.ContentID == contentId));
            db.SaveChanges();
        }

        public List<Tag> ListTag(long contentId)
        {
            var model = (from a in db.Tags
                         join b in db.ContentTags
                         on a.ID equals b.TagID
                         where b.ContentID == contentId
                         select new
                         {
                             ID = b.TagID,
                             Name = a.Name
                         }).AsEnumerable().Select(x => new Tag()
                         {
                             ID = x.ID,
                             Name = x.Name
                         });
            return model.ToList();
        }

        public Tag GetTag(string id)
        {
            return db.Tags.Find(id);
        }
    }
}
