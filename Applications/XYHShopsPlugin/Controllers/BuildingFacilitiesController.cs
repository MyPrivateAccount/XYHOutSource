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
    /// 楼盘配套信息
    /// </summary>
    [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/buildingfacilities")]
    public class BuildingFacilitiesController : Controller
    {
        private readonly BuildingFacilitiesManager _buildingFacilitiesManager;
        private readonly ILogger Logger = LoggerManager.GetLogger("BuildingFacilities");

        public BuildingFacilitiesController(BuildingFacilitiesManager buildingFacilitiesManager)
        {
            _buildingFacilitiesManager = buildingFacilitiesManager;
        }
        /// <summary>
        /// 通过楼盘Id获取楼盘配套信息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="buildingId"></param>
        /// <returns></returns>
        [HttpGet("{buildingId}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "BuildingRetrieve" })]
        public async Task<ResponseMessage<BuildingFacilitiesInfoResponse>> GetBuildingFacilities(UserInfo user, [FromRoute] string buildingId)
        {
            ResponseMessage<BuildingFacilitiesInfoResponse> response = new ResponseMessage<BuildingFacilitiesInfoResponse>();
            try
            {
                response.Extension = await _buildingFacilitiesManager.FindByIdAsync(buildingId, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})通过楼盘Id获取楼盘配套信息(GetBuildingFacilities)报错：{e.ToString()},请求参数为：(buildingId){buildingId ?? ""}");
            }
            return response;
        }
        /// <summary>
        /// 保存楼盘配套信息
        /// </summary>
        /// <param name="User"></param>
        /// <param name="buildingId"></param>
        /// <param name="buildingFacilitiesRequest"></param>
        /// <returns></returns>
        [HttpPut("{buildingId}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "BuildingUpdate" })]
        public async Task<ResponseMessage> PutBuildingFacilities(UserInfo User, [FromRoute] string buildingId, [FromBody] BuildingFacilitiesRequest buildingFacilitiesRequest)
        {
            Logger.Trace($"用户{User?.UserName ?? ""}({User?.Id ?? ""})保存楼盘基本信息(PutBuildingFacilities)：\r\n请求参数为：\r\n(buildingId){buildingId ?? ""},(buildingFacilitiesRequest)" + (buildingFacilitiesRequest != null ? JsonHelper.ToJson(buildingFacilitiesRequest) : ""));

            ResponseMessage response = new ResponseMessage();
            if (!ModelState.IsValid || buildingFacilitiesRequest.Id != buildingId)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Warn($"用户{User?.UserName ?? ""}({User?.Id ?? ""})保存楼盘基本信息(PutBuildingFacilities)模型验证失败：\r\n{response.Message ?? ""},\r\n请求参数为：\r\n(buildingId){buildingId ?? ""},(buildingFacilitiesRequest)" + (buildingFacilitiesRequest != null ? JsonHelper.ToJson(buildingFacilitiesRequest) : ""));
                return response;
            }
            try
            {
                await _buildingFacilitiesManager.SaveAsync(User, buildingFacilitiesRequest, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{User?.UserName ?? ""}({User?.Id ?? ""})保存楼盘基本信息(PutBuildingFacilities)报错：\r\n{e.ToString()},\r\n请求参数为：\r\n(buildingId){buildingId ?? ""},(buildingFacilitiesRequest)" + (buildingFacilitiesRequest != null ? JsonHelper.ToJson(buildingFacilitiesRequest) : ""));
            }
            return response;
        }

    }
}
