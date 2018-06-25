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
    [Route("api/chargeinfo")]
    public class ChargeInfoController : Controller
    {
        private readonly XYH.Core.Log.ILogger Logger = LoggerManager.GetLogger("XYHChargeinfo");
        private readonly ChargeManager _chargeManager;
        private readonly RestClient _restClient;
        protected PermissionExpansionManager _permissionExpansionManager { get; }

        private static string _lastDate;
        private static int _lastNumber;

        public ChargeInfoController(RestClient rsc, ChargeManager charge, PermissionExpansionManager per)
        {
            _permissionExpansionManager = per;
            _restClient = rsc;
            _chargeManager = charge;

            _lastNumber = 1;
            _restClient = rsc;
        }

        [HttpPost]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<ChargeInfoResponse>> Save(UserInfo User, [FromBody]ChargeInfoRequest request)
        {
            var r = new ResponseMessage<ChargeInfoResponse>();
            
            try
            {
                r = await _chargeManager.Save(User, request);
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
                r.Extension = await _chargeManager.GetDetail(User, id);
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
                r = await _chargeManager.Search(User, request, permissionId);
            }
            catch (Exception e)
            {
                r.Code = ResponseCodeDefines.ServiceError;
                r.Message = "服务器错误：" + e.Message;
                Logger.Error("查询报销单失败：\r\n{0}", e.ToString());
            }
            return r;
        }

        [HttpPost("searchdetail")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<PagingResponseMessage<CostInfoResponse>> SearchDetail(UserInfo User, [FromBody]CostSearchRequest request)
        {
            var r = new PagingResponseMessage<CostInfoResponse>();

            try
            {
                r = await _chargeManager.SearchCost(User, request);
            }
            catch (Exception e)
            {
                r.Code = ResponseCodeDefines.ServiceError;
                r.Message = "服务器错误：" + e.Message;
                Logger.Error("查询费用明细失败：\r\n{0}", e.ToString());
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
                r = await _chargeManager.DeleteCharge(User, id) ;
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
                r = await _chargeManager.Submit(User, id);
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
                r = await _chargeManager.Confirm(User, request);
            }
            catch (Exception e)
            {
                r.Code = ResponseCodeDefines.ServiceError;
                r.Message = "服务器错误：" + e.Message;
                Logger.Error("确认报销单失败：\r\n{0}", e.ToString());
            }
            return r;
        }

        [HttpPost("backup")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<ChargeInfoResponse>> Backup(UserInfo User, [FromBody]ChargeInfoRequest request)
        {
            var r = new ResponseMessage<ChargeInfoResponse>();

            try
            {
                r = await _chargeManager.Backup(User, request);
            }
            catch (Exception e)
            {
                r.Code = ResponseCodeDefines.ServiceError;
                r.Message = "服务器错误：" + e.Message;
                Logger.Error("后补发票失败：\r\n{0}", e.ToString());
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
                r = await _chargeManager.Payment(User, request);
            }
            catch (Exception e)
            {
                r.Code = ResponseCodeDefines.ServiceError;
                r.Message = "服务器错误：" + e.Message;
                Logger.Error("后补发票失败：\r\n{0}", e.ToString());
            }
            return r;
        }


        [HttpPost("billSubmit/{id}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage> SubmitBill(UserInfo User, [FromRoute]string id)
        {
            var r = new ResponseMessage();

            try
            {
                r = await _chargeManager.SubmitBill(User, id);
            }
            catch (Exception e)
            {
                r.Code = ResponseCodeDefines.ServiceError;
                r.Message = "服务器错误：" + e.Message;
                Logger.Error("提交后补发票失败：\r\n{0}", e.ToString());
            }
            return r;
        }

        [HttpPost("confirmBill")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage> ConfirmBill(UserInfo User, [FromBody]ConfirmRequest request)
        {
            var r = new ResponseMessage();

            try
            {
                r = await _chargeManager.ConfirmBill(User, request);
            }
            catch (Exception e)
            {
                r.Code = ResponseCodeDefines.ServiceError;
                r.Message = "服务器错误：" + e.Message;
                Logger.Error("确认后补发票失败：\r\n{0}", e.ToString());
            }
            return r;
        }


        [HttpGet("limittip/{id}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<LimitTipResponse>> GetLimitTip(UserInfo User,[FromRoute]string id, [FromQuery]DateTime? date=null, [FromQuery]string chargeId = null)
        {
            var r = new ResponseMessage<LimitTipResponse>();

            try
            {
                r.Extension = await _chargeManager.GetLimitTip(User, id, date, chargeId);
            }
            catch (Exception e)
            {
                r.Code = ResponseCodeDefines.ServiceError;
                r.Message = "服务器错误：" + e.Message;
                Logger.Error("获取费用限额信息失败：\r\n{0}", e.ToString());
            }
            return r;
        }

    }
}
