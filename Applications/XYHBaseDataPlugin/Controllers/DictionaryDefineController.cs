using ApplicationCore;
using ApplicationCore.Dto;
using ApplicationCore.Filters;
using AspNet.Security.OAuth.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XYH.Core.Log;
using XYHBaseDataPlugin.Dto;
using XYHBaseDataPlugin.Managers;
using XYHBaseDataPlugin.Models;

namespace XYHBaseDataPlugin.Controllers
{

    [Produces("application/json")]
    [Route("api/dictionarydefines")]
    public class DictionaryDefineController : Controller
    {
        private readonly DictionaryDefineManager _dictionaryDefineManager;

        private readonly ILogger Logger = LoggerManager.GetLogger("DictionaryDefine");

        public DictionaryDefineController(DictionaryDefineManager dictionaryDefineManager)
        {
            _dictionaryDefineManager = dictionaryDefineManager;
        }

        /// <summary>
        /// 获取字典数据
        /// </summary>
        /// <param name="user"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        [HttpGet("{groupId}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<List<DictionaryDefine>>> GetDictionaryDefineByGroupId(UserInfo user, [FromRoute] string groupId)
        {
            ResponseMessage<List<DictionaryDefine>> response = new ResponseMessage<List<DictionaryDefine>>();
            if (string.IsNullOrEmpty(groupId))
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = "值不能为空";
                return response;
            }
            try
            {
                response.Extension = await _dictionaryDefineManager.FindByGroupIdAsync(groupId, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})获取字典数据(GetDictionaryDefineByGroupId)报错：\r\n{e.ToString()}，\r\n请求参数为：\r\n(groupId){groupId ?? ""}");
            }
            return response;
        }

        /// <summary>
        /// 批量获取字典数据
        /// </summary>
        /// <param name="user"></param>
        /// <param name="groupIds"></param>
        /// <returns></returns>
        [HttpPost("list")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<List<DictionaryDefineListResponse>>> GetDictionaryGroupList(UserInfo user, [FromBody]List<string> groupIds)
        {
            ResponseMessage<List<DictionaryDefineListResponse>> response = new ResponseMessage<List<DictionaryDefineListResponse>>();
            if (groupIds == null)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = "值不能为空";
                return response;
            }
            try
            {
                response.Extension = await _dictionaryDefineManager.FindByGroupIdsAsync(groupIds, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})批量获取字典数据(GetDictionaryGroupList)请求失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (groupIds != null ? JsonHelper.ToJson(groupIds) : ""));
            }
            return response;
        }


        /// <summary>
        /// 批量获取删除字典数据
        /// </summary>
        /// <param name="user"></param>
        /// <param name="groupIds"></param>
        /// <returns></returns>
        [HttpPost("deletedlist")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<List<DictionaryDefineListResponse>>> GetDeletedDictionaryGroupList(UserInfo user, [FromBody]List<string> groupIds)
        {
            ResponseMessage<List<DictionaryDefineListResponse>> response = new ResponseMessage<List<DictionaryDefineListResponse>>();
            if (groupIds == null)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = "值不能为空";
                return response;
            }
            try
            {
                response.Extension = await _dictionaryDefineManager.FindDeletedByGroupIdsAsync(groupIds, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})批量获取已删除字典数据(GetDeletedDictionaryGroupList)请求失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (groupIds != null ? JsonHelper.ToJson(groupIds) : ""));
            }
            return response;
        }



        /// <summary>
        /// 新增字典数据
        /// </summary>
        /// <param name="user"></param>
        /// <param name="dictionaryDefineRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "DictionaryDefinesCreate" })]
        public async Task<ResponseMessage<DictionaryDefine>> PostDictionaryGroup(UserInfo user, [FromBody]DictionaryDefineRequest dictionaryDefineRequest)
        {
            ResponseMessage<DictionaryDefine> response = new ResponseMessage<DictionaryDefine>();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})新增字典数据(PostDictionaryGroup)模型验证失败：\r\n{response.Message ?? ""}，\r\n" + (dictionaryDefineRequest != null ? JsonHelper.ToJson(dictionaryDefineRequest) : ""));
                return response;
            }
            try
            {
                response.Extension = await _dictionaryDefineManager.CreateAsync(dictionaryDefineRequest.ToDataModel(user.Id), HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})新增字典数据(PostDictionaryGroup)请求失败：\r\n{response.Message ?? ""}，\r\n" + (dictionaryDefineRequest != null ? JsonHelper.ToJson(dictionaryDefineRequest) : ""));
            }
            return response;
        }


        /// <summary>
        /// 修改字典数据
        /// </summary>
        /// <param name="user"></param>
        /// <param name="dictionaryDefineRequest"></param>
        /// <returns></returns>
        [HttpPut("{groupId}/{value}")]
        [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "DictionaryDefinesUpdate" })]
        public async Task<ResponseMessage> PutDictionaryGroup(UserInfo user, [FromRoute]string groupId, [FromRoute]string value, [FromBody]DictionaryDefineUpdateRequest dictionaryDefineUpdateRequest)
        {
            ResponseMessage response = new ResponseMessage();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})修改字典数据(PutDictionaryGroup)模型验证失败：\r\n{response.Message ?? ""}，\r\n" + (dictionaryDefineUpdateRequest != null ? JsonHelper.ToJson(dictionaryDefineUpdateRequest) : ""));
                return response;
            }
            try
            {
                await _dictionaryDefineManager.UpdateAsync(groupId, value, dictionaryDefineUpdateRequest, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})修改字典数据(PutDictionaryGroup)请求失败：\r\n{response.Message ?? ""}，\r\n" + (dictionaryDefineUpdateRequest != null ? JsonHelper.ToJson(dictionaryDefineUpdateRequest) : ""));
            }
            return response;
        }


        /// <summary>
        /// 启用已删除字典数据
        /// </summary>
        /// <param name="user"></param>
        /// <param name="dictionaryDefineRequest"></param>
        /// <returns></returns>
        [HttpPut("reuse/{groupId}/{value}")]
        [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "DictionaryDefinesUpdate" })]
        public async Task<ResponseMessage> PutDeletedDictionaryGroup(UserInfo user, [FromRoute]string groupId, [FromRoute]string value)
        {
            ResponseMessage response = new ResponseMessage();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})启用已删除字典数据(PutDeletedDictionaryGroup)模型验证失败：\r\n{response.Message ?? ""}，参数为：(groupId){groupId},(value){value}");
                return response;
            }
            try
            {
                await _dictionaryDefineManager.ReuseAsync(groupId, value, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})启用已删除字典数据(PutDeletedDictionaryGroup)请求失败：\r\n{response.Message ?? ""}，参数为：(groupId){groupId},(value){value}");
            }
            return response;
        }



        /// <summary>
        /// 删除字典数据
        /// </summary>
        /// <param name="user"></param>
        /// <param name="dictionaryDefineDeleteRequestList"></param>
        /// <returns></returns>
        [HttpPost("delete")]
        [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "DictionaryDefinesDelete" })]
        public async Task<ResponseMessage> DeleteDictionaryGroup(UserInfo user, [FromBody]List<DictionaryDefineDeleteRequest> dictionaryDefineDeleteRequestList)
        {
            ResponseMessage response = new ResponseMessage();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})删除字典数据(DeleteDictionaryGroup)模型验证失败：\r\n{response.Message ?? ""}，\r\n" + (dictionaryDefineDeleteRequestList != null ? JsonHelper.ToJson(dictionaryDefineDeleteRequestList) : ""));
                return response;
            }
            try
            {
                await _dictionaryDefineManager.DeleteListAsync(user.Id, dictionaryDefineDeleteRequestList, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})删除字典数据(DeleteDictionaryGroup)请求失败：\r\n{response.Message ?? ""}，\r\n" + (dictionaryDefineDeleteRequestList != null ? JsonHelper.ToJson(dictionaryDefineDeleteRequestList) : ""));
            }
            return response;
        }



    }
}
