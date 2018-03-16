using ApplicationCore;
using ApplicationCore.Dto;
using ApplicationCore.Filters;
using ApplicationCore.Models;
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
    /// 楼盘基础信息
    /// </summary>
    [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/buildingbaseinfos")]
    public class BuildingBaseInfoController : Controller
    {
        private readonly BuildingBaseInfoManager _buildingBaseInfoManager;
        private readonly IMapper _mapper;
        private readonly ILogger Logger = LoggerManager.GetLogger("BuildingBaseInfo");


        public BuildingBaseInfoController(BuildingBaseInfoManager buildingBaseInfoManager, IMapper mapper)
        {
            _buildingBaseInfoManager = buildingBaseInfoManager;
            _mapper = mapper;
        }

        /// <summary>
        /// 通过楼盘Id获取楼盘基础信息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="buildingId"></param>
        /// <returns></returns>
        [HttpGet("{buildingId}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "BuildingRetrieve" })]
        public async Task<ResponseMessage<BuildingBaseInfoResponse>> GetBuildingBaseInfo(UserInfo user, [FromRoute] string buildingId)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})通过楼盘Id获取楼盘基础信息(GetBuildingBaseInfo)：\r\n请求参数为：\r\n(buildingId){buildingId ?? ""}");

            ResponseMessage<BuildingBaseInfoResponse> response = new ResponseMessage<BuildingBaseInfoResponse>();
            try
            {
                response.Extension = await _buildingBaseInfoManager.FindByIdAsync(buildingId, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})通过楼盘Id获取楼盘基础信息(GetBuildingBaseInfo)报错：\r\n{e.ToString()}，\r\n请求参数为：\r\n(buildingId){buildingId ?? ""}");
            }
            return response;
        }

        /// <summary>
        /// 楼盘名称是否重复
        /// </summary>
        /// <param name="user"></param>
        /// <param name="buildingId"></param>
        /// <param name="buildingName"></param>
        /// <param name="city"></param>
        /// <param name="district"></param>
        /// <param name="area"></param>
        /// <returns></returns>
        [HttpPost("exist")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "BuildingCreate" })]
        public async Task<ResponseMessage<bool>> BuildingNameIsExist(UserInfo user, [FromBody]string buildingId, [FromBody]string buildingName, [FromBody]string city, [FromBody]string district, [FromBody]string area)
        {
            ResponseMessage<bool> response = new ResponseMessage<bool>();
            try
            {
                response.Extension = await _buildingBaseInfoManager.CheckDuplicateBuilding(buildingId, buildingName, city, district, area, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})判断楼盘名称是否重复(BuildingNameIsExist)报错：\r\n{e.ToString()}，\r\n请求参数为：\r\n(buildingId){buildingId ?? ""},(buildingName){buildingName ?? ""},(area){city ?? ""}-{district ?? ""}-{area ?? ""}");
            }
            return response;
        }


        /// <summary>
        /// 保存楼盘基础信息
        /// </summary>
        /// <param name="User"></param>
        /// <param name="buildingId"></param>
        /// <param name="buildingBaseInfoRequest"></param>
        /// <returns></returns>
        [HttpPut("{buildingId}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "BuildingUpdate" })]
        public async Task<ResponseMessage> PutBuildingBaseInfo(UserInfo User, [FromRoute] string buildingId, [FromBody] BuildingBaseInfoRequest buildingBaseInfoRequest)
        {
            Logger.Trace($"用户{User?.UserName ?? ""}({User?.Id ?? ""})保存楼盘基础信息(PutBuildingBaseInfo)：\r\n请求参数为：\r\n" + (buildingBaseInfoRequest != null ? JsonHelper.ToJson(buildingBaseInfoRequest) : ""));

            ResponseMessage response = new ResponseMessage();
            if (!ModelState.IsValid || buildingBaseInfoRequest.Id != buildingId)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Warn($"用户{User?.UserName ?? ""}({User?.Id ?? ""})保存楼盘基础信息(PutBuildingBaseInfo)模型验证失败：\r\n{response.Message ?? ""},请求参数为：\r\n" + (buildingBaseInfoRequest != null ? JsonHelper.ToJson(buildingBaseInfoRequest) : ""));
                return response;
            }
            try
            {
                bool isExists = await _buildingBaseInfoManager.CheckDuplicateBuilding(buildingBaseInfoRequest.Id, buildingBaseInfoRequest.Name, buildingBaseInfoRequest.City, buildingBaseInfoRequest.District, buildingBaseInfoRequest.Area, HttpContext.RequestAborted);
                if (isExists)
                {
                    response.Code = ResponseCodeDefines.ObjectAlreadyExists;
                    response.Message = "该区域已经存在相同名字的楼盘！";
                    Logger.Warn($"用户{User?.UserName ?? ""}({User?.Id ?? ""})保存楼盘基础信息(PutBuildingBaseInfo)失败：\r\n该区域已经存在相同名字的楼盘，\r\n请求参数为：\r\n" + (buildingBaseInfoRequest != null ? JsonHelper.ToJson(buildingBaseInfoRequest) : ""));
                    return response;
                }
                await _buildingBaseInfoManager.SaveAsync(User, buildingBaseInfoRequest, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{User?.UserName ?? ""}({User?.Id ?? ""})保存楼盘基本信息(PutBuildingBaseInfo)报错：\r\n{e.ToString()},\r\n请求参数为：\r\n" + (buildingBaseInfoRequest != null ? JsonHelper.ToJson(buildingBaseInfoRequest) : ""));
            }
            return response;
        }


    }
}
