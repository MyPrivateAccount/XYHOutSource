using AuthorizationCenter.Dto;
using AuthorizationCenter.Managers;
using AuthorizationCenter.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace AuthorizationCenter.Filters
{
    public class CheckPermission : IAsyncActionFilter
    {
        public CheckPermission(ExtendUserManager<Users> extendUserManager, PermissionExpansionManager permissionExpansionManager, string permissionitem = "")
        {
            PermissionItem = permissionitem;
            _extendUserManager = extendUserManager;
            _permissionExpansionManager = permissionExpansionManager;
        }
        private string PermissionItem { get; }
        private ExtendUserManager<Users> _extendUserManager { get; }
        private PermissionExpansionManager _permissionExpansionManager { get; }

        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            if (context?.HttpContext?.User == null)
            {
                context.Result = new ContentResult()
                {
                    Content = "用户未登录",
                    StatusCode = 403
                };
                return;
            }
            //直接从令牌获取用户信息
            var identity = context.HttpContext.User;
            ClaimsUserInfo user = null;
            string grantType = identity.FindFirst("grant_type")?.Value;
            if (grantType == "client_credentials")
            {
                
                    user = new ClaimsUserInfo()
                    {
                        Id = identity.FindFirst("sub")?.Value,
                        OrganizationId = identity.FindFirst("org")?.Value,
                        UserName = identity.FindFirst("name")?.Value,
                        grant_type = grantType
                    };
                


            }
            else
            {
                user = new ClaimsUserInfo()
                {
                    Id = identity.FindFirst(ClaimTypes.NameIdentifier)?.Value,
                    OrganizationId = identity.FindFirst("org")?.Value,
                    UserName = identity.FindFirst("name")?.Value
                };
            }
          
            if (user == null)
            {
                context.Result = new ContentResult()
                {
                    Content = "当前用户无效",
                    StatusCode = 403,
                };
                return;
            }
            //if (!await _permissionExpansionManager.HavePermission(user.Id, PermissionItem))
            //{
            //    context.Result = new ContentResult()
            //    {
            //        Content = "没有权限进行该操作",
            //        StatusCode = 403
            //    };
            //    return;
            //}
            context.ActionArguments.Add("UserId", user.Id);
          
            if (context.ActionArguments.ContainsKey("ClaimsUserInfo"))
            {
                context.ActionArguments["ClaimsUserInfo"] = user;
            }


            await next();
            //  do something after the action executes; resultContext.Result will be set
        }


    }
}


