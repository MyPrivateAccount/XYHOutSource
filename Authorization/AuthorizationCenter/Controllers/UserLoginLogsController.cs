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

namespace AuthorizationCenter.Controllers
{
    [Produces("application/json")]
    [Route("api/UserLoginLogs")]
    public class UserLoginLogsController : Controller
    {
        private readonly UserLoginLogManager _userLoginLogManager;
        private readonly PermissionExpansionManager _permissionExpansionManager;

        public UserLoginLogsController(UserLoginLogManager userLoginLogManager, PermissionExpansionManager permissionExpansionManager)
        {
            _userLoginLogManager = userLoginLogManager;
            _permissionExpansionManager = permissionExpansionManager;
        }

        /// <summary>
        /// 通过用户id获取用户登录记录
        /// </summary>
        /// <param name="userId">忽略此参数</param>
        /// <param name="id">用户ID</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "UserLoginLogRetrieve" })]
        public async Task<ResponseMessage<List<UserLoginLog>>> GetUserLoginLog(string userId, [FromRoute] string id)
        {
            ResponseMessage<List<UserLoginLog>> response = new ResponseMessage<List<UserLoginLog>>();
            var organizationIds = await _permissionExpansionManager.GetOrganizationOfPermission(userId, "UserLoginLogRetrieve");
            if (organizationIds?.Count == 0)
            {
                response.Code = ResponseCodeDefines.NotAllow;
                response.Message = "没有权限";
                return response;
            }
            response.Extension = await _userLoginLogManager.FindByUserIdAsync(id, HttpContext.RequestAborted);
            response.Extension = response.Extension.Where(a => organizationIds.Contains(a.OrganizationId)).ToList();
            return response;
        }


        [HttpPost("list")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "UserLoginLogRetrieve" })]
        public async Task<PagingResponseMessage<UserLoginLog>> Search(string userId, [FromBody] UserLoginLogSearchCondition condition)
        {
            PagingResponseMessage<UserLoginLog> response = new PagingResponseMessage<UserLoginLog>();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                return response;
            }
            var organizationIds = await _permissionExpansionManager.GetOrganizationOfPermission(userId, "UserLoginLogRetrieve");
            if (organizationIds?.Count == 0)
            {
                response.Code = ResponseCodeDefines.NotAllow;
                response.Message = "没有权限";
                return response;
            }
            try
            {
                if (condition.OrganizationIds == null)
                {
                    condition.OrganizationIds = new List<string>();
                }
                condition.OrganizationIds.AddRange(organizationIds);
                response = await _userLoginLogManager.Search(condition, HttpContext.RequestAborted);
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