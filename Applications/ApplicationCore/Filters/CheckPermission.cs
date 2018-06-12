using ApplicationCore.Dto;
using ApplicationCore.Managers;
using ApplicationCore.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Primitives;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using System.Linq;

namespace ApplicationCore.Filters
{
    public class CheckPermission : IAsyncActionFilter
    {
        //public CheckPermission(Microsoft.AspNetCore.Identity.UserManager<Users> extendUserManager, PermissionExpansionManager permissionExpansionManager, string permissionitem = "")
        //{
        //    PermissionItem = permissionitem;
        //    _userManager = extendUserManager;
        //    _permissionExpansionManager = permissionExpansionManager;
        //}
        public CheckPermission(PermissionExpansionManager permissionExpansionManager, UserManager userManager, string permissionitem = "")
        {
            PermissionItem = permissionitem;

            _userManager = userManager;
            _permissionExpansionManager = permissionExpansionManager;

        }
        private string PermissionItem { get; }
        private UserManager _userManager { get; }
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


            UserInfo user = null;
            string grantType = identity.FindFirst("grant_type")?.Value;
            if (grantType == "client_credentials")
            {
                StringValues uids;
                if (context.HttpContext.Request.Headers.TryGetValue("User", out uids))
                {
                    string uid = uids.FirstOrDefault();
                    if (!String.IsNullOrEmpty(uid))
                    {
                        user = await _userManager.GetUserAsync(uid);
                    }
                }
                else
                {
                    user = new UserInfo()
                    {
                        Id = identity.FindFirst("sub")?.Value,
                        OrganizationId = identity.FindFirst("org")?.Value,
                        UserName = identity.FindFirst("name")?.Value
                    };
                }


            }
            else
            {
                user = new UserInfo()
                {
                    Id = identity.FindFirst("sub")?.Value,
                    OrganizationId = identity.FindFirst("org")?.Value,
                    UserName = identity.FindFirst("name")?.Value
                };
            }





            //var user = await _userManager.GetUserAsync(context.HttpContext.User);
            if (user == null)
            {
                context.Result = new ContentResult()
                {
                    Content = "当前用户无效",
                    StatusCode = 403,
                };
                return;
            }

            //if (!String.IsNullOrEmpty(PermissionItem))
            //{
            //    if (!await _permissionExpansionManager.HavePermission(user.Id, PermissionItem))
            //    {
            //        context.Result = new ContentResult()
            //        {
            //            Content = "没有权限进行该操作",
            //            StatusCode = 403
            //        };
            //        return;
            //    }
            //}
            context.ActionArguments.Add("UserId", user.Id);
            if (context.ActionArguments.ContainsKey("User"))
            {
                context.ActionArguments["User"] = user;
            }

            await next();
            //do something after the action executes; resultContext.Result will be set
        }


    }
}
