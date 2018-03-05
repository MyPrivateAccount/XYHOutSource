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
using AuthorizationCenter.Managers;
using AuthorizationCenter.Filters;
using AuthorizationCenter.Dto;
using System.Threading;

namespace AuthorizationCenter.Controllers
{
    [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/Organization")]
    public class OrganizationsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly OrganizationsManager _organizationsManager;
        private readonly OrganizationExpansionManager _organizationExpansionManager;
        private readonly PermissionOrganizationManager _permissionOrganizationManager;
        private readonly PermissionExpansionManager _permissionExpansionManager;

        public OrganizationsController(ApplicationDbContext context,
            OrganizationsManager organizationsManager,
            OrganizationExpansionManager organizationExpansionManager,
            PermissionOrganizationManager permissionOrganizationManager,
            PermissionExpansionManager permissionExpansionManager
            )
        {
            _context = context;
            _organizationsManager = organizationsManager;
            _organizationExpansionManager = organizationExpansionManager;
            _permissionOrganizationManager = permissionOrganizationManager;
            _permissionExpansionManager = permissionExpansionManager;
        }

        //GET: api/Organizations/5
        [HttpGet("{id}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "OrganizationRetrieve" })]
        public async Task<ResponseMessage<Organization>> GetOrganization(string userId, [FromRoute] string id)
        {
            ResponseMessage<Organization> response = new ResponseMessage<Organization>();
            //var oIds = await _permissionExpansionManager.GetOrganizationOfPermission(userId, "OrganizationRetrieve");
            //if (oIds == null || oIds.Count == 0 || !oIds.Contains(id))
            //{
            //    response.Code = ResponseCodeDefines.NotAllow;
            //    return response;
            //}
            response.Extension = await _organizationsManager.FindByIdAsync(id, HttpContext.RequestAborted);
            if (response.Extension == null)
            {
                response.Code = ResponseCodeDefines.NotFound;
                return response;
            }
            return response;
        }

        //GET: api/Organizations/5
        //[Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
        [HttpGet]
        [Route("sons/{id}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "GetOrganizationsByParent" })]
        public async Task<ResponseMessage<List<Organization>>> GetOrganizationSons(string userId, [FromRoute] string id = "0")
        {
            ResponseMessage<List<Organization>> response = new ResponseMessage<List<Organization>>();
            response.Extension = await _organizationsManager.FindByParentAsync(id, HttpContext.RequestAborted);
            if (response.Extension?.Count == 0)
            {
                response.Code = ResponseCodeDefines.NotFound;
                return response;
            }
            //var organizationIds = await _permissionExpansionManager.GetOrganizationOfPermission(userId, "GetOrganizationsByParent");
            //response.Extension = response.Extension.Where(a => organizationIds.Contains(a.Id)).ToList();
            return response;
        }

        [HttpGet]
        [Route("sonsandmy/{id}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "GetOrganizationsByParentAndMyOrgan" })]
        public async Task<ResponseMessage<List<Organization>>> GetOrganizationSonsAndMyOrgan(string userId, [FromRoute] string id = "0")
        {
            ResponseMessage<List<Organization>> response = new ResponseMessage<List<Organization>>();
            response.Extension = await _organizationExpansionManager.FindByParentOrMyOrganAsync(id, HttpContext.RequestAborted);
            if (response.Extension?.Count == 0)
            {
                response.Code = ResponseCodeDefines.NotFound;
                return response;
            }
            //var organizationIds = await _permissionExpansionManager.GetOrganizationOfPermission(userId, "GetOrganizationsByParent");
            //response.Extension = response.Extension.Where(a => organizationIds.Contains(a.Id)).ToList();
            return response;
        }

        [HttpGet]
        [Route("allsons/{id}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "GetAllOrganizationsByParent" })]
        public async Task<ResponseMessage<List<Organization>>> GetOrganizationAllSons(string userId, [FromRoute] string id)
        {
            ResponseMessage<List<Organization>> response = new ResponseMessage<List<Organization>>();
            response.Extension = await _organizationExpansionManager.FindByIdAsync(id, HttpContext.RequestAborted);
            if (response.Extension?.Count == 0)
            {
                response.Code = ResponseCodeDefines.NotFound;
                return response;
            }
            //var organizationIds = await _permissionExpansionManager.GetOrganizationOfPermission(userId, "GetAllOrganizationsByParent");
            //response.Extension = response.Extension.Where(a => organizationIds.Contains(a.Id)).ToList();
            return response;
        }

        // PUT: api/Organizations/5
        [HttpPut("{id}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "OrganizationUpdate" })]
        public async Task<ResponseMessage> PutOrganization(string userId, [FromRoute] string id, [FromBody] Organization organization)
        {
            ResponseMessage response = new ResponseMessage();
            if (!ModelState.IsValid || id != organization.Id)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                return response;
            }
            var oIds = await _permissionExpansionManager.GetOrganizationOfPermission(userId, "OrganizationUpdate");
            if (oIds == null || oIds.Count == 0 || !oIds.Contains(id))
            {
                response.Code = ResponseCodeDefines.NotAllow;
                return response;
            }
            try
            {
                organization.IsDeleted = false;
                await _organizationsManager.UpdateAsync(organization, HttpContext.RequestAborted);
                await _organizationExpansionManager.UpdateAsync(organization);
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!await _organizationsManager.OrganizationExists(id, HttpContext.RequestAborted))
                {
                    response.Code = ResponseCodeDefines.NotFound;
                    response.Message = "对象不存在";
                    return response;
                }
                else
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                }
            }
            return response;
        }

        // POST: api/Organizations
        [HttpPost]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "OrganizationCreate" })]
        public async Task<ResponseMessage> PostOrganization(string userId, [FromBody] Organization organization)
        {
            ResponseMessage response = new ResponseMessage();
            if (!await _permissionExpansionManager.HavePermission(userId, "OrganizationCreate"))
            {
                response.Code = ResponseCodeDefines.NotAllow;
                return response;
            }
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                return response;
            }
            organization.IsDeleted = false;
            await _organizationsManager.CreateAsync(organization, HttpContext.RequestAborted);
            await _organizationExpansionManager.CreateAsync(organization);
            return response;
        }

        // DELETE: api/Organizations/5
        [HttpDelete("{id}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "OrganizationDelete" })]
        public async Task<ResponseMessage> DeleteOrganization(string userId, [FromRoute] string id)
        {
            ResponseMessage response = new ResponseMessage();
            var oIds = await _permissionExpansionManager.GetOrganizationOfPermission(userId, "OrganizationDelete");
            if (oIds == null || oIds.Count == 0 || !oIds.Contains(id))
            {
                response.Code = ResponseCodeDefines.NotAllow;
                return response;
            }
            var organization = await _organizationsManager.FindByIdAsync(id, HttpContext.RequestAborted);
            if (organization == null)
            {
                response.Code = ResponseCodeDefines.NotFound;
                response.Message = "对象不存在";
                return response;
            }

            var organizationExpansions = await _organizationExpansionManager.FindByIdAsync(id, HttpContext.RequestAborted);
            organizationExpansions.Add(organization);
            //await _organizationsManager.DeleteAsync(organization, HttpContext.RequestAborted);
            await _organizationsManager.DeleteListAsync(organizationExpansions, HttpContext.RequestAborted);
            await _permissionOrganizationManager.DeleteByOrganizationIdsAsync(organizationExpansions.Select(a => a.Id).ToList(), CancellationToken.None);
            //软删除
            organization.IsDeleted = true;
            organization.OrganizationName = organization.OrganizationName + "(已删除)";
            await _organizationsManager.UpdateAsync(organization, HttpContext.RequestAborted);
            //await _organizationExpansionManager.DeleteAsync(organization);
            await _permissionExpansionManager.RemoveOrganizationsAsync(organizationExpansions.Select(a => a.Id).ToList());
            return response;
        }

    }
}