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
    [Route("api/humanregular")]
    public class HumanInfoRegularController : Controller
    {
        private readonly HumanInfoRegularManager _humanInfoRegularManager;
        private readonly ILogger Logger = LoggerManager.GetLogger("HumanInfoRegularController");

        public HumanInfoRegularController(HumanInfoRegularManager humanInfoRegularManager)
        {
            _humanInfoRegularManager = humanInfoRegularManager;
        }


        /// <summary>
        /// 根据Id获取人事转正信息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<HumanInfoRegularResponse>> GetHumanInfoRegular(UserInfo user, [FromRoute] string id)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})根据Id获取人事转正信息(GetHumanInfoRegular)，请求体为：\r\nid:{id ?? ""}");
            ResponseMessage<HumanInfoRegularResponse> response = new ResponseMessage<HumanInfoRegularResponse>();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})根据Id获取人事转正信息(GetHumanInfoRegular)模型验证失败：{response.Message}请求体为：\r\nid:{id ?? ""}");
                return response;
            }
            try
            {
                return await _humanInfoRegularManager.FindByIdAsync(user, id, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.Message;
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})根据Id获取人事转正信息(GetHumanInfoRegular)失败：{e.ToString()}请求体为：\r\nid:{id ?? ""}");
            }
            return response;
        }

        /// <summary>
        /// 新增人事转正信息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="humanInfoAdjustmentRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<HumanInfoRegularResponse>> CreateHumanInfoRegular(UserInfo user, [FromBody] HumanInfoRegularRequest humanInfoRegularRequest)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})新增人事转正信息(CreateHumanInfoRegular)，请求体为：\r\n" + (humanInfoRegularRequest != null ? JsonHelper.ToJson(humanInfoRegularRequest) : ""));
            ResponseMessage<HumanInfoRegularResponse> response = new ResponseMessage<HumanInfoRegularResponse>();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})新增人事转正信息(CreateHumanInfoRegular)模型验证失败：{response.Message}请求体为：\r\n" + (humanInfoRegularRequest != null ? JsonHelper.ToJson(humanInfoRegularRequest) : ""));
                return response;
            }
            try
            {
                return await _humanInfoRegularManager.CreateAsync(user, humanInfoRegularRequest, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.Message;
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})新增人事转正信息(CreateHumanInfoRegular)失败：{response.Message}请求体为：\r\n" + (humanInfoRegularRequest != null ? JsonHelper.ToJson(humanInfoRegularRequest) : ""));
            }
            return response;
        }

        /// <summary>
        /// 更新人事转正信息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="id"></param>
        /// <param name="humanInfoRegularRequest"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<HumanInfoRegularResponse>> PutHumanInfoRegular(UserInfo user, [FromRoute]string id, [FromBody] HumanInfoRegularRequest humanInfoRegularRequest)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})更新人事转正信息(PutHumanInfoRegular)，请求体为：\r\n" + (humanInfoRegularRequest != null ? JsonHelper.ToJson(humanInfoRegularRequest) : ""));
            ResponseMessage<HumanInfoRegularResponse> response = new ResponseMessage<HumanInfoRegularResponse>();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})更新人事转正信息(PutHumanInfoRegular)模型验证失败：{response.Message}请求体为：\r\n" + (humanInfoRegularRequest != null ? JsonHelper.ToJson(humanInfoRegularRequest) : ""));
                return response;
            }
            try
            {
                return await _humanInfoRegularManager.UpdateAsync(user, id, humanInfoRegularRequest, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.Message;
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})更新人事转正信息(PutHumanInfoRegular)失败：{response.Message}请求体为：\r\n" + (humanInfoRegularRequest != null ? JsonHelper.ToJson(humanInfoRegularRequest) : ""));
            }
            return response;
        }

        /// <summary>
        /// 删除人事转正信息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage> DeleteHumanInfoRegular(UserInfo user, [FromRoute]string id)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})删除人事转正信息(DeleteHumanInfoRegular)，请求参数为：\r\n{id}");
            ResponseMessage response = new ResponseMessage();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})删除人事转正信息(DeleteHumanInfoRegular)模型验证失败：{response.Message}请求体为：\r\n{id}");
                return response;
            }
            try
            {
                return await _humanInfoRegularManager.DeleteAsync(user, id, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.Message;
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})删除人事转正信息(DeleteHumanInfoRegular)失败：{response.Message}请求体为：\r\n{id}");
            }
            return response;
        }






        /// <summary>
        /// 新增人事转正信息回调
        /// </summary>
        /// <param name="examineResponse"></param>
        /// <returns></returns>
        [HttpPost("humanregularcallback")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage> HumanRegularCallback([FromBody] ExamineResponse examineResponse)
        {
            Logger.Trace($"新增人事转正信息回调(HumanRegularCallback)：\r\n请求参数为：\r\n" + (examineResponse != null ? JsonHelper.ToJson(examineResponse) : ""));

            ResponseMessage response = new ResponseMessage();

            if (examineResponse == null)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                Logger.Trace($"新增人事转正信息回调(HumanRegularCallback)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (examineResponse != null ? JsonHelper.ToJson(examineResponse) : ""));
                return response;
            }
            try
            {
                response.Code = ResponseCodeDefines.SuccessCode;
                if (examineResponse.ExamineStatus == ExamineStatus.Examined)
                {
                    await _humanInfoRegularManager.UpdateExamineStatus(examineResponse.SubmitDefineId, ExamineStatusEnum.Approved);
                }
                else if (examineResponse.ExamineStatus == ExamineStatus.Reject)
                {
                    await _humanInfoRegularManager.UpdateExamineStatus(examineResponse.SubmitDefineId, ExamineStatusEnum.Reject);
                }
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Trace($"新增人事转正信息回调(HumanRegularCallback)报错：\r\n{e.ToString()}，\r\n请求参数为：\r\n" + examineResponse != null ? JsonHelper.ToJson(examineResponse) : "");
            }
            return response;
        }



        /// <summary>
        /// 新增人事转正信息步骤回调
        /// </summary>
        /// <param name="examineResponse"></param>
        /// <returns></returns>
        [HttpPost("humanregularstepcallback")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage> HumanRegularCallback([FromBody] ExamineStepResponse examineStepResponse)
        {
            Logger.Trace($"新增人事转正信息步骤回调(HumanRegularCallback)：\r\n请求参数为：\r\n" + (examineStepResponse != null ? JsonHelper.ToJson(examineStepResponse) : ""));

            ResponseMessage response = new ResponseMessage();
            return response;
        }






    }
}
