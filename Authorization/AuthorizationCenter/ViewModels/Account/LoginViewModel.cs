using System.ComponentModel.DataAnnotations;

namespace AuthorizationCenter.ViewModels.Account
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "用户名不能为空!")]
        [Display(Name = "邮箱")]
        // [EmailAddress]
        public string Email { get; set; }

        [Required(ErrorMessage = "密码不能为空!")]
        [Display(Name = "密码")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Display(Name = "记住我")]
        public bool RememberMe { get; set; }
    }
}
