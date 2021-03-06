﻿using ApplicationCore;
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
    [Route("api/humanposition")]
    public class HumanPositionController : Controller
    {

        private readonly HumanPositionManager _humanPositionManager;
        private readonly ILogger Logger = LoggerManager.GetLogger("HumanInfoLeaveController");

        public HumanPositionController(HumanPositionManager humanPositionManager)
        {
            _humanPositionManager = humanPositionManager;
        }

        /// <summary>
        /// 根据Id获取人事职位信息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<HumanPositionResponse>> GetHumanPosition(UserInfo user, [FromRoute] string id)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})根据Id获取人事职位信息(GetHumanPosition)，请求体为：\r\nid:{id ?? ""}");
            ResponseMessage<HumanPositionResponse> response = new ResponseMessage<HumanPositionResponse>();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})根据Id获取人事职位信息(GetHumanPosition)模型验证失败：{response.Message}请求体为：\r\nid:{id ?? ""}");
                return response;
            }
            try
            {
                return await _humanPositionManager.FindByIdAsync(user, id, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.Message;
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})根据Id获取人事职位信息(GetHumanPosition)失败：{e.ToString()}请求体为：\r\nid:{id ?? ""}");
            }
            return response;
        }

        /// <summary>
        /// 新增人事职位信息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="humanPositionRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<HumanPositionResponse>> CreateHumanPosition(UserInfo user, [FromBody] HumanPositionRequest humanPositionRequest)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})新增人事职位信息(CreateHumanPosition)，请求体为：\r\n" + (humanPositionRequest != null ? JsonHelper.ToJson(humanPositionRequest) : ""));
            ResponseMessage<HumanPositionResponse> response = new ResponseMessage<HumanPositionResponse>();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})新增人事职位信息(CreateHumanPosition)模型验证失败：{response.Message}请求体为：\r\n" + (humanPositionRequest != null ? JsonHelper.ToJson(humanPositionRequest) : ""));
                return response;
            }
            try
            {
                return await _humanPositionManager.CreateAsync(user, humanPositionRequest, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.Message;
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})新增人事职位信息(CreateHumanPosition)失败：{response.Message}请求体为：\r\n" + (humanPositionRequest != null ? JsonHelper.ToJson(humanPositionRequest) : ""));
            }
            return response;
        }

        /// <summary>
        /// 更新人事职位信息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="id"></param>
        /// <param name="humanPositionRequest"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<HumanPositionResponse>> PutHumanPosition(UserInfo user, [FromRoute]string id, [FromBody] HumanPositionRequest humanPositionRequest)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})更新人事职位信息(PutHumanPosition)，请求体为：\r\n" + (humanPositionRequest != null ? JsonHelper.ToJson(humanPositionRequest) : ""));
            ResponseMessage<HumanPositionResponse> response = new ResponseMessage<HumanPositionResponse>();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})更新人事职位信息(PutHumanPosition)模型验证失败：{response.Message}请求体为：\r\n" + (humanPositionRequest != null ? JsonHelper.ToJson(humanPositionRequest) : ""));
                return response;
            }
            try
            {
                return await _humanPositionManager.UpdateAsync(user, id, humanPositionRequest, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.Message;
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})更新人事职位信息(PutHumanPosition)失败：{response.Message}请求体为：\r\n" + (humanPositionRequest != null ? JsonHelper.ToJson(humanPositionRequest) : ""));
            }
            return response;
        }

        /// <summary>
        /// 删除人事职位信息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage> DeleteHumanPosition(UserInfo user, [FromRoute]string id)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})删除人事职位信息(DeleteHumanPosition)，请求参数为：\r\n{id}");
            ResponseMessage response = new ResponseMessage();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})删除人事职位信息(DeleteHumanPosition)模型验证失败：{response.Message}请求体为：\r\n{id}");
                return response;
            }
            try
            {
                return await _humanPositionManager.DeleteAsync(user, id, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.Message;
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})删除人事职位信息(DeleteHumanPosition)失败：{response.Message}请求体为：\r\n{id}");
            }
            return response;
        }








        /// <summary>
        /// 新增人事信息回调
        /// </summary>
        /// <param name="examineResponse"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("humanpositioncallback")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage> HumanPositionCallback([FromBody] ExamineResponse examineResponse)
        {
            Logger.Trace($"新增职位信息回调(HumanPositionCallback)：\r\n请求参数为：\r\n" + (examineResponse != null ? JsonHelper.ToJson(examineResponse) : ""));

            ResponseMessage response = new ResponseMessage();

            if (examineResponse == null)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                Logger.Trace($"新增职位信息回调(HumanPositionCallback)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (examineResponse != null ? JsonHelper.ToJson(examineResponse) : ""));
                return response;
            }
            try
            {
                response.Code = ResponseCodeDefines.SuccessCode;
                if (examineResponse.ExamineStatus == ExamineStatus.Examined)
                {
                    await _humanPositionManager.UpdateExamineStatus(examineResponse.SubmitDefineId, ExamineStatusEnum.Approved);
                }
                else if (examineResponse.ExamineStatus == ExamineStatus.Reject)
                {
                    await _humanPositionManager.UpdateExamineStatus(examineResponse.SubmitDefineId, ExamineStatusEnum.Reject);
                }
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Trace($"新增职位信息回调(HumanPositionCallback)报错：\r\n{e.ToString()}，\r\n请求参数为：\r\n" + (examineResponse != null ? JsonHelper.ToJson(examineResponse) : ""));
            }
            return response;
        }



        /// <summary>
        /// 新增职位信息步骤回调
        /// </summary>
        /// <param name="examineResponse"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("humanpositionstepcallback")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage> HumanPositionStepCallback([FromBody] ExamineStepResponse examineStepResponse)
        {
            Logger.Trace($"新增职位信息步骤回调(HumanPositionStepCallback)：\r\n请求参数为：\r\n" + (examineStepResponse != null ? JsonHelper.ToJson(examineStepResponse) : ""));

            ResponseMessage response = new ResponseMessage();
            return response;
        }





    }
}
