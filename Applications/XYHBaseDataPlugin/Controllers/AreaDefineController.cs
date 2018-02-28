using ApplicationCore;
using ApplicationCore.Dto;
using ApplicationCore.Filters;
using AspNet.Security.OAuth.Validation;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XYH.Core.Log;
using XYHBaseDataPlugin.Dto;
using XYHBaseDataPlugin.Managers;

namespace XYHBaseDataPlugin.Controllers
{

    [Produces("application/json")]
    [Route("api/areadefines")]
    public class AreaDefineController : Controller
    {
        private readonly AreaDefineManager _areaDefineManager;

        private readonly ILogger Logger = LoggerManager.GetLogger("AreaDefine");

        public AreaDefineController(AreaDefineManager areaDefineManager)
        {
            _areaDefineManager = areaDefineManager;
        }

        /// <summary>
        /// 获取地区列表
        /// </summary>
        /// <param name="user"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        // GET: api/PermissionItems/5
        [HttpPost("list")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<List<AreaDefineResponse>>> GetAreaDefineList(UserInfo user, [FromBody]AreaDefineSearchCondition condition)
        {
            ResponseMessage<List<AreaDefineResponse>> response = new ResponseMessage<List<AreaDefineResponse>>();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})获取地区列表(GetAreaDefineList)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
                return response;
            }
            try
            {
                response.Extension = await _areaDefineManager.Search(user.Id, condition, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})获取地区列表(GetAreaDefineList)请求失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
            }
            return response;
        }

        /// <summary>
        /// 获取上级地区
        /// </summary>
        /// <param name="user"></param>
        /// <param name="code"></param>
        /// <returns></returns>
        [HttpGet("{code}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<List<AreaDefineResponse>>> GetAreaDefineByParentCode(UserInfo user, [FromRoute] string code)
        {
            ResponseMessage<List<AreaDefineResponse>> response = new ResponseMessage<List<AreaDefineResponse>>();
            if (string.IsNullOrEmpty(code))
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = "值不能为空";
                return response;
            }
            try
            {
                response.Extension = await _areaDefineManager.FindByParentCodeAsync(code, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})获取上级地区(GetAreaDefineByParentCode)报错：\r\n{e.ToString()}，\r\n请求参数为：\r\n(code){code ?? ""}");
            }
            return response;
        }

        /// <summary>
        /// 新增地区
        /// </summary>
        /// <param name="user"></param>
        /// <param name="userId"></param>
        /// <param name="areaDefineRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "AreaDefineCreate" })]
        public async Task<ResponseMessage<AreaDefineResponse>> PostAreaDefine(UserInfo user, [FromBody]AreaDefineRequest areaDefineRequest)
        {
            ResponseMessage<AreaDefineResponse> response = new ResponseMessage<AreaDefineResponse>();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})新增地区(PostAreaDefine)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (areaDefineRequest != null ? JsonHelper.ToJson(areaDefineRequest) : ""));
                return response;
            }
            try
            {
                response.Extension = await _areaDefineManager.CreateAsync(user.Id, areaDefineRequest, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})新增地区(PostAreaDefine)请求失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (areaDefineRequest != null ? JsonHelper.ToJson(areaDefineRequest) : ""));
            }
            return response;
        }

        /// <summary>
        /// 修改地区
        /// </summary>
        /// <param name="user"></param>
        /// <param name="code"></param>
        /// <param name="areaDefineRequest"></param>
        /// <returns></returns>
        // PUT: api/PermissionItems/5
        [HttpPut("{code}")]
        [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "AreaDefineUpdate" })]
        public async Task<ResponseMessage> PutAreaDefine(UserInfo user, [FromRoute] string code, [FromBody] AreaDefineRequest areaDefineRequest)
        {
            ResponseMessage response = new ResponseMessage();
            if (!ModelState.IsValid && string.IsNullOrEmpty(code))
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})修改地区(PutAreaDefine)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n(code){code ?? ""}\r\n" + (areaDefineRequest != null ? JsonHelper.ToJson(areaDefineRequest) : ""));
                return response;
            }
            try
            {
                await _areaDefineManager.UpdateAsync(user.Id, code, areaDefineRequest, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})修改地区(PutAreaDefine)请求失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n(code){code ?? ""}\r\n" + (areaDefineRequest != null ? JsonHelper.ToJson(areaDefineRequest) : ""));
            }
            return response;
        }

        /// <summary>
        /// 删除地区
        /// </summary>
        /// <param name="user"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        [HttpDelete("{groupId}")]
        [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "AreaDefineDelete" })]
        public async Task<ResponseMessage> DeleteAreaDefine(UserInfo user, [FromRoute] string groupId)
        {
            ResponseMessage response = new ResponseMessage();
            if (string.IsNullOrEmpty(groupId))
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = "值不能为空";
                return response;
            }
            try
            {
                await _areaDefineManager.DeleteAsync(user.Id, groupId, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})删除地区(DeleteAreaDefine)请求失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n(groupId){groupId ?? ""}");
            }
            return response;
        }

        /// <summary>
        /// 批量删除地区
        /// </summary>
        /// <param name="user"></param>
        /// <param name="groupIds"></param>
        /// <returns></returns>
        [HttpPost("delete")]
        [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "AreaDefineDelete" })]
        public async Task<ResponseMessage> DeleteAreaDefine(UserInfo user, [FromBody] List<string> groupIds)
        {
            ResponseMessage response = new ResponseMessage();
            if (groupIds == null)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = "值不能为空";
                return response;
            }
            try
            {
                await _areaDefineManager.DeleteListAsync(user.Id, groupIds, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})批量删除地区(DeleteAreaDefine)请求失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (groupIds != null ? JsonHelper.ToJson(groupIds) : ""));
            }
            return response;
        }


    }
}
