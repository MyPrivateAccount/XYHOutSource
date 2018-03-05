using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using AspNet.Security.OAuth.Validation;
using AuthorizationCenter.Dto;
using AuthorizationCenter.Interface;
using XYH.Core.Log;
using AuthorizationCenter.Filters;

namespace AuthorizationCenter.Controllers
{
   
    [Produces("application/json")]
    [Route("api/user/extensions")]
    public class UserExtensionsController : Controller
    {
        private readonly IUserExtensionsManager userExtensionsManager;
        private readonly ILogger Logger = LoggerManager.GetLogger("UserExtensions");

        public UserExtensionsController(IUserExtensionsManager uem)
        {
            this.userExtensionsManager = uem;
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { })]
        public async Task<ResponseMessage> SaveUserExtensions(ClaimsUserInfo ClaimsUserInfo, [FromBody]List<UserExtensionsRequest> request)
        {
            ResponseMessage r = new ResponseMessage();
            try
            {
                if (request == null)
                {
                    r.Code = "1";
                    r.Message = "没有传入用户参数";
                }

                bool isOk = await userExtensionsManager.SaveUserExtensions(ClaimsUserInfo, request, Request.HttpContext.RequestAborted);
                if (!isOk)
                {
                    r.Code = "1";
                    r.Message = "无法保存用户参数";
                }

            }
            catch (Exception e)
            {
                r.Code = "500";
                r.Message = e.Message;
                Logger.Error("保存用户参数失败：\r\n{0}", e.ToString());
            }
            return r;
        }


        [HttpPost("delete")]
        [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { })]
        public async Task<ResponseMessage> DeleteUserExtensions(ClaimsUserInfo ClaimsUserInfo, [FromBody]List<string> parNames)
        {
            ResponseMessage r = new ResponseMessage();
            try
            {
                if (parNames == null)
                {
                    r.Code = "1";
                    r.Message = "没有传入用户参数";
                }

                bool isOk = await userExtensionsManager.DeleteUserExtensions(ClaimsUserInfo, parNames, Request.HttpContext.RequestAborted);
                if (!isOk)
                {
                    r.Code = "1";
                    r.Message = "无法删除用户参数";
                }

            }
            catch (Exception e)
            {
                r.Code = "500";
                r.Message = e.Message;
                Logger.Error("删除用户参数失败：\r\n{0}", e.ToString());
            }
            return r;
        }

        [HttpGet]
        [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { })]
        public async Task<ResponseMessage<UserExtensionsResponse>> GetUserExtensions(ClaimsUserInfo ClaimsUserInfo, string parName)
        {
            ResponseMessage<UserExtensionsResponse> r = new ResponseMessage<UserExtensionsResponse>();
            try
            {
                if (String.IsNullOrEmpty(parName))
                {
                    r.Code = "1";
                    r.Message = "没有传入用户参数";
                }

                var response = await userExtensionsManager.GetUserExtensions(ClaimsUserInfo, new List<string>() { parName }, HttpContext.RequestAborted);
                r.Extension = response.FirstOrDefault();


            }
            catch (Exception e)
            {
                r.Code = "500";
                r.Message = e.Message;
                Logger.Error("获取用户参数失败：\r\n{0}", e.ToString());
            }
            return r;
        }

        [HttpPost("list")]
        [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { })]
        public async Task<ResponseMessage<List<UserExtensionsResponse>>> GetUserExtensionsList(ClaimsUserInfo ClaimsUserInfo, [FromBody]List<string> parNames)
        {
            ResponseMessage<List<UserExtensionsResponse>> r = new ResponseMessage<List<UserExtensionsResponse>>();
            try
            {
                

                var response = await userExtensionsManager.GetUserExtensions(ClaimsUserInfo, parNames, HttpContext.RequestAborted);
                r.Extension = response;


            }
            catch (Exception e)
            {
                r.Code = "500";
                r.Message = e.Message;
                Logger.Error("获取用户参数失败：\r\n{0}", e.ToString());
            }
            return r;
        }



        [HttpPost("search")]
        [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { })]
        public async Task<ResponseMessage<List<UserExtensionsResponse>>> Search(ClaimsUserInfo ClaimsUserInfo,[FromQuery]string parName, [FromBody]List<string> userIds)
        {
            ResponseMessage<List<UserExtensionsResponse>> r = new ResponseMessage<List<UserExtensionsResponse>>();
            try
            {
                if(ClaimsUserInfo.grant_type!= "client_credentials")
                {
                    r.Code = "401";
                    r.Message = "仅允许内部应用访问";
                    return r;
                }

                var response = await userExtensionsManager.GetUserExtensions(ClaimsUserInfo, parName, userIds, HttpContext.RequestAborted);
                r.Extension = response;


            }
            catch (Exception e)
            {
                r.Code = "500";
                r.Message = e.Message;
                Logger.Error("获取用户参数失败：\r\n{0}", e.ToString());
            }
            return r;
        }


        [HttpPost("list/{userId}")]
        public async Task<ResponseMessage<List<UserExtensionsResponse>>> GetUserExtensionsList2([FromRoute]string userId, [FromBody]List<string> parNames)
        {
            ResponseMessage<List<UserExtensionsResponse>> r = new ResponseMessage<List<UserExtensionsResponse>>();
            try
            {
                ClaimsUserInfo ui = new ClaimsUserInfo() { Id = userId };

                var response = await userExtensionsManager.GetUserExtensions(ui, parNames, HttpContext.RequestAborted);
                r.Extension = response;


            }
            catch (Exception e)
            {
                r.Code = "500";
                r.Message = e.Message;
                Logger.Error("获取用户参数失败：\r\n{0}", e.ToString());
            }
            return r;
        }


    }
}