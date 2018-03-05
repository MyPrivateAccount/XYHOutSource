using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuthorizationCenter.Models;
using AuthorizationCenter.Managers;
using AuthorizationCenter.Filters;
using Microsoft.AspNetCore.Authorization;
using AspNet.Security.OAuth.Validation;
using AuthorizationCenter.Dto;
using System.Threading;

namespace AuthorizationCenter.Controllers
{
    [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/PermissionItems")]
    public class PermissionItemsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PermissionItemManager _permissionItemManager;
        private readonly RolePermissionManager _rolePermissionManager;
        private readonly PermissionExpansionManager _permissionExpansionManager;
        private readonly PermissionOrganizationManager _permissionOrganizationManager;

        public PermissionItemsController(ApplicationDbContext context,
            PermissionItemManager permissionItemManager,
            RolePermissionManager rolePermissionManager,
            PermissionOrganizationManager permissionOrganizationManager,
            PermissionExpansionManager permissionExpansionManager)
        {
            _context = context;
            _permissionItemManager = permissionItemManager;
            _rolePermissionManager = rolePermissionManager;
            _permissionOrganizationManager = permissionOrganizationManager;
            _permissionExpansionManager = permissionExpansionManager;
        }

        // GET: api/PermissionItems/5
        [HttpGet("{id}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "PermissionItemRetrieve" })]
        public async Task<ResponseMessage<PermissionItem>> GetPermissionItem(string userId, [FromRoute] string id)
        {
            ResponseMessage<PermissionItem> response = new ResponseMessage<PermissionItem>();
            //if (!await _permissionExpansionManager.HavePermission(userId, "PermissionItemRetrieve"))
            //{
            //    response.Code = ResponseCodeDefines.NotAllow;
            //    return response;
            //}

            response.Extension = await _permissionItemManager.FindByIdAsync(id, HttpContext.RequestAborted);
            if (response.Extension == null)
            {
                response.Code = ResponseCodeDefines.NotFound;
                return response;
            }
            return response;
        }

        [HttpGet("list/{id}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "PermissionItemRetrieve" })]
        public async Task<ResponseMessage<List<PermissionItem>>> GetPermissionItemList(string userId, [FromRoute] string id)
        {
            ResponseMessage<List<PermissionItem>> response = new ResponseMessage<List<PermissionItem>>();
            //if (!await _permissionExpansionManager.HavePermission(userId, "PermissionItemRetrieve"))
            //{
            //    response.Code = ResponseCodeDefines.NotAllow;
            //    return response;
            //}
            var permissionItem = await _permissionItemManager.FindByApplicationAsync(id, HttpContext.RequestAborted);
            response.Extension = permissionItem;
            if (response.Extension == null)
            {
                response.Code = ResponseCodeDefines.NotFound;
                return response;
            }
            return response;
        }


        // PUT: api/PermissionItems/5
        [HttpPut("{id}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "PermissionItemUpdate" })]
        public async Task<ResponseMessage> PutPermissionItem(string userId, [FromRoute] string id, [FromBody] PermissionItem permissionItem)
        {
            ResponseMessage response = new ResponseMessage();
            if (!await _permissionExpansionManager.HavePermission(userId, "PermissionItemUpdate"))
            {
                response.Code = ResponseCodeDefines.NotAllow;
                return response;
            }
            if (!ModelState.IsValid || permissionItem.Id != id)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                return response;
            }
            try
            {
                await _permissionItemManager.UpdateAsync(permissionItem, HttpContext.RequestAborted);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PermissionItemExists(id))
                {
                    response.Code = ResponseCodeDefines.NotFound;
                    response.Message = "对象不存在";
                    return response;
                }
                else
                {
                    throw;
                }
            }
            return response;
        }

        // POST: api/PermissionItems
        [HttpPost]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "PermissionItemCreate" })]
        public async Task<ResponseMessage> PostPermissionItem(string userId, [FromBody] PermissionItem permissionItem)
        {
            ResponseMessage response = new ResponseMessage();
            if (!await _permissionExpansionManager.HavePermission(userId, "PermissionItemCreate"))
            {
                response.Code = ResponseCodeDefines.NotAllow;
                return response;
            }
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                return response;
            }
            await _permissionItemManager.CreateAsync(permissionItem, HttpContext.RequestAborted);
            return response;
        }

        // DELETE: api/PermissionItems/5
        [HttpDelete("{id}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "PermissionItemDelete" })]
        public async Task<ResponseMessage> DeletePermissionItem(string userId, [FromRoute] string id)
        {
            ResponseMessage response = new ResponseMessage();
            if (!await _permissionExpansionManager.HavePermission(userId, "PermissionItemDelete"))
            {
                response.Code = ResponseCodeDefines.NotAllow;
                return response;
            }
            var permissionItem = await _permissionItemManager.FindByIdAsync(id, HttpContext.RequestAborted);
            if (permissionItem == null)
            {
                response.Code = ResponseCodeDefines.NotFound;
                response.Message = "对象不存在";
                return response;
            }
            await _permissionItemManager.DeleteAsync(permissionItem, HttpContext.RequestAborted);
            await _rolePermissionManager.DeleteByPermissionItemIdAsync(id, CancellationToken.None);
            await _permissionExpansionManager.RemovePermissionAsync(id);
            return response;
        }

        [HttpDelete("delete")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "PermissionItemDelete" })]
        public async Task<ResponseMessage> DeletePermissionItems(string userId, [FromBody] List<string> permissionItemIds)
        {
            ResponseMessage response = new ResponseMessage();
            if (!await _permissionExpansionManager.HavePermission(userId, "PermissionItemDelete"))
            {
                response.Code = ResponseCodeDefines.NotAllow;
                return response;
            }
            await _permissionItemManager.DeleteListAsync(permissionItemIds, HttpContext.RequestAborted);
            await _rolePermissionManager.DeleteByPermissionItemIdsAsync(permissionItemIds, CancellationToken.None);
            await _permissionOrganizationManager.DeleteByPermissionIdsAsync(permissionItemIds, CancellationToken.None);
            await _permissionExpansionManager.RemovePermissionsAsync(permissionItemIds);
            return response;
        }

        private bool PermissionItemExists(string id)
        {
            return _permissionItemManager.FindByIdAsync(id, HttpContext.RequestAborted).Result != null;
        }
    }
}