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
using XYHShopsPlugin.Dto;
using XYHShopsPlugin.Managers;

namespace XYHShopsPlugin.Controllers
{
    /// <summary>
    /// 楼盘商铺信息
    /// </summary>
    [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/buildingshopinfos")]
    public class BuildingShopInfoController : Controller
    {
        private readonly BuildingShopInfoManager _buildingShopInfoManager;
        private readonly ILogger Logger = LoggerManager.GetLogger("BuildingShopInfo");

        public BuildingShopInfoController(BuildingShopInfoManager buildingShopInfoManager)
        {
            _buildingShopInfoManager = buildingShopInfoManager;
        }

        /// <summary>
        /// 通过楼盘Id获取楼盘商铺信息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="buildingId"></param>
        /// <returns></returns>
        [HttpGet("{buildingId}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "BuildingRetrieve" })]
        public async Task<ResponseMessage<BuildingShopInfoResponse>> GetBuildingShopInfo(UserInfo user, [FromRoute] string buildingId)
        {
            ResponseMessage<BuildingShopInfoResponse> response = new ResponseMessage<BuildingShopInfoResponse>();
            try
            {
                response.Extension = await _buildingShopInfoManager.FindByIdAsync(buildingId, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})通过楼盘Id获取楼盘商铺信息(GetBuildingShopInfo)报错：\r\n{e.ToString()}，\r\n请求参数为：\r\n(buildingId){buildingId ?? ""}");
            }
            return response;
        }

        /// <summary>
        /// 保存楼盘商铺信息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="buildingId"></param>
        /// <param name="buildingShopInfoRequest"></param>
        /// <returns></returns>
        [HttpPut("{buildingId}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "BuildingUpdate" })]
        public async Task<ResponseMessage> PutBuildingShopsInfo(UserInfo user, [FromRoute] string buildingId, [FromBody] BuildingShopInfoRequest buildingShopInfoRequest)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})保存楼盘商铺信息(PutBuildingBaseInfo)：\r\n请求参数为：\r\n" + (buildingShopInfoRequest != null ? JsonHelper.ToJson(buildingShopInfoRequest) : ""));

            ResponseMessage response = new ResponseMessage();
            if (!ModelState.IsValid || buildingShopInfoRequest.Id != buildingId)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})保存楼盘商铺信息(PutBuildingBaseInfo)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (buildingShopInfoRequest != null ? JsonHelper.ToJson(buildingShopInfoRequest) : ""));
                return response;
            }
            try
            {
                await _buildingShopInfoManager.SaveAsync(user, buildingShopInfoRequest, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})保存楼盘商铺信息(PutBuildingBaseInfo)报错：\r\n{e.ToString()}，\r\n请求参数为：\r\n" + (buildingShopInfoRequest != null ? JsonHelper.ToJson(buildingShopInfoRequest) : ""));
            }
            return response;
        }

    }
}
