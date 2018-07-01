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
                response.Extension = await _humanInfoRegularManager.FindByIdAsync(id, HttpContext.RequestAborted);
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
                response.Extension = await _humanInfoRegularManager.CreateAsync(user, humanInfoRegularRequest, HttpContext.RequestAborted);
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
                response.Extension = await _humanInfoRegularManager.UpdateAsync(user, id, humanInfoRegularRequest, HttpContext.RequestAborted);
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
                await _humanInfoRegularManager.DeleteAsync(user, id, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.Message;
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})删除人事转正信息(DeleteHumanInfoRegular)失败：{response.Message}请求体为：\r\n{id}");
            }
            return response;
        }
    }
}
