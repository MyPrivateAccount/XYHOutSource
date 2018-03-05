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
using XYHShopsPlugin.Dto.Request;
using XYHShopsPlugin.Dto.Response;
using XYHShopsPlugin.Managers;

namespace XYHShopsPlugin.Controllers
{
    /// <summary>
    /// 楼盘报备规则
    /// </summary>
    [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/buildingrule")]
    public class BuildingRuleController : Controller
    {
        private readonly BuildingRuleManager _buildingRuleManager;
        private readonly ILogger Logger = LoggerManager.GetLogger("BuildingRule");

        public BuildingRuleController(BuildingRuleManager buildingRuleManager)
        {
            _buildingRuleManager = buildingRuleManager;
        }
        /// <summary>
        /// 通过楼盘Id获取楼盘报备规则
        /// </summary>
        /// <param name="user"></param>
        /// <param name="buildingId"></param>
        /// <returns></returns>
        [HttpGet("{buildingId}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "BuildingRule" })]
        public async Task<ResponseMessage<BuildingRuleInfoResponse>> GetBuildingRule([FromRoute] string buildingId)
        {
            ResponseMessage<BuildingRuleInfoResponse> response = new ResponseMessage<BuildingRuleInfoResponse>();
            try
            {
                response.Extension = await _buildingRuleManager.FindByIdAsync(buildingId);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户通过楼盘Id获取楼盘报备规则(GetBuildingRule)报错：\r\n{e.ToString()},\r\n请求的参数为：\r\n(buildingId){buildingId}");
            }
            return response;
        }
        /// <summary>
        /// 更新楼盘报备规则
        /// </summary>
        /// <param name="user"></param>
        /// <param name="buildingRuleRequest"></param>
        /// <returns></returns>
        [HttpPut]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "BuildingRuleUpdate" })]
        public async Task<ResponseMessage<BuildingRuleInfoResponse>> PutBuildingRule(UserInfo user, [FromBody] BuildingRuleRequest buildingRuleRequest)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})更新楼盘报备规则(PutBuildingRule)：\r\n请求的参数为：\r\n" + (buildingRuleRequest != null ? JsonHelper.ToJson(buildingRuleRequest) : ""));

            ResponseMessage<BuildingRuleInfoResponse> response = new ResponseMessage<BuildingRuleInfoResponse>();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})更新楼盘报备规则(PutBuildingRule)模型验证失败：\r\n{response.Message ?? ""},\r\n请求的参数为：\r\n" + (buildingRuleRequest != null ? JsonHelper.ToJson(buildingRuleRequest) : ""));
                return response;
            }
            try
            {
                response.Extension = await _buildingRuleManager.SaveAsync(user, buildingRuleRequest, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})更新楼盘报备规则(PutBuildingRule)报错：\r\n{e.ToString()},\r\n请求的参数为：\r\n" + (buildingRuleRequest != null ? JsonHelper.ToJson(buildingRuleRequest) : ""));
            }
            return response;
        }
    }
}