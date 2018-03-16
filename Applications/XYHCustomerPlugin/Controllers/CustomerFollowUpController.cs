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
using System.Threading.Tasks;
using XYH.Core.Log;
using XYHCustomerPlugin.Dto.Request;
using XYHCustomerPlugin.Dto.Response;
using XYHCustomerPlugin.Managers;

namespace XYHCustomerPlugin.Controllers
{
    [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/customerfollowup")]
    public class CustomerFollowUpController : Controller
    {
        #region 成员
        private readonly PermissionExpansionManager _permissionExpansionManager;
        private readonly CustomerFollowUpManager _customerFollowUpManager;
        private readonly IMapper _mapper;
        private readonly ILogger Logger = LoggerManager.GetLogger("CustomerFollowUp");

        #endregion

        /// <summary>
        /// 带看
        /// </summary>
        public CustomerFollowUpController(CustomerFollowUpManager customerFollowUpManager, IMapper mapper, PermissionExpansionManager permissionExpansionManager)
        {
            _permissionExpansionManager = permissionExpansionManager ?? throw new ArgumentNullException(nameof(permissionExpansionManager));
            _customerFollowUpManager = customerFollowUpManager;
            _mapper = mapper;
        }

        /// <summary>
        /// 根据Userid查询跟进信息
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        //[HttpGet("[action]")]
        //[TypeFilter(typeof(CheckPermission), Arguments = new object[] { "CustomerFollowUpById" })]
        //public async Task<ResponseMessage<List<FollowUpResponse>>> GetCustomerFollowUpByUserid(string userid)
        //{
        //    var response = new ResponseMessage<List<FollowUpResponse>>();
        //    if (string.IsNullOrEmpty(userid))
        //    {
        //        response.Code = ResponseCodeDefines.ModelStateInvalid;
        //        return response;
        //    }
        //    try
        //    {
        //        response.Extension = await _customerFollowUpManager.FindByUserIdAsync(userid, HttpContext.RequestAborted);
        //    }
        //    catch (Exception e)
        //    {
        //        response.Code = ResponseCodeDefines.ServiceError;
        //        response.Message = e.ToString();
        //    }
        //    return response;
        //}

        /// <summary>
        /// 根据Costomerid查询跟进信息
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="costomerid"></param>
        /// <returns></returns>
        //[HttpGet("{costomerid}")]
        //[TypeFilter(typeof(CheckPermission), Arguments = new object[] { "CustomerFollowUpByCustomerId" })]
        //public async Task<ResponseMessage<List<FollowUpResponse>>> GetCustomerFollowUpByCostomerid(string userid, [FromRoute]string costomerid)
        //{
        //    var response = new ResponseMessage<List<FollowUpResponse>>();
        //    if (string.IsNullOrEmpty(userid))
        //    {
        //        response.Code = ResponseCodeDefines.ModelStateInvalid;
        //        return response;
        //    }
        //    try
        //    {
        //        response.Extension = await _customerFollowUpManager.FindByCustomerIdAsync(userid, costomerid, HttpContext.RequestAborted);
        //    }
        //    catch (Exception e)
        //    {
        //        response.Code = ResponseCodeDefines.ServiceError;
        //        response.Message = e.ToString();
        //    }
        //    return response;
        //}

        /// <summary>
        /// 根据带看Id查询信息
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="customerFollowUp"></param>
        /// <returns></returns>
        //[HttpGet("{customerFollowUp}")]
        //[TypeFilter(typeof(CheckPermission), Arguments = new object[] { "CustomerFollowUpById" })]
        //public async Task<ResponseMessage<FollowUpResponse>> GetCustomerFollowUp(string userid, [FromRoute] string customerFollowUp)
        //{
        //    var response = new ResponseMessage<FollowUpResponse>();
        //    if (string.IsNullOrEmpty(customerFollowUp))
        //    {
        //        response.Code = ResponseCodeDefines.ModelStateInvalid;
        //        return response;
        //    }
        //    try
        //    {
        //        response.Extension = await _customerFollowUpManager.FindByIdAsync(userid, customerFollowUp, HttpContext.RequestAborted);
        //    }
        //    catch (Exception e)
        //    {
        //        response.Code = ResponseCodeDefines.ServiceError;
        //        response.Message = e.ToString();
        //    }
        //    return response;
        //}

        /// <summary>
        /// 新增带看信息
        /// </summary>
        /// <param name="user">用户Id</param>
        /// <param name="followUpRequest">增加实体</param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "CustomerFollowUpAdd" })]
        public async Task<ResponseMessage<FollowUpResponse>> PostCustomerFollowUp(UserInfo user, [FromBody]FollowUpRequest followUpRequest)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})新增带看信息(PostCustomerFollowUp)：\r\n请求参数为：\r\n" + (followUpRequest != null ? JsonHelper.ToJson(followUpRequest) : ""));

            var response = new ResponseMessage<FollowUpResponse>();
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
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})新增带看信息(PostCustomerFollowUp)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (followUpRequest != null ? JsonHelper.ToJson(followUpRequest) : ""));
                return response;
            }
            response = await _customerFollowUpManager.CreateAsync(user, followUpRequest, HttpContext.RequestAborted);
            if (response == null)
            {
                response.Code = ResponseCodeDefines.NotAllow;
                response.Message = "新增失败";
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})新增带看信息(PostCustomerFollowUp)请求失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (followUpRequest != null ? JsonHelper.ToJson(followUpRequest) : ""));
            }
            return response;
        }
    }
}
