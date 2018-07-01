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
    [Route("api/humanchange")]
    public class HumanInfoChangeController : Controller
    {
        private readonly HumanInfoChangeManager _humanInfoChangeManager;
        private readonly ILogger Logger = LoggerManager.GetLogger("HumanInfoChangeController");

        public HumanInfoChangeController(HumanInfoChangeManager humanInfoChangeManager)
        {
            _humanInfoChangeManager = humanInfoChangeManager;
        }


        /// <summary>
        /// 根据Id获取人事变动信息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<HumanInfoChangeResponse>> GetHumanInfoChange(UserInfo user, [FromRoute] string id)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})根据Id获取人事变动信息(GetHumanInfoChange)，请求体为：\r\nid:{id ?? ""}");
            ResponseMessage<HumanInfoChangeResponse> response = new ResponseMessage<HumanInfoChangeResponse>();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})根据Id获取人事变动信息(GetHumanInfoChange)模型验证失败：{response.Message}请求体为：\r\nid:{id ?? ""}");
                return response;
            }
            try
            {
                response.Extension = await _humanInfoChangeManager.FindByIdAsync(id, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.Message;
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})根据Id获取人事变动信息(GetHumanInfoChange)失败：{e.ToString()}请求体为：\r\nid:{id ?? ""}");
            }
            return response;
        }

        /// <summary>
        /// 新增人事变动信息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="humanInfoAdjustmentRequest"></param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<HumanInfoChangeResponse>> CreateHumanInfoChange(UserInfo user, [FromBody] HumanInfoChangeRequest humanInfoChangeRequest)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})新增人事变动信息(CreateHumanInfoChange)，请求体为：\r\n" + (humanInfoChangeRequest != null ? JsonHelper.ToJson(humanInfoChangeRequest) : ""));
            ResponseMessage<HumanInfoChangeResponse> response = new ResponseMessage<HumanInfoChangeResponse>();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})新增人事变动信息(CreateHumanInfoChange)模型验证失败：{response.Message}请求体为：\r\n" + (humanInfoChangeRequest != null ? JsonHelper.ToJson(humanInfoChangeRequest) : ""));
                return response;
            }
            try
            {
                response.Extension = await _humanInfoChangeManager.CreateAsync(user, humanInfoChangeRequest, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.Message;
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})新增人事变动信息(CreateHumanInfoChange)失败：{response.Message}请求体为：\r\n" + (humanInfoChangeRequest != null ? JsonHelper.ToJson(humanInfoChangeRequest) : ""));
            }
            return response;
        }

        /// <summary>
        /// 更新人事变动信息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="id"></param>
        /// <param name="humanInfoChangeRequest"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<HumanInfoChangeResponse>> PutHumanInfoChange(UserInfo user, [FromRoute]string id, [FromBody] HumanInfoChangeRequest humanInfoChangeRequest)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})更新人事变动信息(PutHumanInfoChange)，请求体为：\r\n" + (humanInfoChangeRequest != null ? JsonHelper.ToJson(humanInfoChangeRequest) : ""));
            ResponseMessage<HumanInfoChangeResponse> response = new ResponseMessage<HumanInfoChangeResponse>();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})更新人事变动信息(PutHumanInfoChange)模型验证失败：{response.Message}请求体为：\r\n" + (humanInfoChangeRequest != null ? JsonHelper.ToJson(humanInfoChangeRequest) : ""));
                return response;
            }
            try
            {
                response.Extension = await _humanInfoChangeManager.UpdateAsync(user, id, humanInfoChangeRequest, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.Message;
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})更新人事变动信息(PutHumanInfoChange)失败：{response.Message}请求体为：\r\n" + (humanInfoChangeRequest != null ? JsonHelper.ToJson(humanInfoChangeRequest) : ""));
            }
            return response;
        }

        /// <summary>
        /// 删除人事变动信息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage> DeleteHumanInfoChange(UserInfo user, [FromRoute]string id)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})删除人事变动信息(DeleteHumanInfoChange)，请求参数为：\r\n{id}");
            ResponseMessage response = new ResponseMessage();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})删除人事变动信息(DeleteHumanInfoChange)模型验证失败：{response.Message}请求体为：\r\n{id}");
                return response;
            }
            try
            {
                await _humanInfoChangeManager.DeleteAsync(user, id, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.Message;
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})删除人事变动信息(DeleteHumanInfoChange)失败：{response.Message}请求体为：\r\n{id}");
            }
            return response;
        }
    }
}
