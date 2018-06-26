using ApplicationCore;
using ApplicationCore.Dto;
using ApplicationCore.Filters;
using ApplicationCore.Managers;
using AspNet.Security.OAuth.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using XYH.Core.Log;
using XYHChargePlugin.Dto;
using XYHChargePlugin.Managers;

namespace XYHChargePlugin.Controllers
{
    [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/borrowing")]
    public class BorrowingController : Controller
    {
        private readonly XYH.Core.Log.ILogger Logger = LoggerManager.GetLogger("XYHBorrowing");
        private readonly BorrowingManager _borrowingManager;
        private readonly RestClient _restClient;
        protected PermissionExpansionManager _permissionExpansionManager { get; }

        public BorrowingController(RestClient rsc, BorrowingManager borrowing, PermissionExpansionManager per)
        {
            _permissionExpansionManager = per;
            _restClient = rsc;
            _borrowingManager = borrowing;

            _restClient = rsc;
        }

        [HttpPost]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<ChargeInfoResponse>> Save(UserInfo User, [FromBody]ChargeInfoRequest request)
        {
            var r = new ResponseMessage<ChargeInfoResponse>();
            
            try
            {
                r = await _borrowingManager.Save(User, request);
            }
            catch (Exception e)
            {
                r.Code = ResponseCodeDefines.ServiceError;
                r.Message = "服务器错误：" + e.Message;
                Logger.Error("保存报销单失败：\r\n{0}", e.ToString());
            }
            return r;
        }

        [HttpGet("{id}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<ChargeInfoResponse>> Get(UserInfo User, [FromRoute]string id)
        {
            var r = new ResponseMessage<ChargeInfoResponse>();

            try
            {
                r.Extension = await _borrowingManager.GetDetail(User, id);
            }
            catch (Exception e)
            {
                r.Code = ResponseCodeDefines.ServiceError;
                r.Message = "服务器错误：" + e.Message;
                Logger.Error("获取报销单详情失败：\r\n{0}", e.ToString());
            }
            return r;
        }


        [HttpPost("search")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<PagingResponseMessage<ChargeInfoResponse>> Search(UserInfo User, [FromBody]ChargeSearchRequest request, [FromQuery]string permissionId)
        {
            var r = new PagingResponseMessage<ChargeInfoResponse>();

            try
            {
                r = await _borrowingManager.Search(User, request, permissionId);
            }
            catch (Exception e)
            {
                r.Code = ResponseCodeDefines.ServiceError;
                r.Message = "服务器错误：" + e.Message;
                Logger.Error("查询报销单失败：\r\n{0}", e.ToString());
            }
            return r;
        }



        [HttpDelete("{id}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage> Delete(UserInfo User, [FromRoute]string id)
        {
            var r = new ResponseMessage();

            try
            {
                r = await _borrowingManager.DeleteBorrowing(User, id) ;
            }
            catch (Exception e)
            {
                r.Code = ResponseCodeDefines.ServiceError;
                r.Message = "服务器错误：" + e.Message;
                Logger.Error("保存报销单失败：\r\n{0}", e.ToString());
            }
            return r;
        }

        [HttpPost("submit/{id}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage> Submit(UserInfo User, [FromRoute]string id)
        {
            var r = new ResponseMessage();

            try
            {
                r = await _borrowingManager.Submit(User, id);
            }
            catch (Exception e)
            {
                r.Code = ResponseCodeDefines.ServiceError;
                r.Message = "服务器错误：" + e.Message;
                Logger.Error("提交报销单失败：\r\n{0}", e.ToString());
            }
            return r;
        }

        [HttpPost("confirm")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage> Confirm(UserInfo User, [FromBody]ConfirmRequest request)
        {
            var r = new ResponseMessage();

            try
            {
                r = await _borrowingManager.Confirm(User, request);
            }
            catch (Exception e)
            {
                r.Code = ResponseCodeDefines.ServiceError;
                r.Message = "服务器错误：" + e.Message;
                Logger.Error("确认报销单失败：\r\n{0}", e.ToString());
            }
            return r;
        }

 
        [HttpPost("payment")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<PaymentInfoResponse>> Payment(UserInfo User, [FromBody]PaymentInfoRequest request)
        {
            var r = new ResponseMessage<PaymentInfoResponse>();

            try
            {
                r = await _borrowingManager.Payment(User, request);
            }
            catch (Exception e)
            {
                r.Code = ResponseCodeDefines.ServiceError;
                r.Message = "服务器错误：" + e.Message;
                Logger.Error("付款失败：\r\n{0}", e.ToString());
            }
            return r;
        }



        [HttpPost("recordingconfirm")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage> RecordingConfirm(UserInfo User, [FromBody]ConfirmRequest request)
        {
            var r = new ResponseMessage();

            try
            {
                r = await _borrowingManager.RecordingConfirm(User, request);
            }
            catch (Exception e)
            {
                r.Code = ResponseCodeDefines.ServiceError;
                r.Message = "服务器错误：" + e.Message;
                Logger.Error("财务确认失败：\r\n{0}", e.ToString());
            }
            return r;
        }


    }
}
