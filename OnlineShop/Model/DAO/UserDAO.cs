using Common;
using Model.Object;
using PagedList;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Model.DAO
{
    public class UserDAO
    {
        OnlineShop db = null;

        public UserDAO()
        {
            db = new OnlineShop();
        }

        public User Get(string UserName)
        {
            return db.Users.SingleOrDefault(n => n.UserName == UserName);
        }

        public User Get(int ID)
        {
            return db.Users.Find(ID);
        }

        public long Post(User Entity)
        {
            User check = db.Users.SingleOrDefault(n => n.UserName == Entity.UserName);
            if (check == null)
            {
                db.Users.Add(Entity);
                db.SaveChanges();
                return 1;
            }
            return 0;
        }

        public int Login(string UserName, string Password, bool isLoginAdmin = false)
        {
            User result = db.Users.SingleOrDefault(n => n.UserName == UserName && n.Password == Password);
            if (result == null) { return 0; }
            else
            {
                if (isLoginAdmin == true) // admin
                {
                    if (result.GroupID == _CommonConstants.ADMIN_GROUP || result.GroupID == _CommonConstants.MOD_GROUP)
                    {
                        if (result.Status == false) { return -1; }
                        else { return 1; }
                    }
                    else
                    {
                        return -2;
                    }
                }
                else // client
                {
                    if (result.Status == false) { return -1; }
                    else { return 1; }
                }
            }
        }

        public List<string> GetListCredential(string userName)
        {
            // GroupID:1 | RoleID:n -> 1 group có nhiều quyền
            User user = db.Users.Single(x => x.UserName == userName);
            IEnumerable<Credential> data = (from a in db.Credentials
                                            join b in db.UserGroups on a.UserGroupID equals b.ID
                                            join c in db.Roles on a.RoleID equals c.ID
                                            where b.ID == user.GroupID // (*)
                                            select new
                                            {
                                                RoleID = a.RoleID,
                                                UserGroupID = a.UserGroupID
                                            }).AsEnumerable().Select(x => new Credential()
                                            {
                                                RoleID = x.RoleID,
                                                UserGroupID = x.UserGroupID
                                            });
            return data.Select(x => x.RoleID).ToList(); // chỉ lấy role
        }

        public IEnumerable<User> GetList(int Page, int Size)
        {
            //FORCE OrderBy rồi mới ToPagedList đc
            return db.Users.OrderBy(n => n.ID).ToPagedList(Page, Size);
        }

        public bool Update(User Entity)
        {
            try
            {
                User user = db.Users.Find(Entity.ID);
                user.Name = Entity.Name;
                if (!string.IsNullOrEmpty(Entity.Password)) //muốn đổi Password
                {
                    user.Password = Entity.Password;
                }
                user.Status = Entity.Status;
                user.Address = Entity.Address;
                user.Email = Entity.Email;
                user.ModifiedBy = Entity.ModifiedBy;
                user.ModifiedDate = DateTime.Now;
                db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool Delete(int ID)
        {
            try
            {
                User user = db.Users.Find(ID);
                db.Users.Remove(user);
                db.SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public IEnumerable<User> GetList(string SearchString, int Page, int Size)
        {
            IQueryable<User> list = db.Users;
            if (!string.IsNullOrEmpty(SearchString)) //có keyword
            {
                list = list.Where(x => x.UserName.Contains(SearchString) || x.Name.Contains(SearchString));
            }
            return list.OrderByDescending(x => x.ID).ToPagedList(Page, Size);
        }

        public bool ChangeStatus(long ID)
        {
            User user = db.Users.Find(ID);
            user.Status = !user.Status;
            db.SaveChanges();
            return user.Status;
        }

        public bool CheckUserName(string userName)
        {
            return db.Users.Count(x => x.UserName == userName) > 0; // 1
        }

        public bool CheckEmail(string email)
        {
            return db.Users.Count(x => x.Email == email) > 0; // 1
        }

        public long Insert(User entity)
        {
            try
            {
                db.Users.Add(entity);
                db.SaveChanges();
                return entity.ID;
            }
            catch (Exception) { return -1; }
        }

        public long InsertForFacebook(User entity)
        {
            User user = db.Users.SingleOrDefault(x => x.UserName == entity.UserName);
            if (user == null)
            {
                db.Users.Add(entity);
                db.SaveChanges();
                return entity.ID;
            }
            else
            {
                return user.ID;
            }
        }
    }
}
