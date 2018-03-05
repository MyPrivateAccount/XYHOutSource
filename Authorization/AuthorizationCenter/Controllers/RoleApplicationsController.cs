using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuthorizationCenter.Models;
using AuthorizationCenter.Managers;
using AuthorizationCenter.Dto;
using AuthorizationCenter.Filters;
using Microsoft.AspNetCore.Authorization;
using AspNet.Security.OAuth.Validation;

namespace AuthorizationCenter.Controllers
{
    [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/RoleApplications")]
    public class RoleApplicationsController : Controller
    {
        private readonly RoleApplicationManager _roleApplicationManager;
        private readonly PermissionExpansionManager _permissionExpansionManager;


        public RoleApplicationsController(RoleApplicationManager roleApplicationManager,
            PermissionExpansionManager permissionExpansionManager
            )
        {
            _roleApplicationManager = roleApplicationManager;
            _permissionExpansionManager = permissionExpansionManager;
        }

        // PUT: api/RoleApplications/5
        [HttpPut("{roleId}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "UpdatePermissionItemByRoleId" })]
        public async Task<ResponseMessage> PutRoleApplication(string userId, [FromRoute] string roleId, [FromBody] List<string> applicationId)
        {
            ResponseMessage response = new ResponseMessage();
            if (!await _permissionExpansionManager.HavePermission(userId, "UpdatePermissionItemByRoleId"))
            {
                response.Code = ResponseCodeDefines.NotAllow;
                return response;
            }
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                return response;
            }
            try
            {
                List<RoleApplication> list = new List<RoleApplication>();
                foreach (var item in applicationId)
                {
                    list.Add(new RoleApplication
                    {
                        RoleId = roleId,
                        ApplicationId = item
                    });
                }
                await _roleApplicationManager.UpdateListAsync(roleId, list, HttpContext.RequestAborted);
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