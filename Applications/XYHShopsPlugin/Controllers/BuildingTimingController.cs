using ApplicationCore;
using AutoMapper;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;
using XYH.Core.Log;
using XYHShopsPlugin.Managers;

namespace XYHShopsPlugin.Controllers
{
    /// <summary>
    /// 定时任务控制器
    /// </summary>
    [Produces("application/json")]
    [Route("api/buildingtiming")]
    public class BuildingTimingController : Controller
    {
        #region 成员

        private readonly BuildingTimingManager _buildingTimingManager;
        private readonly IMapper _mapper;
        private readonly ILogger Logger = LoggerManager.GetLogger("CustomerTiming");

        #endregion

        /// <summary>
        /// 成交
        /// </summary>
        public BuildingTimingController(BuildingTimingManager buildingTimingManager, IMapper mapper)
        {
            _buildingTimingManager = buildingTimingManager;
            _mapper = mapper;
        }

        /// <summary>
        /// 定时更新楼盘推荐
        /// </summary>
        /// <returns></returns>
        [HttpGet("[action]")]
        public async Task<ResponseMessage> GetCustomerTransactionsBeyond()
        {
            Logger.Trace("定时更新楼盘推荐");

            var response = new ResponseMessage();
            try
            {
                await _buildingTimingManager.TimedTaskDeletedBuildingRecommend(HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error("定时更新楼盘推荐报错：" + e.ToString());
            }
            return response;
        }

        /// <summary>
        /// 定时更新商铺锁定状态
        /// </summary>
        /// <returns></returns>
        [HttpGet("taskshoplock")]
        public async Task<ResponseMessage> TaskShopLockStatus()
        {
            Logger.Trace("定时更新商铺锁定状态");

            var response = new ResponseMessage();
            try
            {
                await _buildingTimingManager.TimedTaskLockShops(HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error("定时更新商铺锁定状态：" + e.ToString());
            }
            return response;
        }
    }
}
