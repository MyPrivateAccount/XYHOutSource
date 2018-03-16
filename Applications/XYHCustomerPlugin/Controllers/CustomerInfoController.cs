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
using XYHCustomerPlugin.Dto.Request;
using XYHCustomerPlugin.Dto.Response;
using XYHCustomerPlugin.Managers;
using XYHCustomerPlugin.Models;

namespace XYHCustomerPlugin.Controllers
{
    /// <summary>
    /// 客源
    /// </summary>
    [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/customerInfo")]
    public class CustomerInfoController : Controller
    {
        #region 成员

        private readonly CustomerInfoManager _customerInfoManager;
        private readonly CustomerHandOverManager _customerHandOverManager;
        private readonly PermissionExpansionManager _permissionExpansionManager;
        private readonly IMapper _mapper;
        private readonly ILogger Logger = LoggerManager.GetLogger("CustomerInfo");
        private readonly RestClient _restClient;
        private readonly ILogger MessageLogger = LoggerManager.GetLogger("SendMessageServerError");

        #endregion

        /// <summary>
        /// 客源信息
        /// </summary>
        public CustomerInfoController(CustomerInfoManager customerInfoManager, CustomerHandOverManager customerHandOverManager, PermissionExpansionManager permissionExpansionManager, IMapper mapper, RestClient restClient)
        {
            _customerInfoManager = customerInfoManager;
            _customerHandOverManager = customerHandOverManager;
            _permissionExpansionManager = permissionExpansionManager ?? throw new ArgumentNullException(nameof(permissionExpansionManager));
            _mapper = mapper;
            _restClient = restClient;
        }

        /// <summary>
        /// 查询楼盘下的推荐客户
        /// </summary>
        /// <param name="user">用户Id</param>
        /// <param name="buildingid"></param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        [HttpPost("relationhourselist/{buildingid}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "CustomerInfoRelationHourse" })]
        public async Task<PagingResponseMessage<RelationHouseResponse>> PostCustomerList(UserInfo user, [FromRoute]string buildingid, [FromBody]CustomerPageCondition condition)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})查询楼盘下的推荐客户(PostCustomerList)：\r\n请求参数为：\r\n(buildingid){buildingid ?? ""}\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));

            var pagingResponse = new PagingResponseMessage<RelationHouseResponse>();
            if (!ModelState.IsValid || string.IsNullOrEmpty(buildingid))
            {
                pagingResponse.Code = ResponseCodeDefines.ModelStateInvalid;
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})查询楼盘下的推荐客户(PostCustomerList)模型验证失败：\r\n{pagingResponse.Message ?? ""}，\r\n请求参数为：\r\n(buildingid){buildingid ?? ""}\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
                return pagingResponse;
            }
            try
            {
                pagingResponse = await _customerInfoManager.SearchRelationHouse(user.Id, buildingid, condition, HttpContext.RequestAborted);

            }
            catch (Exception e)
            {
                pagingResponse.Code = ResponseCodeDefines.ServiceError;
                pagingResponse.Message = "服务器错误:" + e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})查询楼盘下的推荐客户(PostCustomerList)请求失败：\r\n{pagingResponse.Message ?? ""}，\r\n请求参数为：\r\n(buildingid){buildingid ?? ""}\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
            }
            return pagingResponse;
        }

        /// <summary>
        /// 查询条件
        /// </summary>
        /// <param name="user">用户Id</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        [HttpPost("list")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "CustomerInfoSearch" })]
        public async Task<PagingResponseMessage<CustomerInfoSearchResponse>> Search(UserInfo user, [FromBody]CustomerListSearchCondition condition)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})查询条件(Search)：\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));

            var pagingResponse = new PagingResponseMessage<CustomerInfoSearchResponse>();
            if (!ModelState.IsValid)
            {
                pagingResponse.Code = ResponseCodeDefines.ModelStateInvalid;
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})查询条件(Search)模型验证失败：\r\n{pagingResponse.Message ?? ""}，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
                return pagingResponse;
            }
            try
            {
                pagingResponse = await _customerInfoManager.Search(user, condition, HttpContext.RequestAborted);

            }
            catch (Exception e)
            {
                pagingResponse.Code = ResponseCodeDefines.ServiceError;
                pagingResponse.Message = "服务器错误:" + e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})查询条件(Search)请求失败：\r\n{pagingResponse.Message ?? ""}，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
            }
            return pagingResponse;
        }

        /// <summary>
        /// 查询业务员条件
        /// </summary>
        /// <param name="user">用户Id</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        [HttpPost("listsalemanrepeat")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "CustomerInfoSearch" })]
        public async Task<CustomerSearchSalemanResponse<CustomerRepeatReponse>> PostCustomerListSaleManRepeat(UserInfo user, [FromBody]CustomerListSearchCondition condition)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})查询业务员条件(PostCustomerListSaleManRepeat)：\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));

            var pagingResponse = new CustomerSearchSalemanResponse<CustomerRepeatReponse>();
            if (!ModelState.IsValid)
            {
                pagingResponse.Code = ResponseCodeDefines.ModelStateInvalid;
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})查询业务员条件(PostCustomerListSaleManRepeat)模型验证失败：\r\n{pagingResponse.Message ?? ""}，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
                return pagingResponse;
            }
            try
            {
                if (await _permissionExpansionManager.HavePermission(user.Id, "CustomerRetrieve"))
                {
                    pagingResponse = await _customerInfoManager.SearchSalemanRepeat(user, condition, HttpContext.RequestAborted);
                }
                else
                {
                    pagingResponse.Code = ResponseCodeDefines.NotAllow;
                    pagingResponse.Message = "权限不足";
                }

            }
            catch (Exception e)
            {
                pagingResponse.Code = ResponseCodeDefines.ServiceError;
                pagingResponse.Message = "服务器错误:" + e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})查询业务员条件(PostCustomerListSaleManRepeat)请求失败：\r\n{pagingResponse.Message ?? ""}，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));

            }
            return pagingResponse;
        }

        /// <summary>
        /// 根据主要电话号码查询用户信息
        /// </summary>
        /// <param name="user">用户Id</param>
        /// <param name="phone">条件</param>
        /// <returns></returns>
        [HttpGet("listsalemanbyphone/{phone}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "CustomerInfoSearch" })]
        public async Task<ResponseMessage<List<CustomerSearchSaleman>>> PostCustomerListSaleManByPhone(UserInfo user, [FromRoute]string phone)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})根据主要电话号码查询用户信息(PostCustomerListSaleManByPhone)：\r\n请求参数为：\r\n phone:" + phone ?? "");

            var pagingResponse = new ResponseMessage<List<CustomerSearchSaleman>>();
            if (!ModelState.IsValid)
            {
                pagingResponse.Code = ResponseCodeDefines.ModelStateInvalid;
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})根据主要电话号码查询用户信息(PostCustomerListSaleManByPhone)模型验证失败：\r\n phone:" + phone ?? "");
                return pagingResponse;
            }
            try
            {
                if (await _permissionExpansionManager.HavePermission(user.Id, "CustomerRetrieve"))
                {
                    pagingResponse.Extension = await _customerInfoManager.GetCustomersByPhone(user, phone, HttpContext.RequestAborted);
                }
                else
                {
                    pagingResponse.Code = ResponseCodeDefines.NotAllow;
                    pagingResponse.Message = "权限不足";
                }

            }
            catch (Exception e)
            {
                pagingResponse.Code = ResponseCodeDefines.ServiceError;
                pagingResponse.Message = "服务器错误:" + e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})根据主要电话号码查询用户信息(PostCustomerListSaleManByPhone)请求失败：\r\n phone:" + phone ?? "");

            }
            return pagingResponse;
        }

        /// <summary>
        /// 查询业务员条件
        /// </summary>
        /// <param name="user">用户Id</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        [HttpPost("listsaleman")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "CustomerInfoSearch" })]
        public async Task<CustomerSearchSalemanResponse<CustomerSearchSaleman>> PostCustomerListSaleMan(UserInfo user, [FromBody]CustomerListSearchCondition condition)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})查询业务员条件(PostCustomerListSaleMan)：\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));

            var pagingResponse = new CustomerSearchSalemanResponse<CustomerSearchSaleman>();
            if (!ModelState.IsValid)
            {
                pagingResponse.Code = ResponseCodeDefines.ModelStateInvalid;
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})查询业务员条件(PostCustomerListSaleMan)模型验证失败：\r\n{pagingResponse.Message ?? ""}，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
                return pagingResponse;
            }
            try
            {
                if (await _permissionExpansionManager.HavePermission(user.Id, "CustomerRetrieve"))
                {
                    pagingResponse = await _customerInfoManager.SearchSaleman(user, condition, HttpContext.RequestAborted);
                }
                else
                {
                    pagingResponse.Code = ResponseCodeDefines.NotAllow;
                    pagingResponse.Message = "权限不足";
                }

            }
            catch (Exception e)
            {
                pagingResponse.Code = ResponseCodeDefines.ServiceError;
                pagingResponse.Message = "服务器错误:" + e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})查询业务员条件(PostCustomerListSaleMan)请求失败：\r\n{pagingResponse.Message ?? ""}，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));

            }
            return pagingResponse;
        }

        /// <summary>
        /// 查询业务员重客条件
        /// </summary>
        /// <param name="user">用户Id</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        [HttpPost("repetitionsaleman")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "CustomerInfoSearch" })]
        public async Task<PagingResponseMessage<CustomerSearchSaleman>> PostCustomerListRePSaleMan(UserInfo user, [FromBody]CustomerListSearchCondition condition)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})查询业务员重客条件(PostCustomerListRePSaleMan)：\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));

            var pagingResponse = new PagingResponseMessage<CustomerSearchSaleman>();
            if (!ModelState.IsValid)
            {
                pagingResponse.Code = ResponseCodeDefines.ModelStateInvalid;
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})查询业务员重客条件(PostCustomerListRePSaleMan)模型验证失败：\r\n{pagingResponse.Message ?? ""}，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
                return pagingResponse;
            }
            try
            {
                if (await _permissionExpansionManager.HavePermission(user.Id, "CustomerRetrieve"))
                {
                    pagingResponse = await _customerInfoManager.SearchRepetitionSaleman(user, condition, HttpContext.RequestAborted);
                }
                else
                {
                    pagingResponse.Code = ResponseCodeDefines.NotAllow;
                    pagingResponse.Message = "权限不足";
                }

            }
            catch (Exception e)
            {
                pagingResponse.Code = ResponseCodeDefines.ServiceError;
                pagingResponse.Message = "服务器错误:" + e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})查询业务员重客条件(PostCustomerListRePSaleMan)请求失败：\r\n{pagingResponse.Message ?? ""}，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));

            }
            return pagingResponse;
        }

        /// <summary>
        /// 判断重客
        /// </summary>
        /// <param name="user">用户Id</param>
        /// <param name="phone">条件</param>
        /// <returns></returns>
        [HttpGet("customerheavy/{phone}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "CustomerHeavySearch" })]
        public async Task<ResponseMessage<CustomerHeavy>> PostCustomerHeavy(UserInfo user, [FromRoute]string phone)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})判断重客(PostCustomerHeavy)：\r\n请求参数为：\r\n(phone){phone ?? ""}");

            var response = new ResponseMessage<CustomerHeavy>();
            if (string.IsNullOrEmpty(phone))
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                return response;
            }
            try
            {
                if (await _permissionExpansionManager.HavePermission(user.Id, "CustomerRetrieve"))
                {
                    response.Extension = await _customerInfoManager.SearchCustomerHeavy(user, phone, HttpContext.RequestAborted);
                    return response;
                }
                else
                {
                    response.Code = ResponseCodeDefines.NotAllow;
                    response.Message = "权限不足";
                }

            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = "服务器错误" + e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})判断重客(PostCustomerHeavy)报错：\r\n{e.ToString()}，\r\n请求参数为：\r\n(phone){phone ?? ""}");
            }
            return response;
        }

        /// <summary>
        /// 查询公客池条件
        /// </summary>
        /// <param name="user">用户Id</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        [HttpPost("listpool")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "CustomerInfoSearch" })]
        public async Task<PagingResponseMessage<CustomerSearchPool>> PostCustomerListPool(UserInfo user, [FromBody]CustomerListSearchCondition condition)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})查询公客池条件(PostCustomerListPool)：\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));

            var pagingResponse = new PagingResponseMessage<CustomerSearchPool>();
            if (!ModelState.IsValid)
            {
                pagingResponse.Code = ResponseCodeDefines.ModelStateInvalid;
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})查询公客池条件(PostCustomerListPool)模型验证失败：\r\n{pagingResponse.Message ?? ""}，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
                return pagingResponse;
            }
            try
            {
                if (await _permissionExpansionManager.HavePermission(user.Id, "CustomerRetrieve"))
                {
                    pagingResponse = await _customerInfoManager.SearchCustomerpool(user, condition, HttpContext.RequestAborted);
                }
                else
                {
                    pagingResponse.Code = ResponseCodeDefines.NotAllow;
                    pagingResponse.Message = "权限不足";
                }
            }
            catch (Exception e)
            {
                pagingResponse.Code = ResponseCodeDefines.ServiceError;
                pagingResponse.Message = "服务器错误" + e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})查询公客池条件(PostCustomerListPool)请求失败：\r\n{pagingResponse.Message ?? ""}，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
            }
            return pagingResponse;
        }

        /// <summary>
        /// 查询业务员公客池条件
        /// </summary>
        /// <param name="user">用户Id</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        [HttpPost("listpoolsaleman")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "CustomerInfoSearch" })]
        public async Task<PagingResponseMessage<CustomerSearchPool>> PostCustomerListPoolSaleman(UserInfo user, [FromBody]CustomerListSearchCondition condition)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})查询业务员公客池条件(PostCustomerListPoolSaleman)：\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));

            var pagingResponse = new PagingResponseMessage<CustomerSearchPool>();
            if (!ModelState.IsValid)
            {
                pagingResponse.Code = ResponseCodeDefines.ModelStateInvalid;
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})查询业务员公客池条件(PostCustomerListPoolSaleman)模型验证失败：\r\n{pagingResponse.Message ?? ""}，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
                return pagingResponse;
            }
            try
            {
                pagingResponse = await _customerInfoManager.SearchCustomerpoolSaleman(user, condition, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                pagingResponse.Code = ResponseCodeDefines.ServiceError;
                pagingResponse.Message = "服务器错误" + e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})查询业务员公客池条件(PostCustomerListPoolSaleman)请求失败：\r\n{pagingResponse.Message ?? ""}，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
            }
            return pagingResponse;
        }

        /// <summary>
        /// 查询已成交条件
        /// </summary>
        /// <param name="user">用户Id</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        [HttpPost("listdeal")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "CustomerInfoSearch" })]
        public async Task<PagingResponseMessage<CustomerSearchSaleman>> PostCustomerListDeal(UserInfo user, [FromBody]CustomerListSearchCondition condition)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})查询已成交条件(PostCustomerListDeal)：\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));

            var pagingResponse = new PagingResponseMessage<CustomerSearchSaleman>();
            if (!ModelState.IsValid)
            {
                pagingResponse.Code = ResponseCodeDefines.ModelStateInvalid;
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})查询已成交条件(PostCustomerListDeal)模型验证失败：\r\n{pagingResponse.Message ?? ""}，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
                return pagingResponse;
            }
            try
            {
                if (await _permissionExpansionManager.HavePermission(user.Id, "CustomerRetrieve"))
                {
                    pagingResponse = await _customerInfoManager.SearchCustomerDeal(user, condition, HttpContext.RequestAborted);
                }
                else
                {
                    pagingResponse.Code = ResponseCodeDefines.NotAllow;
                    pagingResponse.Message = "权限不足";
                }
            }
            catch (Exception e)
            {
                pagingResponse.Code = ResponseCodeDefines.ServiceError;
                pagingResponse.Message = "服务器错误" + e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})查询已成交条件(PostCustomerListDeal)请求失败：\r\n{pagingResponse.Message ?? ""}，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
            }
            return pagingResponse;
        }

        /// <summary>
        /// 查询已失效条件
        /// </summary>
        /// <param name="user">用户Id</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        [HttpPost("listloss")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "CustomerInfoSearch" })]
        public async Task<PagingResponseMessage<CustomerSearchLoss>> PostCustomerListLoss(UserInfo user, [FromBody]CustomerListSearchCondition condition)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})查询已失效条件(PostCustomerListLoss)：\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));

            var pagingResponse = new PagingResponseMessage<CustomerSearchLoss>();
            if (!ModelState.IsValid)
            {
                pagingResponse.Code = ResponseCodeDefines.ModelStateInvalid;
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})查询已失效条件(PostCustomerListLoss)模型验证失败：\r\n{pagingResponse.Message ?? ""}，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
                return pagingResponse;
            }
            try
            {
                if (await _permissionExpansionManager.HavePermission(user.Id, "CustomerRetrieve"))
                {
                    pagingResponse = await _customerInfoManager.SearchCustomerLoss(user, condition, HttpContext.RequestAborted);
                }
                else
                {
                    pagingResponse.Code = ResponseCodeDefines.NotAllow;
                    pagingResponse.Message = "权限不足";
                }
            }
            catch (Exception e)
            {
                pagingResponse.Code = ResponseCodeDefines.ServiceError;
                pagingResponse.Message = "服务器错误" + e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})查询已失效条件(PostCustomerListLoss)请求失败：\r\n{pagingResponse.Message ?? ""}，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
            }
            return pagingResponse;
        }

        /// <summary>
        /// 查询已失效条件业务员
        /// </summary>
        /// <param name="user">用户Id</param>
        /// <param name="condition">条件</param>
        /// <returns></returns>
        [HttpPost("listlosssalesman")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "CustomerInfoSearch" })]
        public async Task<PagingResponseMessage<CustomerSearchLoss>> PostCustomerListLossSalesman(UserInfo user, [FromBody]CustomerListSearchCondition condition)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})查询已失效条件业务员(PostCustomerListLossSalesman)：\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));

            var pagingResponse = new PagingResponseMessage<CustomerSearchLoss>();
            if (!ModelState.IsValid)
            {
                pagingResponse.Code = ResponseCodeDefines.ModelStateInvalid;
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})查询已失效条件业务员(PostCustomerListLossSalesman)模型验证失败：\r\n{pagingResponse.Message ?? ""}，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
                return pagingResponse;
            }
            try
            {
                pagingResponse = await _customerInfoManager.SearchCustomerLossSalesMan(user.Id, condition, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                pagingResponse.Code = ResponseCodeDefines.ServiceError;
                pagingResponse.Message = "服务器错误" + e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})查询已失效条件业务员(PostCustomerListLossSalesman)请求失败：\r\n{pagingResponse.Message ?? ""}，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
            }
            return pagingResponse;
        }

        /// <summary>
        /// 根据客户Id查询信息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="customerInfoId"></param>
        /// <returns></returns>
        [HttpGet("{customerInfoId}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "CustomerInfoById" })]
        public async Task<ResponseMessage<CustomerInfoCreateResponse>> GetCustomerInfo(UserInfo user, [FromRoute] string customerInfoId)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})根据客户Id查询信息(GetCustomerInfo)：\r\n请求参数为：\r\n(dealid){customerInfoId ?? ""}");

            var response = new ResponseMessage<CustomerInfoCreateResponse>();
            if (string.IsNullOrEmpty(customerInfoId))
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                return response;
            }
            try
            {
                response.Extension = await _customerInfoManager.FindByIdAsync(user.Id, customerInfoId, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})根据客户Id查询信息(GetCustomerInfo)报错：\r\n{e.ToString()}，\r\n请求参数为：\r\n(dealid){customerInfoId ?? ""}");
            }
            return response;
        }

        /// <summary>
        /// 根据客户Id查询信息(经理)
        /// </summary>
        /// <param name="user"></param>
        /// <param name="customerInfoId"></param>
        /// <returns></returns>
        [HttpGet("retrieve/{customerInfoId}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "CustomerInfoById" })]
        public async Task<ResponseMessage<CustomerInfoCreateResponse>> GetCustomerInfoOrgan(UserInfo user, [FromRoute] string customerInfoId)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})根据客户Id查询信息(经理)(GetCustomerInfoOrgan)：\r\n请求参数为：\r\n(dealid){customerInfoId ?? ""}");

            var response = new ResponseMessage<CustomerInfoCreateResponse>();
            if (string.IsNullOrEmpty(customerInfoId))
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                return response;
            }
            try
            {
                if (await _permissionExpansionManager.HavePermission(user.Id, "CustomerRetrieve"))
                {
                    response.Extension = await _customerInfoManager.FindByIdOrganAsync(user, customerInfoId, HttpContext.RequestAborted);
                }
                else
                {
                    response.Code = ResponseCodeDefines.NotAllow;
                    response.Message = "权限不足";
                }
            }
            catch (Exception e)
            {
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})根据客户Id查询信息(经理)(GetCustomerInfoOrgan)报错：\r\n{e.ToString()}，\r\n请求参数为：\r\n(dealid){customerInfoId ?? ""}");
            }

            return response;
        }

        /// <summary>
        /// 查询无效客户
        /// </summary>
        /// <returns></returns>
        [HttpPost("[action]")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "CustomerInfoByStatus" })]
        public async Task<PagingResponseMessage<CustomerListResponse>> GetCustomerInfoLapse(UserInfo user, [FromBody]CustomerPageCondition pageCondition)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})查询无效客户(GetCustomerInfoLapse)：\r\n请求参数为：\r\n" + (pageCondition != null ? JsonHelper.ToJson(pageCondition) : ""));

            var response = new PagingResponseMessage<CustomerListResponse>();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})查询无效客户(GetCustomerInfoLapse)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (pageCondition != null ? JsonHelper.ToJson(pageCondition) : ""));
                return response;
            }
            try
            {
                if (await _permissionExpansionManager.HavePermission(user.Id, "CustomerRetrieve"))
                {
                    response = await _customerInfoManager.FindByCustomerLapseAsync(user.Id, pageCondition, HttpContext.RequestAborted);
                }
                else
                {
                    response.Code = ResponseCodeDefines.NotAllow;
                    response.Message = "权限不足";
                }
                return response;
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = "服务器错误：" + e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})查询无效客户(GetCustomerInfoLapse)请求失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (pageCondition != null ? JsonHelper.ToJson(pageCondition) : ""));
            }
            return response;
        }

        /// <summary>
        /// 查询我的客户
        /// </summary>
        /// <returns></returns>
        [HttpPost("[action]")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "CustomerInfoByMy" })]
        public async Task<PagingResponseMessage<CustomerListResponse>> GetMyCustomerInfo(UserInfo user, [FromBody]CustomerListSearchCondition pageCondition)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})查询我的客户(GetMyCustomerInfo)：\r\n请求参数为：\r\n" + (pageCondition != null ? JsonHelper.ToJson(pageCondition) : ""));

            var response = new PagingResponseMessage<CustomerListResponse>();
            if (pageCondition == null)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})查询我的客户(GetMyCustomerInfo)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (pageCondition != null ? JsonHelper.ToJson(pageCondition) : ""));
                return response;
            }
            try
            {
                response = await _customerInfoManager.FindByUserIdAsync(user.Id, pageCondition, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = "服务器错误：" + e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})查询我的客户(GetMyCustomerInfo)请求失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (pageCondition != null ? JsonHelper.ToJson(pageCondition) : ""));
            }
            return response;
        }

        /// <summary>
        /// 查询楼盘关联的推荐客户
        /// </summary>
        /// <param name="user"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpPost("buildings/recommend")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "RecommendMyCustomerInfo" })]
        public async Task<PagingResponseMessage<CustomerListResponse>> RecommendMyCustomerInfo(UserInfo user, [FromBody]BuildingRecommendRequest condition)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})查询楼盘推荐客户(RecommendMyCustomerInfo)：\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));

            PagingResponseMessage<CustomerListResponse> r = new PagingResponseMessage<CustomerListResponse>();
            if (!ModelState.IsValid)
            {
                r.Code = ResponseCodeDefines.ModelStateInvalid;
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})查询楼盘推荐客户(RecommendMyCustomerInfo)模型验证失败：\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
                return r;
            }
            try
            {

                r = await _customerInfoManager.RecommendFromBuilding(user, condition);

            }
            catch (Exception e)
            {
                r.Code = "500";
                r.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})查询楼盘推荐客户(RecommendMyCustomerInfo)请求失败：\r\n{r.Message ?? ""}，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
            }

            return r;

        }

        /// <summary>
        /// 根据业务员ID查询客户
        /// </summary>
        /// <returns></returns>
        [HttpPost("salesmancustomer/{salesmanid}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "CustomerInfoByMy" })]
        public async Task<PagingResponseMessage<CustomerListResponse>> GetCustomerInfoBySalesman(UserInfo user, [FromRoute]string salesmanid, [FromBody]CustomerListSearchCondition pageCondition)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})根据业务员ID查询客户(GetCustomerInfoBySalesman)：\r\n请求参数为：\r\n(salesmanid){salesmanid ?? ""}\r\n" + (pageCondition != null ? JsonHelper.ToJson(pageCondition) : ""));

            var response = new PagingResponseMessage<CustomerListResponse>();
            if (pageCondition == null && !ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})根据业务员ID查询客户(GetCustomerInfoBySalesman)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n(salesmanid){salesmanid ?? ""}\r\n" + (pageCondition != null ? JsonHelper.ToJson(pageCondition) : ""));
                return response;
            }
            try
            {
                if (await _permissionExpansionManager.HavePermission(user.Id, "CustomerRetrieve"))
                {
                    response = await _customerInfoManager.FindByUserIdAsync(salesmanid, pageCondition, HttpContext.RequestAborted);
                }
                else
                {
                    response.Code = ResponseCodeDefines.NotAllow;
                    response.Message = "权限不足";
                }
                return response;
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = "服务器错误：" + e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})根据业务员ID查询客户(GetCustomerInfoBySalesman)请求失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n(salesmanid){salesmanid ?? ""}\r\n" + (pageCondition != null ? JsonHelper.ToJson(pageCondition) : ""));
            }
            return response;
        }

        /// <summary>
        /// 移交客户
        /// </summary>
        /// <returns></returns>
        [HttpPost("[action]")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "CustomerInfoHandOver" })]
        public async Task<ResponseMessage> HandOverCustomerInfo(UserInfo user, [FromBody]HandOverCondition condition)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})移交客户(HandOverCustomerInfo)：\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));

            var response = new ResponseMessage();
            if (condition == null)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})移交客户(HandOverCustomerInfo)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
                return response;
            }
            try
            {
                if (await _permissionExpansionManager.HavePermission(user.Id, "CustomerHandOver"))
                {
                    response = await _customerHandOverManager.HandOver(user, condition, HttpContext.RequestAborted);
                }
                else
                {
                    response.Code = ResponseCodeDefines.NotAllow;
                    response.Message = "权限不足";
                }
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})移交客户(HandOverCustomerInfo)请求失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
            }
            return response;
        }

        /// <summary>
        /// 将多个客户移到公客池
        /// </summary>
        /// <returns></returns>
        [HttpPost("[action]")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "CustomerInfoHandOverPool" })]
        public async Task<ResponseMessage> HandOverListCustomerPool(SimpleUser user, [FromBody]List<string> CustomerIds)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})移交客户(HandOverCustomerInfo)：\r\n请求参数为：\r\n" + (CustomerIds != null ? JsonHelper.ToJson(CustomerIds) : ""));

            var response = new ResponseMessage();
            if (CustomerIds == null)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                return response;
            }
            try
            {
                if (await _permissionExpansionManager.HavePermission(user.Id, "CustomerTransferPool"))
                {
                    response = await _customerHandOverManager.HandOverCustomerPool(user, CustomerIds, HttpContext.RequestAborted);
                }
                else
                {
                    response.Code = ResponseCodeDefines.NotAllow;
                    response.Message = "权限不足";
                }
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})移交客户(HandOverCustomerInfo)请求失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (CustomerIds != null ? JsonHelper.ToJson(CustomerIds) : ""));
            }
            return response;
        }

        /// <summary>
        /// 根据Userid查询用户手机列表
        /// </summary>
        /// <returns></returns>
        [HttpGet("[action]/{customerid}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "UserInfoPhoneByUserid" })]
        public async Task<ResponseMessage<List<CustomerPhoneResponse>>> GetUseridCustomerInfoPhone(UserInfo user, [FromRoute] string customerid)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})根据Userid查询用户手机列表(GetUseridCustomerInfoPhone)：\r\n请求参数为：\r\n(customerid){customerid ?? ""}");

            var response = new ResponseMessage<List<CustomerPhoneResponse>>();
            if (string.IsNullOrEmpty(customerid))
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = "用户ID为空";
                return response;
            }
            try
            {
                response.Extension = await _customerInfoManager.FindPhoneByCustomerAsync(user, customerid, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = "服务器错误：" + e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})根据Userid查询用户手机列表(GetUseridCustomerInfoPhone)报错：\r\n{e.ToString()}，\r\n请求参数为：\r\n(customerid){customerid ?? ""}");
            }
            return response;
        }

        /// <summary>
        /// 新增客源信息
        /// </summary>
        /// <param name="user">用户</param>
        /// <param name="customerInfoRequest">增加实体</param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "CustomerInfoCreate" })]
        public async Task<ResponseMessage<CustomerInfoCreateResponse>> PostCustomerInfo(UserInfo user, [FromBody]CustomerInfoCreateRequest customerInfoRequest)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})新增客源信息(PostCustomerInfo)：\r\n请求参数为：\r\n" + (customerInfoRequest != null ? JsonHelper.ToJson(customerInfoRequest) : ""));

            var response = new ResponseMessage<CustomerInfoCreateResponse>();
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
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})新增客源信息(PostCustomerInfo)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (customerInfoRequest != null ? JsonHelper.ToJson(customerInfoRequest) : ""));
                return response;
            }
            try
            {
                response = await _customerInfoManager.CreateAsync(user, customerInfoRequest, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = "服务器错误" + e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})新增客源信息(PostCustomerInfo)请求失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (customerInfoRequest != null ? JsonHelper.ToJson(customerInfoRequest) : ""));
            }
            return response;
        }

        /// <summary>
        /// 修改客源信息
        /// </summary>
        /// <param name="user">用户</param>
        /// <param name="customerInfoRequest">修改实体</param>
        /// <returns></returns>
        [HttpPut]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "CustomerInfoUpdate" })]
        public async Task<ResponseMessage> PutCustomerInfo(UserInfo user, [FromBody]CustomerInfoCreateRequest customerInfoRequest)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})修改客源信息(PutCustomerInfo)：\r\n请求参数为：\r\n" + (customerInfoRequest != null ? JsonHelper.ToJson(customerInfoRequest) : ""));

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
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})修改客源信息(PutCustomerInfo)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (customerInfoRequest != null ? JsonHelper.ToJson(customerInfoRequest) : ""));
                return response;
            }
            try
            {
                var dictionaryGroup = await _customerInfoManager.FindByIdAsync(user.Id, customerInfoRequest.Id, HttpContext.RequestAborted);
                if (dictionaryGroup == null)
                {
                    await _customerInfoManager.CreateAsync(user, customerInfoRequest, HttpContext.RequestAborted);
                }
                if (dictionaryGroup.UserId == user.Id)
                {

                    await _customerInfoManager.UpdateAsync(user.Id, customerInfoRequest, HttpContext.RequestAborted);
                    response.Code = ResponseCodeDefines.SuccessCode;
                    response.Message = "修改成功";
                }
                else
                {
                    response.Code = ResponseCodeDefines.NotAllow;
                    response.Message = "修改失败";
                }
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})修改客源信息(PutCustomerInfo)请求失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (customerInfoRequest != null ? JsonHelper.ToJson(customerInfoRequest) : ""));
            }
            return response;
        }

        /// <summary>
        /// 删除单个客源
        /// </summary>
        /// <param name="user">删除用户</param>
        /// <param name="customerId"></param>
        /// <returns></returns>
        [HttpDelete("{customerId}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "CustomerDelete" })]
        public async Task<ResponseMessage> DeleteCustomerInfo(UserInfo user, [FromRoute] string customerId)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})删除单个客源(DeleteCustomerInfo)：\r\n请求参数为：\r\n(customerId){customerId ?? ""}");

            ResponseMessage response = new ResponseMessage();
            if (string.IsNullOrEmpty(customerId))
            {
                response.Code = ResponseCodeDefines.ArgumentNullError;
                response.Message = "参数出错";
                return response;
            }
            try
            {
                await _customerInfoManager.DeleteAsync(user, customerId, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})删除单个客源(DeleteCustomerInfo)报错：\r\n{e.ToString()}，\r\n请求参数为：\r\n(customerId){customerId ?? ""}");
            }
            return response;
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="customerIds">客源idList</param>
        /// <returns></returns>
        [HttpPost("delete")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "CustomerDeletes" })]
        public async Task<ResponseMessage> DeletePermissionItems(UserInfo user, [FromBody] List<string> customerIds)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})批量删除(DeletePermissionItems)：\r\n请求参数为：\r\n" + (customerIds != null ? JsonHelper.ToJson(customerIds) : ""));

            ResponseMessage response = new ResponseMessage();
            if (customerIds == null)
            {
                response.Code = ResponseCodeDefines.ArgumentNullError;
                response.Message = "参数出错";
                return response;
            }
            try
            {

                await _customerInfoManager.DeleteListAsync(user.Id, customerIds, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ArgumentNullError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})批量删除(DeletePermissionItems)请求失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (customerIds != null ? JsonHelper.ToJson(customerIds) : ""));
            }
            return response;
        }

        /// <summary>
        /// 调客
        /// </summary>
        /// <param name="user">用户</param>
        /// <param name="transferringRequest"></param>
        /// <returns></returns>
        [HttpPost("transfercustomer")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage> Transferring(UserInfo user, [FromBody] TransferringRequest transferringRequest)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})调客(Transferring)：\r\n请求参数为：\r\n" + (transferringRequest != null ? JsonHelper.ToJson(transferringRequest) : ""));

            ResponseMessage response = new ResponseMessage();
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
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})调客(Transferring)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (transferringRequest != null ? JsonHelper.ToJson(transferringRequest) : ""));
                return response;
            }
            try
            {
                if (await _permissionExpansionManager.HavePermission(user.Id, "CustomerAdjust"))
                {
                    GatewayInterface.Dto.ExamineSubmitRequest examineSubmitRequest = new GatewayInterface.Dto.ExamineSubmitRequest();
                    examineSubmitRequest.ContentId = Guid.NewGuid().ToString();
                    examineSubmitRequest.ContentType = "TransferCustomer";
                    examineSubmitRequest.ContentName = "调客";
                    examineSubmitRequest.Content = JsonHelper.ToJson(transferringRequest);
                    examineSubmitRequest.Source = "";
                    examineSubmitRequest.CallbackUrl = "通过http回调时再设置回调地址";
                    examineSubmitRequest.Action = "TransferCustomer";
                    examineSubmitRequest.TaskName = user.UserName + "提交的调客申请";
                    examineSubmitRequest.Desc = $"{transferringRequest.SourceDepartmentName}部门{transferringRequest.SourceUserName}业务员{transferringRequest.Customers.Count}个客户【调客至】{transferringRequest.TerDepartmentName}部门{transferringRequest.TerUserName}业务员";

                    GatewayInterface.Dto.UserInfo userInfo = new GatewayInterface.Dto.UserInfo()
                    {
                        Id = user.Id,
                        KeyWord = user.KeyWord,
                        OrganizationId = user.OrganizationId,
                        OrganizationName = user.OrganizationName,
                        UserName = user.UserName
                    };

                    //Logger.Info("nwf协议：\r\n{0}", JsonHelper.ToJson(examineSubmitRequest));
                    //string result = await _restClient.Post(ApplicationContext.Current.ExamineUrl, examineSubmitRequest, "POST", new NameValueCollection());
                    //Logger.Info("返回：\r\n{0}", result);

                    var _examineInterface = ApplicationContext.Current.Provider.GetRequiredService<IExamineInterface>();
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
                    response.Message = "权限不足";
                }


            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})调客(Transferring)请求失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (transferringRequest != null ? JsonHelper.ToJson(transferringRequest) : ""));
            }
            return response;
        }

        /// <summary>
        /// 调客回调(内部使用)
        /// </summary>\
        /// <param name="examineResponse"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpPost("transfercallback")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage> TransferCallback([FromBody] ExamineResponse examineResponse)
        {
            Logger.Trace($"调客回调(TransferCallback)：\r\n请求参数为：\r\n" + (examineResponse != null ? JsonHelper.ToJson(examineResponse) : ""));

            ResponseMessage response = new ResponseMessage();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Warn("模型验证失败：\r\n{0}", response.Message ?? "");
                return response;
            }
            if (examineResponse.ContentType != "TransferCustomer")
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = "该回调实现不支持TransferCustomer的内容类型";
                Logger.Warn("调客业务层回调失败：\r\n{0}", response.Message ?? "");
                return response;
            }
            try
            {
                var transfer = JsonHelper.ToObject<TransferringRequest>(examineResponse.Content);
                if (examineResponse.ExamineStatus == ExamineStatus.Examined)
                {
                    await _customerInfoManager.TransferringListAsync(transfer);

                    //发送通知消息
                    SendMessageRequest sendMessageRequest = new SendMessageRequest();
                    sendMessageRequest.MessageTypeCode = "CustomerTransfer";
                    MessageItem messageItem = new MessageItem();
                    messageItem.UserIds = new List<string> { transfer.TerUserId };
                    messageItem.MessageTypeItems = new List<TypeItem> {
                    new TypeItem{ Key="NUM",Value=transfer.Customers.Count.ToString()  },
                    new TypeItem { Key="NAME",Value=transfer.SourceUserName},
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
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error("调客审核业务层回调失败：\r\n{0}", e.ToString());
            }
            return response;
        }




        /// <summary>
        /// 修改是否仍有购买意向
        /// </summary>
        /// <param name="user"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpPost("issellintention")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "CustomerInfoSellIntention" })]
        public async Task<ResponseMessage> UpdateSellIntention(UserInfo user, [FromBody]SellIntention condition)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})修改是否仍有购买意向(UpdateSellIntention)：\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));

            var response = new ResponseMessage();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Warn("模型验证失败：\r\n{0}", response.Message ?? "");
                return response;
            }
            try
            {
                response = await _customerInfoManager.UpdateSellIntention(user.Id, condition.CustomerId, condition.Mark, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = "服务器错误：" + e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})修改是否仍有购买意向(UpdateSellIntention)请求失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
            }
            return response;
        }
    }
}
