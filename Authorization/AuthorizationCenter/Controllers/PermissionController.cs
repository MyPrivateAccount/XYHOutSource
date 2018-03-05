using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AspNet.Security.OAuth.Validation;
using AuthorizationCenter.Managers;
using AuthorizationCenter.Filters;
using AuthorizationCenter.Dto;
using AuthorizationCenter.Models;

namespace AuthorizationCenter.Controllers
{
    [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/Permission")]
    public class PermissionController : Controller
    {
        private readonly PermissionExpansionManager _permissionExpansionManager;
        private readonly OrganizationsManager _organizationManager;

        public PermissionController(PermissionExpansionManager permissionExpansionManager, OrganizationsManager organizationManager)
        {
            _permissionExpansionManager = permissionExpansionManager;
            _organizationManager = organizationManager;
        }

        [HttpPost]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "GetIsHavePermission" })]
        public async Task<ResponseMessage<bool>> HavePermission(string userId, [FromBody]List<string> permissionItems)
        {
            ResponseMessage<bool> response = new ResponseMessage<bool>();
            if (permissionItems == null)
            {
                response.Code = ResponseCodeDefines.ArgumentNullError;
                return response;
            }
            response.Extension = await _permissionExpansionManager.HavePermission(userId, permissionItems);
            return response;
        }

        [HttpPost("each")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "GetIsHavePermission" })]
        public async Task<ResponseMessage<List<HavePermissionResponse>>> HaveEachPermission(string userId, [FromBody]List<string> permissionItems)
        {
            ResponseMessage<List<HavePermissionResponse>> response = new ResponseMessage<List<HavePermissionResponse>>();
            if (permissionItems == null)
            {
                response.Code = ResponseCodeDefines.ArgumentNullError;
                return response;
            }
            try
            {
                response.Extension = await _permissionExpansionManager.GetEachPermission(userId, permissionItems);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.Message;
            }
            return response;
        }



        [HttpGet("{primissionId}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "GetPermissionOrganization" })]
        public async Task<ResponseMessage<List<Organization>>> GetPermissionOrganizations(string userId, [FromRoute]string primissionId)
        {
            ResponseMessage<List<Organization>> response = new ResponseMessage<List<Organization>>();
            try
            {
                var organizaitonIds = await _permissionExpansionManager.GetOrganizationOfPermission(userId, primissionId);
                response.Extension = await _organizationManager.Search(new OrganizationSearchCondition { idList = organizaitonIds }, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.Message;
            }
            return response;
        }



    }
}