using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace AuthorizationCenter.ViewModels.Authorization
{
    public class LogoutViewModel
    {
        [BindNever]
        public string RequestId { get; set; }
    }
}
