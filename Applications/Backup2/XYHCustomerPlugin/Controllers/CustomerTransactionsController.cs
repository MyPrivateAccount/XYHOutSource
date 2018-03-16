using ApplicationCore;
using ApplicationCore.Dto;
using ApplicationCore.Filters;
using ApplicationCore.Managers;
using AspNet.Security.OAuth.Validation;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XYH.Core.Log;
using XYHCustomerPlugin.Dto.Request;
using XYHCustomerPlugin.Dto.Response;
using XYHCustomerPlugin.Managers;

namespace XYHCustomerPlugin.Controllers
{
    /// <summary>
    /// 成交信息控制器
    /// </summary>
    [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/customertransactions")]
    public class CustomerTransactionsController : Controller
    {
        #region 成员
        private readonly PermissionExpansionManager _permissionExpansionManager;
        private readonly CustomerTransactionsManager _customerTransactionsManager;
        private readonly IMapper _mapper;
        private readonly ILogger Logger = LoggerManager.GetLogger("CustomerTransactions");

        #endregion

        /// <summary>
        /// 成交
        /// </summary>
        public CustomerTransactionsController(CustomerTransactionsManager customerTransactionsManager, IMapper mapper,
             PermissionExpansionManager permissionExpansionManager)
        {
            _permissionExpansionManager = permissionExpansionManager ?? throw new ArgumentNullException(nameof(permissionExpansionManager));
            _customerTransactionsManager = customerTransactionsManager;
            _mapper = mapper;
        }

        /// <summary>
        /// 查询我创建的
        /// </summary>
        /// <returns></returns>
        [HttpGet("[action]")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "CustomerTransactionsByMy" })]
        public async Task<ResponseMessage<List<TransactionsResponse>>> GetMyCustomerTransactions(UserInfo user)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})查询我创建的(GetMyCustomerTransactions)");

            var response = new ResponseMessage<List<TransactionsResponse>>();
            try
            {
                response.Extension = await _customerTransactionsManager.FindByUserIdAsync(user.Id, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})查询我创建的(GetMyCustomerTransactions)报错：\r\n{e.ToString()}");
            }
            return response;
        }

        /// <summary>
        /// 根据Costomerid查询跟进信息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="costomerid"></param>
        /// <returns></returns>
        [HttpGet("[action]/{costomerid}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "CustomerTransactionsByCustomerId" })]
        public async Task<ResponseMessage<List<TransactionsResponse>>> GetCustomerFollowUpByCostomerid(UserInfo user, [FromRoute]string costomerid)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})  根据Costomerid查询跟进信息(GetCustomerFollowUpByCostomerid)：\r\n请求参数为：\r\n(costomerid){costomerid ?? ""}");

            var response = new ResponseMessage<List<TransactionsResponse>>();
            if (string.IsNullOrEmpty(costomerid))
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                return response;
            }
            try
            {
                response.Extension = await _customerTransactionsManager.FindByCustomerIdAsync(user.Id, costomerid, HttpContext.RequestAborted);
            }


            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})  根据Costomerid查询跟进信息(GetCustomerFollowUpByCostomerid)报错：\r\n{e.ToString()}，\r\n请求参数为：\r\n(costomerid){costomerid ?? ""}");
            }
            return response;
        }

        /// <summary>
        /// 根据报备ids查询跟进信息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("transactionbyids")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "CustomerTransactionsByIds" })]
        public async Task<ResponseMessage<List<TransactionsResponse>>> PostCustomerFollowUpByIds(UserInfo user, [FromBody]CustomersTranRequest request)
        {
            var response = new ResponseMessage<List<TransactionsResponse>>();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})根据报备ids查询跟进信息(PostCustomerFollowUpByIds)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (request != null ? JsonHelper.ToJson(request) : ""));
                return response;
            }
            try
            {
                response.Extension = await _customerTransactionsManager.FindByCustomerIdsAsync(user.Id, request.transactionsids, request.valphone, HttpContext.RequestAborted);

            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})根据报备ids查询跟进信息(PostCustomerFollowUpByIds)请求失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (request != null ? JsonHelper.ToJson(request) : ""));
            }
            return response;
        }

        /// <summary>
        /// 查询客户在该楼盘的报备跟进信息信息
        /// </summary>
        /// <param name="user">用户Id</param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("searchtransfollowups")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "GetBuildingFollowUpByCustomerId" })]
        public async Task<ResponseMessage<List<TransactionsFollowUpResponse>>> GetBuildingFollowUpByCustomerId(UserInfo user, [FromBody]BuildingFollowUpRequest request)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})查询客户在该楼盘的报备跟进信息信息(GetBuildingFollowUpByCustomerId)：\r\n请求参数为：\r\n" + (request != null ? JsonHelper.ToJson(request) : ""));

            var response = new ResponseMessage<List<TransactionsFollowUpResponse>>();
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
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})查询客户在该楼盘的报备跟进信息信息(GetBuildingFollowUpByCustomerId)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (request != null ? JsonHelper.ToJson(request) : ""));
                return response;
            }
            try
            {
                response.Extension = await _customerTransactionsManager.FindBuildingFollowupByCustomerIdAsync(user.Id, request, HttpContext.RequestAborted);

            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})查询客户在该楼盘的报备跟进信息信息(GetBuildingFollowUpByCustomerId)模型验证把报错：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (request != null ? JsonHelper.ToJson(request) : ""));
            }
            return response;
        }

        /// <summary>
        /// 查询报备信息
        /// </summary>
        /// <param name="user">用户Id</param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("[action]/{id}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "CustomerReportSelectById" })]
        public async Task<ResponseMessage<TransactionsResponse>> GetTransactionsById(UserInfo user, [FromRoute]string id)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})  查询报备信息(GetTransactionsById)：\r\n请求参数为：\r\n(id){id ?? ""}");

            var response = new ResponseMessage<TransactionsResponse>();
            if (string.IsNullOrEmpty(id))
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                return response;
            }
            try
            {
                response.Extension = await _customerTransactionsManager.FindByIdAsync(user.Id, id, HttpContext.RequestAborted);

            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})  查询报备信息(GetTransactionsById)报错：\r\n{e.ToString()}，\r\n请求参数为：\r\n(id){id ?? ""}");
            }
            return response;
        }

        /// <summary>
        /// 验证报备信息
        /// </summary>
        /// <param name="user">用户Id</param>
        /// <param name="transactionValidation">验证实体</param>
        /// <returns></returns>
        [HttpPost("validationtransaction")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "TransactionValidation" })]
        public async Task<ResponseMessage<List<string>>> TransactionValidation(UserInfo user, [FromBody]TransactionValidation transactionValidation)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})验证报备信息(TransactionValidation)：\r\n请求参数为：\r\n" + (transactionValidation != null ? JsonHelper.ToJson(transactionValidation) : ""));

            var response = new ResponseMessage<List<string>>();
            response.Code = ResponseCodeDefines.NotFound;
            response.Message = "未查询到信息";
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
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})验证报备信息(TransactionValidation)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (transactionValidation != null ? JsonHelper.ToJson(transactionValidation) : ""));
            }
            else
            {
                try
                {
                    response.Extension = await _customerTransactionsManager.ValidationTransaction(user.Id, transactionValidation, HttpContext.RequestAborted);
                    response.Code = ResponseCodeDefines.SuccessCode;
                    response.Message = "查询成功";
                }
                catch (Exception e)
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Message = "服务器错误：" + e.ToString();
                    Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})验证报备信息(TransactionValidation)请求失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (transactionValidation != null ? JsonHelper.ToJson(transactionValidation) : ""));
                }

            }
            return response;
        }

        /// <summary>
        /// 验证带看信息
        /// </summary>
        /// <param name="user">用户Id</param>
        /// <param name="transactionValidation">验证实体</param>
        /// <returns></returns>
        [HttpPost("validationbeltlook")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "BeltLookValidation" })]
        public async Task<ResponseMessage<List<string>>> BeltLookValidation(UserInfo user, [FromBody]TransactionValidation transactionValidation)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})验证带看信息(BeltLookValidation)：\r\n请求参数为：\r\n" + (transactionValidation != null ? JsonHelper.ToJson(transactionValidation) : ""));

            var response = new ResponseMessage<List<string>>();
            response.Code = ResponseCodeDefines.NotFound;
            response.Message = "未查询到信息";
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
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})验证带看信息(BeltLookValidation)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (transactionValidation != null ? JsonHelper.ToJson(transactionValidation) : ""));
            }
            else
            {
                try
                {
                    response.Extension = await _customerTransactionsManager.ValidationBeltLook(user.Id, transactionValidation, HttpContext.RequestAborted);
                    response.Code = ResponseCodeDefines.SuccessCode;
                    response.Message = "查询成功";
                }
                catch (Exception e)
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Message = "服务器错误：" + e.ToString();
                    Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})验证带看信息(BeltLookValidation)请求失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (transactionValidation != null ? JsonHelper.ToJson(transactionValidation) : ""));
                }

            }
            return response;
        }

        /// <summary>
        /// 查询报备信息
        /// </summary>
        /// <param name="user">用户Id</param>
        /// <param name="searchcondition">修改实体</param>
        /// <returns></returns>
        [HttpPost("searchsalesman")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "SearchSalesman" })]
        public async Task<PagingResponseMessage<TransactionsResponse>> SearchSalesman(UserInfo user, [FromBody]CustomerTransactionsSearchRequest searchcondition)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})查询报备信息(SearchSalesman)：\r\n请求参数为：\r\n" + (searchcondition != null ? JsonHelper.ToJson(searchcondition) : ""));

            var response = new PagingResponseMessage<TransactionsResponse>();
            response.Code = ResponseCodeDefines.NotFound;
            response.Message = "未查询到信息";
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
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})查询报备信息(SearchSalesman)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (searchcondition != null ? JsonHelper.ToJson(searchcondition) : ""));
            }
            else
            {
                try
                {
                    response = await _customerTransactionsManager.SearchSalesman(user.Id, searchcondition, HttpContext.RequestAborted);
                    response.Code = ResponseCodeDefines.SuccessCode;
                    response.Message = "查询成功";
                }
                catch (Exception e)
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Message = "服务器错误：" + e.ToString();
                    Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})查询报备信息(SearchSalesman)请求失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (searchcondition != null ? JsonHelper.ToJson(searchcondition) : ""));
                }

            }
            return response;
        }

        /// <summary>
        /// 查询报备信息
        /// </summary>
        /// <param name="user">用户Id</param>
        /// <param name="searchcondition">修改实体</param>
        /// <returns></returns>
        [HttpPost("[action]")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "CustomerReportSelectMy" })]
        public async Task<PagingResponseMessage<TransactionsResponse>> Search(UserInfo user, [FromBody]CustomerTransactionsSearchRequest searchcondition)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})查询报备信息(Search)：\r\n请求参数为：\r\n" + (searchcondition != null ? JsonHelper.ToJson(searchcondition) : ""));

            var response = new PagingResponseMessage<TransactionsResponse>();
            response.Code = ResponseCodeDefines.NotFound;
            response.Message = "未查询到信息";
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
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})查询报备信息(Search)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (searchcondition != null ? JsonHelper.ToJson(searchcondition) : ""));
            }
            else
            {
                try
                {
                    response = await _customerTransactionsManager.Search(user.Id, searchcondition, HttpContext.RequestAborted);
                    if (response == null)
                    {
                        response = new PagingResponseMessage<TransactionsResponse>();
                        response.Code = ResponseCodeDefines.NotAllow;
                        response.Message = "当前不能查看该楼盘数据";
                        return response;
                    }
                    response.Code = ResponseCodeDefines.SuccessCode;
                    response.Message = "查询成功";
                }
                catch (Exception e)
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Message = "服务器错误：" + e.ToString();
                    Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})查询报备信息(Search)请求失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (searchcondition != null ? JsonHelper.ToJson(searchcondition) : ""));
                }

            }
            return response;
        }

        /// <summary>
        /// 根据buildingid查询跟进信息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="buildingid"></param>
        /// <returns></returns>
        [HttpGet("{buildingid}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "CustomerTransactionsByBuildingId" })]
        public async Task<ResponseMessage<List<TransactionsResponse>>> GetCustomerTransactionsByBuildingId(UserInfo user, [FromRoute]string buildingid)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})  根据buildingid查询跟进信息(GetCustomerTransactionsByBuildingId)：\r\n请求参数为：\r\n(buildingid){buildingid ?? ""}");

            var response = new ResponseMessage<List<TransactionsResponse>>();
            if (string.IsNullOrEmpty(buildingid))
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                return response;
            }
            try
            {
                response.Extension = await _customerTransactionsManager.FindByBuildingIdAsync(user.Id, buildingid, HttpContext.RequestAborted);

                if (response.Extension == null)
                {
                    response.Code = ResponseCodeDefines.NotAllow;
                    response.Message = "查询失败";
                }
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})  根据buildingid查询跟进信息(GetCustomerTransactionsByBuildingId)报错：\r\n{e.ToString()}，\r\n请求参数为：\r\n(buildingid){buildingid ?? ""}");
            }
            return response;
        }

        /// <summary>
        /// 根据状态查询跟进信息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="tranStatusByBuildingRequest"></param>
        /// <returns></returns>
        [HttpPost("transactionsstatus")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "CustomerTransactionsByStatus" })]
        public async Task<ResponseMessage<List<TransactionsResponse>>> PostTransactionByStatus(UserInfo user, [FromBody]TranStatusByBuildingRequest tranStatusByBuildingRequest)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})根据状态查询跟进信息(PostTransactionByStatus)：\r\n请求参数为：\r\n" + (tranStatusByBuildingRequest != null ? JsonHelper.ToJson(tranStatusByBuildingRequest) : ""));

            var response = new ResponseMessage<List<TransactionsResponse>>();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})根据状态查询跟进信息(PostTransactionByStatus)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (tranStatusByBuildingRequest != null ? JsonHelper.ToJson(tranStatusByBuildingRequest) : ""));
                return response;
            }
            try
            {
                response.Extension = await _customerTransactionsManager.FindCustomerByBuildingIdStatusAsync(user.Id, tranStatusByBuildingRequest, HttpContext.RequestAborted);
                if (response.Extension == null)
                {
                    response.Code = ResponseCodeDefines.NotAllow;
                    response.Message = "查询失败";
                }
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})根据状态查询跟进信息(PostTransactionByStatus)请求失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (tranStatusByBuildingRequest != null ? JsonHelper.ToJson(tranStatusByBuildingRequest) : ""));
            }
            return response;
        }

        /// <summary>
        /// 根据buildingid查询报备各状态条数信息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="buildingid"></param>
        /// <returns></returns>
        [HttpPost("retrivestatuscount")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "PostTransactionsStatusCountByBuildingId" })]
        public async Task<ResponseMessage<TransactionsStatisticsResponse>> PostTransactionsStatusCountByBuildingId(UserInfo user, [FromBody]TransactionsStatusCountRequest transactionsStatusCountRequest)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})  根据buildingid查询报备各状态条数信息(PostTransactionsStatusCountByBuildingId)：\r\n请求参数为：\r\n" + (transactionsStatusCountRequest != null ? JsonHelper.ToJson(transactionsStatusCountRequest) : ""));

            var response = new ResponseMessage<TransactionsStatisticsResponse>();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})  根据buildingid查询报备各状态条数信息(PostTransactionsStatusCountByBuildingId)报错：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (transactionsStatusCountRequest != null ? JsonHelper.ToJson(transactionsStatusCountRequest) : ""));
                return response;
            }
            try
            {
                response.Extension = await _customerTransactionsManager.FindStatusNumByBuildingIdAsync(user.Id, transactionsStatusCountRequest, HttpContext.RequestAborted);
                if (response.Extension == null)
                {
                    response.Code = ResponseCodeDefines.NotAllow;
                    response.Message = "你没有权限查看该楼盘信息";
                }
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})  根据buildingid查询报备各状态条数信息(PostTransactionsStatusCountByBuildingId)报错：\r\n请求参数为：\r\n" + (transactionsStatusCountRequest != null ? JsonHelper.ToJson(transactionsStatusCountRequest) : ""));
            }
            return response;
        }

        /// <summary>
        /// 根据buildingid查询报备各状态条数信息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="transactionsStatusCountRequest"></param>
        /// <returns></returns>
        [HttpPost("retrivetransstatuscount")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "PostTransactionsStatusCountByBuildingId" })]
        public async Task<ResponseMessage<TransactionsStatisticsResponse>> PostTransactionsStatusCountByBuildingId2(UserInfo user, [FromBody]TransactionsStatusCountRequest transactionsStatusCountRequest)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})  根据buildingid查询报备各状态条数信息(PostTransactionsStatusCountByBuildingId)：\r\n请求参数为：\r\n" + (transactionsStatusCountRequest != null ? JsonHelper.ToJson(transactionsStatusCountRequest) : ""));

            var response = new ResponseMessage<TransactionsStatisticsResponse>();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})  根据buildingid查询报备各状态条数信息(PostTransactionsStatusCountByBuildingId)报错：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (transactionsStatusCountRequest != null ? JsonHelper.ToJson(transactionsStatusCountRequest) : ""));
                return response;
            }
            try
            {
                response.Extension = await _customerTransactionsManager.FindStatusNumByBuildingId2Async(user.Id, transactionsStatusCountRequest, HttpContext.RequestAborted);
                if (response.Extension == null)
                {
                    response.Code = ResponseCodeDefines.NotAllow;
                    response.Message = "你没有权限查看该楼盘信息";
                }
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})  根据buildingid查询报备各状态条数信息(PostTransactionsStatusCountByBuildingId)报错：\r\n请求参数为：\r\n" + (transactionsStatusCountRequest != null ? JsonHelper.ToJson(transactionsStatusCountRequest) : ""));
            }
            return response;
        }

        /// <summary>
        /// 根据buildingId获取驻场待确认行数
        /// </summary>
        /// <param name="user"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpPost("transwaitconfirmcount")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "PostTransactionsStatusCountByBuildingId" })]
        public async Task<ResponseMessage<TransactionsFieldResponse>> PostTransactionsStatusCountByField(UserInfo user, [FromBody]TransWaitConfirmRequest condition)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})根据buildingId获取驻场待确认行数(PostTransactionsStatusCountByField)：\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));

            var response = new ResponseMessage<TransactionsFieldResponse>();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})根据buildingId获取驻场待确认行数(PostTransactionsStatusCountByField)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
                return response;
            }
            try
            {
                response.Extension = await _customerTransactionsManager.FindStatusNumByFieldAsync(user.Id, condition.BuildingId, condition.Completephone, HttpContext.RequestAborted);
                if (response.Extension == null)
                {
                    response.Code = ResponseCodeDefines.NotAllow;
                    response.Message = "权限不足";
                }
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})根据buildingId获取驻场待确认行数(PostTransactionsStatusCountByField)请求失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
            }
            return response;
        }

        /// <summary>
        /// 新增报备信息
        /// </summary>
        /// <param name="user">用户</param>
        /// <param name="transactionsRequest">增加实体</param>
        /// <returns></returns>
        [HttpPost("addtransactions")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "CustomerTransactionsAdd" })]
        public async Task<ResponseMessage> PostCustomer(UserInfo user, [FromBody]TransactionsCreateRequest transactionsRequest)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})新增成交信息(PostCustomer)：\r\n请求参数为：\r\n" + (transactionsRequest != null ? JsonHelper.ToJson(transactionsRequest) : ""));

            var response = new ResponseMessage();
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
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})新增成交信息(PostCustomer)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (transactionsRequest != null ? JsonHelper.ToJson(transactionsRequest) : ""));
                return response;
            }
            try
            {
                await _customerTransactionsManager.CreateAsync(user, transactionsRequest, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = "服务器错误：" + e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})新增成交信息(PostCustomer)请求失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (transactionsRequest != null ? JsonHelper.ToJson(transactionsRequest) : ""));
            }
            return response;
        }

        /// <summary>
        /// 改为确认
        /// </summary>
        /// <param name="user">用户</param>
        /// <param name="buildingid"></param>
        /// <returns></returns>
        [HttpPut("[action]/{buildingid}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "CustomerTransactionsUpdateStatus" })]
        public async Task<ResponseMessage> PutCustomerReportByStatusCon(UserInfo user, [FromRoute]string buildingid)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})  改为确认(PutCustomerReportByStatusCon)：\r\n请求参数为：\r\n(buildingid){buildingid ?? ""}");

            var response = new ResponseMessage();
            response.Code = ResponseCodeDefines.NotFound;
            response.Message = "修改失败";
            if (string.IsNullOrEmpty(buildingid))
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = "成交ID为空";
            }
            else
            {
                try
                {
                    await _customerTransactionsManager.UpdateStatusConByBuildingIdAsync(user, buildingid, HttpContext.RequestAborted);
                    response.Code = ResponseCodeDefines.SuccessCode;
                    response.Message = "修改成功";
                }
                catch (Exception e)
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Message = "服务器错误：" + e.ToString();
                    Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})  改为确认(PutCustomerReportByStatusCon)报错：\r\n{e.ToString()}，\r\n请求参数为：\r\n(buildingid){buildingid ?? ""}");
                }
            }
            return response;
        }

        /// <summary>
        /// 批量改为确认
        /// </summary>
        /// <param name="user">用户</param>
        /// <param name="transactionsids">修改实体</param>
        /// <returns></returns>
        [HttpPost("[action]")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "CustomerTransactionsUpdateStatus" })]
        public async Task<ResponseMessage> PostListCustomerReportByStatusCon(UserInfo user, [FromBody]List<string> transactionsids)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})批量改为确认(PostListCustomerReportByStatusCon)：\r\n请求参数为：\r\n" + (transactionsids != null ? JsonHelper.ToJson(transactionsids) : ""));

            var response = new ResponseMessage();
            response.Code = ResponseCodeDefines.NotFound;
            response.Message = "修改失败";
            if (transactionsids == null)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = "成交ID为空";
            }
            else
            {
                try
                {
                    await _customerTransactionsManager.UpdateStatusConAsync(user, transactionsids, HttpContext.RequestAborted);
                    response.Code = ResponseCodeDefines.SuccessCode;
                    response.Message = "修改成功";
                }
                catch (Exception e)
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Message = "服务器错误：" + e.ToString();
                    Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})批量改为确认(PostListCustomerReportByStatusCon)请求失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (transactionsids != null ? JsonHelper.ToJson(transactionsids) : ""));
                }
            }
            return response;
        }

        /// <summary>
        /// 改为已报备
        /// </summary>
        /// <param name="user">用户</param>
        /// <param name="buildingid"></param>
        /// <returns></returns>
        [HttpPut("[action]/{buildingid}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "CustomerTransactionsUpdateStatus" })]
        public async Task<ResponseMessage> PutCustomerReportByStatusRep(UserInfo user, [FromRoute]string buildingid)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})  改为已报备(PutCustomerReportByStatusRep)：\r\n请求参数为：\r\n(buildingid){buildingid ?? ""}");

            var response = new ResponseMessage();
            response.Code = ResponseCodeDefines.NotFound;
            response.Message = "修改失败";
            if (string.IsNullOrEmpty(buildingid))
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = "成交ID为空";
            }
            else
            {
                try
                {
                    await _customerTransactionsManager.UpdateStatusRepByBuildingIdAsync(user, buildingid, HttpContext.RequestAborted);
                    response.Code = ResponseCodeDefines.SuccessCode;
                    response.Message = "修改成功";
                }
                catch (Exception e)
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Message = "服务器错误：" + e.ToString();
                    Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})  改为已报备(PutCustomerReportByStatusRep)报错：\r\n{e.ToString()}，\r\n请求参数为：\r\n(buildingid){buildingid ?? ""}");
                }
            }
            return response;
        }

        /// <summary>
        /// 批量改为已报备
        /// </summary>
        /// <param name="user">用户</param>
        /// <param name="transactionsids">修改实体</param>
        /// <returns></returns>
        [HttpPost("[action]")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "CustomerTransactionsUpdateStatus" })]
        public async Task<ResponseMessage> PostListCustomerReportByStatusRep(UserInfo user, [FromBody]List<string> transactionsids)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})批量改为已报备(PostListCustomerReportByStatusRep)：\r\n请求参数为：\r\n" + (transactionsids != null ? JsonHelper.ToJson(transactionsids) : ""));

            var response = new ResponseMessage();
            response.Code = ResponseCodeDefines.NotFound;
            response.Message = "修改失败";
            if (transactionsids == null)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = "成交ID为空";
            }
            else
            {
                try
                {
                    await _customerTransactionsManager.UpdateStatusRepAsync(user, transactionsids, HttpContext.RequestAborted);
                    response.Code = ResponseCodeDefines.SuccessCode;
                    response.Message = "修改成功";
                }
                catch (Exception e)
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Message = "服务器错误：" + e.ToString();
                    Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})批量改为已报备(PostListCustomerReportByStatusRep)请求失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (transactionsids != null ? JsonHelper.ToJson(transactionsids) : ""));
                }
            }
            return response;
        }

        /// <summary>
        /// 批量改为已带看
        /// </summary>
        /// <param name="user">用户</param>
        /// <param name="transactionsids">修改实体</param>
        /// <returns></returns>
        [HttpPost("[action]")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "CustomerTransactionsUpdateStatus" })]
        public async Task<ResponseMessage> PostListCustomerReportByStatusBel(UserInfo user, [FromBody]List<string> transactionsids)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})批量改为已带看(PostListCustomerReportByStatusBel)：\r\n请求参数为：\r\n" + (transactionsids != null ? JsonHelper.ToJson(transactionsids) : ""));

            var response = new ResponseMessage();
            response.Code = ResponseCodeDefines.NotFound;
            response.Message = "修改失败";
            if (transactionsids == null)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = "成交ID为空";
            }
            else
            {
                try
                {
                    await _customerTransactionsManager.UpdateStatusBleAsync(user, transactionsids, HttpContext.RequestAborted);
                    response.Code = ResponseCodeDefines.SuccessCode;
                    response.Message = "修改成功";
                }
                catch (Exception e)
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Message = "服务器错误：" + e.ToString();
                    Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})批量改为已带看(PostListCustomerReportByStatusBel)请求失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (transactionsids != null ? JsonHelper.ToJson(transactionsids) : ""));
                }
            }
            return response;
        }

        /// <summary>
        /// 再次带看
        /// </summary>
        /// <param name="user">用户</param>
        /// <param name="againBeltLookRequest"></param>
        /// <returns></returns>
        [HttpPost("[action]")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "CustomerTransactionsUpdateStatus" })]
        public async Task<ResponseMessage> PostListCustomerAgainBletLook(UserInfo user, [FromBody]AgainBeltLookRequest againBeltLookRequest)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})再次带看(PostListCustomerAgainBletLook)：\r\n请求参数为：\r\n" + (againBeltLookRequest != null ? JsonHelper.ToJson(againBeltLookRequest) : ""));

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
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})再次带看(PostListCustomerAgainBletLook)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (againBeltLookRequest != null ? JsonHelper.ToJson(againBeltLookRequest) : ""));
                return response;
            }
            else
            {
                try
                {
                    await _customerTransactionsManager.UpdateStatusAgainBleAsync(user, againBeltLookRequest.TransactionsId, againBeltLookRequest.ExpectedTime, HttpContext.RequestAborted);
                    response.Code = ResponseCodeDefines.SuccessCode;
                    response.Message = "修改成功";
                }
                catch (Exception e)
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Message = "服务器错误：" + e.ToString();
                    Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})再次带看(PostListCustomerAgainBletLook)请求失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (againBeltLookRequest != null ? JsonHelper.ToJson(againBeltLookRequest) : ""));
                }
            }
            return response;
        }
    }
}

