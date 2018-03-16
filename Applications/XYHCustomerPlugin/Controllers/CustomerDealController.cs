using ApplicationCore;
using ApplicationCore.Dto;
using ApplicationCore.Filters;
using ApplicationCore.Managers;
using AspNet.Security.OAuth.Validation;
using AutoMapper;
using GatewayInterface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XYH.Core.Log;
using XYHCustomerPlugin.Dto;
using XYHCustomerPlugin.Dto.Common;
using XYHCustomerPlugin.Dto.Request;
using XYHCustomerPlugin.Dto.Response;
using XYHCustomerPlugin.Managers;
using XYHCustomerPlugin.Models;

namespace XYHCustomerPlugin.Controllers
{
    [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/customerdeal")]
    public class CustomerDealController : Controller
    {
        #region 成员
        private readonly PermissionExpansionManager _permissionExpansionManager;
        private readonly FileInfoManager _fileInfoManager;
        private readonly FileScopeManager _fileScopeManager;
        private readonly CustomerDealManager _customerDealManager;
        private readonly RestClient _restClient;
        private readonly IMapper _mapper;
        private readonly CustomerInfoManager _customerInfoManager;
        private readonly ILogger Logger = LoggerManager.GetLogger("CustomerDeal");
        private readonly ILogger MessageLogger = LoggerManager.GetLogger("SendMessageServerError");

        #endregion

        /// <summary>
        /// 成交
        /// </summary>
        public CustomerDealController(FileInfoManager fileInfoManager,
             FileScopeManager fileScopeManager,
             CustomerDealManager customerDealManager,
             RestClient restClient,
             IMapper mapper,
             CustomerInfoManager customerInfoManager,
             PermissionExpansionManager permissionExpansionManager)
        {
            _permissionExpansionManager = permissionExpansionManager ?? throw new ArgumentNullException(nameof(permissionExpansionManager));
            _fileInfoManager = fileInfoManager;
            _fileScopeManager = fileScopeManager;
            _customerDealManager = customerDealManager;
            _restClient = restClient;
            _mapper = mapper;
            _customerInfoManager = customerInfoManager;
        }

        /// <summary>
        /// 根据成交信息Id查询信息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="dealid"></param>
        /// <returns></returns>
        [HttpGet("{dealid}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "CustomerDealById" })]
        public async Task<ResponseMessage<CustomerDealResponse>> GetCustomerDealById(UserInfo user, [FromRoute] string dealid)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})根据成交信息Id查询信息(GetCustomerDealById)：\r\n请求参数为：\r\n(dealid){dealid ?? ""}");

            var response = new ResponseMessage<CustomerDealResponse>();
            if (string.IsNullOrEmpty(dealid))
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                return response;
            }
            try
            {
                response.Extension = await _customerDealManager.FindByIdAsync(user.Id, dealid, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = "服务器错误:" + e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})根据成交信息Id查询信息(GetCustomerDealById)报错：\r\n{e.ToString()}，\r\n请求参数为：\r\n(dealid){dealid ?? ""}");
            }
            return response;
        }

        /// <summary>
        /// 根据成交信息Id查询信息（经理）
        /// </summary>
        /// <param name="user"></param>
        /// <param name="dealid"></param>
        /// <returns></returns>
        [HttpGet("manager/{dealid}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "CustomerDealById" })]
        public async Task<ResponseMessage<CustomerDealResponse>> GetCustomerDealByDealId(UserInfo user, [FromRoute] string dealid)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""}) 根据成交信息Id查询信息（经理）(GetCustomerDealByDealId)：\r\n请求参数为：\r\n(dealid){dealid ?? ""}");

            var response = new ResponseMessage<CustomerDealResponse>();
            if (string.IsNullOrEmpty(dealid))
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                return response;
            }
            try
            {
                if (await _permissionExpansionManager.HavePermission(user.Id, "CustomerDealRetrieve"))
                    response.Extension = await _customerDealManager.FindByIdManagerAsync(user, dealid, HttpContext.RequestAborted);
                else
                {
                    response.Code = ResponseCodeDefines.NotAllow;
                    response.Message = "权限不足";
                }
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = "服务器错误:" + e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""}) 根据成交信息Id查询信息（经理）(GetCustomerDealByDealId)报错：\r\n{e.ToString()}，\r\n请求参数为：\r\n(dealid){dealid ?? ""}");
            }
            return response;
        }

        /// <summary>
        /// 根据报备流程ID查询信息（驻场）
        /// </summary>
        /// <param name="user"></param>
        /// <param name="Transactionsid"></param>
        /// <returns></returns>
        [HttpGet("transaction/{Transactionsid}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "CustomerDealByTransactionsid" })]
        public async Task<ResponseMessage<CustomerDealResponse>> GetCustomerDealByTransactionsid(UserInfo user, [FromRoute] string Transactionsid)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})  根据报备流程ID查询信息（驻场）(GetCustomerDealByTransactionsid)：\r\n请求参数为：\r\n(Transactionsid){Transactionsid ?? ""}");

            var response = new ResponseMessage<CustomerDealResponse>();
            if (string.IsNullOrEmpty(Transactionsid))
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                return response;
            }
            try
            {
                response.Extension = await _customerDealManager.FindByTransactionsIdAsync(user.Id, Transactionsid, HttpContext.RequestAborted);
                if (response.Extension == null)
                {
                    response.Message = "尚未找到该成交信息";
                }
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = "服务器错误:" + e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})  根据报备流程ID查询信息（驻场）(GetCustomerDealByTransactionsid)报错：\r\n{e.ToString()}，\r\n请求参数为：\r\n(Transactionsid){Transactionsid ?? ""}");
            }
            return response;
        }

        /// <summary>
        /// 根据报备流程ID查询信息(业务员)
        /// </summary>
        /// <param name="user"></param>
        /// <param name="Transactionsid"></param>
        /// <returns></returns>
        [HttpGet("transactionsaleman/{Transactionsid}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "CustomerDealByTransactionsid" })]
        public async Task<ResponseMessage<CustomerDealResponse>> GetCustomerDealByTransactionsidSaleman(UserInfo user, [FromRoute] string Transactionsid)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""}) 根据报备流程ID查询信息(业务员)(GetCustomerDealByTransactionsidSaleman)：\r\n请求参数为：\r\n(Transactionsid){Transactionsid ?? ""}");

            var response = new ResponseMessage<CustomerDealResponse>();
            if (string.IsNullOrEmpty(Transactionsid))
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                return response;
            }
            try
            {
                response.Extension = await _customerDealManager.FindByTransactionsIdSaleManAsync(user.Id, Transactionsid, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = "服务器错误:" + e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""}) 根据报备流程ID查询信息(业务员)(GetCustomerDealByTransactionsidSaleman)报错：\r\n{e.ToString()}，\r\n请求参数为：\r\n(Transactionsid){Transactionsid ?? ""}");
            }
            return response;
        }

        /// <summary>
        /// 根据报备流程ID查询信息(经理)
        /// </summary>
        /// <param name="user"></param>
        /// <param name="Transactionsid"></param>
        /// <returns></returns>
        [HttpGet("transactionmanager/{Transactionsid}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "CustomerDealByTransactionsid" })]
        public async Task<ResponseMessage<CustomerDealResponse>> GetCustomerDealByTransactionsidSalemanager(UserInfo user, [FromRoute] string Transactionsid)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""}) 根据报备流程ID查询信息(经理)(GetCustomerDealByTransactionsidSalemanager)：\r\n请求参数为：\r\n(Transactionsid){Transactionsid ?? ""}");

            var response = new ResponseMessage<CustomerDealResponse>();
            if (string.IsNullOrEmpty(Transactionsid))
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                return response;
            }
            try
            {
                if (await _permissionExpansionManager.HavePermission(user.Id, "CustomerDealRetrieve"))
                    response.Extension = await _customerDealManager.FindByTransactionsIdSaleManagerAsync(user, Transactionsid, HttpContext.RequestAborted);
                else
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Message = "权限不足";
                }
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = "服务器错误:" + e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""}) 根据报备流程ID查询信息(经理)(GetCustomerDealByTransactionsidSalemanager)报错：\r\n{e.ToString()}，\r\n请求参数为：\r\n(Transactionsid){Transactionsid ?? ""}");
            }
            return response;
        }

        /// <summary>
        /// 根据楼商铺Id查询成交信息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("shopsdeal/{id}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "ShopsDealById" })]
        public async Task<ResponseMessage<List<CustomerDealResponse>>> GetCustomerDealByShopsId(UserInfo user, [FromRoute] string id)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""}) 根据楼商铺Id查询成交信息(GetCustomerDealByShopsId)：\r\n请求参数为：\r\n(id){id ?? ""}");

            var response = new ResponseMessage<List<CustomerDealResponse>>();
            if (string.IsNullOrEmpty(id))
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                return response;
            }
            try
            {
                response.Extension = await _customerDealManager.FindByShopsIdAsync(user.Id, id, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = "服务器错误:" + e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""}) 根据楼商铺Id查询成交信息(GetCustomerDealByShopsId)报错：\r\n{e.ToString()}，\r\n请求参数为：\r\n(id){id ?? ""}");
            }
            return response;
        }

        /// <summary>
        /// 成交信息统计
        /// </summary>
        /// <param name="user"></param>
        /// <param name="buildingDealStatisticsRequest"></param>
        /// <returns></returns>
        [HttpPost("buildingstatistics")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "BuildingStatistics" })]
        public async Task<ResponseMessage<BuildingDealStatisticsResponse>> GetBuildingStatistics(UserInfo user, [FromBody] BuildingDealStatisticsRequest buildingDealStatisticsRequest)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})成交信息统计(GetBuildingStatistics)：\r\n请求参数为：\r\n" + (buildingDealStatisticsRequest != null ? JsonHelper.ToJson(buildingDealStatisticsRequest) : ""));

            var response = new ResponseMessage<BuildingDealStatisticsResponse>();
            if (!ModelState.IsValid)
            {
                var error = "";
                var errors = ModelState.Values.ToList();
                foreach (var item in errors)
                {
                    foreach (var e in item.Errors)
                    {
                        error += e.ErrorMessage + "  ";
                    }
                }
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = error;
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})成交信息统计(GetBuildingStatistics)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (buildingDealStatisticsRequest != null ? JsonHelper.ToJson(buildingDealStatisticsRequest) : ""));
                return response;
            }
            try
            {
                if (await _permissionExpansionManager.HavePermission(user.Id, "CustomerDealStatistical"))
                    response.Extension = await _customerDealManager.BuildingDealStatistics(user.Id, buildingDealStatisticsRequest, HttpContext.RequestAborted);
                else
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Message = "权限不足";
                }

            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = "服务器错误:" + e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})成交信息统计(GetBuildingStatistics)请求失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (buildingDealStatisticsRequest != null ? JsonHelper.ToJson(buildingDealStatisticsRequest) : ""));
            }
            return response;
        }

        #region 退售流程
        /// <summary>
        /// 新增退售信息
        /// </summary>
        /// <param name="user">用户</param>
        /// <param name="id">成交信息Id</param>
        /// <returns></returns>
        [HttpPost("backsell/{id}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "CustomerTransactionsDeal" })]
        public async Task<ResponseMessage<CustomerDealResponse>> CustomerDealBackSellSubmit(UserInfo user, [FromRoute]string id)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})新增退售信息(CustomerDealBackSellSubmit)：\r\n请求参数为：\r\n(id){id ?? ""}");

            var response = new ResponseMessage<CustomerDealResponse>();
            if (string.IsNullOrEmpty(id))
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = "传入参数为空";
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})新增退售信息(CustomerDealBackSellSubmit)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n(id){id ?? ""}");
                return response;
            }
            try
            {
                var r = await _customerDealManager.FindByIdSimpleAsync(id, HttpContext.RequestAborted);
                if (r == null)
                {
                    response = new ResponseMessage<CustomerDealResponse>();
                    response.Code = ResponseCodeDefines.PartialFailure;
                    response.Message = "保存信息失败,当前商铺尚未找到成交信息";
                }
                else
                {
                    GatewayInterface.Dto.ExamineSubmitRequest examineSubmitRequest = new GatewayInterface.Dto.ExamineSubmitRequest();
                    examineSubmitRequest.ContentId = r.ShopId;
                    examineSubmitRequest.ContentType = "CustomerDealBackSell";
                    examineSubmitRequest.ContentName = r.ShopName;
                    examineSubmitRequest.Content = "";
                    examineSubmitRequest.SubmitDefineId = r.Id;
                    examineSubmitRequest.Source = "";
                    examineSubmitRequest.CallbackUrl = "通过http回调时再设置回调地址";
                    examineSubmitRequest.Action = "CustomerDealBack";
                    examineSubmitRequest.TaskName = $"客户成交:{r.Customer}";
                    examineSubmitRequest.Desc = $"客户成交退售";
                    examineSubmitRequest.Ext1 = r.BuildingName;
                    examineSubmitRequest.Ext7 = user.Id;
                    examineSubmitRequest.Ext8 = user.OrganizationId;

                    GatewayInterface.Dto.UserInfo userInfo = new GatewayInterface.Dto.UserInfo()
                    {
                        Id = user.Id,
                        KeyWord = user.KeyWord,
                        OrganizationId = user.OrganizationId,
                        OrganizationName = user.OrganizationName,
                        UserName = user.UserName
                    };

                    var _examineInterface = ApplicationContext.Current.Provider.GetRequiredService<IExamineInterface>();

                    if (await _customerDealManager.UpdateShopStatusAsync(user.Id, r.ProjectId, r.ShopId, "35") && await _customerDealManager.UpdateStatusDealBackAsync(r.Id))
                    {
                        var response2 = await _examineInterface.Submit(userInfo, examineSubmitRequest);
                        if (response2.Code != ResponseCodeDefines.SuccessCode)
                        {
                            response.Code = ResponseCodeDefines.ServiceError;
                            response.Message = "向审核中心发起审核请求失败：" + response2.Message;
                            return response;
                        }
                    }
                    else
                    {
                        response.Code = ResponseCodeDefines.NotAllow;
                        response.Message = "发生了意外的错误";
                        return response;
                    }
                }
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = "服务器错误：" + e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})新增退售信息(CustomerDealBackSellSubmit)请求失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n(id){id ?? ""}");
            }
            return response;
        }

        /// <summary>
        /// 新增退售信息回调(内部使用)
        /// </summary>
        /// <param name="examineResponse">修改实体</param>
        /// <returns></returns>
        [HttpPost("backsellcallback")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "CustomerTransactionsDeal" })]
        public async Task<ResponseMessage<ExamineStatusEnum>> CustomerDealBackSellCallback([FromBody]ExamineResponse examineResponse)
        {
            Logger.Trace($"新增退售信息回调(CustomerDealBackSellCallback)：\r\n请求参数为：\r\n" + (examineResponse != null ? JsonHelper.ToJson(examineResponse) : ""));

            var response = new ResponseMessage<ExamineStatusEnum>();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Error($"新增退售信息回调(CustomerDealBackSellCallback)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (examineResponse != null ? JsonHelper.ToJson(examineResponse) : ""));
                return response;
            }
            try
            {
                var customerDeal = await _customerDealManager.FindByIdSimpleAsync(examineResponse.SubmitDefineId);
                if (customerDeal == null)
                {
                    response.Code = ResponseCodeDefines.NotFound;
                    response.Message = "成交信息不存在：" + examineResponse.SubmitDefineId;
                    Logger.Error($"新增退售信息回调(CustomerDealBackSellCallback)失败：成交信息不存在，\r\n请求参数为：\r\n" + (examineResponse != null ? JsonHelper.ToJson(examineResponse) : ""));
                    return response;
                }
                if (examineResponse.ExamineStatus == ExamineStatus.Examined)
                {
                    await _customerDealManager.UpdateStatusDealBackApprovedAsync(examineResponse.SubmitDefineId);
                    response.Extension = ExamineStatusEnum.Approved;
                }
                else if (examineResponse.ExamineStatus == ExamineStatus.Reject)
                {
                    await _customerDealManager.UpdateStatusDealBackRejectAsync(examineResponse.SubmitDefineId);
                    response.Extension = ExamineStatusEnum.Reject;
                }
                if (response == null)
                {
                    response.Code = ResponseCodeDefines.PartialFailure;
                    response.Message = "保存信息失败";
                }
                else
                {
                    if (!string.IsNullOrEmpty(customerDeal.Salesman) && !string.IsNullOrEmpty(customerDeal.Customer))
                    {
                        var customer = await _customerInfoManager.FindByIdAsync(customerDeal.Salesman, customerDeal.Customer);

                        //发送通知消息
                        SendMessageRequest sendMessageRequest = new SendMessageRequest();
                        sendMessageRequest.MessageTypeCode = "CustomerDealBack";
                        MessageItem messageItem = new MessageItem();
                        messageItem.UserIds = new List<string> { customerDeal.Salesman };
                        messageItem.MessageTypeItems = new List<TypeItem> {
                        new TypeItem{ Key="PHONE",Value=customer.MainPhone },
                        new TypeItem{ Key="NAME",Value=customer.CustomerName },
                        new TypeItem { Key="BUILDINGNAME",Value=string.IsNullOrEmpty( customerDeal.BuildingName)?customerDeal.ShopName:"" },
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
                }
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = "服务器错误：" + e.ToString();
                Logger.Error($"新增退售信息回调(CustomerDealBackSellCallback)报错：\r\n{e.ToString()}，\r\n请求参数为：\r\n" + examineResponse != null ? JsonHelper.ToJson(examineResponse) : "");
            }
            return response;
        }
        #endregion

        #region 成交流程

        /// <summary>
        /// 新增成交信息
        /// </summary>
        /// <param name="user">用户</param>
        /// <param name="source"></param>
        /// <param name="customerDealRequest">修改实体</param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "CustomerTransactionsDeal" })]
        public async Task<ResponseMessage<CustomerDealResponse>> CustomerDealSubmit(UserInfo user, [FromQuery]string source, [FromBody]CustomerDealRequest customerDealRequest)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})新增成交信息(CustomerRDealSubmit)：\r\n请求参数为：\r\n(source){source ?? ""}\r\n" + (customerDealRequest != null ? JsonHelper.ToJson(customerDealRequest) : ""));

            var response = new ResponseMessage<CustomerDealResponse>();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})新增成交信息(CustomerRDealSubmit)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n(source){source ?? ""}\r\n" + (customerDealRequest != null ? JsonHelper.ToJson(customerDealRequest) : ""));
                return response;
            }
            try
            {
                string dealId = Guid.NewGuid().ToString();
                var r = await _customerDealManager.CustiomerDealSubmitAsync(user, dealId, customerDealRequest, HttpContext.RequestAborted);

                if (r == null)
                {
                    response = new ResponseMessage<CustomerDealResponse>();
                    response.Code = ResponseCodeDefines.PartialFailure;
                    response.Message = "保存信息失败,当前商铺已有成交信息";
                }
                else
                {
                    GatewayInterface.Dto.ExamineSubmitRequest examineSubmitRequest = new GatewayInterface.Dto.ExamineSubmitRequest();
                    examineSubmitRequest.ContentId = customerDealRequest.ShopId;
                    examineSubmitRequest.ContentType = "CustomerDeal";
                    examineSubmitRequest.ContentName = customerDealRequest.ShopName;
                    examineSubmitRequest.Content = "";
                    examineSubmitRequest.SubmitDefineId = r.Extension.Id;
                    examineSubmitRequest.Source = "";
                    examineSubmitRequest.CallbackUrl = "通过http回调时再设置回调地址";
                    examineSubmitRequest.Action = "CustomerDeal";
                    examineSubmitRequest.TaskName = $"客户成交:{customerDealRequest.Customer}";
                    examineSubmitRequest.Desc = $"客户成交";
                    examineSubmitRequest.Ext1 = customerDealRequest.BuildingName;
                    examineSubmitRequest.Ext7 = user.Id;
                    examineSubmitRequest.Ext8 = user.OrganizationId;

                    GatewayInterface.Dto.UserInfo userInfo = new GatewayInterface.Dto.UserInfo()
                    {
                        Id = user.Id,
                        KeyWord = user.KeyWord,
                        OrganizationId = user.OrganizationId,
                        OrganizationName = user.OrganizationName,
                        UserName = user.UserName
                    };

                    var _examineInterface = ApplicationContext.Current.Provider.GetRequiredService<IExamineInterface>();
                    var response2 = await _examineInterface.Submit(userInfo, examineSubmitRequest);
                    if (response2.Code != ResponseCodeDefines.SuccessCode)
                    {
                        response.Code = ResponseCodeDefines.ServiceError;
                        response.Message = "向审核中心发起审核请求失败：" + response2.Message;
                        return response;
                    }
                    else if (customerDealRequest.SellerType == SellerType.ThirdPartySale || customerDealRequest.SellerType == SellerType.Unknown)
                    {
                        await _customerDealManager.UpdateStatusDealAsync(user.Id, user.OrganizationId, r.Extension.Id);
                    }
                }
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = "服务器错误：" + e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})新增成交信息(PostListCustomerReportByStatusDea)请求失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (customerDealRequest != null ? JsonHelper.ToJson(customerDealRequest) : ""));
            }
            return response;
        }

        /// <summary>
        /// 新增成交信息
        /// </summary>
        /// <param name="user">用户</param>
        /// <param name="source"></param>
        /// <param name="customerDealRequest">修改实体</param>
        /// <returns></returns>
        [HttpPost("addcustomerdeal")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "CustomerTransactionsDeal" })]
        public async Task<ResponseMessage<CustomerDealResponse>> AddCustomerDealSubmit(UserInfo user, [FromQuery]string source, [FromBody]CustomerDealRequest customerDealRequest)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})新增成交信息(CustomerRDealSubmit)：\r\n请求参数为：\r\n(source){source ?? ""}\r\n" + (customerDealRequest != null ? JsonHelper.ToJson(customerDealRequest) : ""));

            var response = new ResponseMessage<CustomerDealResponse>();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})新增成交信息(CustomerRDealSubmit)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n(source){source ?? ""}\r\n" + (customerDealRequest != null ? JsonHelper.ToJson(customerDealRequest) : ""));
                return response;
            }
            try
            {
                string dealId = Guid.NewGuid().ToString();
                var r = await _customerDealManager.CustiomerDealSubmitAsync(user, dealId, customerDealRequest, HttpContext.RequestAborted);

                if (r == null)
                {
                    response = new ResponseMessage<CustomerDealResponse>();
                    response.Code = ResponseCodeDefines.PartialFailure;
                    response.Message = "保存信息失败,当前商铺已有成交信息";
                }
                else
                {
                    if (customerDealRequest.SellerType == SellerType.SinceSale)
                    {
                        GatewayInterface.Dto.ExamineSubmitRequest examineSubmitRequest = new GatewayInterface.Dto.ExamineSubmitRequest();
                        examineSubmitRequest.ContentId = customerDealRequest.ShopId;
                        examineSubmitRequest.ContentType = "CustomerDeal";
                        examineSubmitRequest.ContentName = customerDealRequest.ShopName;
                        examineSubmitRequest.Content = "";
                        examineSubmitRequest.SubmitDefineId = r.Extension.Id;
                        examineSubmitRequest.Source = "";
                        examineSubmitRequest.CallbackUrl = "通过http回调时再设置回调地址";
                        examineSubmitRequest.Action = "CustomerDeal";
                        examineSubmitRequest.TaskName = $"客户成交:{customerDealRequest.Customer}";
                        examineSubmitRequest.Desc = $"客户成交";
                        examineSubmitRequest.Ext1 = customerDealRequest.BuildingName;
                        examineSubmitRequest.Ext7 = user.Id;
                        examineSubmitRequest.Ext8 = user.OrganizationId;

                        GatewayInterface.Dto.UserInfo userInfo = new GatewayInterface.Dto.UserInfo()
                        {
                            Id = user.Id,
                            KeyWord = user.KeyWord,
                            OrganizationId = user.OrganizationId,
                            OrganizationName = user.OrganizationName,
                            UserName = user.UserName
                        };

                        var _examineInterface = ApplicationContext.Current.Provider.GetRequiredService<IExamineInterface>();

                        if (await _customerDealManager.UpdateShopStatusAsync(user.Id, r.Extension.ProjectId, r.Extension.ShopId, "18"))
                        {
                            var response2 = await _examineInterface.Submit(userInfo, examineSubmitRequest);
                            if (response2.Code != ResponseCodeDefines.SuccessCode)
                            {
                                response.Code = ResponseCodeDefines.ServiceError;
                                response.Message = "向审核中心发起审核请求失败：" + response2.Message;
                                return response;
                            }
                        }
                        else
                        {
                            response.Code = ResponseCodeDefines.NotAllow;
                            response.Message = "发生了意外的错误";
                            return response;
                        }
                    }
                    else if (customerDealRequest.SellerType == SellerType.ThirdPartySale || customerDealRequest.SellerType == SellerType.Unknown)
                    {
                        await _customerDealManager.UpdateStatusDealAsync(user.Id, user.OrganizationId, r.Extension.Id);
                    }
                }
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = "服务器错误：" + e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})新增成交信息(PostListCustomerReportByStatusDea)请求失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (customerDealRequest != null ? JsonHelper.ToJson(customerDealRequest) : ""));
            }
            return response;
        }


        /// <summary>
        /// 新增成交信息回调(内部使用)
        /// </summary>
        /// <param name="user">用户</param>
        /// <param name="source"></param>
        /// <param name="customerDealRequest">修改实体</param>
        /// <returns></returns>
        [HttpPost("callback")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "CustomerTransactionsDeal" })]
        public async Task<ResponseMessage<ExamineStatusEnum>> CustomerDealCallback([FromBody]ExamineResponse examineResponse)
        {
            Logger.Trace($"新增成交信息审核中心回调(CustomerrDealCallback)：\r\n请求参数为：\r\n" + (examineResponse != null ? JsonHelper.ToJson(examineResponse) : ""));

            var response = new ResponseMessage<ExamineStatusEnum>();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Error($"新增成交信息审核中心回调(CustomerrDealCallback)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (examineResponse != null ? JsonHelper.ToJson(examineResponse) : ""));
                return response;
            }
            try
            {
                var customerDeal = await _customerDealManager.FindByIdSimpleAsync(examineResponse.SubmitDefineId);
                if (customerDeal == null)
                {
                    response.Code = ResponseCodeDefines.NotFound;
                    response.Message = "成交信息不存在：" + examineResponse.SubmitDefineId;
                    Logger.Error($"新增成交信息审核中心回调(SubmitBuildingCallback)失败：成交信息不存在，\r\n请求参数为：\r\n" + (examineResponse != null ? JsonHelper.ToJson(examineResponse) : ""));
                    return response;
                }
                if (examineResponse.ExamineStatus == ExamineStatus.Examined)
                {
                    await _customerDealManager.UpdateStatusDealAsync(examineResponse.Ext7, examineResponse.Ext8, examineResponse.SubmitDefineId);
                    response.Extension = ExamineStatusEnum.Approved;
                }
                else if (examineResponse.ExamineStatus == ExamineStatus.Reject)
                {
                    await _customerDealManager.UpdateStatusDealAsync(examineResponse.SubmitDefineId);
                    response.Extension = ExamineStatusEnum.Reject;
                }
                if (response == null)
                {
                    response.Code = ResponseCodeDefines.PartialFailure;
                    response.Message = "保存信息失败";
                }
                else
                {
                    if (!string.IsNullOrEmpty(customerDeal.Salesman) && !string.IsNullOrEmpty(customerDeal.Customer))
                    {
                        var customer = await _customerInfoManager.FindByIdAsync(customerDeal.Salesman, customerDeal.Customer);

                        //发送通知消息
                        SendMessageRequest sendMessageRequest = new SendMessageRequest();
                        sendMessageRequest.MessageTypeCode = "CustomerDeal";
                        MessageItem messageItem = new MessageItem();
                        messageItem.UserIds = new List<string> { customerDeal.Salesman };
                        messageItem.MessageTypeItems = new List<TypeItem> {
                        new TypeItem{ Key="PHONE",Value=customer.MainPhone },
                        new TypeItem{ Key="NAME",Value=customer.CustomerName },
                        new TypeItem { Key="BUILDINGNAME",Value=string.IsNullOrEmpty( customerDeal.BuildingName)?customerDeal.ShopName:"" },
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
                }
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = "服务器错误：" + e.ToString();
                Logger.Error($"新增成交信息回调(SubmitBuildingCallback)报错：\r\n{e.ToString()}，\r\n请求参数为：\r\n" + examineResponse != null ? JsonHelper.ToJson(examineResponse) : "");
            }
            return response;
        }

        #endregion





        /// <summary>
        /// 修改成交信息
        /// </summary>
        /// <param name="user">用户</param>
        /// <param name="dealid"></param>
        /// <param name="customerDealRequest">修改实体</param>
        /// <returns></returns>
        [HttpPut("{dealid}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "CustomerTransactionsDeal" })]
        public async Task<ResponseMessage> PutListCustomerReportByStatusDea(UserInfo user, [FromRoute]string dealid, [FromBody]CustomerDealRequest customerDealRequest)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})修改成交信息(PutListCustomerReportByStatusDea)：\r\n请求参数为：\r\n(dealid){dealid ?? ""}\r\n" + (customerDealRequest != null ? JsonHelper.ToJson(customerDealRequest) : ""));

            var response = new ResponseMessage();
            response.Code = ResponseCodeDefines.NotFound;
            response.Message = "修改失败";
            if (!ModelState.IsValid)
            {
                var error = "";
                var errors = ModelState.Values.ToList();
                foreach (var item in errors)
                {
                    foreach (var e in item.Errors)
                    {
                        error += e.ErrorMessage + "  ";
                    }
                }
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = error;
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})修改成交信息(PutListCustomerReportByStatusDea)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n(dealid){dealid ?? ""}\r\n" + (customerDealRequest != null ? JsonHelper.ToJson(customerDealRequest) : ""));
                return response;
            }
            try
            {
                await _customerDealManager.UpdateAsync(user.Id, dealid, customerDealRequest, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = "服务器错误：" + e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})修改成交信息(PutListCustomerReportByStatusDea)请求失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n(dealid){dealid ?? ""}\r\n" + (customerDealRequest != null ? JsonHelper.ToJson(customerDealRequest) : ""));
            }
            return response;
        }

        private NWF CreateNwf(UserInfo user, string source, DealFileInfoRequest fileInfoRequest)
        {
            NWF nwf = new NWF();
            var bodyinfo = new BodyInfoType();
            var header = new HeaderType();
            bodyinfo.FileInfo = new List<FileInfoType>();

            nwf.BodyInfo = bodyinfo;
            nwf.Header = header;


            header.TaskGuid = "";
            header.ContentGuid = fileInfoRequest.SourceId;
            header.Action = "ImageProcess";
            header.SourceSystem = source;

            header.ExtraAttribute = new List<AttributeType>();
            header.ExtraAttribute.Add(new AttributeType() { Name = "UserID", Value = user.Id });

            bodyinfo.Priority = 0;
            bodyinfo.TaskName = fileInfoRequest.Name;
            if (String.IsNullOrEmpty(bodyinfo.TaskName))
            {
                bodyinfo.TaskName = $"{user.UserName}-{source ?? ""}";
            }

            var extra = new List<AttributeType>();
            extra.Add(new AttributeType { Name = "WXAppID", Value = fileInfoRequest.AppId });
            extra.Add(new AttributeType { Name = "From", Value = fileInfoRequest.From });
            extra.Add(new AttributeType { Name = "Source", Value = fileInfoRequest.Source });
            extra.Add(new AttributeType { Name = "Name", Value = fileInfoRequest.Name });
            extra.Add(new AttributeType { Name = "FileExt", Value = fileInfoRequest.FileExt });
            bodyinfo.ExtraAttribute = extra;

            FileInfoType fileInfoType = new FileInfoType();
            fileInfoType.FilePath = fileInfoRequest.WXPath;
            fileInfoType.FileExt = fileInfoRequest.FileExt;
            fileInfoType.FileGuid = fileInfoRequest.FileGuid;
            fileInfoType.QualityType = 0;
            fileInfoType.FileTypeId = "ROW";
            fileInfoType.ExtraAttribute = new List<AttributeType>();

            nwf.BodyInfo.FileInfo.Add(fileInfoType);
            return nwf;
        }
    }
}
