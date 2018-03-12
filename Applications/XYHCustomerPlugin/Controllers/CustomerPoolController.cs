using ApplicationCore;
using ApplicationCore.Dto;
using ApplicationCore.Filters;
using ApplicationCore.Managers;
using AspNet.Security.OAuth.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XYH.Core.Log;
using XYHCustomerPlugin.Dto;
using XYHCustomerPlugin.Managers;

namespace XYHCustomerPlugin.Controllers
{
    [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/customerpool")]
    public class CustomerPoolController : Controller
    {
        private readonly PermissionExpansionManager _permissionExpansionManager;
        private readonly CustomerPoolManager _customerPoolManager;

        private readonly ILogger Logger = LoggerManager.GetLogger("CustomerPool");

        public CustomerPoolController(CustomerPoolManager customerPoolManager,
             PermissionExpansionManager permissionExpansionManager)
        {
            _permissionExpansionManager = permissionExpansionManager ?? throw new ArgumentNullException(nameof(permissionExpansionManager));
            _customerPoolManager = customerPoolManager ?? throw new ArgumentNullException(nameof(customerPoolManager));
        }

        /// <summary>
        /// 获取一个公客池中所有的客户
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="poolId"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        //[HttpPost("list/{poolId}")]
        //[TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        //public async Task<PagingResponseMessage<CustomerPoolResponse>> GetCustomerListByPoolId(string userid, [FromRoute] string poolId, [FromBody]CustomerPoolCondition condition)
        //{
        //    PagingResponseMessage<CustomerPoolResponse> response = new PagingResponseMessage<CustomerPoolResponse>();
        //    try
        //    {
        //        return await _customerPoolManager.FindByPoolIdAsync(poolId, condition, HttpContext.RequestAborted);
        //    }
        //    catch (Exception e)
        //    {
        //        response.Code = ResponseCodeDefines.ServiceError;
        //        response.Message = e.ToString();
        //    }
        //    return response;
        //}

        /// <summary>
        /// 认领客户
        /// </summary>
        /// <param name="user"></param>
        /// <param name="customerPoolClaimRequest"></param>
        /// <returns></returns>
        [HttpPost("claim")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage> ClaimCustomer(UserInfo user, [FromBody]CustomerPoolClaimRequest customerPoolClaimRequest)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})认领客户(ClaimCustomer)：\r\n请求参数为：\r\n" + (customerPoolClaimRequest != null ? JsonHelper.ToJson(customerPoolClaimRequest) : ""));

            ResponseMessage response = new ResponseMessage();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})认领客户(ClaimCustomer)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (customerPoolClaimRequest != null ? JsonHelper.ToJson(customerPoolClaimRequest) : ""));
                return response;
            }
            try
            {
                await _customerPoolManager.ClaimCustomer(user, customerPoolClaimRequest, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})认领客户(ClaimCustomer)请求失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (customerPoolClaimRequest != null ? JsonHelper.ToJson(customerPoolClaimRequest) : ""));
            }
            return response;
        }

        /// <summary>
        /// 将客户移到公客池
        /// </summary>
        /// <param name="user"></param>
        /// <param name="customerPoolJoinRequest"></param>
        /// <returns></returns>
        [HttpGet("join")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage> JoinCustomer(UserInfo user, [FromBody]CustomerPoolJoinRequest customerPoolJoinRequest)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})将客户移到公客池(JoinCustomer)：\r\n请求参数为：\r\n" + (customerPoolJoinRequest != null ? JsonHelper.ToJson(customerPoolJoinRequest) : ""));

            ResponseMessage response = new ResponseMessage();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})将客户移到公客池(JoinCustomer)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (customerPoolJoinRequest != null ? JsonHelper.ToJson(customerPoolJoinRequest) : ""));
                return response;
            }
            try
            {
                await _customerPoolManager.JoinCustomer(user, customerPoolJoinRequest, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})将客户移到公客池(JoinCustomer)请求失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (customerPoolJoinRequest != null ? JsonHelper.ToJson(customerPoolJoinRequest) : ""));
            }
            return response;
        }


        /// <summary>
        /// 将客户从一个公客池移动到另一个公客池
        /// </summary>
        /// <param name="user"></param>
        /// <param name="customerChangePoolRequest"></param>
        /// <returns></returns>
        [HttpPost("changepool")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage> CustomerChangePool(UserInfo user, [FromBody]CustomerChangePoolRequest customerChangePoolRequest)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})将客户从一个公客池移动到另一个公客池(CustomerChangePool)：\r\n请求参数为：\r\n" + (customerChangePoolRequest != null ? JsonHelper.ToJson(customerChangePoolRequest) : ""));

            ResponseMessage<CustomerPoolDefineResponse> response = new ResponseMessage<CustomerPoolDefineResponse>();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})将客户从一个公客池移动到另一个公客池(CustomerChangePool)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (customerChangePoolRequest != null ? JsonHelper.ToJson(customerChangePoolRequest) : ""));
                return response;
            }
            try
            {
                await _customerPoolManager.ChangePool(user, customerChangePoolRequest, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})将客户从一个公客池移动到另一个公客池(CustomerChangePool)请求失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (customerChangePoolRequest != null ? JsonHelper.ToJson(customerChangePoolRequest) : ""));
            }
            return response;
        }

    }
}
