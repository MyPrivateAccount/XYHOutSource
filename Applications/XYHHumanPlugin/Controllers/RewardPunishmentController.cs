using ApplicationCore;
using AspNet.Security.OAuth.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using XYH.Core.Log;
using XYHHumanPlugin.Managers;
using RewardPunishmentRequest = XYHHumanPlugin.Dto.Response.RewardPunishmentResponse;

namespace XYHHumanPlugin.Controllers
{
    [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/rewardpunishment")]
    public class RewardPunishmentController : Controller
    {
        private readonly ILogger Logger = LoggerManager.GetLogger("XYHHumanrewardpunishment");
        private readonly RewardPunishmentManager _rpManage;
        public RewardPunishmentController( RewardPunishmentManager sta)
        {
            _rpManage = sta;
        }

        [HttpPost("addrewardpunishment")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage> AddRPInfo(UserInfo User, [FromBody]RewardPunishmentRequest item)
        {
            var pagingResponse = new ResponseMessage();
            if (!ModelState.IsValid)
            {
                pagingResponse.Code = ResponseCodeDefines.ModelStateInvalid;
                Logger.Warn($"用户{User?.UserName ?? ""}({User?.Id ?? ""})创建行政奖惩(PostCustomerListSaleMan)模型验证失败：\r\n{pagingResponse.Message ?? ""}，\r\n请求参数为：\r\n" + (lst != null ? JsonHelper.ToJson(lst) : ""));
                return pagingResponse;
            }

            try
            {
                await _attendanceManage.AddRPInfo(item, HttpContext.RequestAborted);
                pagingResponse.Message = "importattendenceLst ok";
            }
            catch (Exception e)
            {
                pagingResponse.Code = ResponseCodeDefines.ServiceError;
                pagingResponse.Message = "服务器错误:" + e.ToString();
                Logger.Error($"用户{User?.UserName ?? ""}({User?.Id ?? ""})创建行政奖惩(PostCustomerListSaleMan)请求失败：\r\n{pagingResponse.Message ?? ""}，\r\n请求参数为：\r\n" + (lst != null ? JsonHelper.ToJson(lst) : ""));
            }
            return pagingResponse;
        }

        [HttpPost("searchrewardpunishment")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<HumanSearchResponse<RewardPunishmentResponse>>> SearchRewardPunishment(UserInfo User, [FromBody]RewardPunishmentSearchRequest condition)
        {
            var pagingResponse = new ResponseMessage<HumanSearchResponse<RewardPunishmentResponse>>();
            if (!ModelState.IsValid)
            {
                pagingResponse.Code = ResponseCodeDefines.ModelStateInvalid;
                Logger.Warn($"用户{User?.UserName ?? ""}({User?.Id ?? ""})查询行政奖惩(PostCustomerListSaleMan)模型验证失败：\r\n{pagingResponse.Message ?? ""}，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
                return pagingResponse;
            }

            try
            {
                pagingResponse.Extension = await _attendanceManage.SearchRewardPunishmentInfo(User, condition, HttpContext.RequestAborted);
                pagingResponse.Message = "searchattendencelst ok";
            }
            catch (Exception e)
            {
                pagingResponse.Code = ResponseCodeDefines.ServiceError;
                pagingResponse.Message = "服务器错误:" + e.ToString();
                Logger.Error($"用户{User?.UserName ?? ""}({User?.Id ?? ""})查询行政奖惩(PostCustomerListSaleMan)请求失败：\r\n{pagingResponse.Message ?? ""}，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
            }
            return pagingResponse;
        }

        [HttpPost("deleterewardpunishment/{id}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage> DeleteAttendenceItem(UserInfo User, [FromRoute]string id)
        {
            var pagingResponse = new ResponseMessage();
            if (!ModelState.IsValid)
            {
                pagingResponse.Code = ResponseCodeDefines.ModelStateInvalid;
                Logger.Warn($"用户{User?.UserName ?? ""}({User?.Id ?? ""})删除行政奖惩(PostCustomerListSaleMan)模型验证失败：\r\n{pagingResponse.Message ?? ""}，\r\n请求参数为：\r\n" + (id != null ? JsonHelper.ToJson(id) : ""));
                return pagingResponse;
            }

            try
            {
                await _attendanceManage.DeleteRPInfo(id, HttpContext.RequestAborted);
                pagingResponse.Message = "searchattendencelst ok";
            }
            catch (Exception e)
            {
                pagingResponse.Code = ResponseCodeDefines.ServiceError;
                pagingResponse.Message = "服务器错误:" + e.ToString();
                Logger.Error($"用户{User?.UserName ?? ""}({User?.Id ?? ""})删除行政奖惩(PostCustomerListSaleMan)请求失败：\r\n{pagingResponse.Message ?? ""}，\r\n请求参数为：\r\n" + (id != null ? JsonHelper.ToJson(id) : ""));

            }
            return pagingResponse;
        }

    }
}
