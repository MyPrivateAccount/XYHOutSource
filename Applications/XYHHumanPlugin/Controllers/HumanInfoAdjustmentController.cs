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
    [Route("api/humanadjustment")]
    public class HumanInfoAdjustmentController : Controller
    {
        private readonly HumanInfoAdjustmentManager _humanInfoAdjustmentManager;
        private readonly PermissionExpansionManager _permissionExpansionManager;
        private readonly ILogger Logger = LoggerManager.GetLogger("HumanInfoAdjustmentController");

        public HumanInfoAdjustmentController(HumanInfoAdjustmentManager humanInfoAdjustmentManager,
            PermissionExpansionManager permissionExpansionManager
            )
        {
            _humanInfoAdjustmentManager = humanInfoAdjustmentManager;
            _permissionExpansionManager = permissionExpansionManager;
        }


        /// <summary>
        /// 根据Id获取异动调薪信息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<HumanInfoAdjustmentResponse>> GetHumanInfoAdjustment(UserInfo user, [FromRoute] string id)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})根据Id获取异动调薪信息(GetHumanInfoAdjustment)，请求体为：\r\nid:{id ?? ""}");
            ResponseMessage<HumanInfoAdjustmentResponse> response = new ResponseMessage<HumanInfoAdjustmentResponse>();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})根据Id获取异动调薪信息(GetHumanInfoAdjustment)模型验证失败：{response.Message}请求体为：\r\nid:{id ?? ""}");
                return response;
            }
            try
            {
                return await _humanInfoAdjustmentManager.FindByIdAsync(user, id, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.Message;
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})根据Id获取异动调薪信息(GetHumanInfoAdjustment)失败：{e.ToString()}请求体为：\r\nid:{id ?? ""}");
            }
            return response;
        }

        /// <summary>
        /// 新增异动调薪信息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="humanInfoAdjustmentRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<HumanInfoAdjustmentResponse>> CreateHumanInfoAdjustment(UserInfo user, [FromBody] HumanInfoAdjustmentRequest humanInfoAdjustmentRequest)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})新增异动调薪信息(CreateHumanInfoAdjustment)，请求体为：\r\n" + (humanInfoAdjustmentRequest != null ? JsonHelper.ToJson(humanInfoAdjustmentRequest) : ""));
            ResponseMessage<HumanInfoAdjustmentResponse> response = new ResponseMessage<HumanInfoAdjustmentResponse>();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})新增异动调薪信息(CreateHumanInfoAdjustment)模型验证失败：{response.Message}请求体为：\r\n" + (humanInfoAdjustmentRequest != null ? JsonHelper.ToJson(humanInfoAdjustmentRequest) : ""));
                return response;
            }
            try
            {
                return await _humanInfoAdjustmentManager.CreateAsync(user, humanInfoAdjustmentRequest, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.Message;
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})新增异动调薪信息(CreateHumanInfoAdjustment)失败：{response.Message}请求体为：\r\n" + (humanInfoAdjustmentRequest != null ? JsonHelper.ToJson(humanInfoAdjustmentRequest) : ""));
            }
            return response;
        }

        /// <summary>
        /// 更新异动调薪信息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="id"></param>
        /// <param name="humanInfoAdjustmentRequest"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<HumanInfoAdjustmentResponse>> PutHumanInfoAdjustment(UserInfo user, [FromRoute]string id, [FromBody] HumanInfoAdjustmentRequest humanInfoAdjustmentRequest)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})更新异动调薪信息(PutHumanInfoAdjustment)，请求体为：\r\n" + (humanInfoAdjustmentRequest != null ? JsonHelper.ToJson(humanInfoAdjustmentRequest) : ""));
            ResponseMessage<HumanInfoAdjustmentResponse> response = new ResponseMessage<HumanInfoAdjustmentResponse>();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})更新异动调薪信息(PutHumanInfoAdjustment)模型验证失败：{response.Message}请求体为：\r\n" + (humanInfoAdjustmentRequest != null ? JsonHelper.ToJson(humanInfoAdjustmentRequest) : ""));
                return response;
            }
            try
            {
                return await _humanInfoAdjustmentManager.UpdateAsync(user, id, humanInfoAdjustmentRequest, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.Message;
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})更新异动调薪信息(PutHumanInfoAdjustment)失败：{response.Message}请求体为：\r\n" + (humanInfoAdjustmentRequest != null ? JsonHelper.ToJson(humanInfoAdjustmentRequest) : ""));
            }
            return response;
        }

        /// <summary>
        /// 删除异动调薪信息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage> DeleteHumanInfoAdjustment(UserInfo user, [FromRoute]string id)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})删除异动调薪信息(DeleteHumanInfoAdjustment)，请求参数为：\r\n{id}");
            ResponseMessage response = new ResponseMessage();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})删除异动调薪信息(DeleteHumanInfoAdjustment)模型验证失败：{response.Message}请求体为：\r\n{id}");
                return response;
            }
            try
            {
                return await _humanInfoAdjustmentManager.DeleteAsync(user, id, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.Message;
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})删除异动调薪信息(DeleteHumanInfoAdjustment)失败：{response.Message}请求体为：\r\n{id}");
            }
            return response;
        }





        /// <summary>
        /// 新增人事信息调动回调
        /// </summary>
        /// <param name="examineResponse"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("humanadjustmentcallback")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage> HumanInfoAdjustmentCreateCallback([FromBody] ExamineResponse examineResponse)
        {
            Logger.Trace($"新增人事信息调动回调(HumanInfoAdjustmentCreateCallback)：\r\n请求参数为：\r\n" + (examineResponse != null ? JsonHelper.ToJson(examineResponse) : ""));

            ResponseMessage response = new ResponseMessage();

            if (examineResponse == null)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                Logger.Trace($"新增人事信息调动回调(HumanInfoAdjustmentCreateCallback)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (examineResponse != null ? JsonHelper.ToJson(examineResponse) : ""));
                return response;
            }
            try
            {
                response.Code = ResponseCodeDefines.SuccessCode;
                if (examineResponse.ExamineStatus == ExamineStatus.Examined)
                {
                    await _humanInfoAdjustmentManager.UpdateExamineStatus(examineResponse.SubmitDefineId, ExamineStatusEnum.Approved);
                }
                else if (examineResponse.ExamineStatus == ExamineStatus.Reject)
                {
                    await _humanInfoAdjustmentManager.UpdateExamineStatus(examineResponse.SubmitDefineId, ExamineStatusEnum.Reject);
                }
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Trace($"新增人事信息调动回调(HumanInfoAdjustmentCreateCallback)报错：\r\n{e.ToString()}，\r\n请求参数为：\r\n" + (examineResponse != null ? JsonHelper.ToJson(examineResponse) : ""));
            }
            return response;
        }



        /// <summary>
        /// 新增人事信息调动步骤回调
        /// </summary>
        /// <param name="examineResponse"></param>
        /// <returns></returns>
        [HttpPost("humanadjustmentstepcallback")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage> HumanInfoAdjustmentStepCallback([FromBody] ExamineStepResponse examineStepResponse)
        {
            Logger.Trace($"新增人事信息调动步骤回调(HumanInfoAdjustmentStepCallback)：\r\n请求参数为：\r\n" + (examineStepResponse != null ? JsonHelper.ToJson(examineStepResponse) : ""));

            ResponseMessage response = new ResponseMessage();
            return response;
        }





    }
}
