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
    [Route("api/positionsalary")]
    public class PositionSalaryController : Controller
    {
        private readonly PositionSalaryManager _positionSalaryManager;
        private readonly ILogger Logger = LoggerManager.GetLogger("PositionSalaryController");

        public PositionSalaryController(PositionSalaryManager positionSalaryManager)
        {
            _positionSalaryManager = positionSalaryManager;
        }

        /// <summary>
        /// 根据Id获取职位薪酬信息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<PositionSalaryResponse>> GetPositionSalary(UserInfo user, [FromRoute] string id)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})根据Id获取职位薪酬信息(GetPositionSalary)，请求体为：\r\nid:{id ?? ""}");
            ResponseMessage<PositionSalaryResponse> response = new ResponseMessage<PositionSalaryResponse>();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})根据Id获取职位薪酬信息(GetPositionSalary)模型验证失败：{response.Message}请求体为：\r\nid:{id ?? ""}");
                return response;
            }
            try
            {
                return await _positionSalaryManager.FindByIdAsync(user, id, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.Message;
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})根据Id获取职位薪酬信息(GetPositionSalary)失败：{e.ToString()}请求体为：\r\nid:{id ?? ""}");
            }
            return response;
        }


        /// <summary>
        /// 新增职位薪酬信息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="positionSalaryRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<PositionSalaryResponse>> CreatePositionSalary(UserInfo user, [FromBody] PositionSalaryRequest positionSalaryRequest)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})新增职位薪酬信息(CreatePositionSalary)，请求体为：\r\n" + (positionSalaryRequest != null ? JsonHelper.ToJson(positionSalaryRequest) : ""));
            ResponseMessage<PositionSalaryResponse> response = new ResponseMessage<PositionSalaryResponse>();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})新增职位薪酬信息(CreatePositionSalary)模型验证失败：{response.Message}请求体为：\r\n" + (positionSalaryRequest != null ? JsonHelper.ToJson(positionSalaryRequest) : ""));
                return response;
            }
            try
            {
                return await _positionSalaryManager.CreateAsync(user, positionSalaryRequest, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.Message;
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})新增职位薪酬信息(CreatePositionSalary)失败：{response.Message}请求体为：\r\n" + (positionSalaryRequest != null ? JsonHelper.ToJson(positionSalaryRequest) : ""));
            }
            return response;
        }

        /// <summary>
        /// 更新职位薪酬信息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="id"></param>
        /// <param name="positionSalaryRequest"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<PositionSalaryResponse>> PutPositionSalary(UserInfo user, [FromRoute]string id, [FromBody] PositionSalaryRequest positionSalaryRequest)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})更新职位薪酬信息(PutPositionSalary)，请求体为：\r\n" + (positionSalaryRequest != null ? JsonHelper.ToJson(positionSalaryRequest) : ""));
            ResponseMessage<PositionSalaryResponse> response = new ResponseMessage<PositionSalaryResponse>();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})更新职位薪酬信息(PutPositionSalary)模型验证失败：{response.Message}请求体为：\r\n" + (positionSalaryRequest != null ? JsonHelper.ToJson(positionSalaryRequest) : ""));
                return response;
            }
            try
            {
                return await _positionSalaryManager.UpdateAsync(user, id, positionSalaryRequest, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.Message;
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})更新职位薪酬信息(PutPositionSalary)失败：{response.Message}请求体为：\r\n" + (positionSalaryRequest != null ? JsonHelper.ToJson(positionSalaryRequest) : ""));
            }
            return response;
        }

        /// <summary>
        /// 删除职位薪酬信息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage> DeletePositionSalary(UserInfo user, [FromRoute]string id)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})删除职位薪酬信息(DeletePositionSalary)，请求参数为：\r\n{id}");
            ResponseMessage response = new ResponseMessage();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})删除职位薪酬信息(DeletePositionSalary)模型验证失败：{response.Message}请求体为：\r\n{id}");
                return response;
            }
            try
            {
                return await _positionSalaryManager.DeleteAsync(user, id, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.Message;
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})删除职位薪酬信息(DeletePositionSalary)失败：{response.Message}请求体为：\r\n{id}");
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
                    await _positionSalaryManager.UpdateExamineStatus(examineResponse.SubmitDefineId, ExamineStatusEnum.Approved);
                }
                else if (examineResponse.ExamineStatus == ExamineStatus.Reject)
                {
                    await _positionSalaryManager.UpdateExamineStatus(examineResponse.SubmitDefineId, ExamineStatusEnum.Reject);
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
