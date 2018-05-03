using ApplicationCore;
using ApplicationCore.Dto;
using ApplicationCore.Filters;
using AspNet.Security.OAuth.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XYH.Core.Log;
using XYHHumanPlugin.Dto.Request;
using XYHHumanPlugin.Dto.Response;
using XYHHumanPlugin.Managers;

namespace XYHHumanPlugin.Controllers
{
    [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/month")]
    public class MonthInfoController : Controller
    {
        private readonly ILogger Logger = LoggerManager.GetLogger("XYHMonthinfo");
        private readonly MonthManager _monthManage;
        private readonly RestClient _restClient;

        public MonthInfoController(RestClient rsc, MonthManager month)
        {
            _monthManage = month;
            _restClient = rsc;
        }

        [HttpGet("lastmonth")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<DateTime>> GetLastMonth([FromRoute]string testinfo)
        {
            var Response = new ResponseMessage<DateTime>();
            if (string.IsNullOrEmpty(testinfo))
            {
                Response.Code = ResponseCodeDefines.ModelStateInvalid;
                Response.Message = "请求参数不正确";
            }
            try
            {
                var lastmonth = await _monthManage.GetLastMonth();
                Response.Extension = lastmonth.SettleTime.Value;
            }
            catch (Exception e)
            {
                Response.Code = ResponseCodeDefines.ServiceError;
                Response.Message = "服务器错误：" + e.ToString();
                Logger.Error("error");
            }
            return Response;
        }

        [HttpPost("monthlist")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<SearchMonthInfoResponse>> PostMonthList([FromBody] MonthRequest req)
        {
            var Response = new ResponseMessage<SearchMonthInfoResponse>();
           
            try
            {
                Response.Extension = await _monthManage.GetAllMonthInfo(req, HttpContext.RequestAborted);
                Response.Message = "获取月结成功";
            }
            catch (Exception e)
            {
                Response.Code = ResponseCodeDefines.ServiceError;
                Response.Message = "服务器错误：" + e.ToString();
                Logger.Error("error");
            }
            return Response;
        }

        [HttpPost("backmonth")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<MonthInfoResponse>> BackLastMonth(UserInfo User, [FromBody]DateTime lasttime)
        {
            var Response = new ResponseMessage<MonthInfoResponse>();
            if (!ModelState.IsValid)
            {
                Response.Code = ResponseCodeDefines.ModelStateInvalid;
                Logger.Warn($"用户{User?.UserName ?? ""}({User?.Id ?? ""})查询月结条件(PostCustomerListSaleMan)模型验证失败：\r\n{Response.Message ?? ""}，\r\n请求参数为：\r\n");
                return Response;
            }

            try
            {
                await _monthManage.DeleteMonth(lasttime, HttpContext.RequestAborted);
                Response.Message = "恢复月结成功";
            }
            catch (Exception e)
            {
                Response.Code = ResponseCodeDefines.ServiceError;
                Response.Message = "服务器错误:" + e.ToString();
                Logger.Error($"用户{User?.UserName ?? ""}({User?.Id ?? ""})查询月结条件(PostCustomerListSaleMan)请求失败：\r\n{Response.Message ?? ""}，\r\n请求参数为：\r\n");

            }
            return Response;
        }

        [HttpPost("createmonth")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<HumanSearchResponse<HumanInfoResponse>> CreateMonth(UserInfo User, [FromBody]DateTime nextmonth)
        {
            var pagingResponse = new HumanSearchResponse<HumanInfoResponse>();
            if (!ModelState.IsValid)
            {
                pagingResponse.Code = ResponseCodeDefines.ModelStateInvalid;
                Logger.Warn($"用户{User?.UserName ?? ""}({User?.Id ?? ""})创建月结信息失败：\r\n{pagingResponse.Message ?? ""}，\r\n请求参数为：\r\n");
                return pagingResponse;
            }

            try
            {
                DateTime now = DateTime.Now;
                var temp = await _monthManage.GetLastMonth();

                if (temp != null && temp.SettleTime.Value.ToString("Y") == now.ToString("Y"))
                {
                    pagingResponse.Code = ResponseCodeDefines.ServiceError;
                    pagingResponse.Message = "日期重复";
                    return pagingResponse;
                }

                await _monthManage.CreateMonth(User, now, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                pagingResponse.Code = ResponseCodeDefines.ServiceError;
                pagingResponse.Message = "服务器错误:" + e.ToString();
                Logger.Error($"用户{User?.UserName ?? ""}({User?.Id ?? ""})创建月结信息失败：\r\n{pagingResponse.Message ?? ""}，\r\n请求参数为：\r\n");

            }
            return pagingResponse;
        }
    }
}
