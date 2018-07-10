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
    [Route("api/humaninfoblack")]
    public class HumanInfoBlackController : Controller
    {
        private readonly ILogger Logger = LoggerManager.GetLogger("HumanInfoBlackController");
        private readonly HumanInfoBlackManager _humanInfoBlackManager;
        private readonly RestClient _restClient;

        public HumanInfoBlackController(RestClient restClient, HumanInfoBlackManager humanInfoBlackManager)
        {
            _restClient = restClient;
            _humanInfoBlackManager = humanInfoBlackManager;
        }

        /// <summary>
        /// 获取黑名单列表
        /// </summary>
        /// <param name="user"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpPost("list")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<PagingResponseMessage<HumanInfoBlackResponse>> Search(UserInfo user, [FromBody]HumanInfoBlackSearchCondition condition)
        {
            Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})查询黑名单(Search)，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
            var Response = new PagingResponseMessage<HumanInfoBlackResponse>();
            if (!ModelState.IsValid)
            {
                Response.Code = ResponseCodeDefines.ModelStateInvalid;
                Response.Message = ModelState.GetAllErrors();
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})查询黑名单(Search)模型验证失败：\r\n{Response.Message ?? ""}，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
                return Response;
            }
            try
            {
                return await _humanInfoBlackManager.SearchAsync(condition, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                Response.Code = ResponseCodeDefines.ServiceError;
                Response.Message = "服务器错误：" + e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})查询黑名单(Search)请求失败：\r\n{e.ToString() ?? ""}，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
            }
            return Response;
        }

        /// <summary>
        /// 新增黑名单，如果更新则传Id
        /// </summary>
        /// <param name="user"></param>
        /// <param name="humanInfoBlackRequest"></param>
        /// <returns></returns>
        [HttpPut]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<HumanInfoBlackResponse>> CreateHumanInfoBlack(UserInfo user, [FromBody]HumanInfoBlackRequest humanInfoBlackRequest)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})新增黑名单(CreateHumanInfoBlack)，\r\n请求参数为：\r\n" + (humanInfoBlackRequest != null ? JsonHelper.ToJson(humanInfoBlackRequest) : ""));
            var response = new ResponseMessage<HumanInfoBlackResponse>();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})新增黑名单(CreateHumanInfoBlack)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (humanInfoBlackRequest != null ? JsonHelper.ToJson(humanInfoBlackRequest) : ""));
                return response;
            }
            try
            {
                await _humanInfoBlackManager.SaveAsync(user, humanInfoBlackRequest, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.Message;
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})新增黑名单(CreateHumanInfoBlack)请求失败：\r\n{e.ToString() ?? ""}，\r\n请求参数为：\r\n" + (humanInfoBlackRequest != null ? JsonHelper.ToJson(humanInfoBlackRequest) : ""));
            }
            return response;
        }

        /// <summary>
        /// 删除黑名单用户
        /// </summary>
        /// <param name="user"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage> DeleteBlackInfo(UserInfo user, [FromRoute]string id)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})删除黑名单员工(DeleteBlackInfo)，\r\n请求参数为：Id:{id}");
            var pagingResponse = new ResponseMessage();
            if (!ModelState.IsValid)
            {
                pagingResponse.Code = ResponseCodeDefines.ModelStateInvalid;
                pagingResponse.Message = ModelState.GetAllErrors();
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})删除黑名单员工(DeleteBlackInfo)模型验证失败：\r\n{pagingResponse.Message ?? ""}，\r\n请求参数为：Id:{id}");
                return pagingResponse;
            }
            try
            {
                await _humanInfoBlackManager.DeleteAsync(user, id, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                pagingResponse.Code = ResponseCodeDefines.ServiceError;
                pagingResponse.Message = e.Message;
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})删除黑名单员工(DeleteBlackInfo)请求失败：\r\n{e.ToString() ?? ""}，\r\n请求参数为：Id:{id}");
            }
            return pagingResponse;
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
                    await _humanInfoBlackManager.UpdateExamineStatus(examineResponse.SubmitDefineId, ExamineStatusEnum.Approved);
                }
                else if (examineResponse.ExamineStatus == ExamineStatus.Reject)
                {
                    await _humanInfoBlackManager.UpdateExamineStatus(examineResponse.SubmitDefineId, ExamineStatusEnum.Reject);
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
