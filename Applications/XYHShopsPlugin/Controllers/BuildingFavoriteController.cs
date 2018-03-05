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
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XYH.Core.Log;
using XYHShopsPlugin.Dto.Request;
using XYHShopsPlugin.Dto.Response;
using XYHShopsPlugin.Managers;
using XYHShopsPlugin.Models;

namespace XYHShopsPlugin.Controllers
{
    /// <summary>
    /// 楼盘收藏
    /// </summary>
    [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/buildingFavorite")]
    public class BuildingFavoriteController : Controller
    {
        #region 成员

        private readonly BuildingFavoriteManager _buildingFavoriteManager;
        private readonly IMapper _mapper;
        private readonly ILogger Logger = LoggerManager.GetLogger("BuildingFavorite");

        #endregion

        /// <summary>
        /// 楼盘收藏信息
        /// </summary>
        public BuildingFavoriteController(BuildingFavoriteManager buildingFavoriteManager, IMapper mapper)
        {
            _buildingFavoriteManager = buildingFavoriteManager;
            _mapper = mapper;
        }

        /// <summary>
        /// 获取我收藏的楼盘
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpPost("[action]")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "BuildingRetrieve" })]
        public async Task<PagingResponseMessage<BuildingFavoriteResponse>> GetMyBuildingFavoriteList(UserInfo user, [FromBody]PageCondition condition)
        {
            PagingResponseMessage<BuildingFavoriteResponse> pagingResponse = new PagingResponseMessage<BuildingFavoriteResponse>();
            if (!ModelState.IsValid)
            {
                pagingResponse.Code = ResponseCodeDefines.ModelStateInvalid;
                pagingResponse.Message = ModelState.GetAllErrors();
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})获取我收藏的楼盘(GetMyBuildingFavoriteList)模型验证失败：\r\n{pagingResponse.Message ?? ""},\r\n请求的参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
                return pagingResponse;
            }
            try
            {
                pagingResponse = await _buildingFavoriteManager.FindMyBuildingFavoriteAsync(user.Id, condition, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                pagingResponse.Code = ResponseCodeDefines.ServiceError;
                pagingResponse.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})获取我收藏的楼盘(GetMyBuildingFavoriteList)报错：\r\n{e.ToString()},\r\n请求的参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
            }
            return pagingResponse;
        }

        /// <summary>
        /// 根据收藏Id查询信息
        /// </summary>
        /// <param name="buildingFavoriteId"></param>
        /// <returns></returns>
        [HttpGet("{buildingFavoriteId}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "BuildingFavoriteById" })]
        public async Task<ResponseMessage<BuildingFavoriteResponse>> GetBuildingFavorite(UserInfo user, [FromRoute] string buildingFavoriteId)
        {
            var response = new ResponseMessage<BuildingFavoriteResponse>();
            if (string.IsNullOrEmpty(buildingFavoriteId))
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})根据收藏Id查询信息(GetBuildingFavorite)模型验证失败：\r\n{response.Message ?? ""},\r\n请求的参数为：\r\n(buildingFavoriteId){buildingFavoriteId ?? ""}");
                return response;
            }
            try
            {
                response.Extension = await _buildingFavoriteManager.FindByIdAsync(buildingFavoriteId, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})根据收藏Id查询信息(GetBuildingFavorite)报错：\r\n{e.ToString()},\r\n请求的参数为：\r\n(buildingFavoriteId){buildingFavoriteId ?? ""}");
            }
            return response;
        }

        /// <summary>
        /// 新增楼盘收藏信息
        /// </summary>
        /// <param name="user">用户Id</param>
        /// <param name="buildingFavoriteRequest">增加实体</param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "BuildingFavoriteCreate" })]
        public async Task<ResponseMessage<BuildingFavoriteResponse>> PostBuildingFavorite(UserInfo user, [FromBody]BuildingFavoriteRequest buildingFavoriteRequest)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})保存楼盘收藏信息(PostBuildingFavorite)：\r\n请求的参数为：\r\n" + (buildingFavoriteRequest != null ? JsonHelper.ToJson(buildingFavoriteRequest) : ""));

            var response = new ResponseMessage<BuildingFavoriteResponse>();
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
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})保存楼盘收藏信息(PostBuildingFavorite)模型验证失败：\r\n{error},\r\n请求的参数为：\r\n" + (buildingFavoriteRequest != null ? JsonHelper.ToJson(buildingFavoriteRequest) : ""));
                return response;
            }
            try
            {
                if (await _buildingFavoriteManager.FindByUserIdAndBuildingIdAsync(user.Id, buildingFavoriteRequest.BuildingId) != null)
                {
                    response.Code = ResponseCodeDefines.NotAllow;
                    response.Message = "当前已收藏该楼盘";
                    Logger.Info($"用户{user?.UserName ?? ""}({user?.Id ?? ""})保存楼盘收藏信息(PostBuildingFavorite)失败：该楼盘已经被收藏,\r\n请求的参数为：\r\n" + (buildingFavoriteRequest != null ? JsonHelper.ToJson(buildingFavoriteRequest) : ""));
                    return response;
                }
                response.Extension = await _buildingFavoriteManager.CreateAsync(user, buildingFavoriteRequest, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})保存楼盘收藏信息(PostBuildingFavorite)报错：\r\n{e.ToString()},\r\n请求的参数为：\r\n" + (buildingFavoriteRequest != null ? JsonHelper.ToJson(buildingFavoriteRequest) : ""));
            }
            return response;
        }


        /// <summary>
        /// 修改楼盘收藏信息
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="buildingFavoriteRequest">修改实体</param>
        /// <returns></returns>
        [HttpPut]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "BuildingFavoriteUpdate" })]
        public async Task<ResponseMessage> PutBuildingFavorite(UserInfo user, [FromBody]BuildingFavoriteRequest buildingFavoriteRequest)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})更新楼盘收藏信息(PutBuildingFavorite)：\r\n请求的参数为：\r\n" + (buildingFavoriteRequest != null ? JsonHelper.ToJson(buildingFavoriteRequest) : ""));

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
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})更新楼盘收藏信息(PutBuildingFavorite)模型验证失败：\r\n{error},\r\n请求的参数为：\r\n" + (buildingFavoriteRequest != null ? JsonHelper.ToJson(buildingFavoriteRequest) : ""));
                return response;
            }
            try
            {
                var dictionaryGroup = await _buildingFavoriteManager.FindByIdAsync(buildingFavoriteRequest.Id, HttpContext.RequestAborted);
                if (dictionaryGroup == null)
                {
                    await _buildingFavoriteManager.CreateAsync(user, buildingFavoriteRequest, HttpContext.RequestAborted);
                }
                else
                {
                    await _buildingFavoriteManager.UpdateAsync(user.Id, buildingFavoriteRequest, HttpContext.RequestAborted);
                }
                response.Code = ResponseCodeDefines.SuccessCode;
                response.Message = "修改成功";
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})更新楼盘收藏信息(PutBuildingFavorite)报错：\r\n{e.ToString()},\r\n请求的参数为：\r\n" + (buildingFavoriteRequest != null ? JsonHelper.ToJson(buildingFavoriteRequest) : ""));
            }
            return response;
        }

        /// <summary>
        /// 删除单个楼盘收藏
        /// </summary>
        /// <param name="User">删除用户</param>
        /// <param name="favoriteId"></param>
        /// <returns></returns>
        [HttpDelete("{buildingId}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "FavoriteDelete" })]
        public async Task<ResponseMessage> DeleteBuildingFavorite(UserInfo user, [FromRoute] string buildingId)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})删除单个楼盘收藏(DeleteBuildingFavorite)：\r\n请求的参数为：\r\n(buildingId){buildingId}");

            ResponseMessage response = new ResponseMessage();
            if (string.IsNullOrEmpty(buildingId))
            {
                response.Code = ResponseCodeDefines.ArgumentNullError;
                response.Message = "参数不能为空";
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})删除单个楼盘收藏(DeleteBuildingFavorite)模型验证失败：参数不能为空,\r\n请求的参数为：\r\n(buildingId){buildingId}");
                return response;
            }
            try
            {
                await _buildingFavoriteManager.DeleteAsync(user, buildingId, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})删除单个楼盘收藏(DeleteBuildingFavorite)报错：\r\n{e.ToString()},\r\n请求的参数为：\r\n(buildingId){buildingId}");
            }
            return response;
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="user">用户Id</param>
        /// <param name="favoriteIds">楼盘收藏idList</param>
        /// <returns></returns>
        [HttpPost("delete")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "FavoriteDeletes" })]
        public async Task<ResponseMessage> DeleteBuildingFavoriteList(UserInfo user, [FromBody] List<string> favoriteIds)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})批量删除多个楼盘收藏(DeleteBuildingFavoriteList)：\r\n请求的参数为：\r\n" + (favoriteIds != null ? JsonHelper.ToJson(favoriteIds) : ""));

            ResponseMessage response = new ResponseMessage();
            if (favoriteIds == null)
            {
                response.Code = ResponseCodeDefines.ArgumentNullError;
                response.Message = "参数出错";
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})批量删除多个楼盘收藏(DeleteBuildingFavoriteList)模型验证失败：\r\n{response.Message},\r\n请求的参数为：\r\n" + (favoriteIds != null ? JsonHelper.ToJson(favoriteIds) : ""));
                return response;
            }
            try
            {
                await _buildingFavoriteManager.DeleteListAsync(user.Id, favoriteIds, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})批量删除多个楼盘收藏(DeleteBuildingFavoriteList)报错：\r\n{e.ToString()},\r\n请求的参数为：\r\n" + (favoriteIds != null ? JsonHelper.ToJson(favoriteIds) : ""));
            }
            return response;
        }

    }
}