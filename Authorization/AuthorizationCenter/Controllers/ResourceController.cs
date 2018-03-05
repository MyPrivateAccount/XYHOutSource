using AspNet.Security.OAuth.Validation;
using AuthorizationCenter.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace AuthorizationCenter.Controllers
{
    [Route("api")]
    public class ResourceController : Controller
    {
        private readonly UserManager<Users> _userManager;

        public ResourceController(UserManager<Users> userManager)
        {
            _userManager = userManager;
        }

        [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
        [HttpGet("message")]
        public async Task<IActionResult> GetMessage()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return BadRequest();
            }

            return Content($"{user.UserName} has been successfully authenticated.");
        }
    }
}