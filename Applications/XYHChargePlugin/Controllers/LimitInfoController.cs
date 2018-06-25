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
    [Route("api/chargeinfo/limit")]
    public class LimitInfoController : Controller
    {
        private readonly XYH.Core.Log.ILogger Logger = LoggerManager.GetLogger("XYHChargeLimitInfo");
        private readonly LimitManager _limitManager;
    

        public LimitInfoController( LimitManager limit)
        {
            _limitManager = limit;
        }

        [HttpPost]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<LimitInfoResponse>> Save(UserInfo User, [FromBody]LimitInfoRequest request)
        {
            var r = new ResponseMessage<LimitInfoResponse>();
            
            try
            {
                r = await _limitManager.Save(User, request);
            }
            catch (Exception e)
            {
                r.Code = ResponseCodeDefines.ServiceError;
                r.Message = "服务器错误：" + e.Message;
                Logger.Error("保存费用限额设置失败：\r\n{0}", e.ToString());
            }
            return r;
        }

        [HttpGet("{id}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<LimitInfoResponse>> Get(UserInfo User, [FromRoute]string userId)
        {
            var r = new ResponseMessage<LimitInfoResponse>();

            try
            {
                r = await _limitManager.GetDetail(User, userId);
            }
            catch (Exception e)
            {
                r.Code = ResponseCodeDefines.ServiceError;
                r.Message = "服务器错误：" + e.Message;
                Logger.Error("获取费用限额设置详情失败：\r\n{0}", e.ToString());
            }
            return r;
        }


        [HttpPost("search")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<PagingResponseMessage<LimitInfoResponse>> Search(UserInfo User, [FromBody]LimitSearchRequest request)
        {
            var r = new PagingResponseMessage<LimitInfoResponse>();

            try
            {
                r = await _limitManager.Search(User, request);
            }
            catch (Exception e)
            {
                r.Code = ResponseCodeDefines.ServiceError;
                r.Message = "服务器错误：" + e.Message;
                Logger.Error("查询费用限额设置失败：\r\n{0}", e.ToString());
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
                r = await _limitManager.Delete(User, id) ;
            }
            catch (Exception e)
            {
                r.Code = ResponseCodeDefines.ServiceError;
                r.Message = "服务器错误：" + e.Message;
                Logger.Error("保存费用限额设置失败：\r\n{0}", e.ToString());
            }
            return r;
        }

    }
}
