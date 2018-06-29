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
using AttendanceSettingInfoRequest = XYHHumanPlugin.Dto.Response.AttendanceSettingInfoResponse;
using AttendanceInfoRequest = XYHHumanPlugin.Dto.Response.AttendanceInfoResponse;

namespace XYHHumanPlugin.Controllers
{
    [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/humanattendance")]
    public class AttendanceController : Controller
    {
        private readonly ILogger Logger = LoggerManager.GetLogger("XYHHumanattendance");
        private readonly AttendanceManager _attendanceManage;
        public AttendanceController(AttendanceManager sta)
        {
            _attendanceManage = sta;
        }

        [HttpGet("attendancesetting")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<List<AttendanceSettingInfoResponse>>> GetAttendanceList(UserInfo User)
        {
            var Response = new ResponseMessage<List<AttendanceSettingInfoResponse>>();
            try
            {
                Response.Extension = await _attendanceManage.GetAttendenceSetting(HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                Response.Code = ResponseCodeDefines.ServiceError;
                Response.Message = "服务器错误：" + e.ToString();
                Logger.Error("error");
            }
            return Response;
        }

        [HttpPost("setattendancesetting")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage> SetAttendanceInfo(UserInfo User, [FromBody]List<AttendanceSettingInfoRequest> lst)
        {
            var pagingResponse = new ResponseMessage();
            if (!ModelState.IsValid)
            {
                pagingResponse.Code = ResponseCodeDefines.ModelStateInvalid;
                Logger.Warn($"用户{User?.UserName ?? ""}({User?.Id ?? ""})设置考勤金额信息(PostCustomerListSaleMan)模型验证失败：\r\n{pagingResponse.Message ?? ""}，\r\n请求参数为：\r\n" + (lst != null ? JsonHelper.ToJson(lst) : ""));
                return pagingResponse;
            }

            try
            {
                await _attendanceManage.SetAttendenceSetting(lst, HttpContext.RequestAborted);
                pagingResponse.Message = "setattendancesetting ok";
            }
            catch (Exception e)
            {
                pagingResponse.Code = ResponseCodeDefines.ServiceError;
                pagingResponse.Message = "服务器错误:" + e.ToString();
                Logger.Error($"用户{User?.UserName ?? ""}({User?.Id ?? ""})设置考勤金额信息(PostCustomerListSaleMan)请求失败：\r\n{pagingResponse.Message ?? ""}，\r\n请求参数为：\r\n" + (lst != null ? JsonHelper.ToJson(lst) : ""));

            }
            return pagingResponse;
        }

        [HttpPost("importattendancelst")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<HumanSearchResponse<AttendanceInfoResponse>>> ImportAttendenceLst(UserInfo User, [FromBody]List<AttendanceInfoRequest> lst)
        {
            var pagingResponse = new ResponseMessage<HumanSearchResponse<AttendanceInfoResponse>>();
            if (!ModelState.IsValid)
            {
                pagingResponse.Code = ResponseCodeDefines.ModelStateInvalid;
                Logger.Warn($"用户{User?.UserName ?? ""}({User?.Id ?? ""})导入考勤信息(PostCustomerListSaleMan)模型验证失败：\r\n{pagingResponse.Message ?? ""}，\r\n请求参数为：\r\n" + (lst != null ? JsonHelper.ToJson(lst) : ""));
                return pagingResponse;
            }

            try
            {
                await _attendanceManage.AddAttendence(lst, HttpContext.RequestAborted);
                AttendenceSearchRequest condition = new AttendenceSearchRequest() { pageIndex = 0, pageSize = 10, CreateDate = lst[0].Date.GetValueOrDefault() };
                pagingResponse.Extension = await _attendanceManage.SearchAttendenceInfo(User, condition, HttpContext.RequestAborted);

            }
            catch (Exception e)
            {
                pagingResponse.Code = ResponseCodeDefines.ServiceError;
                pagingResponse.Message = "服务器错误:" + e.ToString();
                Logger.Error($"用户{User?.UserName ?? ""}({User?.Id ?? ""})导入考勤信息(PostCustomerListSaleMan)请求失败：\r\n{pagingResponse.Message ?? ""}，\r\n请求参数为：\r\n" + (lst != null ? JsonHelper.ToJson(lst) : ""));

            }
            return pagingResponse;
        }

        [HttpPost("searchattendancelst")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<HumanSearchResponse<AttendanceInfoResponse>>> SearchAttendenceLst(UserInfo User, [FromBody]AttendenceSearchRequest condition)
        {
            var pagingResponse = new ResponseMessage<HumanSearchResponse<AttendanceInfoResponse>>();
            if (!ModelState.IsValid)
            {
                pagingResponse.Code = ResponseCodeDefines.ModelStateInvalid;
                Logger.Warn($"用户{User?.UserName ?? ""}({User?.Id ?? ""})查询考勤信息(PostCustomerListSaleMan)模型验证失败：\r\n{pagingResponse.Message ?? ""}，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
                return pagingResponse;
            }

            try
            {
                pagingResponse.Extension = await _attendanceManage.SearchAttendenceInfo(User, condition, HttpContext.RequestAborted);
                pagingResponse.Message = "searchattendencelst ok";
            }
            catch (Exception e)
            {
                pagingResponse.Code = ResponseCodeDefines.ServiceError;
                pagingResponse.Message = "服务器错误:" + e.ToString();
                Logger.Error($"用户{User?.UserName ?? ""}({User?.Id ?? ""})查询考勤信息(PostCustomerListSaleMan)请求失败：\r\n{pagingResponse.Message ?? ""}，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));

            }
            return pagingResponse;
        }

        [HttpPost("deleteattendanceitem/{id}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage> DeleteAttendenceItem(UserInfo User, [FromRoute]string id)
        {
            var pagingResponse = new ResponseMessage();
            if (!ModelState.IsValid)
            {
                pagingResponse.Code = ResponseCodeDefines.ModelStateInvalid;
                Logger.Warn($"用户{User?.UserName ?? ""}({User?.Id ?? ""})查询考勤信息(PostCustomerListSaleMan)模型验证失败：\r\n{pagingResponse.Message ?? ""}，\r\n请求参数为：\r\n" + (id != null ? JsonHelper.ToJson(id) : ""));
                return pagingResponse;
            }

            try
            {
                await _attendanceManage.DeleteAttendence(id, HttpContext.RequestAborted);
                pagingResponse.Message = "searchattendencelst ok";
            }
            catch (Exception e)
            {
                pagingResponse.Code = ResponseCodeDefines.ServiceError;
                pagingResponse.Message = "服务器错误:" + e.ToString();
                Logger.Error($"用户{User?.UserName ?? ""}({User?.Id ?? ""})查询考勤信息(PostCustomerListSaleMan)请求失败：\r\n{pagingResponse.Message ?? ""}，\r\n请求参数为：\r\n" + (id != null ? JsonHelper.ToJson(id) : ""));

            }
            return pagingResponse;
        }

    }
}
