using System;

namespace OnlineShop.Common
{
    [Serializable] // muốn gửi đi phải convert thành json, xml
    public class UserLogin
    {
        public long UserID { get; set; }

        public string UserName { get; set; }

        public string GroupID { set; get; }
    }
    // lưu user login này lại
}