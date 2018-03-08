using ApplicationCore;
using ApplicationCore.Dto;
using ApplicationCore.Filters;
using AspNet.Security.OAuth.Validation;
using ExamineCenterPlugin.Dto;
using ExamineCenterPlugin.Managers;
using ExamineCenterPlugin.Models;
using GatewayInterface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Threading.Tasks;
using XYH.Core.Log;

namespace ExamineCenterPlugin.Controllers
{
    /// <summary>
    /// 审核流程相关
    /// </summary>
    [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/examines")]
    public class ExamineFlowController : Controller
    {

        private readonly ExamineFlowManager _examineFlowManager;
        private readonly RestClient _restClient;
        private readonly ILogger Logger = LoggerManager.GetLogger("ExamineFlowController");
        private readonly ILogger MessageLogger = LoggerManager.GetLogger("SendMessageServerError");

        public ExamineFlowController(ExamineFlowManager examineFlowManager, RestClient restClient)
        {
            _examineFlowManager = examineFlowManager ?? throw new ArgumentNullException(nameof(examineFlowManager));
            _restClient = restClient ?? throw new ArgumentNullException(nameof(restClient));
        }

        /// <summary>
        /// 提交审核
        /// </summary>
        /// <param name="user"></param>
        /// <param name="examineSubmitRequest"></param>
        /// <returns></returns>
        [HttpPost("submit")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage> Submit(UserInfo user, [FromBody]ExamineSubmitRequest examineSubmitRequest)
        {
            ResponseMessage response = new ResponseMessage();
            try
            {
                Logger.Trace("收到提交审核请求(Submit)，请求体为：\r\n" + (examineSubmitRequest != null ? JsonHelper.ToJson(examineSubmitRequest) : ""));
                var taskGuid = Guid.NewGuid().ToString();//生成一个任务guid
                var flow = await _examineFlowManager.GetExamineFlow(examineSubmitRequest.ContentId, examineSubmitRequest.Action);
                if (flow != null)
                {
                    taskGuid = flow.TaskGuid;
                    Logger.Info("收到提交审核请求(Submit)，已经存在对应的已经在审核中的审核流程，跳过本地流程保存，请求体为：\r\n" + (examineSubmitRequest != null ? JsonHelper.ToJson(examineSubmitRequest) : ""));
                }
                else
                {
                    await _examineFlowManager.SaveExamineFlow(user, taskGuid, examineSubmitRequest);
                }
                NameValueCollection nameValueCollection = new NameValueCollection();
                nameValueCollection.Add("appToken", "app:nwf");
                var nwf = CreateNwf(user.Id, taskGuid, examineSubmitRequest);
                Logger.Info("向NWF提交请求审核协议：\r\n{0}", JsonHelper.ToJson(nwf));
                string result = await _restClient.Post(ApplicationContext.Current.NWFUrl, nwf, "POST", nameValueCollection);
                Logger.Info("返回：\r\n{0}", result);
                var response2 = JsonHelper.ToObject<ResponseMessage>(result);
                if (response2.Code != "0")
                {
                    Logger.Error($"向NWF({ApplicationContext.Current.NWFUrl})发送审核流程请求失败,请求体为：\r\n{JsonHelper.ToJson(nwf)}");
                    return response2;
                }
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})向审核中心提交审核(Submit)报错：\r\n{e.ToString()}，\r\n请求体为：\r\n" + (examineSubmitRequest != null ? JsonHelper.ToJson(examineSubmitRequest) : ""));
            }
            return response;
        }


        /// <summary>
        /// 根据ID获取单条审核内容
        /// </summary>
        /// <returns></returns>
        [HttpGet("{flowid}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<ExamineFlowResponse>> GetExamineFlowRecordById(UserInfo user, [FromRoute]string flowid)
        {
            ResponseMessage<ExamineFlowResponse> response = new ResponseMessage<ExamineFlowResponse>();
            try
            {
                response.Extension = await _examineFlowManager.FindExamineFlowById(flowid);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})根据ID获取单条审核内容(GetExamineFlowRecordById)报错：\r\n{e.ToString()}，\r\n请求id为：\r\n{flowid ?? ""}");
            }
            return response;
        }

        /// <summary>
        /// 查询审核流程，一般examinaction与contenttype一致
        /// </summary>
        /// <param name="user">不传</param>
        /// <param name="contenttype"></param>
        /// <param name="contentid"></param>
        /// <param name="examinaction"></param>
        /// <returns></returns>
        [HttpPost("search")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<PagingResponseMessage<ExamineFlow>> GetExamineFlows(UserInfo user, [FromBody]ExamineFlowSearchCondition conditon)
        {
            PagingResponseMessage<ExamineFlow> response = new PagingResponseMessage<ExamineFlow>();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})查询审核流程(GetExamineFlows)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (conditon != null ? JsonHelper.ToJson(conditon) : ""));
                return response;
            }
            try
            {
                return await _examineFlowManager.Search(conditon, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})查询审核流程(GetExamineFlows)报错：\r\n{e.ToString()},\r\n请求参数为：\r\n" + conditon != null ? JsonHelper.ToJson(conditon) : "");
            }
            return response;
        }


        /// <summary>
        /// 获取一个审核资源所有审核项目的当前审核状态
        /// </summary>
        /// <param name="user"></param>
        /// <param name="contentid"></param>
        /// <returns></returns>
        [HttpGet("status/{contentid}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<List<ExamineStatusListResponse>>> GetExamineStatus(UserInfo user, [FromRoute] string contentid)
        {
            ResponseMessage<List<ExamineStatusListResponse>> response = new ResponseMessage<List<ExamineStatusListResponse>>();
            try
            {
                response.Extension = await _examineFlowManager.GetCurrentExamineStatus(contentid, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})获取一个审核资源所有审核项目状态(GetExamineStatus)报错：\r\n{e.ToString()},\r\n请求contentid：\r\n{contentid ?? ""}");
            }
            return response;
        }




        /// <summary>
        /// 获取用户待审核数量
        /// </summary>
        /// <returns></returns>
        [HttpGet("{userId}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<int>> GetExamineCount(UserInfo user, [FromRoute]string userId)
        {
            ResponseMessage<int> response = new ResponseMessage<int>();
            try
            {
                response.Extension = await _examineFlowManager.GetExamineCount(userId);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})获取用户待审核数量(GetExamineCount)报错：\r\n{e.ToString()}，\r\n用户Id：{userId ?? ""}");
            }
            return response;
        }

        /// <summary>
        /// 获取用户待审核列表
        /// </summary>
        /// <param name="user"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpPost("waiting")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<PagingResponseMessage<ExamineFlowListResponse>> GetUserWaitingExamineFlowList(UserInfo user, [FromBody]UserWaitingExamineFlowListCondition condition)
        {
            PagingResponseMessage<ExamineFlowListResponse> pagingResponse = new PagingResponseMessage<ExamineFlowListResponse>();
            if (!ModelState.IsValid)
            {
                pagingResponse.Code = ResponseCodeDefines.ModelStateInvalid;
                pagingResponse.Message = ModelState.GetAllErrors();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})获取待审核列表(GetUserWaitingExamineFlowList)模型验证失败：\r\n{pagingResponse.Message}，请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
                return pagingResponse;
            }
            try
            {
                return await _examineFlowManager.GetUserWaitingExamineFlowList(user.Id, condition, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                pagingResponse.Code = ResponseCodeDefines.ServiceError;
                pagingResponse.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})获取待审核列表(GetUserWaitingExamineFlowList)报错：\r\n{e.ToString()}，请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
            }
            return pagingResponse;
        }

        /// <summary>
        /// 根据审核状态获取用户发起的审核列表
        /// </summary>
        /// <param name="user"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpPost("submitlist")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<PagingResponseMessage<ExamineFlowListResponse>> GetUserSubmitExamineFlowList(UserInfo user, [FromBody]UserExamineFlowListCondition condition)
        {
            PagingResponseMessage<ExamineFlowListResponse> pagingResponse = new PagingResponseMessage<ExamineFlowListResponse>();
            try
            {
                return await _examineFlowManager.GetUserSubmitExamineFlowList(user.Id, condition, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                pagingResponse.Code = ResponseCodeDefines.ServiceError;
                pagingResponse.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})获取发起的审核列表(GetUserSubmitExamineFlowList)报错：\r\n{e.ToString()}，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
            }
            return pagingResponse;
        }

        /// <summary>
        /// 根据审核状态获取用户参与的审核列表
        /// </summary>
        /// <param name="user"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpPost("list")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<PagingResponseMessage<ExamineFlowListResponse>> GetUserExamineFlowList(UserInfo user, [FromBody]UserExamineFlowListCondition condition)
        {
            PagingResponseMessage<ExamineFlowListResponse> pagingResponse = new PagingResponseMessage<ExamineFlowListResponse>();
            try
            {
                return await _examineFlowManager.GetUserExamineFlowList(user.Id, condition, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                pagingResponse.Code = ResponseCodeDefines.ServiceError;
                pagingResponse.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})获取参与的审核列表(GetUserExamineFlowList)报错：\r\n{e.ToString()}，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
            }
            return pagingResponse;
        }


        /// <summary>
        /// 通过审核
        /// </summary>
        /// <param name="user"></param>
        /// <param name="recordId"></param>
        /// <param name="desc"></param>
        /// <returns></returns>
        [HttpPut("pass/{recordId}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage> ExaminePass(UserInfo user, [FromRoute]string recordId, [FromBody]string desc)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})发起通过审核请求(ExaminePass)，请求体为：\r\n(recordId){recordId ?? ""},(desc){desc ?? ""}");
            ResponseMessage response = new ResponseMessage();
            if (string.IsNullOrEmpty(recordId))
            {
                response.Code = ResponseCodeDefines.ArgumentNullError;
                return response;
            }
            try
            {
                var record = await _examineFlowManager.FindExamineRecordById(recordId, HttpContext.RequestAborted);
                if (record?.ExamineFlow == null)
                {
                    response.Code = ResponseCodeDefines.NotFound;
                    Logger.Info($"用户{user?.UserName ?? ""}({user?.Id ?? ""})发起通过审核请求(ExaminePass)失败：未找到相应的审核记录:\r\n{response.Message}，\r\n请求参数为：\r\n(recordId){recordId ?? ""},(desc){desc ?? ""}");
                    return response;
                }
                NameValueCollection nameValueCollection = new NameValueCollection();
                nameValueCollection.Add("appToken", "app:nwf");
                var taskCallback = new TaskCallback
                {
                    Message = "",
                    TaskGuid = record.ExamineFlow.TaskGuid,
                    StepID = record.ExamineFlow.CurrentStepId,
                    CallbackProtocol = new FlowProtocol { ProtocolType = "", Protocol = "true" },
                    Status = TaskStatusEnum.Finished
                };
                Logger.Info($"用户{user?.UserName ?? ""}({user?.Id ?? ""})向NWF发起通过审核请求协议：\r\n{0}", JsonHelper.ToJson(taskCallback));
                string response2 = await _restClient.Post(ApplicationContext.Current.NWFExamineCallbackUrl, taskCallback, "POST", nameValueCollection);
                Logger.Info($"用户{user?.UserName ?? ""}({user?.Id ?? ""})向NWF发起通过审核请求返回：\r\n{0}", response2);

                var nwfresponse = JsonHelper.ToObject<ResponseMessage>(response2);
                if (nwfresponse.Code != "0")
                {
                    Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})像NWF发起通过审核请求(ExaminePass)失败：NWF返回失败:\r\n{nwfresponse.Message}，\r\n请求参数为：\r\n(recordId){recordId ?? ""},(desc){desc ?? ""}");
                    return nwfresponse;
                }
                var response1 = await _examineFlowManager.ExaminePass(user.Id, recordId, desc);
                if (response1.Code != "0")
                {
                    Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})发起通过审核请求(ExaminePass)失败：通过记录保存失败:\r\n{response1.Message}，\r\n请求参数为：\r\n(recordId){recordId ?? ""},(desc){desc ?? ""}");
                    return response1;
                }
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})发起通过审核请求(ExaminePass)报错：\r\n{e.ToString()}，\r\n请求参数为：\r\n(recordId){recordId ?? ""},(desc){desc ?? ""}");
            }
            return response;
        }


        /// <summary>
        /// 驳回审核
        /// </summary>
        /// <param name="user"></param>
        /// <param name="recordId"></param>
        /// <param name="desc"></param>
        /// <returns></returns>
        [HttpPut("reject/{recordId}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage> ExamineReject(UserInfo user, [FromRoute]string recordId, [FromBody]string desc)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})发起审核驳回请求(ExamineReject)，请求体为：\r\n(recordId){recordId ?? ""},(desc){desc ?? ""}");
            ResponseMessage response = new ResponseMessage();
            if (string.IsNullOrEmpty(recordId))
            {
                response.Code = ResponseCodeDefines.ArgumentNullError;
                return response;
            }
            try
            {
                var record = await _examineFlowManager.FindExamineRecordById(recordId);
                if (record == null)
                {
                    response.Code = ResponseCodeDefines.NotFound;
                    response.Message = "未找到审核流程";
                    Logger.Info($"用户{user?.UserName ?? ""}({user?.Id ?? ""})发起审核驳回请求(ExamineReject)失败：\r\n未找到相应的审核记录{response.Message}，\r\n请求参数为：\r\n(recordId){recordId ?? ""},(desc){desc ?? ""}");
                    return response;
                }
                GatewayInterface.Dto.ExamineResponse examineResponse = new GatewayInterface.Dto.ExamineResponse();
                examineResponse.ContentId = record.ExamineFlow.ContentId;
                examineResponse.ContentType = record.ExamineFlow.ContentType;
                examineResponse.FlowId = record.ExamineFlow.Id;
                examineResponse.Content = record.ExamineFlow.Content;
                examineResponse.Ext1 = record.ExamineFlow.Ext1;
                examineResponse.Ext2 = record.ExamineFlow.Ext2;
                examineResponse.Ext3 = record.ExamineFlow.Ext3;
                examineResponse.Ext4 = record.ExamineFlow.Ext4;
                examineResponse.Ext5 = record.ExamineFlow.Ext5;
                examineResponse.Ext6 = record.ExamineFlow.Ext6;
                examineResponse.Ext7 = record.ExamineFlow.Ext7;
                examineResponse.Ext8 = record.ExamineFlow.Ext8;
                examineResponse.SubmitDefineId = record.ExamineFlow.SubmitDefineId;
                examineResponse.ExamineStatus = GatewayInterface.Dto.ExamineStatus.Reject;

                var _shopsInterface = ApplicationContext.Current.Provider.GetRequiredService<IShopsInterface>();
                var _customerInterface = ApplicationContext.Current.Provider.GetRequiredService<ICustomerInterface>();
                var response3 = new GatewayInterface.Dto.ResponseMessage();
                if (record.ExamineFlow.ContentType == "building")
                {
                    response3 = await _shopsInterface.SubmitBuildingCallback(examineResponse);
                }
                else if (record.ExamineFlow.ContentType == "shops")
                {
                    response3 = await _shopsInterface.SubmitShopsCallback(examineResponse);
                }
                else if (record.ExamineFlow.ContentType == "TransferCustomer")
                {
                    response3 = await _customerInterface.TransferCallback(examineResponse);
                }
                else if (record.ExamineFlow.ContentType == "BuildingsOnSite")
                {
                    response3 = await _shopsInterface.BuildingsOnSiteCallback(examineResponse);
                }
                else if (record.ExamineFlow.ContentType == "CustomerDeal")
                {
                    response3 = await _customerInterface.CustomerDealCallback(examineResponse);
                }
                else
                {
                    response3 = await _shopsInterface.UpdateRecordSubmitCallback(examineResponse);
                }
                //var response3 = JsonHelper.ToObject<ResponseMessage>(result);
                if (response3.Code != ResponseCodeDefines.SuccessCode)
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Message = "回调客户端返回错误：" + response3.Message;
                    Logger.Info($"用户{user?.UserName ?? ""}({user?.Id ?? ""})发起审核驳回请求(ExamineReject)失败：\r\n回调相关应用失败{response3.Message}，\r\n请求参数为：\r\n(recordId){recordId ?? ""},(desc){desc ?? ""}");
                    return response;
                }

                NameValueCollection nameValueCollection = new NameValueCollection();
                nameValueCollection.Add("appToken", "app:nwf");
                var taskCallback = new TaskCallback
                {
                    Message = "",
                    TaskGuid = record.ExamineFlow.TaskGuid,
                    StepID = record.ExamineFlow.CurrentStepId,
                    CallbackProtocol = new FlowProtocol { ProtocolType = "", Protocol = "false" },
                    Status = TaskStatusEnum.Finished
                };
                Logger.Info($"用户{user?.UserName ?? ""}({user?.Id ?? ""})审核驳回回调nwf协议：\r\n{0}", JsonHelper.ToJson(taskCallback));
                string response4 = await _restClient.Post(ApplicationContext.Current.NWFExamineCallbackUrl, taskCallback, "POST", nameValueCollection);
                Logger.Info($"用户{user?.UserName ?? ""}({user?.Id ?? ""})审核驳回回调nwf返回：\r\n{0}", response4);

                var nwfresponse = JsonHelper.ToObject<ResponseMessage>(response4);
                if (nwfresponse.Code != "0")
                {
                    Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})像NWF发起驳回审核请求(ExaminePass)失败：NWF返回失败:\r\n{nwfresponse.Message}，\r\n请求参数为：\r\n(recordId){recordId ?? ""},(desc){desc ?? ""}");
                    return nwfresponse;
                }
                var response2 = await _examineFlowManager.ExamineReject(user.Id, recordId, desc);
                if (response2.Code != ResponseCodeDefines.SuccessCode)
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Message = "审核中心处理出错";
                    Logger.Info($"用户{user?.UserName ?? ""}({user?.Id ?? ""})发起审核驳回请求(ExamineReject)失败：\r\n未找到相应的审核记录{response2.Message}，\r\n请求参数为：\r\n(recordId){recordId ?? ""},(desc){desc ?? ""}");
                    return response;
                }

                //发送通知消息
                SendMessageRequest sendMessageRequest = new SendMessageRequest();
                sendMessageRequest.MessageTypeCode = "ExamineReject";
                MessageItem messageItem = new MessageItem();
                messageItem.UserIds = new List<string> { record.ExamineFlow.SubmitUserId };
                messageItem.MessageTypeItems = new List<TypeItem> {
                    new TypeItem{ Key="NOTICETYPE",Value=ExamineContentTypeConvert.GetContentTypeString(record.ExamineFlow.ContentType) },
                    new TypeItem { Key="NAME",Value=record.ExamineFlow.ContentName},
                    new TypeItem{ Key="TIME",Value=DateTime.Now.ToString("MM-dd hh:mm")}
                };
                sendMessageRequest.MessageList = new List<MessageItem> { messageItem };
                try
                {
                    MessageLogger.Info("发送通知消息协议：\r\n{0}", JsonHelper.ToJson(sendMessageRequest));
                    _restClient.Post(ApplicationContext.Current.MessageServerUrl, sendMessageRequest, "POST", new NameValueCollection());
                }
                catch (Exception e)
                {
                    MessageLogger.Error("发送通知消息出错：\r\n{0}", e.ToString());
                }
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})发起审核驳回请求(ExamineReject)报错：\r\n{e.ToString()}，\r\n请求参数为：\r\n(recordId){recordId ?? ""},(desc){desc ?? ""}");
            }
            return response;
        }


        /// <summary>
        /// 审核步骤中回调(内部使用)
        /// </summary>
        /// <param name="examineCallbackRequest"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("stepcallback")]
        public async Task<ResponseMessage> ExamineCallback([FromBody]ExamineCallbackRequest examineCallbackRequest)
        {
            Logger.Trace("收到审核步骤回调请求(ExamineCallback)api/examines/stepcallback，请求体为：\r\n" + (examineCallbackRequest != null ? JsonHelper.ToJson(examineCallbackRequest) : ""));
            ResponseMessage response = new ResponseMessage();
            if (examineCallbackRequest == null)
            {
                response.Code = ResponseCodeDefines.ArgumentNullError;
                Logger.Error($"审核步骤中回调(ExamineCallback)失败：请求参数ExamineCallbackRequest为空");
                return response;
            }
            try
            {
                return await _examineFlowManager.StepCallback(examineCallbackRequest);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"审核步骤中回调(ExamineCallback)报错：\r\n{e.ToString()}，\r\n请求参数为：\r\n" + (examineCallbackRequest != null ? JsonHelper.ToJson(examineCallbackRequest) : ""));
                return response;
            }
        }


        /// <summary>
        /// 审核流程回调(内部使用)
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("flowcallback")]
        public async Task<ResponseMessage> ExamineTaskCallback([FromBody]ExamineCallbackRequest examineCallbackRequest)
        {
            Logger.Trace("收到审核流程回调请求(ExamineTaskCallback)api/examines/flowcallback,请求参数为：\r\n" + (examineCallbackRequest != null ? JsonHelper.ToJson(examineCallbackRequest) : ""));
            ResponseMessage response = new ResponseMessage();
            if (examineCallbackRequest == null)
            {
                response.Code = ResponseCodeDefines.ArgumentNullError;
                response.Message = "参数为空";
                Logger.Error($"审核流程回调(ExamineTaskCallback)失败：请求参数ExamineCallbackRequest为空");
                return response;
            }
            try
            {
                var flow = await _examineFlowManager.FindExamineFlowByTaskGuid(examineCallbackRequest.TaskGuid);
                if (flow == null)
                {
                    response.Code = ResponseCodeDefines.NotFound;
                    response.Message = "未找到审核流程";
                    Logger.Error($"审核流程回调(ExamineTaskCallback)失败：\r\n未找到相应审核流程，\r\n请求参数为：\r\n" + (examineCallbackRequest != null ? JsonHelper.ToJson(examineCallbackRequest) : ""));
                    return response;
                }

                GatewayInterface.Dto.ExamineResponse examineResponse = new GatewayInterface.Dto.ExamineResponse();
                examineResponse.ContentId = flow.ContentId;
                examineResponse.FlowId = flow.Id;
                examineResponse.ContentType = flow.ContentType;
                examineResponse.Content = flow.Content;
                examineResponse.Ext1 = flow.Ext1;
                examineResponse.Ext2 = flow.Ext2;
                examineResponse.Ext3 = flow.Ext3;
                examineResponse.Ext4 = flow.Ext4;
                examineResponse.Ext5 = flow.Ext5;
                examineResponse.Ext6 = flow.Ext6;
                examineResponse.Ext7 = flow.Ext7;
                examineResponse.Ext8 = flow.Ext8;
                examineResponse.SubmitDefineId = flow.SubmitDefineId;
                examineResponse.ExamineStatus = GatewayInterface.Dto.ExamineStatus.Examined;

                var _shopsInterface = ApplicationContext.Current.Provider.GetRequiredService<IShopsInterface>();
                var _customerInterface = ApplicationContext.Current.Provider.GetRequiredService<ICustomerInterface>();
                var _contractInterface = ApplicationContext.Current.Provider.GetRequiredService<I>();
                var response3 = new GatewayInterface.Dto.ResponseMessage();

                if (flow.ContentType == "building")
                {
                    response3 = await _shopsInterface.SubmitBuildingCallback(examineResponse);
                }
                else if (flow.ContentType == "shops")
                {
                    response3 = await _shopsInterface.SubmitShopsCallback(examineResponse);
                }
                else if (flow.ContentType == "TransferCustomer")
                {
                    response3 = await _customerInterface.TransferCallback(examineResponse);
                }
                else if (flow.ContentType == "BuildingsOnSite")
                {
                    response3 = await _shopsInterface.BuildingsOnSiteCallback(examineResponse);
                }
                else if (flow.ContentType == "CustomerDeal")
                {
                    response3 = await _customerInterface.CustomerDealCallback(examineResponse);
                }
                else if (flow.ContentType == "ContractCommit")
                {
                    response3 = await _customerInterface.CustomerDealCallback(examineResponse);
                }
                else
                {
                    response3 = await _shopsInterface.UpdateRecordSubmitCallback(examineResponse);
                }
                if (response3.Code != ResponseCodeDefines.SuccessCode)
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Message = "回调客户端返回错误：" + response3.Message;
                    Logger.Error($"审核流程回调(ExamineTaskCallback)失败：\r\n审核中心回调相应的应用失败{response3.Message}，\r\n请求参数为：\r\n" + (examineCallbackRequest != null ? JsonHelper.ToJson(examineCallbackRequest) : ""));
                    return response;
                }
                var response2 = await _examineFlowManager.FlowCallback(examineCallbackRequest);
                if (response2.Code != ResponseCodeDefines.SuccessCode)
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Message = response2.Message;
                    Logger.Error($"审核流程回调(ExamineTaskCallback)失败：\r\n审核中心保存失败{response2.Message}，\r\n请求参数为：\r\n" + (examineCallbackRequest != null ? JsonHelper.ToJson(examineCallbackRequest) : ""));
                    return response;
                }
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"审核流程回调(ExamineTaskCallback)失败：\r\n{e.ToString()}，\r\n请求参数为：\r\n" + (examineCallbackRequest != null ? JsonHelper.ToJson(examineCallbackRequest) : ""));
                return response;
            }
            return response;
        }

        private NWF CreateNwf(string userId, string taskGuid, ExamineSubmitRequest examineSubmitRequest)
        {
            NWF nwf = new NWF();
            var bodyinfo = new BodyInfoType();
            var header = new HeaderType();
            bodyinfo.FileInfo = new List<FileInfoType>();

            nwf.BodyInfo = bodyinfo;
            nwf.Header = header;

            header.TaskGuid = taskGuid;
            header.ContentGuid = examineSubmitRequest.ContentId;
            header.Action = examineSubmitRequest.Action;
            header.SourceSystem = examineSubmitRequest.Source;
            header.ExtraAttribute = new List<AttributeType>();

            bodyinfo.Priority = 0;
            bodyinfo.TaskName = examineSubmitRequest.TaskName;
            if (String.IsNullOrEmpty(bodyinfo.TaskName))
            {
                bodyinfo.TaskName = $"{examineSubmitRequest.ContentName}-{userId}";
            }
            var extra = new List<AttributeType>();
            extra.Add(new AttributeType { Name = "ContentType", Value = examineSubmitRequest.ContentType });
            extra.Add(new AttributeType { Name = "Desc", Value = examineSubmitRequest.Desc });
            extra.Add(new AttributeType { Name = "ContentName", Value = examineSubmitRequest.ContentName });
            bodyinfo.ExtraAttribute = extra;
            return nwf;
        }


    }
}
