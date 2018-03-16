using ApplicationCore;
using ApplicationCore.Dto;
using ApplicationCore.Filters;
using AspNet.Security.OAuth.Validation;
using AutoMapper;
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
    [Route("api/dictionarygroups")]
    public class DictionaryGroupController : Controller
    {
        private readonly DictionaryGroupManager _dictionaryGroupManager;
        private readonly IMapper _mapper;

        private readonly ILogger Logger = LoggerManager.GetLogger("DictionaryGroup");
        public DictionaryGroupController(DictionaryGroupManager dictionaryGroupManager, IMapper mapper)
        {
            _dictionaryGroupManager = dictionaryGroupManager;
            _mapper = mapper;
        }

        /// <summary>
        /// 获取字典分组
        /// </summary>
        /// <param name="user"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        // GET: api/PermissionItems/5
        [HttpPost("list")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<List<DictionaryGroup>>> GetDictionaryGroupList(UserInfo user, [FromBody]DictionaryGroupSearchCondition condition)
        {
            ResponseMessage<List<DictionaryGroup>> response = new ResponseMessage<List<DictionaryGroup>>();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})获取字典分组(GetDictionaryGroupList)模型验证失败：\r\n{response.Message ?? ""}，\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
                return response;
            }
            try
            {
                response.Extension = await _dictionaryGroupManager.Search(condition, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})获取字典分组(GetDictionaryGroupList)请求失败：\r\n{response.Message ?? ""}，\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
            }
            return response;
        }

        /// <summary>
        /// 获取分组详情
        /// </summary>
        /// <param name="user"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        [HttpGet("{groupId}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<DictionaryGroup>> GetDictionaryGroup(UserInfo user, [FromRoute] string groupId)
        {
            ResponseMessage<DictionaryGroup> response = new ResponseMessage<DictionaryGroup>();
            if (string.IsNullOrEmpty(groupId))
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = "值不能为空";
                return response;
            }
            try
            {
                response.Extension = await _dictionaryGroupManager.FindByIdAsync(groupId, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})获取分组详情(GetDictionaryGroup)报错：\r\n{e.ToString()}，\r\n请求参数为：\r\n(groupId){groupId ?? ""}");
            }
            return response;
        }

        /// <summary>
        /// 新增字典分组
        /// </summary>
        /// <param name="user"></param>
        /// <param name="dictionaryGroupRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "DictionaryGroupsCreate" })]
        public async Task<ResponseMessage<DictionaryGroup>> PostDictionaryGroup(UserInfo user, [FromBody]DictionaryGroupCreateRequest dictionaryGroupRequest)
        {
            ResponseMessage<DictionaryGroup> response = new ResponseMessage<DictionaryGroup>();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})新增字典分组(PostDictionaryGroup)模型验证失败：\r\n{response.Message ?? ""}，\r\n" + (dictionaryGroupRequest != null ? JsonHelper.ToJson(dictionaryGroupRequest) : ""));
                return response;
            }
            try
            {
                response.Extension = await _dictionaryGroupManager.CreateAsync(dictionaryGroupRequest.ToDataModel(user.Id), HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})新增字典分组(PostDictionaryGroup)请求失败：\r\n{response.Message ?? ""}，\r\n" + (dictionaryGroupRequest != null ? JsonHelper.ToJson(dictionaryGroupRequest) : ""));
            }
            return response;
        }

        /// <summary>
        /// 修改字典分组
        /// </summary>
        /// <param name="user"></param>
        /// <param name="groupId"></param>
        /// <param name="dictionaryGroupUpdateRequest"></param>
        /// <returns></returns>
        // PUT: api/PermissionItems/5
        [HttpPut("{groupId}")]
        [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "DictionaryGroupsUpdate" })]
        public async Task<ResponseMessage> PutDictionaryGroup(UserInfo user, [FromRoute] string groupId, [FromBody] DictionaryGroupUpdateRequest dictionaryGroupUpdateRequest)
        {
            ResponseMessage response = new ResponseMessage();
            if (!ModelState.IsValid && string.IsNullOrEmpty(groupId))
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})修改字典分组(PutDictionaryGroup)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n(groupId){groupId ?? ""}\r\n" + (dictionaryGroupUpdateRequest != null ? JsonHelper.ToJson(dictionaryGroupUpdateRequest) : ""));
                return response;
            }
            try
            {
                var dictionaryGroup = await _dictionaryGroupManager.FindByIdAsync(groupId, HttpContext.RequestAborted);
                if (dictionaryGroup == null || dictionaryGroup.IsDeleted)
                {
                    response.Code = ResponseCodeDefines.NotFound;
                    response.Message = "对象不存在";
                    return response;
                }
                await _dictionaryGroupManager.UpdateAsync(user.Id, dictionaryGroupUpdateRequest.ToDataModel(user.Id, dictionaryGroup), HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})修改字典分组(PutDictionaryGroup)请求失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n(groupId){groupId ?? ""}\r\n" + (dictionaryGroupUpdateRequest != null ? JsonHelper.ToJson(dictionaryGroupUpdateRequest) : ""));
            }
            return response;
        }

        /// <summary>
        /// 删除分组
        /// </summary>
        /// <param name="user"></param>
        /// <param name="groupId"></param>
        /// <returns></returns>
        [HttpDelete("{groupId}")]
        [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "DictionaryGroupsDelete" })]
        public async Task<ResponseMessage> DeleteDictionaryGroup(UserInfo user, [FromRoute] string groupId)
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
                var dictionaryGroup = await _dictionaryGroupManager.FindByIdAsync(groupId, HttpContext.RequestAborted);
                if (dictionaryGroup == null || dictionaryGroup.IsDeleted)
                {
                    response.Code = ResponseCodeDefines.NotFound;
                    response.Message = "对象不存在";
                    return response;
                }
                await _dictionaryGroupManager.DeleteAsync(user.Id, dictionaryGroup, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})删除分组(DeleteDictionaryGroup)报错：\r\n{e.ToString()}，\r\n请求参数为：\r\n(groupId){groupId ?? ""}");
            }
            return response;
        }

        /// <summary>
        /// 批量删除分组
        /// </summary>
        /// <param name="user"></param>
        /// <param name="groupIds"></param>
        /// <returns></returns>
        [HttpPost("delete")]
        [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "DictionaryGroupsDelete" })]
        public async Task<ResponseMessage> DeletePermissionItems(UserInfo user, [FromBody] List<string> groupIds)
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
                await _dictionaryGroupManager.DeleteListAsync(user.Id, groupIds, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})批量删除分组(DeletePermissionItems)请求失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (groupIds != null ? JsonHelper.ToJson(groupIds) : ""));
            }
            return response;
        }
    }
}
