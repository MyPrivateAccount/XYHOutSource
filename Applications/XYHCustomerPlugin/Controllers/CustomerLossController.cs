using ApplicationCore;
using ApplicationCore.Dto;
using ApplicationCore.Filters;
using ApplicationCore.Managers;
using AspNet.Security.OAuth.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XYH.Core.Log;
using XYHCustomerPlugin.Dto;
using XYHCustomerPlugin.Managers;
using XYHCustomerPlugin.Models;

namespace XYHCustomerPlugin.Controllers
{
    [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/customerloss")]
    public class CustomerLossController : Controller
    {
        private readonly PermissionExpansionManager _permissionExpansionManager;
        private readonly CustomerLossManager _customerLossManager;
        private readonly ILogger Logger = LoggerManager.GetLogger("CustomerLoss");

        public CustomerLossController(CustomerLossManager customerLossManager, PermissionExpansionManager permissionExpansionManager)
        {
            _permissionExpansionManager = permissionExpansionManager ?? throw new ArgumentNullException(nameof(permissionExpansionManager));
            _customerLossManager = customerLossManager ?? throw new ArgumentNullException(nameof(customerLossManager));
        }

        //[HttpGet("{id}")]
        //[TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        //public async Task<ResponseMessage<CustomerLossResponse>> GetCustomerLossByCoustomerId(string userid, [FromRoute] string id)
        //{
        //    ResponseMessage<CustomerLossResponse> response = new ResponseMessage<CustomerLossResponse>();
        //    try
        //    {
        //        response.Extension = await _customerLossManager.FindByIdAsync(userid, id, HttpContext.RequestAborted);
        //    }
        //    catch (Exception e)
        //    {
        //        response.Code = ResponseCodeDefines.ServiceError;
        //        response.Message = e.ToString();
        //    }
        //    return response;
        //}

        /// <summary>
        /// 获取失效列表 
        /// </summary>
        /// <param name="user"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpPost("list")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<PagingResponseMessage<CustomerLossResponse>> GetList(UserInfo user, [FromBody]CustomerLossCondition condition)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})获取失效列表(GetList)：\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));

            PagingResponseMessage<CustomerLossResponse> response = new PagingResponseMessage<CustomerLossResponse>();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})获取失效列表(GetList)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
                return response;
            }
            try
            {
                if (await _permissionExpansionManager.HavePermission(user.Id, "CustomerLossRetrieve"))
                    return await _customerLossManager.Search(user.Id, condition, HttpContext.RequestAborted);
                else
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Message = "权限不足";
                }
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})获取失效列表(GetList)请求失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
            }
            return response;
        }


        /// <summary>
        /// 新增客户失效
        /// </summary>
        /// <param name="user"></param>
        /// <param name="customerLossRequest"></param>
        /// <returns></returns>
        [HttpPost("customerloss")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<CustomerLossResponse>> CreateCustomerLoss(UserInfo user, [FromBody]CustomerLossRequest customerLossRequest)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})新增客户失效(CreateCustomerLoss)：\r\n请求参数为：\r\n" + (customerLossRequest != null ? JsonHelper.ToJson(customerLossRequest) : ""));

            ResponseMessage<CustomerLossResponse> response = new ResponseMessage<CustomerLossResponse>();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})新增客户失效(CreateCustomerLoss)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (customerLossRequest != null ? JsonHelper.ToJson(customerLossRequest) : ""));
                return response;
            }
            try
            {
                return await _customerLossManager.CreateAsync(user, customerLossRequest, HttpContext.RequestAborted);
                //if (response.Extension == null)
                //{
                //    response.Code = ResponseCodeDefines.NotAllow;
                //    response.Message = "拉无效失败";
                //}
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})新增客户失效(CreateCustomerLoss)请求失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (customerLossRequest != null ? JsonHelper.ToJson(customerLossRequest) : ""));
            }
            return response;
        }

        /// <summary>
        /// 激活客户
        /// </summary>
        /// <param name="user"></param>
        /// <param name="customerId"></param>
        /// <param name="isDeleteOldData"></param>
        /// <returns></returns>
        [HttpGet("{customerId}/activation/{isDeleteOldData}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage> ActivateLossCustomer(UserInfo user, [FromRoute]string customerId, [FromRoute]bool isDeleteOldData)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})激活客户(ActivateLossCustomer)：\r\n请求参数为：\r\n(customerId){customerId ?? ""}\r\n(isDeleteOldData){isDeleteOldData}");

            ResponseMessage response = new ResponseMessage();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                return response;
            }
            try
            {
                if (!await _customerLossManager.Activate(user, customerId, isDeleteOldData, HttpContext.RequestAborted))
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                }
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})激活客户(ActivateLossCustomer)请求失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n(customerId){customerId ?? ""}\r\n(isDeleteOldData){isDeleteOldData}");
            }
            return response;
        }


        //[HttpPut("{id}")]
        //[TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        //public async Task<ResponseMessage<DictionaryDefine>> PostDictionaryGroup(string userId, [FromBody]DictionaryDefineRequest dictionaryDefineRequest)
        //{
        //    ResponseMessage<DictionaryDefine> response = new ResponseMessage<DictionaryDefine>();
        //    if (!ModelState.IsValid)
        //    {
        //        response.Code = ResponseCodeDefines.ModelStateInvalid;
        //        return response;
        //    }
        //    try
        //    {
        //        response.Extension = await _dictionaryDefineManager.CreateAsync(dictionaryDefineRequest.ToDataModel(userId), HttpContext.RequestAborted);
        //    }
        //    catch (Exception e)
        //    {
        //        response.Code = ResponseCodeDefines.ServiceError;
        //        response.Message = e.ToString();
        //    }
        //    return response;
        //}

        /// <summary>
        /// 删除失效
        /// </summary>
        /// <param name="user"></param>
        /// <param name="customerIdList"></param>
        /// <returns></returns>
        [HttpDelete("delete")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage> DeleteCustomerLoss(UserInfo user, [FromBody]List<string> customerIdList)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})删除失效(DeleteCustomerLoss)：\r\n请求参数为：\r\n" + (customerIdList != null ? JsonHelper.ToJson(customerIdList) : ""));

            ResponseMessage response = new ResponseMessage();
            try
            {
                await _customerLossManager.DeleteListAsync(user.Id, customerIdList, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})删除失效(DeleteCustomerLoss)请求失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (customerIdList != null ? JsonHelper.ToJson(customerIdList) : ""));
            }
            return response;
        }



    }
}
