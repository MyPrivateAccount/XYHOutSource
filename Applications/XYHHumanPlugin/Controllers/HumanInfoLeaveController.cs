using ApplicationCore;
using ApplicationCore.Dto;
using ApplicationCore.Filters;
using ApplicationCore.Managers;
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
    [Route("api/humanleave")]
    public class HumanInfoLeaveController : Controller
    {
        private readonly HumanInfoLeaveManager _humanInfoLeaveManager;
        private readonly ILogger Logger = LoggerManager.GetLogger("HumanInfoLeaveController");

        public HumanInfoLeaveController(HumanInfoLeaveManager humanInfoLeaveManager)
        {
            _humanInfoLeaveManager = humanInfoLeaveManager;
        }

        /// <summary>
        /// 根据Id获取人事离职信息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<HumanInfoLeaveResponse>> GetHumanInfoLeave(UserInfo user, [FromRoute] string id)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})根据Id获取人事离职信息(GetHumanInfoLeave)，请求体为：\r\nid:{id ?? ""}");
            ResponseMessage<HumanInfoLeaveResponse> response = new ResponseMessage<HumanInfoLeaveResponse>();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})根据Id获取人事离职信息(GetHumanInfoLeave)模型验证失败：{response.Message}请求体为：\r\nid:{id ?? ""}");
                return response;
            }
            try
            {
                return await _humanInfoLeaveManager.FindByIdAsync(user, id, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.Message;
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})根据Id获取人事离职信息(GetHumanInfoLeave)失败：{e.ToString()}请求体为：\r\nid:{id ?? ""}");
            }
            return response;
        }

        /// <summary>
        /// 新增人事离职信息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="humanInfoAdjustmentRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<HumanInfoLeaveResponse>> CreateHumanInfoLeave(UserInfo user, [FromBody] HumanInfoLeaveRequest humanInfoLeaveRequest)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})新增人事离职信息(CreateHumanInfoLeave)，请求体为：\r\n" + (humanInfoLeaveRequest != null ? JsonHelper.ToJson(humanInfoLeaveRequest) : ""));
            ResponseMessage<HumanInfoLeaveResponse> response = new ResponseMessage<HumanInfoLeaveResponse>();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})新增人事离职信息(CreateHumanInfoLeave)模型验证失败：{response.Message}请求体为：\r\n" + (humanInfoLeaveRequest != null ? JsonHelper.ToJson(humanInfoLeaveRequest) : ""));
                return response;
            }
            try
            {
                return await _humanInfoLeaveManager.CreateAsync(user, humanInfoLeaveRequest, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.Message;
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})新增人事离职信息(CreateHumanInfoLeave)失败：{response.Message}请求体为：\r\n" + (humanInfoLeaveRequest != null ? JsonHelper.ToJson(humanInfoLeaveRequest) : ""));
            }
            return response;
        }

        /// <summary>
        /// 更新人事离职信息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="id"></param>
        /// <param name="humanInfoLeaveRequest"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<HumanInfoLeaveResponse>> PutHumanInfoLeave(UserInfo user, [FromRoute]string id, [FromBody] HumanInfoLeaveRequest humanInfoLeaveRequest)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})更新人事离职信息(PutHumanInfoLeave)，请求体为：\r\n" + (humanInfoLeaveRequest != null ? JsonHelper.ToJson(humanInfoLeaveRequest) : ""));
            ResponseMessage<HumanInfoLeaveResponse> response = new ResponseMessage<HumanInfoLeaveResponse>();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})更新人事离职信息(PutHumanInfoLeave)模型验证失败：{response.Message}请求体为：\r\n" + (humanInfoLeaveRequest != null ? JsonHelper.ToJson(humanInfoLeaveRequest) : ""));
                return response;
            }
            try
            {
                return await _humanInfoLeaveManager.UpdateAsync(user, id, humanInfoLeaveRequest, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.Message;
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})更新人事离职信息(PutHumanInfoLeave)失败：{response.Message}请求体为：\r\n" + (humanInfoLeaveRequest != null ? JsonHelper.ToJson(humanInfoLeaveRequest) : ""));
            }
            return response;
        }

        /// <summary>
        /// 删除人事离职信息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage> DeleteHumanInfoLeave(UserInfo user, [FromRoute]string id)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})删除人事离职信息(DeleteHumanInfoLeave)，请求参数为：\r\n{id}");
            ResponseMessage response = new ResponseMessage();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})删除人事离职信息(DeleteHumanInfoLeave)模型验证失败：{response.Message}请求体为：\r\n{id}");
                return response;
            }
            try
            {
                return await _humanInfoLeaveManager.DeleteAsync(user, id, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.Message;
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})删除人事离职信息(DeleteHumanInfoLeave)失败：{response.Message}请求体为：\r\n{id}");
            }
            return response;
        }



        /// <summary>
        /// 新增人事信息回调
        /// </summary>
        /// <param name="examineResponse"></param>
        /// <returns></returns>
        [HttpPost("humaninfocallback")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage> HumanInfoCreateCallback([FromBody] ExamineResponse examineResponse)
        {
            Logger.Trace($"新增人事信息回调(HumanInfoCreateCallback)：\r\n请求参数为：\r\n" + (examineResponse != null ? JsonHelper.ToJson(examineResponse) : ""));

            ResponseMessage response = new ResponseMessage();

            if (examineResponse == null)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                Logger.Trace($"新增人事信息回调(HumanInfoCreateCallback)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (examineResponse != null ? JsonHelper.ToJson(examineResponse) : ""));
                return response;
            }
            try
            {
                response.Code = ResponseCodeDefines.SuccessCode;
                if (examineResponse.ExamineStatus == ExamineStatus.Examined)
                {
                    await _humanInfoLeaveManager.UpdateExamineStatus(examineResponse.SubmitDefineId, ExamineStatusEnum.Approved);
                }
                else if (examineResponse.ExamineStatus == ExamineStatus.Reject)
                {
                    await _humanInfoLeaveManager.UpdateExamineStatus(examineResponse.SubmitDefineId, ExamineStatusEnum.Reject);
                }
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Trace($"新增人事信息回调(HumanInfoCreateCallback)报错：\r\n{e.ToString()}，\r\n请求参数为：\r\n" + examineResponse != null ? JsonHelper.ToJson(examineResponse) : "");
            }
            return response;
        }



        /// <summary>
        /// 新增人事信息步骤回调
        /// </summary>
        /// <param name="examineResponse"></param>
        /// <returns></returns>
        [HttpPost("humaninfostepcallback")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage> HumanInfoCreateStepCallback([FromBody] ExamineStepResponse examineStepResponse)
        {
            Logger.Trace($"新增人事信息步骤回调(HumanInfoCreateStepCallback)：\r\n请求参数为：\r\n" + (examineStepResponse != null ? JsonHelper.ToJson(examineStepResponse) : ""));

            ResponseMessage response = new ResponseMessage();
            return response;
        }



    }
}
