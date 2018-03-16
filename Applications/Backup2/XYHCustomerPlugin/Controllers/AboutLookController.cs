using ApplicationCore;
using ApplicationCore.Dto;
using ApplicationCore.Filters;
using AspNet.Security.OAuth.Validation;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XYH.Core.Log;
using XYHCustomerPlugin.Dto;
using XYHCustomerPlugin.Dto.Request;
using XYHCustomerPlugin.Dto.Response;
using XYHCustomerPlugin.Managers;

namespace XYHCustomerPlugin.Controllers
{
    [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/aboutLook")]
    public class AboutLookController : Controller
    {
        #region 成员

        private readonly AboutLookManager _aboutLookManager;
        private readonly IMapper _mapper;
        private readonly ILogger Logger = LoggerManager.GetLogger("AboutLook");

        #endregion

        /// <summary>
        /// 带看
        /// </summary>
        public AboutLookController(AboutLookManager aboutLookManager, IMapper mapper)
        {
            _aboutLookManager = aboutLookManager;
            _mapper = mapper;
        }

        /// <summary>
        /// 根据带看Id查询信息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="aboutLookId"></param>
        /// <returns></returns>
        [HttpGet("{aboutLookId}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "AboutLookById" })]
        public async Task<ResponseMessage<AboutLookResponse>> GetAboutLook(UserInfo user, [FromRoute] string aboutLookId)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})发起请求带看Id查询信息(GetAboutLook)： \r\n请求参数为：\r\n(aboutLookId){aboutLookId ?? ""}");

            var response = new ResponseMessage<AboutLookResponse>();
            if (string.IsNullOrEmpty(aboutLookId))
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = "参数不能为空";
                return response;
            }
            try
            {
                response.Extension = await _aboutLookManager.FindByIdAsync(aboutLookId, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})根据带看Id查询信息(GetAboutLook)报错：\r\n{e.ToString()}，\r\n请求参数为：\r\n(aboutLookId){aboutLookId ?? ""}");
            }
            return response;
        }

        /// <summary>
        /// 新增带看信息
        /// </summary>
        /// <param name="user">用户Id</param>
        /// <param name="aboutLookRequest">增加实体</param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "AboutLookAdd" })]
        public async Task<ResponseMessage<AboutLookResponse>> PostAboutLook(UserInfo user, [FromBody]AboutLookRequest aboutLookRequest)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})发起请求新增带看信息(PostAboutLook)：\r\n请求参数为：\r\n" + (aboutLookRequest != null ? JsonHelper.ToJson(aboutLookRequest) : ""));
            var response = new ResponseMessage<AboutLookResponse>();
            if (!ModelState.IsValid)
            {
                var error = "";
                var errors = ModelState.Values.ToList();
                foreach (var item in errors)
                {
                    foreach (var e in item.Errors)
                    {
                        error += e.ErrorMessage + "  ";
                    }
                }
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = error;
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})新增带看信息(PostAboutLook)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (aboutLookRequest != null ? JsonHelper.ToJson(aboutLookRequest) : ""));
                return response;
            }
            try
            {
                response.Extension = await _aboutLookManager.CreateAsync(user, aboutLookRequest, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})新增带看信息(PostAboutLook)请求失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (aboutLookRequest != null ? JsonHelper.ToJson(aboutLookRequest) : ""));
            }
            return response;
        }

        /// <summary>
        /// 删除带看
        /// </summary>
        /// <param name="user">删除用户</param>
        /// <param name="aboutLookId"></param>
        /// <returns></returns>
        [HttpDelete("{aboutLookId}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "AboutLookDelete" })]
        public async Task<ResponseMessage> DeleteAboutLook(UserInfo user, [FromRoute] string aboutLookId)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})发起请求删除带看(DeleteAboutLook)：\r\n请求参数为：\r\n(aboutLookId){aboutLookId ?? ""}");
            ResponseMessage response = new ResponseMessage();
            if (string.IsNullOrEmpty(aboutLookId))
            {
                response.Code = ResponseCodeDefines.ArgumentNullError;
                response.Message = "参数出错";
                return response;
            }
            try
            {
                await _aboutLookManager.DeleteAsync(user, aboutLookId, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})删除带看(DeleteAboutLook)报错：\r\n{e.ToString()}，\r\n请求参数为：\r\n(aboutLookId){aboutLookId ?? ""}");
                return response;
            }
            return response;
        }

        /// <summary>
        /// 修改客源信息
        /// </summary>
        /// <param name="user">用户Id</param>
        /// <param name="aboutLookRequest">修改实体</param>
        /// <returns></returns>
        [HttpPut]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "AboutLookUpdate" })]
        public async Task<ResponseMessage> PutAboutLook(UserInfo user, [FromBody]AboutLookRequest aboutLookRequest)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})发起请求修改客源信息(PutAboutLook)：\r\n请求参数为：\r\n" + (aboutLookRequest != null ? JsonHelper.ToJson(aboutLookRequest) : ""));
            var response = new ResponseMessage();
            if (!ModelState.IsValid)
            {
                var error = "";
                var errors = ModelState.Values.ToList();
                foreach (var item in errors)
                {
                    foreach (var e in item.Errors)
                    {
                        error += e.ErrorMessage + "  ";
                    }
                }
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = error;
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})修改客源信息(PutAboutLook)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (aboutLookRequest != null ? JsonHelper.ToJson(aboutLookRequest) : ""));
                return response;
            }
            try
            {
                var dictionaryGroup = await _aboutLookManager.FindByIdAsync(aboutLookRequest.Id, HttpContext.RequestAborted);
                if (dictionaryGroup == null)
                {
                    await _aboutLookManager.CreateAsync(user, aboutLookRequest, HttpContext.RequestAborted);
                }
                await _aboutLookManager.UpdateAsync(user.Id, aboutLookRequest, HttpContext.RequestAborted);
                response.Code = ResponseCodeDefines.SuccessCode;
                response.Message = "修改成功";
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})修改客源信息(PutAboutLook)请求失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (aboutLookRequest != null ? JsonHelper.ToJson(aboutLookRequest) : ""));
            }
            return response;
        }

        /// <summary>
        /// 根据条件获取我的带看信息
        /// </summary>
        /// <param name="user">用户Id</param>
        /// <param name="condition">客源idList</param>
        /// <returns></returns>
        [HttpPost("[action]")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "AboutLookSreach" })]
        public async Task<PagingResponseMessage<AboutLookResponse>> GetMyAboutLook(UserInfo user, [FromBody] MyAboutLookCondition condition)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})发起请求根据条件获取我的带看信息(GetMyAboutLook)：\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));

            var response = new PagingResponseMessage<AboutLookResponse>();
            if (condition == null)
            {
                var error = "";
                var errors = ModelState.Values.ToList();
                foreach (var item in errors)
                {
                    foreach (var e in item.Errors)
                    {
                        error += e.ErrorMessage + "  ";
                    }
                }
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = error;
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})根据条件获取我的带看信息(GetMyAboutLook)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
                return response;
            }
            try
            {
                response = await _aboutLookManager.SelectMyCustomer(user.Id, condition, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})根据条件获取我的带看信息(GetMyAboutLook)请求失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
            }
            return response;
        }
    }
}
