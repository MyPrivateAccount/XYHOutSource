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
    [Route("api/humansalary")]
    public class SalaryController : Controller
    {
        private readonly ILogger Logger = LoggerManager.GetLogger("XYHHumaninfo");
        private readonly StationManager _stationManage;
        private readonly RestClient _restClient;

        public SalaryController(RestClient rsc, StationManager sta)
        {
            _restClient = rsc;
            _stationManage = sta;
        }

        [HttpGet("stationlist/{department}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<List<PositionInfoResponse>>> GetStationList(UserInfo User, [FromRoute]string department)
        {
            var Response = new ResponseMessage<List<PositionInfoResponse>>();
            if (string.IsNullOrEmpty(department))
            {
                Response.Code = ResponseCodeDefines.ModelStateInvalid;
                Response.Message = "请求参数不正确";
            }

            try
            {
                Response.Extension = await _stationManage.GetStationListByDepartment(department, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                Response.Code = ResponseCodeDefines.ServiceError;
                Response.Message = "服务器错误：" + e.ToString();
                Logger.Error("error");
            }
            return Response;
        }

        [HttpPost("setstation")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage> SetStationInfo(UserInfo User, [FromBody]PositionInfoRequest position)
        {
            var pagingResponse = new ResponseMessage();
            if (!ModelState.IsValid)
            {
                pagingResponse.Code = ResponseCodeDefines.ModelStateInvalid;
                Logger.Warn($"用户{User?.UserName ?? ""}({User?.Id ?? ""})查询人事条件(PostCustomerListSaleMan)模型验证失败：\r\n{pagingResponse.Message ?? ""}，\r\n请求参数为：\r\n" + (position != null ? JsonHelper.ToJson(position) : ""));
                return pagingResponse;
            }

            try
            {
                await _stationManage.SetStation(position, HttpContext.RequestAborted);
                pagingResponse.Message = "setstation ok";
            }
            catch (Exception e)
            {
                pagingResponse.Code = ResponseCodeDefines.ServiceError;
                pagingResponse.Message = "服务器错误:" + e.ToString();
                Logger.Error($"用户{User?.UserName ?? ""}({User?.Id ?? ""})查询业务员条件(PostCustomerListSaleMan)请求失败：\r\n{pagingResponse.Message ?? ""}，\r\n请求参数为：\r\n" + (position != null ? JsonHelper.ToJson(position) : ""));

            }
            return pagingResponse;
        }

        [HttpPost("deletestation")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage> DeleteStationInfo(UserInfo User, [FromBody]PositionInfoRequest position)
        {
            var pagingResponse = new ResponseMessage();
            if (!ModelState.IsValid)
            {
                pagingResponse.Code = ResponseCodeDefines.ModelStateInvalid;
                Logger.Warn($"用户{User?.UserName ?? ""}({User?.Id ?? ""})查询人事条件(PostCustomerListSaleMan)模型验证失败：\r\n{pagingResponse.Message ?? ""}，\r\n请求参数为：\r\n" + (position != null ? JsonHelper.ToJson(position) : ""));
                return pagingResponse;
            }

            try
            {
                await _stationManage.DeleteStation(position, HttpContext.RequestAborted);
                pagingResponse.Message = "deletestation ok";
            }
            catch (Exception e)
            {
                pagingResponse.Code = ResponseCodeDefines.ServiceError;
                pagingResponse.Message = "服务器错误:" + e.ToString();
                Logger.Error($"用户{User?.UserName ?? ""}({User?.Id ?? ""})查询业务员条件(PostCustomerListSaleMan)请求失败：\r\n{pagingResponse.Message ?? ""}，\r\n请求参数为：\r\n" + (position != null ? JsonHelper.ToJson(position) : ""));

            }
            return pagingResponse;
        }
    }
}
