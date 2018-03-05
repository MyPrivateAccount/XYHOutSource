using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OpenIddict.Models;
using AuthorizationCenter.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using OpenIddict.Core;
using Microsoft.AspNetCore.Authorization;
using AspNet.Security.OAuth.Validation;
using AspNet.Security.OpenIdConnect.Primitives;
using Microsoft.AspNetCore.Identity;
using System.Threading;
using AuthorizationCenter.Managers;
using AuthorizationCenter.Filters;
using AuthorizationCenter.Dto;
using System.Collections.Immutable;

namespace AuthorizationCenter.Controllers
{
    [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/Application")]
    public class ApplicationController : Controller
    {
        private readonly ApplicationManager _applicationManager;
        private readonly UserManager<Users> _userManager;
        private readonly RoleManager<Roles> _roleManager;
        private readonly PermissionItemManager _permissionItemManager;
        private readonly PermissionOrganizationManager _permissionOrganizationManager;
        private readonly RolePermissionManager _rolePermissionManager;
        private readonly RoleApplicationManager _roleApplicationManager;
        private readonly PermissionExpansionManager _permissionExpansionManager;

        private ApplicationDbContext _Context = null;


        public ApplicationController(ApplicationManager applicationManager,
            ApplicationDbContext context,
            RoleManager<Roles> roleManager,
            PermissionItemManager permissionItemManager,
            PermissionOrganizationManager permissionOrganizationManager,
            RolePermissionManager rolePermissionManager,
            PermissionExpansionManager permissionExpansionManager,
            RoleApplicationManager roleApplicationManager,
            UserManager<Users> userManager)
        {
            _applicationManager = applicationManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _Context = context;
            _permissionItemManager = permissionItemManager;
            _permissionOrganizationManager = permissionOrganizationManager;
            _rolePermissionManager = rolePermissionManager;
            _permissionExpansionManager = permissionExpansionManager;
            _roleApplicationManager = roleApplicationManager;
        }

        // GET: api/Application
        [HttpPost]
        [Route("list")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "ApplicationRetrieve" })]
        public async Task<ResponseMessage<List<Applications>>> Search(string userId, [FromBody]ApplicationSearchCondition condition)
        {
            ResponseMessage<List<Applications>> response = new ResponseMessage<List<Applications>>();
            //if (!await _permissionExpansionManager.HavePermission(userId, "ApplicationRetrieve"))
            //{
            //    response.Code = ResponseCodeDefines.NotAllow;
            //    return response;
            //}
            var applicationIds = await _roleApplicationManager.FindApplicationIdsByUserIdAsync(userId, HttpContext.RequestAborted);
            if (applicationIds?.Count() == 0)
            {
                response.Code = ResponseCodeDefines.NotAllow;
                response.Extension = null;
                return response;
            }
            if (condition.Ids == null)
            {
                condition.Ids = new List<string>();
            }
            condition.Ids.AddRange(applicationIds);
            response.Extension = await _applicationManager.ListAsync(condition);
            return response;
        }

        // GET: api/Application/5
        [HttpGet("{id}", Name = "Get")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "ApplicationRetrieve" })]
        public async Task<ResponseMessage<Applications>> Get(string userId, [FromRoute]string id)
        {
            ResponseMessage<Applications> response = new ResponseMessage<Applications>();
            if (!await _permissionExpansionManager.HavePermission(userId, "ApplicationRetrieve"))
            {
                response.Code = ResponseCodeDefines.NotAllow;
                return response;
            }
            var applicationIds = await _roleApplicationManager.FindApplicationIdsByUserIdAsync(userId, HttpContext.RequestAborted);
            if (applicationIds?.Count() == 0 || !applicationIds.Contains(id))
            {
                response.Code = ResponseCodeDefines.NotAllow;
                response.Extension = null;
                return response;
            }
            response.Extension = await _applicationManager.FindByIdAsync(id, HttpContext.RequestAborted);
            if (response.Extension == null)
            {
                response.Code = ResponseCodeDefines.NotFound;
                response.Message = "对象不存在";
            }
            return response;
        }

        // POST: api/Application
        [HttpPost]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "ApplicationCreate" })]
        public async Task<ResponseMessage> Post(string userId, [FromBody]Applications application)
        {
            ResponseMessage response = new ResponseMessage();
            if (!await _permissionExpansionManager.HavePermission(userId, "ApplicationCreate"))
            {
                response.Code = ResponseCodeDefines.NotAllow;
                return response;
            }
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                return response;
            }
            var app = new Applications();
            if (application.Type == "confidential")
            {
                var secret = application.ClientSecret;
                application.ClientSecret = "";
                app = await _applicationManager.CreateAsync(application, secret, HttpContext.RequestAborted);
            }
            else
            {
                app = await _applicationManager.CreateAsync(application, HttpContext.RequestAborted);
            }
            var role = await _roleManager.FindByNameAsync("admin");
            await _roleApplicationManager.CreateAsync(new RoleApplication { ApplicationId = app.Id, RoleId = role.Id }, CancellationToken.None);
            return response;
        }

        // PUT: api/Application/5
        [HttpPut("{id}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "ApplicationUpdate" })]
        public async Task<ResponseMessage> Put(string userId, [FromRoute] string id, [FromBody]Applications application)
        {
            ResponseMessage response = new ResponseMessage();
            if (!ModelState.IsValid || id != application.Id)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                return response;
            }
            if (!await _permissionExpansionManager.HavePermission(userId, "ApplicationUpdate"))
            {
                response.Code = ResponseCodeDefines.NotAllow;
                return response;
            }
            var applicationIds = await _roleApplicationManager.FindApplicationIdsByUserIdAsync(userId, HttpContext.RequestAborted);
            if (applicationIds?.Count() == 0)
            {
                response.Code = ResponseCodeDefines.NotAllow;
                return response;
            }
            if (!applicationIds.Contains(id))
            {
                response.Code = ResponseCodeDefines.NotAllow;
                return response;
            }
            try
            {
                if (application.Type == "confidential")
                {
                    var secret = application.ClientSecret;
                    application.ClientSecret = "";
                    await _applicationManager.UpdateAsync(application, secret, HttpContext.RequestAborted);
                }
                else
                {
                    await _applicationManager.UpdateAsync(application, HttpContext.RequestAborted);
                }
                return response;
            }
            catch (DbUpdateConcurrencyException)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                return response;
            }
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "ApplicationDelete" })]
        public async Task<ResponseMessage> Delete(string userId, [FromRoute]string id)
        {
            ResponseMessage response = new ResponseMessage();
            if (!await _permissionExpansionManager.HavePermission(userId, "ApplicationDelete"))
            {
                response.Code = ResponseCodeDefines.NotAllow;
                return response;
            }
            var applicationIds = await _roleApplicationManager.FindApplicationIdsByUserIdAsync(userId, HttpContext.RequestAborted);
            if (!applicationIds.Contains(id))
            {
                response.Code = ResponseCodeDefines.NotAllow;
                return response;
            }
            var application = await _applicationManager.FindByIdAsync(id, HttpContext.RequestAborted);
            if (application == null)
            {
                response.Code = ResponseCodeDefines.NotFound;
                response.Message = "对象不存在";
                return response;
            }
            await _applicationManager.DeleteAsync(application, HttpContext.RequestAborted);
            var permissionItems = await _permissionItemManager.FindByApplicationAsync(id, CancellationToken.None);
            var permissionItemIds = permissionItems.Select(a => a.Id).ToList();
            await _permissionExpansionManager.RemovePermissionsAsync(permissionItemIds);
            await _permissionOrganizationManager.DeleteByPermissionIdsAsync(permissionItemIds, CancellationToken.None);
            await _rolePermissionManager.DeleteByPermissionItemIdsAsync(permissionItemIds, CancellationToken.None);
            await _permissionItemManager.DeleteListAsync(permissionItems, CancellationToken.None);
            return response;
        }

    }
}
