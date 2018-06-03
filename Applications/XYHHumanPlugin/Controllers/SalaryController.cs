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
using SalaryInfoRequest = XYHHumanPlugin.Dto.Response.SalaryInfoResponse;

namespace XYHHumanPlugin.Controllers
{
    [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/humansalary")]
    public class SalaryController : Controller
    {
        private readonly ILogger Logger = LoggerManager.GetLogger("XYHHumansalary");
        private readonly SalaryManager _salaryManage;
        private readonly RestClient _restClient;

        public SalaryController(RestClient rsc, SalaryManager sta)
        {
            _restClient = rsc;
            _salaryManage = sta;
        }
        
        [HttpPost("salarylist")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<HumanSearchResponse<SalaryInfoResponse>>> GetSalaryList(UserInfo User, [FromBody]HumanSearchRequest searchinfo)
        {
            var Response = new ResponseMessage<HumanSearchResponse<SalaryInfoResponse>>();
            if (!ModelState.IsValid)
            {
                Response.Code = ResponseCodeDefines.ModelStateInvalid;
                Logger.Warn($"用户{User?.UserName ?? ""}({User?.Id ?? ""})查询薪酬条件(PostCustomerListSaleMan)模型验证失败：\r\n{Response.Message ?? ""}，\r\n请求参数为：\r\n" + (searchinfo != null ? JsonHelper.ToJson(searchinfo) : ""));
                return Response;
            }

            try
            {
                Response.Extension = await _salaryManage.SearchSalaryInfo(User, searchinfo, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                Response.Code = ResponseCodeDefines.ServiceError;
                Response.Message = "服务器错误：" + e.ToString();
                Logger.Error("error");
            }
            return Response;
        }

        [HttpGet("salaryitem/{positonid}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<SalaryInfoResponse>> GetSalaryItem(UserInfo User, [FromRoute]string positonid)
        {
            var Response = new ResponseMessage<SalaryInfoResponse>();
            if (!ModelState.IsValid)
            {
                Response.Code = ResponseCodeDefines.ModelStateInvalid;
                Logger.Warn($"用户{User?.UserName ?? ""}({User?.Id ?? ""})查询薪酬条件(PostCustomerListSaleMan)模型验证失败：\r\n{Response.Message ?? ""}，\r\n请求参数为：\r\n" + (positonid != null ? JsonHelper.ToJson(positonid) : ""));
                return Response;
            }

            try
            {
                Response.Extension = await _salaryManage.GetSalaryItem(positonid, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                Response.Code = ResponseCodeDefines.ServiceError;
                Response.Message = "服务器错误：" + e.ToString();
                Logger.Error("error");
            }
            return Response;
        }

        [HttpPost("setsalary")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage> SetSalaryInfo(UserInfo User, [FromBody]SalaryInfoRequest position)
        {
            var pagingResponse = new ResponseMessage();
            if (!ModelState.IsValid)
            {
                pagingResponse.Code = ResponseCodeDefines.ModelStateInvalid;
                Logger.Warn($"用户{User?.UserName ?? ""}({User?.Id ?? ""})设置薪酬条件(PostCustomerListSaleMan)模型验证失败：\r\n{pagingResponse.Message ?? ""}，\r\n请求参数为：\r\n" + (position != null ? JsonHelper.ToJson(position) : ""));
                return pagingResponse;
            }

            try
            {
                await _salaryManage.SetSalary(position, HttpContext.RequestAborted);
                pagingResponse.Message = "setstation ok";
            }
            catch (Exception e)
            {
                pagingResponse.Code = ResponseCodeDefines.ServiceError;
                pagingResponse.Message = "服务器错误:" + e.ToString();
                Logger.Error($"用户{User?.UserName ?? ""}({User?.Id ?? ""})查询薪酬条件(PostCustomerListSaleMan)请求失败：\r\n{pagingResponse.Message ?? ""}，\r\n请求参数为：\r\n" + (position != null ? JsonHelper.ToJson(position) : ""));

            }
            return pagingResponse;
        }

        [HttpPost("deletesalary")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage> DeleteSalaryInfo(UserInfo User, [FromBody]SalaryInfoRequest position)
        {
            var pagingResponse = new ResponseMessage();
            if (!ModelState.IsValid)
            {
                pagingResponse.Code = ResponseCodeDefines.ModelStateInvalid;
                Logger.Warn($"用户{User?.UserName ?? ""}({User?.Id ?? ""})删除薪酬条件(PostCustomerListSaleMan)模型验证失败：\r\n{pagingResponse.Message ?? ""}，\r\n请求参数为：\r\n" + (position != null ? JsonHelper.ToJson(position) : ""));
                return pagingResponse;
            }

            try
            {
                await _salaryManage.DeleteSalary(position, HttpContext.RequestAborted);
                pagingResponse.Message = "deletestation ok";
            }
            catch (Exception e)
            {
                pagingResponse.Code = ResponseCodeDefines.ServiceError;
                pagingResponse.Message = "服务器错误:" + e.ToString();
                Logger.Error($"用户{User?.UserName ?? ""}({User?.Id ?? ""})删除薪酬条件(PostCustomerListSaleMan)请求失败：\r\n{pagingResponse.Message ?? ""}，\r\n请求参数为：\r\n" + (position != null ? JsonHelper.ToJson(position) : ""));

            }
            return pagingResponse;
        }
    }
}
