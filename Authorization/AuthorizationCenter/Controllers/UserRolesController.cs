using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuthorizationCenter.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using AspNet.Security.OAuth.Validation;
using AuthorizationCenter.Filters;
using AuthorizationCenter.Dto;
using AuthorizationCenter.Managers;

namespace AuthorizationCenter.Controllers
{
    [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/UserRoles")]
    public class UserRolesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<Users> _userManager;
        private readonly RoleManager<Roles> _roleManager;
        private readonly PermissionExpansionManager _permissionExpansionManager;

        public UserRolesController(ApplicationDbContext context,
            UserManager<Users> userManager,
            RoleManager<Roles> roleManager,
            PermissionExpansionManager permissionExpansionManager
            )
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
            _permissionExpansionManager = permissionExpansionManager;
        }

        // GET: api/UserRoles/5
        [HttpGet("Users/{rolename}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "GetUserInfoByRoleName" })]
        public async Task<ResponseMessage<List<Users>>> GetUserByRole(string userId, [FromRoute] string rolename)
        {
            ResponseMessage<List<Users>> response = new ResponseMessage<List<Users>>();
            var userList = await _userManager.GetUsersInRoleAsync(rolename);
            response.Extension = userList?.ToList();
            if (response.Extension?.Count > 0)
            {
                var organizationIds = await _permissionExpansionManager.GetOrganizationOfPermission(userId, "GetUserInfoByRoleName");
                response.Extension = response.Extension.Where(a => organizationIds.Contains(a.OrganizationId)).ToList();
            }
            return response;
        }

        [HttpGet("Roles/{username}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "GetRoleInfoByUserName" })]
        public async Task<ResponseMessage<List<string>>> GetRoleByUser(string userId, [FromRoute] string username)
        {
            ResponseMessage<List<string>> response = new ResponseMessage<List<string>>();
            var users = await _userManager.FindByNameAsync(username);
            if (users == null)
            {
                response.Code = ResponseCodeDefines.NotFound;
                return response;
            }
            var organizationIds = await _permissionExpansionManager.GetOrganizationOfPermission(userId, "GetRoleInfoByUserName");
            if (!organizationIds.Contains(users.OrganizationId))
            {
                response.Code = ResponseCodeDefines.NotAllow;
                return response;
            }
            var roleList = await _userManager.GetRolesAsync(users);
            response.Extension = roleList.ToList();
            return response;
        }


        // POST: api/UserRoles
        [HttpPost("AddToRoles/{username}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "AddUserInRoles" })]
        public async Task<ResponseMessage> AddUserInRole(string userId, [FromRoute]string username, [FromBody]List<string> rolenames)
        {
            ResponseMessage response = new ResponseMessage();
            var user = await _userManager.FindByNameAsync(username);
            if (user == null || rolenames?.Count == 0)
            {
                response.Code = ResponseCodeDefines.NotFound;
                return response;
            }
            //判断用户权限
            var organizationIds = await _permissionExpansionManager.GetOrganizationOfPermission(userId, "UserInfoUpdate");
            if (!organizationIds.Contains(user.OrganizationId))
            {
                response.Code = ResponseCodeDefines.NotAllow;
                return response;
            }
            try
            {
                var exitnames = await _userManager.GetRolesAsync(user);
                rolenames = rolenames.Where(a => !exitnames.Contains(a)).ToList();
                if (rolenames?.Count == 0)
                {
                    return response;
                }
                var r = _roleManager.Roles.Where(a => rolenames.Contains(a.Name));
                if (r?.Count() == 0)
                {
                    response.Code = ResponseCodeDefines.NotFound;
                    return response;
                }
                //判断角色权限
                var urIds = await _permissionExpansionManager.GetOrganizationOfPermission(userId, "AddUserInRoles");
                r = r.Where(a => urIds.Contains(a.OrganizationId));
                var result = _userManager.AddToRolesAsync(user, r.Select(a => a.Name)).GetAwaiter().GetResult();
                //var roles = _roleManager.Roles.Where(a => rolenames.Contains(a.Name));
                await _permissionExpansionManager.AddUserInRolesAsync(r.Select(a => a.Id).ToList(), username);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
            }
            return response;
        }


        [HttpPost("RemoveFromRoles/{userName}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "RemoveUserFromRoles" })]
        public async Task<ResponseMessage> RemoveUserFormRole(string userId, [FromRoute]string userName, [FromBody]List<string> rolenames)
        {
            ResponseMessage response = new ResponseMessage();
            var user = await _userManager.FindByNameAsync(userName);
            if (user == null)
            {
                response.Code = ResponseCodeDefines.NotFound;
                return response;
            }
            var organizationIds = await _permissionExpansionManager.GetOrganizationOfPermission(userId, "UserInfoUpdate");
            if (!organizationIds.Contains(user.OrganizationId))
            {
                response.Code = ResponseCodeDefines.NotAllow;
                return response;
            }
            try
            {
                var r = _roleManager.Roles.Where(a => rolenames.Contains(a.Name));
                if (r?.Count() == 0)
                {
                    response.Code = ResponseCodeDefines.NotFound;
                    return response;
                }
                var urIds = await _permissionExpansionManager.GetOrganizationOfPermission(userId, "RemoveUserFromRoles");
                r = r.Where(a => urIds.Contains(a.OrganizationId));
                await _userManager.RemoveFromRolesAsync(user, r.Select(a => a.Name));
                await _permissionExpansionManager.RemoveUserFromRoleAsync(r.Select(a => a.Id).ToList(), user.Id);
            }
            catch (DbUpdateException)
            {
                response.Code = ResponseCodeDefines.ServiceError;
            }
            return response;
        }
    }
}