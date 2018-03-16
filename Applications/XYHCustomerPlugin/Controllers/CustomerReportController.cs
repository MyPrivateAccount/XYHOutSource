using ApplicationCore;
using ApplicationCore.Dto;
using ApplicationCore.Filters;
using AspNet.Security.OAuth.Validation;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
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
    [Route("api/customerReport")]
    public class CustomerReportController : Controller
    {

        #region 成员

        private readonly CustomerReportManager _customerReportManager;
        private readonly IMapper _mapper;
        private readonly ILogger Logger = LoggerManager.GetLogger("CustomerReport");

        #endregion

        /// <summary>
        /// 报备信息
        /// </summary>
        public CustomerReportController(CustomerReportManager customerReportManager, IMapper mapper)
        {
            _customerReportManager = customerReportManager;
            _mapper = mapper;
        }

        /// <summary>
        /// 新增报备信息
        /// </summary>
        /// <param name="user">用户Id</param>
        /// <param name="customerReportRequest">增加实体</param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "CustomerReportCreate" })]
        public async Task<ResponseMessage<CustomerReportResponse>> PostCustomerInfo(UserInfo user, [FromBody]CustomerReportRequest customerReportRequest)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})新增报备信息(PostCustomerInfo)：\r\n请求参数为：\r\n" + (customerReportRequest != null ? JsonHelper.ToJson(customerReportRequest) : ""));

            var response = new ResponseMessage<CustomerReportResponse>();
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
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})新增报备信息(PostCustomerInfo)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (customerReportRequest != null ? JsonHelper.ToJson(customerReportRequest) : ""));
                return response;
            }
            try
            {
                response.Extension = await _customerReportManager.CreateAsync(user, customerReportRequest, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})新增报备信息(PostCustomerInfo)请求失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (customerReportRequest != null ? JsonHelper.ToJson(customerReportRequest) : ""));

            }
            return response;
        }

        /// <summary>
        /// 删除单个报备
        /// </summary>
        /// <param name="User">删除用户</param>
        /// <param name="customerReportId"></param>
        /// <returns></returns>
        [HttpDelete("{customerReportId}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "CustomerReportDelete" })]
        public async Task<ResponseMessage> DeleteCustomerReport(UserInfo user, [FromRoute] string customerReportId)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})  删除单个报备(DeleteCustomerReport)：\r\n请求参数为：\r\n(customerReportId){customerReportId ?? ""}");

            ResponseMessage response = new ResponseMessage();
            if (string.IsNullOrEmpty(customerReportId))
            {
                response.Code = ResponseCodeDefines.ArgumentNullError;
                response.Message = "参数出错";
                return response;
            }
            try
            {
                await _customerReportManager.DeleteAsync(user, customerReportId, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})  删除单个报备(DeleteCustomerReport)报错：\r\n{e.ToString()}，\r\n请求参数为：\r\n(customerReportId){customerReportId ?? ""}");
            }
            return response;
        }

        /// <summary>
        /// 修改客源信息
        /// </summary>
        /// <param name="user">用户Id</param>
        /// <param name="customerReportRequest">修改实体</param>
        /// <returns></returns>
        [HttpPut]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "CustomerReportUpdate" })]
        public async Task<ResponseMessage> PutCustomerInfo(UserInfo user, [FromBody]CustomerReportRequest customerReportRequest)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})修改客源信息(PutCustomerInfo)：\r\n请求参数为：\r\n" + (customerReportRequest != null ? JsonHelper.ToJson(customerReportRequest) : ""));

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
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})修改客源信息(PutCustomerInfo)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (customerReportRequest != null ? JsonHelper.ToJson(customerReportRequest) : ""));
                return response;
            }
            try
            {
                var dictionaryGroup = await _customerReportManager.FindByIdAsync(customerReportRequest.Id, HttpContext.RequestAborted);
                if (dictionaryGroup == null)
                {
                    await _customerReportManager.CreateAsync(user, customerReportRequest, HttpContext.RequestAborted);
                }
                await _customerReportManager.UpdateAsync(user.Id, customerReportRequest, HttpContext.RequestAborted);
                response.Code = ResponseCodeDefines.SuccessCode;
                response.Message = "修改成功";
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})修改客源信息(PutCustomerInfo)请求失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (customerReportRequest != null ? JsonHelper.ToJson(customerReportRequest) : ""));
            }
            return response;
        }

        /// <summary>
        /// 驻场确认报备信息
        /// </summary>
        /// <param name="user">用户</param>
        /// <param name="reportId">修改实体</param>
        /// <returns></returns>
        [HttpPut("{reportId}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "CustomerReportUpdateStatus" })]
        public async Task<ResponseMessage> PutCustomerReportByStatus(UserInfo user, [FromRoute]string reportId)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})  驻场确认报备信息(PutCustomerReportByStatus)：\r\n请求参数为：\r\n(reportId){reportId ?? ""}");

            var response = new ResponseMessage();
            response.Code = ResponseCodeDefines.NotFound;
            response.Message = "修改失败";
            if (string.IsNullOrEmpty(reportId))
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = "报备ID为空";
            }
            else
            {
                try
                {
                    await _customerReportManager.UpdateAsyncStatus(user.Id, reportId, HttpContext.RequestAborted);
                    response.Code = ResponseCodeDefines.SuccessCode;
                    response.Message = "修改成功";
                }
                catch (Exception e)
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Message = "服务器错误：" + e.ToString();
                    Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})  驻场确认报备信息(PutCustomerReportByStatus)报错：\r\n{e.ToString()}，\r\n请求参数为：\r\n(reportId){reportId ?? ""}");
                }
            }
            return response;
        }

        /// <summary>
        /// 查看已报备的信息
        /// </summary>
        /// <param name="user">用户Id</param>
        /// <param name="condition">修改实体</param>
        /// <returns></returns>
        [HttpPost("[action]")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "CustomerReportSelectMy" })]
        public async Task<PagingResponseMessage<CustomerReportResponse>> GetMyCustomerReport(UserInfo user, [FromBody]CustomerPageCondition condition)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})查看已报备的信息(GetMyCustomerReport)：\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));

            var response = new PagingResponseMessage<CustomerReportResponse>();
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
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})查看已报备的信息(GetMyCustomerReport)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
            }
            else
            {
                try
                {
                    response = await _customerReportManager.FindMyCustomerReport(user.Id, condition, HttpContext.RequestAborted);
                    response.Code = ResponseCodeDefines.SuccessCode;
                    response.Message = "查询成功";
                }
                catch (Exception e)
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Message = "服务器错误：" + e.ToString();
                    Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})查看已报备的信息(GetMyCustomerReport)请求失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
                }

            }
            return response;
        }

        /// <summary>
        /// 查询报备信息
        /// </summary>
        /// <param name="userId">用户Id</param>
        /// <param name="customerReportRequest">修改实体</param>
        /// <returns></returns>
        //[HttpPost("[action]")]
        //[TypeFilter(typeof(CheckPermission), Arguments = new object[] { "CustomerReportSelectMy" })]
        //public async Task<PagingResponseMessage<CustomerReportResponse>> Search(string userid, [FromBody]CustomerReportSearchRequest searchcondition)
        //{
        //    var response = new PagingResponseMessage<CustomerReportResponse>();
        //    response.Code = ResponseCodeDefines.NotFound;
        //    response.Message = "未查询到信息";
        //    if (!ModelState.IsValid)
        //    {
        //        var error = "";
        //        var errors = ModelState.Values.ToList();
        //        foreach (var item in errors)
        //        {
        //            foreach (var e in item.Errors)
        //            {
        //                error += e.ErrorMessage + "  ";
        //            }
        //        }
        //        response.Code = ResponseCodeDefines.ModelStateInvalid;
        //        response.Message = error;
        //    }
        //    else
        //    {
        //        try
        //        {
        //            response = await _customerReportManager.Search(userid, searchcondition, HttpContext.RequestAborted);
        //            response.Code = ResponseCodeDefines.SuccessCode;
        //            response.Message = "查询成功";
        //        }
        //        catch (Exception e)
        //        {
        //            response.Code = ResponseCodeDefines.ServiceError;
        //            response.Message = "服务器错误：" + e.ToString();
        //        }

        //    }
        //    return response;
        //}
    }
}
