using System.ComponentModel.DataAnnotations;

namespace OnlineShop.Areas.Admin.Models
{
    public class LoginModel
    {
        [Required(ErrorMessage = "Nhập Username")]
        [StringLength(20, ErrorMessage = "Tối đa 20 kí tự")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Nhập Password")]
        [StringLength(20, ErrorMessage = "Tối đa 20 kí tự")]
        public string Password { get; set; }

        public bool Remember { get; set; }
    }
}