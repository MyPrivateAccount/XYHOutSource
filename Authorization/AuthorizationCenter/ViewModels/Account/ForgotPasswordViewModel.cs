using System.ComponentModel.DataAnnotations;

namespace AuthorizationCenter.ViewModels.Account
{
    public class ForgotPasswordViewModel
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }
    }
}
