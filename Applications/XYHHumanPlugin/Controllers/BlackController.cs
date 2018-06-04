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
using BlackInfoRequest = XYHHumanPlugin.Dto.Response.BlackInfoResponse;

namespace XYHHumanPlugin.Controllers
{
    [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/humanblack")]
    public class BlackController : Controller
    {
        private readonly ILogger Logger = LoggerManager.GetLogger("XYHHumanblack");
        private readonly BlackManager _blackManage;
        private readonly RestClient _restClient;

        public BlackController(RestClient rsc, BlackManager sta)
        {
            _restClient = rsc;
            _blackManage = sta;
        }

        [HttpGet("blacklist")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<HumanSearchResponse<BlackInfoResponse>>> GetBlackList(UserInfo User, [FromBody]HumanSearchRequest searchinfo)
        {
            var Response = new ResponseMessage<HumanSearchResponse<BlackInfoResponse>>();
            if (!ModelState.IsValid)
            {
                Response.Code = ResponseCodeDefines.ModelStateInvalid;
                Logger.Warn($"用户{User?.UserName ?? ""}({User?.Id ?? ""})查询薪酬条件(PostCustomerListSaleMan)模型验证失败：\r\n{Response.Message ?? ""}，\r\n请求参数为：\r\n" + (searchinfo != null ? JsonHelper.ToJson(searchinfo) : ""));
                return Response;
            }

            try
            {
                Response.Extension = await _stationManage.SearchBlackInfo(User, searchinfo, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                Response.Code = ResponseCodeDefines.ServiceError;
                Response.Message = "服务器错误：" + e.ToString();
                Logger.Error("error");
            }
            return Response;
        }

        [HttpPost("setblack")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage> SetBlackInfo(UserInfo User, [FromBody]BlackInfoRequest black)
        {
            var pagingResponse = new ResponseMessage();
            if (!ModelState.IsValid)
            {
                pagingResponse.Code = ResponseCodeDefines.ModelStateInvalid;
                Logger.Warn($"用户{User?.UserName ?? ""}({User?.Id ?? ""})查询黑名单(PostCustomerListSaleMan)模型验证失败：\r\n{pagingResponse.Message ?? ""}，\r\n请求参数为：\r\n" + (black != null ? JsonHelper.ToJson(black) : ""));
                return pagingResponse;
            }

            try
            {
                await _stationManage.SetBlack(black, HttpContext.RequestAborted);
                pagingResponse.Message = "setblack ok";
            }
            catch (Exception e)
            {
                pagingResponse.Code = ResponseCodeDefines.ServiceError;
                pagingResponse.Message = "服务器错误:" + e.ToString();
                Logger.Error($"用户{User?.UserName ?? ""}({User?.Id ?? ""})查询黑名单(PostCustomerListSaleMan)请求失败：\r\n{pagingResponse.Message ?? ""}，\r\n请求参数为：\r\n" + (black != null ? JsonHelper.ToJson(black) : ""));

            }
            return pagingResponse;
        }

        [HttpPost("deleteblack")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage> DeleteBlackInfo(UserInfo User, [FromBody]BlackInfoRequest black)
        {
            var pagingResponse = new ResponseMessage();
            if (!ModelState.IsValid)
            {
                pagingResponse.Code = ResponseCodeDefines.ModelStateInvalid;
                Logger.Warn($"用户{User?.UserName ?? ""}({User?.Id ?? ""})查询黑名单(PostCustomerListSaleMan)模型验证失败：\r\n{pagingResponse.Message ?? ""}，\r\n请求参数为：\r\n" + (black != null ? JsonHelper.ToJson(black) : ""));
                return pagingResponse;
            }

            try
            {
                await _stationManage.DeleteBlack(black, HttpContext.RequestAborted);
                pagingResponse.Message = "deleteblack ok";
            }
            catch (Exception e)
            {
                pagingResponse.Code = ResponseCodeDefines.ServiceError;
                pagingResponse.Message = "服务器错误:" + e.ToString();
                Logger.Error($"用户{User?.UserName ?? ""}({User?.Id ?? ""})查询黑名单(PostCustomerListSaleMan)请求失败：\r\n{pagingResponse.Message ?? ""}，\r\n请求参数为：\r\n" + (position != null ? JsonHelper.ToJson(position) : ""));

            }
            return pagingResponse;
        }
    }
}
