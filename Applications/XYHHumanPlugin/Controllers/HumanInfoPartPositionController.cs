using ApplicationCore;
using ApplicationCore.Dto;
using ApplicationCore.Filters;
using AspNet.Security.OAuth.Validation;
using GatewayInterface.Dto.Response;
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
using XYHHumanPlugin.Models;

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
                return await _humanInfoPartPositionManager.FindByIdAsync(user, id, HttpContext.RequestAborted);
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
        /// 根据人事Id获取人事兼职信息列表
        /// </summary>
        /// <param name="user"></param>
        /// <param name="humanId"></param>
        /// <returns></returns>
        [HttpGet("list/{humanId}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<List<HumanInfoPartPositionResponse>>> GetHumanInfoPartPositionList(UserInfo user, [FromRoute] string humanId)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})根据人事Id获取人事兼职信息列表(GetHumanInfoPartPositionList)，请求体为：\r\nid:{humanId ?? ""}");
            ResponseMessage<List<HumanInfoPartPositionResponse>> response = new ResponseMessage<List<HumanInfoPartPositionResponse>>();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})根据人事Id获取人事兼职信息列表(GetHumanInfoPartPositionList)模型验证失败：{response.Message}请求体为：\r\nid:{humanId ?? ""}");
                return response;
            }
            try
            {
                return await _humanInfoPartPositionManager.FindByHumanIdAsync(user, humanId, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.Message;
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})根据人事Id获取人事兼职信息列表(GetHumanInfoPartPositionList)失败：{e.ToString()}请求体为：\r\nid:{humanId ?? ""}");
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
                return await _humanInfoPartPositionManager.CreateAsync(user, humanInfoPartPositionRequest, HttpContext.RequestAborted);
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
                return await _humanInfoPartPositionManager.UpdateAsync(user, id, humanInfoPartPositionRequest, HttpContext.RequestAborted);
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
                return await _humanInfoPartPositionManager.DeleteAsync(user, id, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.Message;
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})删除人事兼职信息(DeleteHumanInfoPartPosition)失败：{response.Message}请求体为：\r\n{id}");
            }
            return response;
        }





        /// <summary>
        /// 新增人事信息回调
        /// </summary>
        /// <param name="examineResponse"></param>
        /// <returns></returns>
        [HttpPost("humanpartpositioncallback")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage> HumanPartPositionCallback([FromBody] ExamineResponse examineResponse)
        {
            Logger.Trace($"新增人事信息回调(HumanPartPositionCallback)：\r\n请求参数为：\r\n" + (examineResponse != null ? JsonHelper.ToJson(examineResponse) : ""));

            ResponseMessage response = new ResponseMessage();

            if (examineResponse == null)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                Logger.Trace($"新增人事信息回调(HumanPartPositionCallback)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (examineResponse != null ? JsonHelper.ToJson(examineResponse) : ""));
                return response;
            }
            try
            {
                response.Code = ResponseCodeDefines.SuccessCode;
                if (examineResponse.ExamineStatus == ExamineStatus.Examined)
                {
                    await _humanInfoPartPositionManager.UpdateExamineStatus(examineResponse.SubmitDefineId, ExamineStatusEnum.Approved);
                }
                else if (examineResponse.ExamineStatus == ExamineStatus.Reject)
                {
                    await _humanInfoPartPositionManager.UpdateExamineStatus(examineResponse.SubmitDefineId, ExamineStatusEnum.Reject);
                }
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Trace($"新增人事信息回调(HumanPartPositionCallback)报错：\r\n{e.ToString()}，\r\n请求参数为：\r\n" + examineResponse != null ? JsonHelper.ToJson(examineResponse) : "");
            }
            return response;
        }



        /// <summary>
        /// 新增人事信息步骤回调
        /// </summary>
        /// <param name="examineStepResponse"></param>
        /// <returns></returns>
        [HttpPost("humanpartpositionstepcallback")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage> HumanPartPositionStepCallback([FromBody] ExamineStepResponse examineStepResponse)
        {
            Logger.Trace($"新增人事信息步骤回调(HumanPartPositionStepCallback)：\r\n请求参数为：\r\n" + (examineStepResponse != null ? JsonHelper.ToJson(examineStepResponse) : ""));

            ResponseMessage response = new ResponseMessage();
            return response;
        }







    }
}
