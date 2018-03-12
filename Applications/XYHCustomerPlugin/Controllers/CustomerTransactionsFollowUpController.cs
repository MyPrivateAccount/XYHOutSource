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
    [Route("api/customertransactionsfollowup")]
    public class CustomerTransactionsFollowUpController : Controller
    {
        #region 成员
        private readonly PermissionExpansionManager _permissionExpansionManager;
        private readonly CustomerTransactionsFollowUpManager _customerTransactionsFollowUpManager;
        private readonly IMapper _mapper;
        private readonly ILogger Logger = LoggerManager.GetLogger("CustomerFollowUp");

        #endregion

        /// <summary>
        /// 成交
        /// </summary>
        public CustomerTransactionsFollowUpController(CustomerTransactionsFollowUpManager customerTransactionsFollowUpManager, IMapper mapper,
             PermissionExpansionManager permissionExpansionManager)
        {
            _permissionExpansionManager = permissionExpansionManager ?? throw new ArgumentNullException(nameof(permissionExpansionManager));
            _customerTransactionsFollowUpManager = customerTransactionsFollowUpManager;
            _mapper = mapper;
        }

        /// <summary>
        /// 根据成交Id查询信息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="customerTransactionsFollowUpId"></param>
        /// <returns></returns>
        [HttpGet("{customerFollowUp}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "CustomerFollowUpById" })]
        public async Task<ResponseMessage<TransactionsFollowUpResponse>> GetCustomerFollowUp(UserInfo user, [FromRoute] string customerTransactionsFollowUpId)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})根据成交Id查询信息(GetCustomerFollowUp)：\r\n请求参数为：\r\n" + (customerTransactionsFollowUpId != null ? JsonHelper.ToJson(customerTransactionsFollowUpId) : ""));

            var response = new ResponseMessage<TransactionsFollowUpResponse>();
            if (string.IsNullOrEmpty(customerTransactionsFollowUpId))
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                return response;
            }
            try
            {
                response.Extension = await _customerTransactionsFollowUpManager.FindByIdAsync(customerTransactionsFollowUpId, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})根据成交Id查询信息(GetCustomerFollowUp)请求失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (customerTransactionsFollowUpId != null ? JsonHelper.ToJson(customerTransactionsFollowUpId) : ""));
            }
            return response;
        }

        /// <summary>
        /// 新增成交信息
        /// </summary>
        /// <param name="user">用户</param>
        /// <param name="transactionsFollowUpRequest">增加实体</param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "CustomerFollowUpAdd" })]
        public async Task<ResponseMessage<TransactionsFollowUpResponse>> PostCustomerFollowUp(UserInfo user, [FromBody]TransactionsFollowUpRequest transactionsFollowUpRequest)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})新增成交信息(PostCustomerFollowUp)：\r\n请求参数为：\r\n" + (transactionsFollowUpRequest != null ? JsonHelper.ToJson(transactionsFollowUpRequest) : ""));

            var response = new ResponseMessage<TransactionsFollowUpResponse>();
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
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})新增成交信息(PostCustomerFollowUp)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (transactionsFollowUpRequest != null ? JsonHelper.ToJson(transactionsFollowUpRequest) : ""));
                return response;
            }
            try
            {
                response = await _customerTransactionsFollowUpManager.CreateAsync(user, transactionsFollowUpRequest, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})新增成交信息(PostCustomerFollowUp)请求失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (transactionsFollowUpRequest != null ? JsonHelper.ToJson(transactionsFollowUpRequest) : ""));
            }
            return response;
        }
    }
}
