using ApplicationCore;
using ApplicationCore.Dto;
using ApplicationCore.Filters;
using ApplicationCore.Models;
using AspNet.Security.OAuth.Validation;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using XYH.Core.Log;
using XYHShopsPlugin.Dto.Request;
using XYHShopsPlugin.Dto.Response;
using XYHShopsPlugin.Managers;
using XYHShopsPlugin.Models;

namespace XYHShopsPlugin.Controllers
{
    [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/buildingRecommend")]
    public class BuildingRecommendController : Controller
    {
        #region 成员

        private readonly BuildingRecommendManager _buildingRecommendManager;
        private readonly IMapper _mapper;
        private readonly ILogger Logger = LoggerManager.GetLogger("BuildingRecommend");

        #endregion

        /// <summary>
        /// 楼盘收藏信息
        /// </summary>
        public BuildingRecommendController(BuildingRecommendManager buildingRecommendManager, IMapper mapper)
        {
            _buildingRecommendManager = buildingRecommendManager;
            _mapper = mapper;
        }

        #region 待取消
        /// <summary>
        /// 获取大区级楼盘
        /// </summary>
        /// <param name="user"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpPost("listregion")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "BuildingRecommendSearchRegion" })]
        public async Task<PagingResponseMessage<BuildingRecommendItem>> GetBuildingRecommendListByRegion(UserInfo user, [FromBody]PageCondition condition)
        {
            PagingResponseMessage<BuildingRecommendItem> pagingResponse = new PagingResponseMessage<BuildingRecommendItem>();
            if (!ModelState.IsValid)
            {
                pagingResponse.Code = ResponseCodeDefines.ModelStateInvalid;
                return pagingResponse;
            }
            return await _buildingRecommendManager.SearchMainArea(user, condition, HttpContext.RequestAborted);
        }
        #endregion

        /// <summary>
        /// 获取大区级楼盘
        /// </summary>
        /// <param name="user"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpPost("listregiongroup")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "BuildingRecommendSearchRegion" })]
        public async Task<PagingResponseMessage<BuildingRecommendResponse>> GetBuildingRecommendListByRegion2(UserInfo user, [FromBody]PageCondition condition)
        {
            PagingResponseMessage<BuildingRecommendResponse> pagingResponse = new PagingResponseMessage<BuildingRecommendResponse>();
            if (!ModelState.IsValid)
            {
                pagingResponse.Code = ResponseCodeDefines.ModelStateInvalid;
                return pagingResponse;
            }
            return await _buildingRecommendManager.SearchMainArea2(user, condition, HttpContext.RequestAborted);
        }

        /// <summary>
        /// 获取公司级楼盘
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpPost("listfiliale")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "BuildingRecommendSearchFiliale" })]
        public async Task<PagingResponseMessage<BuildingRecommendItem>> GetBuildingRecommendListByFiliale(UserInfo user, [FromBody]PageCondition condition)
        {
            PagingResponseMessage<BuildingRecommendItem> Response = new PagingResponseMessage<BuildingRecommendItem>();
            if (!ModelState.IsValid)
            {
                Response.Code = ResponseCodeDefines.ModelStateInvalid;
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})获取公司级楼盘(GetBuildingRecommendListByFiliale)模型验证失败：\r\n请求的参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
                return Response;
            }
            try
            {
                Response = await _buildingRecommendManager.SearchFiliale(user, condition, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                Response.Code = ResponseCodeDefines.ServiceError;
                Response.Message = "服务器发生错误：" + e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})获取公司级楼盘(GetBuildingRecommendListByFiliale)报错：\r\n{e.ToString()},\r\n请求的参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
            }
            return Response;
        }

        /// <summary>
        /// 获取我推荐的楼盘
        /// </summary>
        /// <param name="user"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpPost("[action]")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "BuildingRetrieve" })]
        public async Task<PagingResponseMessage<BuildingRecommendItem>> GetMyBuildingRecommendList(UserInfo user, [FromBody]PageCondition condition)
        {
            PagingResponseMessage<BuildingRecommendItem> pagingResponse = new PagingResponseMessage<BuildingRecommendItem>();
            if (!ModelState.IsValid)
            {
                pagingResponse.Code = ResponseCodeDefines.ModelStateInvalid;
                return pagingResponse;
            }
            return await _buildingRecommendManager.FindMyBuildingRecommendAsync(user, condition, HttpContext.RequestAborted);
        }

        /// <summary>
        /// 根据收藏Id查询信息
        /// </summary>
        /// <param name="buildingRecommendId"></param>
        /// <returns></returns>
        [HttpGet("{buildingRecommendId}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "BuildingRecommendById" })]
        public async Task<ResponseMessage<BuildingRecommendResponse>> GetBuildingRecommend([FromRoute] string buildingRecommendId)
        {
            var response = new ResponseMessage<BuildingRecommendResponse>();
            if (string.IsNullOrEmpty(buildingRecommendId))
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                return response;
            }
            response.Extension = await _buildingRecommendManager.FindByIdAsync(buildingRecommendId, HttpContext.RequestAborted);
            return response;
        }

        /// <summary>
        /// 新增楼盘收藏信息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="buildingRecommendRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "BuildingRecommendCreate" })]
        public async Task<ResponseMessage<BuildingRecommendResponse>> PostBuildingRecommend(UserInfo user, [FromBody]BuildingRecommendRequest buildingRecommendRequest)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})新增楼盘收藏信息(PostBuildingRecommend)：\r\n请求的参数为：\r\n" + (buildingRecommendRequest != null ? JsonHelper.ToJson(buildingRecommendRequest) : ""));

            var response = new ResponseMessage<BuildingRecommendResponse>();
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
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})新增楼盘收藏信息(PostBuildingRecommend)模型验证失败：\r\n{error},\r\n请求的参数为：\r\n" + (buildingRecommendRequest != null ? JsonHelper.ToJson(buildingRecommendRequest) : ""));

                return response;
            }
            if (!buildingRecommendRequest.IsRegion)
            {
                var count = await _buildingRecommendManager.AllCountAsync(buildingRecommendRequest);
                if (count == 5)
                {
                    response.Code = ResponseCodeDefines.NotAllow;
                    response.Message = "当前公司推荐数量大于5";
                    return response;
                }
                else if (count == -1)
                {
                    response.Code = ResponseCodeDefines.NotAllow;
                    response.Message = "暂未找到该楼盘或者该楼盘已推荐";
                    return response;
                }
                else if (count == -2)
                {
                    response.Code = ResponseCodeDefines.NotAllow;
                    response.Message = "暂未找到该部门分公司";
                    return response;
                }
            }
            try
            {
                response.Extension = await _buildingRecommendManager.CreateAsync(user, buildingRecommendRequest, HttpContext.RequestAborted);

                if (response.Extension == null)
                {
                    response.Code = ResponseCodeDefines.NotFound;
                    response.Message = "新增失败";

                }
            }
            catch(Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.Message;
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})新增楼盘收藏信息(PostBuildingRecommend)报错：\r\n{e.ToString()},\r\n请求的参数为：\r\n" + (buildingRecommendRequest != null ? JsonHelper.ToJson(buildingRecommendRequest) : ""));

            }

            return response;
        }


        /// <summary>
        /// 修改楼盘收藏信息
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="buildingRecommendRequest">修改实体</param>
        /// <returns></returns>
        [HttpPut]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "BuildingRecommendUpdate" })]
        public async Task<ResponseMessage> PutBuildingRecommend(UserInfo user, [FromBody]BuildingRecommendRequest buildingRecommendRequest)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})修改楼盘收藏信息(PutBuildingRecommend)：\r\n请求的参数为：\r\n" + (buildingRecommendRequest != null ? JsonHelper.ToJson(buildingRecommendRequest) : ""));

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
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})修改楼盘收藏信息(PutBuildingRecommend)模型验证失败：\r\n{error},\r\n请求的参数为：\r\n" + (buildingRecommendRequest != null ? JsonHelper.ToJson(buildingRecommendRequest) : ""));

                return response;
            }
            try
            {
                var dictionaryGroup = await _buildingRecommendManager.FindByIdAsync(buildingRecommendRequest.Id, HttpContext.RequestAborted);
                if (dictionaryGroup == null)
                {
                    await _buildingRecommendManager.CreateAsync(user, buildingRecommendRequest, HttpContext.RequestAborted);
                }
                await _buildingRecommendManager.UpdateAsync(user.Id, buildingRecommendRequest, HttpContext.RequestAborted);
                response.Code = ResponseCodeDefines.SuccessCode;
                response.Message = "修改成功";
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})修改楼盘收藏信息(PutBuildingRecommend)报错：\r\n{e.ToString()},\r\n请求的参数为：\r\n" + (buildingRecommendRequest != null ? JsonHelper.ToJson(buildingRecommendRequest) : ""));

            }
            return response;
        }

        /// <summary>
        /// 删除单个楼盘收藏
        /// </summary>
        /// <param name="User">删除用户</param>
        /// <param name="favoriteId"></param>
        /// <returns></returns>
        [HttpDelete("{favoriteId}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "BuildingRecommendDelete" })]
        public async Task<ResponseMessage> DeleteBuildingRecommend(UserInfo User, [FromRoute] string favoriteId)
        {
            Logger.Trace($"用户{User?.UserName ?? ""}({User?.Id ?? ""})删除单个楼盘收藏(DeleteBuildingRecommend)：\r\n请求的参数为：\r\n(favoriteId){favoriteId}");

            ResponseMessage response = new ResponseMessage();
            if (string.IsNullOrEmpty(favoriteId))
            {
                response.Code = ResponseCodeDefines.ArgumentNullError;
                response.Message = "参数出错";
                Logger.Error($"用户{User?.UserName ?? ""}({User?.Id ?? ""})删除单个楼盘收藏(DeleteBuildingRecommend)模型验证失败：\r\n请求的参数为：\r\n(favoriteId){favoriteId}");

                return response;
            }
            try
            {
                await _buildingRecommendManager.DeleteAsync(User, favoriteId, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{User?.UserName ?? ""}({User?.Id ?? ""})删除单个楼盘收藏(DeleteBuildingRecommend)报错：\r\n{e.ToString()},\r\n请求的参数为：\r\n(favoriteId){favoriteId}");

                return response;
            }
            return response;
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="favoriteIds">楼盘收藏idList</param>
        /// <returns></returns>
        [HttpPost("delete")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "BuildingRecommendDeletes" })]
        public async Task<ResponseMessage> DeletePermissionItems(UserInfo user, [FromBody] List<string> favoriteIds)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})删除单个楼盘收藏(DeleteBuildingRecommend)：\r\n请求的参数为：\r\n" + (favoriteIds != null ? JsonHelper.ToJson(favoriteIds) : ""));

            ResponseMessage response = new ResponseMessage();
            if (favoriteIds == null)
            {
                response.Code = ResponseCodeDefines.ArgumentNullError;
                response.Message = "参数出错";
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})删除单个楼盘收藏(DeleteBuildingRecommend)模型验证失败：\r\n请求的参数为：\r\n" + (favoriteIds != null ? JsonHelper.ToJson(favoriteIds) : ""));

                return response;
            }
            try
            {
                await _buildingRecommendManager.DeleteListAsync(user.Id, favoriteIds, HttpContext.RequestAborted);

            }
            catch(Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.Message;
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})删除单个楼盘收藏(DeleteBuildingRecommend)报错：\r\n{e.ToString()},\r\n请求的参数为：\r\n" + (favoriteIds != null ? JsonHelper.ToJson(favoriteIds) : ""));

            }
            return response;
        }

    }
}

