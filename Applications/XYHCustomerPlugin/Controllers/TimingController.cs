using ApplicationCore;
using ApplicationCore.Filters;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XYH.Core.Log;
using XYHCustomerPlugin.Managers;

namespace XYHCustomerPlugin.Controllers
{
    /// <summary>
    /// 定时任务控制器
    /// </summary>
    [Produces("application/json")]
    [Route("api/customertiming")]
    public class TimingController : Controller
    {
        #region 成员

        private readonly CustomerTimingManager _customerTimingManager;
        private readonly IMapper _mapper;
        private readonly ILogger Logger = LoggerManager.GetLogger("CustomerTiming");

        #endregion

        /// <summary>
        /// 成交
        /// </summary>
        public TimingController(CustomerTimingManager customerTimingManager, IMapper mapper)
        {
            _customerTimingManager = customerTimingManager;
            _mapper = mapper;
        }

        // 报备只是未带看未成交
        //[HttpGet("transactionlapse")]
        //public async Task<ResponseMessage> GetCustomerTransactionsBeyond()
        //{
        //    var response = new ResponseMessage();
        //    try
        //    {
        //        await _customerTimingManager.TimedTaskBeyondTheStatus(HttpContext.RequestAborted);
        //    }
        //    catch (Exception e)
        //    {
        //        response.Code = ResponseCodeDefines.ServiceError;
        //        response.Message = e.ToString();
        //        Logger.Error("定时更新超期任务：" + e.ToString());
        //    }
        //    return response;
        //}

        /// <summary>
        /// 定时更新提交报备超期任务
        /// </summary>
        /// <returns></returns>
        [HttpGet("transactionconfirmlapse")]
        public async Task<ResponseMessage> GetTransConfirmlapse()
        {
            Logger.Trace("定时更新超期任务(transactionconfirmlapse)");

            var response = new ResponseMessage();
            try
            {
                await _customerTimingManager.TimedTaskConfirmReport(HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error("定时更新超期任务：" + e.ToString());
            }
            return response;
        }

        /// <summary>
        /// 定时更新带看报备超期任务
        /// </summary>
        /// <returns></returns>
        [HttpGet("transactionsubmitlapse")]
        public async Task<ResponseMessage> GetTransSubmitlapse()
        {
            Logger.Trace("定时更新超期任务(transactionsubmitlapse)");

            var response = new ResponseMessage();
            try
            {
                await _customerTimingManager.TimedTaskSubmitReport(HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error("定时更新超期任务：" + e.ToString());
            }
            return response;
        }

        /// <summary>
        /// 定时更新带看超期任务
        /// </summary>
        /// <returns></returns>
        [HttpGet("transactionbeltlapse")]
        public async Task<ResponseMessage> GetTransBeltLooklapse()
        {
            Logger.Trace("定时更新带看超期任务(transactionbeltlapse)");
            var response = new ResponseMessage();
            try
            {
                await _customerTimingManager.TimedTaskBeltLookReport(HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error("定时更新超期任务：" + e.ToString());
            }
            return response;
        }

        /// <summary>
        /// 定时更新超期任务
        /// </summary>
        /// <returns></returns>
        [HttpGet("handpool")]
        public async Task<ResponseMessage> GetHandOverCustomerPool()
        {
            Logger.Trace("定时更新超期任务(handpool)");
            var response = new ResponseMessage();
            try
            {
                await _customerTimingManager.HandOverCustomerPool(HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error("定时更新超期任务：" + e.ToString());
            }
            return response;
        }

        /// <summary>
        /// 定时更新超期任务
        /// </summary>
        /// <returns></returns>
        [HttpGet("poolupdate")]
        public async Task<ResponseMessage> GetCustomerPoolHandOver()
        {
            Logger.Trace("定时更新超期任务(poolupdate)");
            var response = new ResponseMessage();
            try
            {
                await _customerTimingManager.CustomerPoolHandOver(HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error("定时更新超期任务：" + e.ToString());
            }
            return response;
        }
    }
}
