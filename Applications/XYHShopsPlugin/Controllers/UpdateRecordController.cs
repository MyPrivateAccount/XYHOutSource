using ApplicationCore;
using ApplicationCore.Dto;
using ApplicationCore.Filters;
using AspNet.Security.OAuth.Validation;
using GatewayInterface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XYH.Core.Log;
using XYHShopsPlugin.Dto;
using XYHShopsPlugin.Managers;

namespace XYHShopsPlugin.Controllers
{
    [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/updaterecord")]
    public class UpdateRecordController : Controller
    {
        private readonly ILogger Logger = LoggerManager.GetLogger("UpdateRecord");

        private readonly UpdateRecordManager _updateRecordManager;

        public UpdateRecordController(UpdateRecordManager updateRecordManager)
        {
            _updateRecordManager = updateRecordManager ?? throw new ArgumentNullException(nameof(UpdateRecordManager));
        }


        /// <summary>
        /// 获取房源动态
        /// </summary>
        /// <param name="user"></param>
        /// <param name="updateId"></param>
        /// <returns></returns>
        [HttpGet("{updateId}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<UpdateRecordResponse>> GetUpdateRecord(UserInfo user, [FromRoute] string updateId)
        {
            ResponseMessage<UpdateRecordResponse> response = new ResponseMessage<UpdateRecordResponse>();
            if (string.IsNullOrEmpty(updateId))
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = "参数不能为空";
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})获取房源动态(GetUpdateRecord)报错：参数不能为空，\r\n请求参数为：\r\n(updateId){updateId ?? ""}");
            }
            try
            {
                response.Extension = await _updateRecordManager.FindByIdAsync(updateId, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})获取房源动态(GetUpdateRecord)报错：\r\n{e.ToString()}，\r\n请求参数为：\r\n(updateId){updateId ?? ""}");
            }
            return response;
        }

        /// <summary>
        /// 获取房源动态列表
        /// </summary>
        /// <param name="user"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpPost("list")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<PagingResponseMessage<UpdateRecordListResponse>> GetUpdateRecordList(UserInfo user, [FromBody]UpdateRecordListCondition condition)
        {
            PagingResponseMessage<UpdateRecordListResponse> response = new PagingResponseMessage<UpdateRecordListResponse>();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})获取房源动态列表(GetUpdateRecordList)报错：{response.Message ?? ""}，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
            }
            try
            {
                return await _updateRecordManager.GetListAsync(condition, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})获取房源动态列表(GetUpdateRecordList)报错：{e.ToString()}，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
            }
            return response;
        }

        [HttpPost("followlist")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<PagingResponseMessage<UpdateRecordListResponse>> GetUpdateRecordFollowList(UserInfo user, [FromBody]UpdateRecordFollowListCondition condition)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})新增房源动态列表(GetUpdateRecordFollowList)：\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));

            PagingResponseMessage<UpdateRecordListResponse> response = new PagingResponseMessage<UpdateRecordListResponse>();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})新增房源动态列表(GetUpdateRecordFollowList)模型验证失败：{response.Message ?? ""}，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
                return response;
            }
            try
            {
                return await _updateRecordManager.GetFollowListAsync(user.Id, condition, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})新增房源动态列表(GetUpdateRecordFollowList)报错：{response.Message ?? ""}，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
            }
            return response;
        }

        [HttpPut("{updateId}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage> PutShops(UserInfo user, [FromRoute] string updateId, [FromBody] UpdateRecordRequest updateRecordRequest)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})修改房源动态列表(PutShops)：\r\n请求参数为：\r\n" + (updateRecordRequest != null ? JsonHelper.ToJson(updateRecordRequest) : ""));

            ResponseMessage response = new ResponseMessage();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})修改房源动态列表(PutShops)模型验证失败：{response.Message ?? ""}，\r\n请求参数为：\r\n" + (updateRecordRequest != null ? JsonHelper.ToJson(updateRecordRequest) : ""));
                return response;
            }
            try
            {
                await _updateRecordManager.UpdateAsync(updateId, updateRecordRequest, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})修改房源动态列表(PutShops)错误：{response.Message ?? ""}，\r\n请求参数为：\r\n" + (updateRecordRequest != null ? JsonHelper.ToJson(updateRecordRequest) : ""));
            }
            return response;
        }

        /// <summary>
        /// 热卖、加推、报备规则、佣金方案、楼栋批次、优惠政策审核提交接口
        /// </summary>
        /// <param name="User"></param>
        /// <param name="shopsId"></param>
        /// <returns></returns>
        [HttpPost("submit")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "ShopsUpdate" })]
        public async Task<ResponseMessage> UpdateRecordSubmit(UserInfo user, [FromBody] UpdateRecordRequest updateRecordRequest)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})热卖、加推、报备规则、佣金方案、楼栋批次、优惠政策审核提交接口(UpdateRecordSubmit)请求参数为：\r\n" + (updateRecordRequest != null ? JsonHelper.ToJson(updateRecordRequest) : ""));

            ResponseMessage response = new ResponseMessage();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                return response;
            }
            try
            {
                GatewayInterface.Dto.ExamineSubmitRequest examineSubmitRequest = new GatewayInterface.Dto.ExamineSubmitRequest();
                examineSubmitRequest.ContentId = updateRecordRequest.ContentId;
                examineSubmitRequest.ContentType = updateRecordRequest.ContentType;
                examineSubmitRequest.ContentName = updateRecordRequest.ContentName;
                examineSubmitRequest.SubmitDefineId = updateRecordRequest.Id;
                examineSubmitRequest.Source = "";
                examineSubmitRequest.CallbackUrl = ApplicationContext.Current.UpdateExamineCallbackUrl;
                examineSubmitRequest.Action = updateRecordRequest.ContentType;
                examineSubmitRequest.TaskName = $"{user.UserName}提交楼盘{updateRecordRequest.ContentName}的动态{updateRecordRequest.ContentType}";
                examineSubmitRequest.Ext1 = updateRecordRequest.Ext1;
                examineSubmitRequest.Ext2 = updateRecordRequest.Ext2;
                GatewayInterface.Dto.UserInfo userInfo = new GatewayInterface.Dto.UserInfo()
                {
                    Id = user.Id,
                    KeyWord = user.KeyWord,
                    OrganizationId = user.OrganizationId,
                    OrganizationName = user.OrganizationName,
                    UserName = user.UserName
                };
                Logger.Info($"用户{user?.UserName ?? ""}({user?.Id ?? ""})房源动态提交审核(UpdateRecordSubmit)请求参数为：\r\n" + (examineSubmitRequest != null ? JsonHelper.ToJson(examineSubmitRequest) : ""));

                var _examineInterface = ApplicationContext.Current.Provider.GetRequiredService<IExamineInterface>();
                var response2 = await _examineInterface.Submit(userInfo, examineSubmitRequest);
                if (response2.Code != ResponseCodeDefines.SuccessCode)
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Message = "向审核中心发起审核请求失败：" + response2.Message;
                    Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})房源动态提交审核(UpdateRecordSubmit)报错：\r\n{response.Message ?? ""},\r\n请求参数为：\r\n" + (examineSubmitRequest != null ? JsonHelper.ToJson(examineSubmitRequest) : ""));
                    return response;
                }
                await _updateRecordManager.CreateUpdateRecordAsync(user, updateRecordRequest, Models.ExamineStatusEnum.Auditing, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})房源动态提交审核(UpdateRecordSubmit)报错：\r\n{e.ToString()},\r\n请求参数为：\r\n" + (updateRecordRequest != null ? JsonHelper.ToJson(updateRecordRequest) : ""));

            }
            return response;
        }

        /// <summary>
        /// 加推、报备规则、佣金方案、楼栋批次、优惠政策审核回调接口
        /// </summary>
        /// <param name="User"></param>
        /// <param name="shopsId"></param>
        /// <returns></returns>
        [HttpPost("submitcallback")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage> UpdateRecordSubmitCallback([FromBody] ExamineResponse examineResponse)
        {
            Logger.Warn($" 加推、报备规则、佣金方案、楼栋批次、优惠政策审核回调接口(UpdateRecordSubmitCallback)：\r\n请求参数为：\r\n" + (examineResponse != null ? JsonHelper.ToJson(examineResponse) : ""));

            ResponseMessage response = new ResponseMessage();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Warn($"房源动态审核回调(UpdateRecordSubmitCallback)报错：\r\n{response.Message ?? ""},\r\n请求参数为：\r\n" + (examineResponse != null ? JsonHelper.ToJson(examineResponse) : ""));

                return response;
            }
            try
            {
                await _updateRecordManager.UpdateRecordSubmitCallback(examineResponse);

            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"房源动态审核回调(UpdateRecordSubmitCallback)报错：\r\n{response.Message ?? ""},\r\n请求参数为：\r\n" + (examineResponse != null ? JsonHelper.ToJson(examineResponse) : ""));
            }
            return response;
        }




        [HttpDelete("{updateId}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "ShopsDelete" })]
        public async Task<ResponseMessage> DeleteShops(UserInfo User, [FromRoute] string updateId)
        {
            ResponseMessage response = new ResponseMessage();
            try
            {
                await _updateRecordManager.DeleteAsync(User.Id, updateId, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
            }
            return response;
        }



    }
}
