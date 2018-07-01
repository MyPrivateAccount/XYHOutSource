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
using XYHHumanPlugin.Dto.Request;
using XYHHumanPlugin.Dto.Response;
using XYHHumanPlugin.Managers;

namespace XYHHumanPlugin.Controllers
{
    [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/humanpartposition")]
    public class HumanInfoPartPositionController : Controller
    {
        private readonly HumanInfoPartPositionManager _humanInfoPartPositionManager;
        private readonly ILogger Logger = LoggerManager.GetLogger("HumanInfoPartPositionController");

        public HumanInfoPartPositionController(HumanInfoPartPositionManager humanInfoPartPositionManager)
        {
            _humanInfoPartPositionManager = humanInfoPartPositionManager;
        }


        /// <summary>
        /// 根据Id获取人事兼职信息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<HumanInfoPartPositionResponse>> GetHumanInfoPartPosition(UserInfo user, [FromRoute] string id)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})根据Id获取人事兼职信息(GetHumanInfoPartPosition)，请求体为：\r\nid:{id ?? ""}");
            ResponseMessage<HumanInfoPartPositionResponse> response = new ResponseMessage<HumanInfoPartPositionResponse>();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})根据Id获取人事兼职信息(GetHumanInfoPartPosition)模型验证失败：{response.Message}请求体为：\r\nid:{id ?? ""}");
                return response;
            }
            try
            {
                response.Extension = await _humanInfoPartPositionManager.FindByIdAsync(id, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.Message;
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})根据Id获取人事兼职信息(GetHumanInfoPartPosition)失败：{e.ToString()}请求体为：\r\nid:{id ?? ""}");
            }
            return response;
        }

        /// <summary>
        /// 新增人事兼职信息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="humanInfoAdjustmentRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<HumanInfoPartPositionResponse>> CreateHumanInfoPartPosition(UserInfo user, [FromBody]HumanInfoPartPositionRequest humanInfoPartPositionRequest)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})新增人事兼职信息(CreateHumanInfoPartPosition)，请求体为：\r\n" + (humanInfoPartPositionRequest != null ? JsonHelper.ToJson(humanInfoPartPositionRequest) : ""));
            ResponseMessage<HumanInfoPartPositionResponse> response = new ResponseMessage<HumanInfoPartPositionResponse>();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})新增人事兼职信息(CreateHumanInfoPartPosition)模型验证失败：{response.Message}请求体为：\r\n" + (humanInfoPartPositionRequest != null ? JsonHelper.ToJson(humanInfoPartPositionRequest) : ""));
                return response;
            }
            try
            {
                response.Extension = await _humanInfoPartPositionManager.CreateAsync(user, humanInfoPartPositionRequest, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.Message;
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})新增人事兼职信息(CreateHumanInfoPartPosition)失败：{response.Message}请求体为：\r\n" + (humanInfoPartPositionRequest != null ? JsonHelper.ToJson(humanInfoPartPositionRequest) : ""));
            }
            return response;
        }

        /// <summary>
        /// 更新人事兼职信息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="id"></param>
        /// <param name="humanInfoPartPositionRequest"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<HumanInfoPartPositionResponse>> PutHumanInfoPartPosition(UserInfo user, [FromRoute]string id, [FromBody] HumanInfoPartPositionRequest humanInfoPartPositionRequest)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})更新人事兼职信息(PutHumanInfoPartPosition)，请求体为：\r\n" + (humanInfoPartPositionRequest != null ? JsonHelper.ToJson(humanInfoPartPositionRequest) : ""));
            ResponseMessage<HumanInfoPartPositionResponse> response = new ResponseMessage<HumanInfoPartPositionResponse>();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})更新人事兼职信息(PutHumanInfoPartPosition)模型验证失败：{response.Message}请求体为：\r\n" + (humanInfoPartPositionRequest != null ? JsonHelper.ToJson(humanInfoPartPositionRequest) : ""));
                return response;
            }
            try
            {
                response.Extension = await _humanInfoPartPositionManager.UpdateAsync(user, id, humanInfoPartPositionRequest, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.Message;
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})更新人事兼职信息(PutHumanInfoPartPosition)失败：{response.Message}请求体为：\r\n" + (humanInfoPartPositionRequest != null ? JsonHelper.ToJson(humanInfoPartPositionRequest) : ""));
            }
            return response;
        }

        /// <summary>
        /// 删除人事兼职信息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage> DeleteHumanInfoPartPosition(UserInfo user, [FromRoute]string id)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})删除人事兼职信息(DeleteHumanInfoPartPosition)，请求参数为：\r\n{id}");
            ResponseMessage response = new ResponseMessage();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})删除人事兼职信息(DeleteHumanInfoPartPosition)模型验证失败：{response.Message}请求体为：\r\n{id}");
                return response;
            }
            try
            {
                await _humanInfoPartPositionManager.DeleteAsync(user, id, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.Message;
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})删除人事兼职信息(DeleteHumanInfoPartPosition)失败：{response.Message}请求体为：\r\n{id}");
            }
            return response;
        }
    }
}
