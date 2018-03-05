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
using System.Threading;

namespace AuthorizationCenter.Controllers
{
    [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/Roleinfo")]
    public class RoleinfoController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<Roles> _roleManager;
        private readonly RolePermissionManager _rolePermissionManager;
        private readonly ExtendUserManager<Users> _extendUserManager;
        private readonly PermissionExpansionManager _permissionExpansionManager;

        public RoleinfoController(ApplicationDbContext context,
            RoleManager<Roles> roleManager,
            RolePermissionManager rolePermissionManager,
            PermissionExpansionManager permissionExpansionManager,
            ExtendUserManager<Users> extendUserManager
            )
        {
            _context = context;
            _roleManager = roleManager;
            _extendUserManager = extendUserManager;
            _rolePermissionManager = rolePermissionManager;
            _permissionExpansionManager = permissionExpansionManager;
        }

        // GET: api/Roleinfo
        //[HttpGet]
        //public IEnumerable<Roles> GetRoles()
        //{
        //    return _context.Roles;
        //}

        // GET: api/Roleinfo/5
        [HttpGet("{id}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "RoleRetrieve" })]
        public async Task<ResponseMessage<Roles>> GetRole(string userId, [FromRoute] string id)
        {
            ResponseMessage<Roles> response = new ResponseMessage<Roles>();
            response.Extension = await _roleManager.FindByIdAsync(id);
            if (response.Extension == null)
            {
                response.Code = ResponseCodeDefines.NotFound;
                return response;
            }
            if (response.Extension.Type == RoleType.Public)
            {
                return response;
            }
            else
            {
                if (!await _permissionExpansionManager.HavePermission(userId, "RoleRetrieve", response.Extension.OrganizationId))
                {
                    response.Extension = null;
                    response.Code = ResponseCodeDefines.NotAllow;
                    return response;
                }
            }
            return response;
        }

        [HttpPost("list")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "RoleRetrieve" })]
        public async Task<PagingResponseMessage<Roles>> GetRoles(string userId, [FromBody]RoleSearchCondition condition)
        {
            PagingResponseMessage<Roles> pagingResponse = new PagingResponseMessage<Roles>();
            var organizationIds = await _permissionExpansionManager.GetOrganizationOfPermission(userId, "RoleRetrieve");
            //if (organizationIds?.Count == 0)
            //{
            //    pagingResponse.Code = ResponseCodeDefines.NotAllow;
            //    pagingResponse.Message = "没有权限";
            //    return pagingResponse;
            //}
            var roles = _roleManager.Roles.Where(a => organizationIds.Contains(a.OrganizationId) || a.Type == RoleType.Public);
            if (condition == null)
            {
                pagingResponse.Code = ResponseCodeDefines.ArgumentNullError;
                return pagingResponse;
            }
            if (!string.IsNullOrEmpty(condition.KeyWords))
            {
                roles = roles.Where(a => a.Name.Contains(condition.KeyWords));
            }
            roles.OrderBy(a => a.Name).Skip(condition.PageIndex * condition.PageSize).Take(condition.PageSize);
            pagingResponse.TotalCount = await roles.CountAsync();
            pagingResponse.PageIndex = condition.PageIndex;
            pagingResponse.PageSize = condition.PageSize;
            pagingResponse.Extension = await roles.ToListAsync();
            return pagingResponse;
        }

        // GET: api/Roleinfo/5
        [HttpGet("name/{name}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "RoleRetrieve" })]
        public async Task<ResponseMessage<Roles>> GetRolesByName(string userId, [FromRoute] string name)
        {
            ResponseMessage<Roles> response = new ResponseMessage<Roles>();
            response.Extension = await _roleManager.FindByNameAsync(name);
            if (response.Extension == null)
            {
                response.Code = ResponseCodeDefines.NotFound;
                return response;
            }
            if (response.Extension.Type == RoleType.Public)
            {
                return response;
            }
            else
            {
                if (!await _permissionExpansionManager.HavePermission(userId, "RoleRetrieve", response.Extension.OrganizationId))
                {
                    response.Extension = null;
                    response.Code = ResponseCodeDefines.NotAllow;
                    return response;
                }
            }
            return response;
        }


        // PUT: api/Roleinfo/5
        [HttpPut("{id}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "RoleUpdate" })]
        public async Task<ResponseMessage> PutRoles(string userId, [FromRoute] string id, [FromBody] Roles roles)
        {
            ResponseMessage response = new ResponseMessage();
            if (!ModelState.IsValid || id != roles.Id)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                return response;
            }
            if (roles.Type == RoleType.Public)
            {
                if (!await _permissionExpansionManager.HavePermission(userId, "PublicRoleOper", roles.OrganizationId))
                {
                    response.Code = ResponseCodeDefines.NotAllow;
                    return response;
                }
            }
            else
            {
                if (!await _permissionExpansionManager.HavePermission(userId, "RoleUpdate", roles.OrganizationId))
                {
                    response.Code = ResponseCodeDefines.NotAllow;
                    return response;
                }
            }
            try
            {
                await _roleManager.UpdateAsync(roles);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!RolesExists(id))
                {
                    response.Code = ResponseCodeDefines.NotFound;
                    response.Message = "角色不存在";
                }
                else
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                }
            }
            return response;
        }

        // POST: api/Roleinfo
        [HttpPost]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "RoleCreate,PublicRoleOper" })]
        public async Task<ResponseMessage> PostRoles(string userId, [FromBody]Roles roles)
        {
            ResponseMessage response = new ResponseMessage();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                return response;
            }
            if (roles.Type == RoleType.Public)
            {
                if (!await _permissionExpansionManager.HavePermission(userId, "PublicRoleOper", roles.OrganizationId))
                {
                    response.Code = ResponseCodeDefines.NotAllow;
                    return response;
                }
            }
            else
            {
                if (!await _permissionExpansionManager.HavePermission(userId, "RoleCreate", roles.OrganizationId))
                {
                    response.Code = ResponseCodeDefines.NotAllow;
                    return response;
                }
            }
            await _roleManager.CreateAsync(roles);
            return response;
        }

        // DELETE: api/Roleinfo/5
        [HttpDelete("{roleId}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "RoleDelete" })]
        public async Task<ResponseMessage> DeleteRole(string userId, [FromRoute] string roleId)
        {
            ResponseMessage response = new ResponseMessage();
            var roles = await _roleManager.FindByIdAsync(roleId);
            if (roles == null)
            {
                response.Code = ResponseCodeDefines.NotFound;
                response.Message = "角色不存在";
                return response;
            }
            if (roles.Type == RoleType.Public)
            {
                if (!await _permissionExpansionManager.HavePermission(userId, "PublicRoleOper", roles.OrganizationId))
                {
                    response.Code = ResponseCodeDefines.NotAllow;
                    return response;
                }
            }
            else
            {
                if (!await _permissionExpansionManager.HavePermission(userId, "RoleDelete", roles.OrganizationId))
                {
                    response.Code = ResponseCodeDefines.NotAllow;
                    return response;
                }
            }
            await _permissionExpansionManager.RemoveRoleAsync(roleId);
            await _rolePermissionManager.DeleteByRoleIdAsync(roleId, CancellationToken.None);
            await _roleManager.DeleteAsync(roles);
            return response;
        }

        [HttpPost("delete")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "RoleDelete" })]
        public async Task<ResponseMessage> DeleteRoles(string userId, [FromRoute] List<string> roleIds)
        {
            ResponseMessage response = new ResponseMessage();
            foreach (var roleid in roleIds)
            {
                var roles = await _roleManager.FindByIdAsync(roleid);
                if (roles == null)
                {
                    continue;
                }
                if (roles.Type == RoleType.Public)
                {
                    if (!await _permissionExpansionManager.HavePermission(userId, "PublicRoleOper", roles.OrganizationId))
                    {
                        response.Code = ResponseCodeDefines.NotAllow;
                        return response;
                    }
                }
                else
                {
                    if (!await _permissionExpansionManager.HavePermission(userId, "RoleDelete", roles.OrganizationId))
                    {
                        continue;
                    }
                }
                await _permissionExpansionManager.RemoveRoleAsync(roleid);
                await _rolePermissionManager.DeleteByRoleIdAsync(roleid, CancellationToken.None);
                await _roleManager.DeleteAsync(roles);
            }
            return response;
        }


        private bool RolesExists(string name)
        {
            return _roleManager.RoleExistsAsync(name).Result;
        }
    }
}