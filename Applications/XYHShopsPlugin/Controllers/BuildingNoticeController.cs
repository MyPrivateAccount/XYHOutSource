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
using XYHShopsPlugin.Dto;
using XYHShopsPlugin.Managers;

namespace XYHShopsPlugin.Controllers
{
    /// <summary>
    /// 楼盘公告跑马灯
    /// </summary>
    [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/buildingnotice")]
    public class BuildingNoticeController : Controller
    {
        private readonly BuildingNoticeManager _buildingNoticeManager;
        private readonly ILogger Logger = LoggerManager.GetLogger("BuildingNotice");

        public BuildingNoticeController(BuildingNoticeManager buildingNoticeManager)
        {
            _buildingNoticeManager = buildingNoticeManager ?? throw new ArgumentNullException(nameof(buildingNoticeManager));
        }

        /// <summary>
        /// 通过跑马灯Id获取跑马灯内容
        /// </summary>
        /// <param name="user"></param>
        /// <param name="noticeId"></param>
        /// <returns></returns>
        [HttpGet("{noticeId}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<BuildingNoticeResponse>> GetBuildingNotice(UserInfo user, [FromRoute] string noticeId)
        {
            ResponseMessage<BuildingNoticeResponse> response = new ResponseMessage<BuildingNoticeResponse>();
            try
            {
                response.Extension = await _buildingNoticeManager.FindByIdAsync(noticeId, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})通过跑马灯Id获取跑马灯内容(GetBuildingNotice)报错：\r\n{e.ToString()}，\r\n请求参数为：\r\n(noticeId){noticeId ?? ""}");
            }
            return response;
        }


        /// <summary>
        /// 获取跑马灯列表
        /// </summary>
        /// <param name="user"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpPost("list")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<PagingResponseMessage<BuildingNoticeListResponse>> GetBuildingNoticeList(UserInfo user, [FromBody]BuildingNoticeListCondition condition)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})接收到获取跑马灯列表(GetBuildingNoticeList)请求：\r\n，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
            PagingResponseMessage<BuildingNoticeListResponse> response = new PagingResponseMessage<BuildingNoticeListResponse>();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})获取跑马灯列表(GetBuildingNoticeList)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
                return response;
            }
            try
            {
                return await _buildingNoticeManager.GetListAsync(condition, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})获取跑马灯列表(GetBuildingBaseInfo)报错：\r\n{e.ToString()}，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));

            }
            return response;
        }

        /// <summary>
        /// 添加一条跑马灯
        /// </summary>
        /// <param name="user"></param>
        /// <param name="buildingNoticeRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<BuildingNoticeResponse>> PostBuildingNotice(UserInfo user, [FromBody]BuildingNoticeRequest buildingNoticeRequest)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})添加一条跑马灯(PostBuildingNotice)：\r\n请求参数为：\r\n" + (buildingNoticeRequest != null ? JsonHelper.ToJson(buildingNoticeRequest) : ""));

            ResponseMessage<BuildingNoticeResponse> response = new ResponseMessage<BuildingNoticeResponse>();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})添加一条跑马灯(PostBuildingNotice)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (buildingNoticeRequest != null ? JsonHelper.ToJson(buildingNoticeRequest) : ""));
                return response;
            }
            try
            {
                response.Extension = await _buildingNoticeManager.CreateAsync(user, buildingNoticeRequest, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})添加一条跑马灯(PostBuildingNotice)报错：\r\n{e.ToString() ?? ""}，\r\n请求参数为：\r\n" + (buildingNoticeRequest != null ? JsonHelper.ToJson(buildingNoticeRequest) : ""));
            }
            return response;
        }

        /// <summary>
        /// 更新一条跑马灯
        /// </summary>
        /// <param name="user"></param>
        /// <param name="noticeId"></param>
        /// <param name="buildingNoticeRequest"></param>
        /// <returns></returns>
        [HttpPut("{noticeId}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage> PutBuildingNotice(UserInfo user, [FromRoute] string noticeId, [FromBody] BuildingNoticeRequest buildingNoticeRequest)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})更新一条跑马灯(PutBuildingNotice)：\r\n请求参数为：\r\n" + (buildingNoticeRequest != null ? JsonHelper.ToJson(buildingNoticeRequest) : ""));

            ResponseMessage response = new ResponseMessage();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})更新一条跑马灯(PutBuildingNotice)报错：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (buildingNoticeRequest != null ? JsonHelper.ToJson(buildingNoticeRequest) : ""));
                return response;
            }
            try
            {
                await _buildingNoticeManager.UpdateAsync(noticeId, buildingNoticeRequest, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})更新一条跑马灯(PutBuildingNotice)报错：\r\n{e.ToString()}，\r\n请求参数为：\r\n" + (buildingNoticeRequest != null ? JsonHelper.ToJson(buildingNoticeRequest) : ""));
            }
            return response;
        }

        /// <summary>
        /// 删除一条跑马灯
        /// </summary>
        /// <param name="user"></param>
        /// <param name="noticeId"></param>
        /// <returns></returns>
        [HttpDelete("{noticeId}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage> DeleteBuildingNotice(UserInfo user, [FromRoute] string noticeId)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})删除一条跑马灯(DeleteBuildingNotice)：\r\n请求参数为：\r\n(noticeId){noticeId ?? ""}");

            ResponseMessage response = new ResponseMessage();
            try
            {
                await _buildingNoticeManager.DeleteAsync(user.Id, noticeId, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})删除一条跑马灯(DeleteBuildingNotice)报错：\r\n{e.ToString()}，\r\n请求参数为：\r\n(noticeId){noticeId ?? ""}");
            }
            return response;
        }




    }
}
