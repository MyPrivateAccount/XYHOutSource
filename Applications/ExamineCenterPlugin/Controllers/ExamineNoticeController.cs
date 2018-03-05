using ApplicationCore;
using ApplicationCore.Dto;
using ApplicationCore.Filters;
using AspNet.Security.OAuth.Validation;
using ExamineCenterPlugin.Dto;
using ExamineCenterPlugin.Managers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XYH.Core.Log;

namespace ExamineCenterPlugin.Controllers
{
    /// <summary>
    /// 审核告知相关
    /// </summary>
    [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/examinenotices")]
    public class ExamineNoticeController : Controller
    {
        private readonly ILogger Logger = LoggerManager.GetLogger("ExamineNoticeController");

        public ExamineNoticeController(ExamineNoticeManager examineNoticeManager)
        {
            _examineNoticeManager = examineNoticeManager ?? throw new ArgumentNullException(nameof(examineNoticeManager));
        }

        protected ExamineNoticeManager _examineNoticeManager { get; set; }


        /// <summary>
        /// 添加审核告知信息(内部使用)
        /// </summary>
        /// <param name="userNoticeCallbackRequest"></param>
        /// <returns></returns>
        [HttpPost("noticecallback")]
        [AllowAnonymous]
        public async Task<ResponseMessage> UserNoticeCallback([FromBody]UserNoticeCallbackRequest userNoticeCallbackRequest)
        {
            ResponseMessage response = new ResponseMessage();
            try
            {
                return await _examineNoticeManager.NoticeCallbackAsync(userNoticeCallbackRequest);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"审核告知回调报错：\r\n{e.ToString()},\r\n请求参数为：\r\n" + (userNoticeCallbackRequest != null ? JsonHelper.ToJson(userNoticeCallbackRequest) : ""));
            }
            return response;
        }

        /// <summary>
        /// 获取用户未读审核告知数量
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpGet]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<int>> GetUserNoticeCount(UserInfo user)
        {
            ResponseMessage<int> response = new ResponseMessage<int>();
            try
            {
                response.Extension = await _examineNoticeManager.GetUserNoticeCount(user.Id, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})获取用户未读审核告知数量(GetUserNoticeCount)报错：\r\n{e.ToString()}");
            }
            return response;
        }


        /// <summary>
        /// 获取用户审核告知列表
        /// </summary>
        /// <param name="user"></param>
        /// <param name="conditon"></param>
        /// <returns></returns>
        [HttpPost("list")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<PagingResponseMessage<ExamineNoticeResponse>> GetUserNotices(UserInfo user, [FromBody]UserExamineNoticeListConditon conditon)
        {
            PagingResponseMessage<ExamineNoticeResponse> pagingResponse = new PagingResponseMessage<ExamineNoticeResponse>();
            if (conditon == null)
            {
                pagingResponse.Code = ResponseCodeDefines.ArgumentNullError;
                Logger.Info($"用户{user?.UserName ?? ""}({user?.Id ?? ""})获取用户审核告知列表(GetUserNotices)失败：参数conditon为null");
                return pagingResponse;
            }
            try
            {
                return await _examineNoticeManager.UserNoticeList(user.Id, conditon, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                pagingResponse.Code = ResponseCodeDefines.ServiceError;
                pagingResponse.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})获取用户审核告知列表(GetUserNotices)报错：\r\n{e.ToString()}，参数为：\r\n" + (conditon != null ? JsonHelper.ToJson(conditon) : ""));
            }
            return pagingResponse;
        }



        /// <summary>
        /// 获取用户审核告知详情
        /// </summary>
        /// <param name="user"></param>
        /// <param name="noticeId"></param>
        /// <returns></returns>
        [HttpGet("noticeId")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<ExamineNoticeResponse>> GetNoticeById(UserInfo user, [FromRoute]string noticeId)
        {
            ResponseMessage<ExamineNoticeResponse> response = new ResponseMessage<ExamineNoticeResponse>();
            try
            {
                response.Extension = await _examineNoticeManager.GetNoticeById(user.Id, noticeId, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})获取用户审核告知详情(GetNoticeById)报错：\r\n{e.ToString()}，参数为：\r\n(noticeId){noticeId ?? ""}");
            }
            return response;
        }



    }
}
