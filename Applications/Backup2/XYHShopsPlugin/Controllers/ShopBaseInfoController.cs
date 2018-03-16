using ApplicationCore;
using ApplicationCore.Dto;
using ApplicationCore.Filters;
using AspNet.Security.OAuth.Validation;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XYH.Core.Log;
using XYHShopsPlugin.Dto;
using XYHShopsPlugin.Managers;

namespace XYHShopsPlugin.Controllers
{
    [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/shopsbaseinfos")]
    public class ShopBaseInfoController : Controller
    {
        private readonly ShopBaseInfoManager _shopBaseInfoManager;
        private readonly IMapper _mapper;
        private readonly ILogger Logger = LoggerManager.GetLogger("ShopBaseInfo");

        public ShopBaseInfoController(ShopBaseInfoManager shopBaseInfoManager, IMapper mapper)
        {
            _shopBaseInfoManager = shopBaseInfoManager;
            _mapper = mapper;
        }

        /// <summary>
        /// 根据商铺ID获取商铺详情
        /// </summary>
        /// <param name="user"></param>
        /// <param name="shopsId"></param>
        /// <returns></returns>
        [HttpGet("{shopsId}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "ShopsRetrieve" })]
        public async Task<ResponseMessage<ShopBaseInfoResponse>> GetBuildingBaseInfo(UserInfo user, [FromRoute] string shopsId)
        {
            ResponseMessage<ShopBaseInfoResponse> response = new ResponseMessage<ShopBaseInfoResponse>();
            if (string.IsNullOrEmpty(shopsId))
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = "传入值不符合规则";
                return response;
            }
            try
            {
                response.Extension = await _shopBaseInfoManager.FindByIdAsync(shopsId, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})根据商铺ID获取商铺详情(GetBuildingBaseInfo)报错：\r\n{e.ToString()}，\r\n请求参数为：\r\n(shopsId){shopsId ?? ""}");
            }
            return response;
        }

        //[HttpPost]
        //[TypeFilter(typeof(CheckPermission), Arguments = new object[] { "ShopsCreate" })]
        //public async Task<ResponseMessage<ShopBaseInfoResponse>> PostBuildingBaseInfo([FromBody]ShopBaseInfoRequest shopBaseInfoRequest)
        //{
        //    ResponseMessage<ShopBaseInfoResponse> response = new ResponseMessage<ShopBaseInfoResponse>();
        //    if (!ModelState.IsValid)
        //    {
        //        response.Code = ResponseCodeDefines.ModelStateInvalid;
        //        return response;
        //    }
        //    if (await _shopBaseInfoManager.ShopsIsExist(new ShopsIsExistRequest
        //    {
        //        BuildingId = shopBaseInfoRequest.BuildingId,
        //        BuildingNo = shopBaseInfoRequest.BuildingNo,
        //        FloorNo = shopBaseInfoRequest.FloorNo,
        //        Number = shopBaseInfoRequest.Number
        //    }, HttpContext.RequestAborted))
        //    {
        //        response.Code = ResponseCodeDefines.ObjectAlreadyExists;
        //        return response;
        //    }
        //    response.Extension = await _shopBaseInfoManager.CreateAsync(shopBaseInfoRequest, HttpContext.RequestAborted);
        //    return response;
        //}

        /// <summary>
        /// 检查是否具有重复的商铺
        /// </summary>
        /// <param name="user"></param>
        /// <param name="shopsIsExistRequest"></param>
        /// <returns></returns>
        [HttpPost("exist")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "ShopsCreate" })]
        public async Task<ResponseMessage<bool>> PostBuildingBaseInfo(UserInfo user, [FromBody]ShopsIsExistRequest shopsIsExistRequest)
        {
            ResponseMessage<bool> response = new ResponseMessage<bool>();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                return response;
            }
            try
            {
                response.Extension = await _shopBaseInfoManager.ShopsIsExist(shopsIsExistRequest, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})检查是否具有重复的商铺(PostBuildingBaseInfo)请求失败：\r\n{e.ToString()}，\r\n请求参数为：\r\n" + (shopsIsExistRequest != null ? JsonHelper.ToJson(shopsIsExistRequest) : ""));
            }
            return response;
        }

        /// <summary>
        /// 修改楼盘详细信息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="buildingId"></param>
        /// <param name="shopId"></param>
        /// <param name="shopBaseInfoRequest"></param>
        /// <returns></returns>
        [HttpPut("{buildingId}/{shopId}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "ShopsUpdate" })]
        public async Task<ResponseMessage> PutBuildingBaseInfo(UserInfo user, [FromRoute] string buildingId, [FromRoute]  string shopId, [FromBody] ShopBaseInfoRequest shopBaseInfoRequest)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})修改楼盘详细信息(PutBuildingBaseInfo)：\r\n请求参数为：\r\n(buildingId){buildingId},(shopId){shopId}," + (shopBaseInfoRequest != null ? JsonHelper.ToJson(shopBaseInfoRequest) : ""));

            ResponseMessage response = new ResponseMessage();
            if (!ModelState.IsValid || shopBaseInfoRequest.Id != shopId)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})修改楼盘详细信息(PutBuildingBaseInfo)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n(buildingId){buildingId},(shopId){shopId}," + (shopBaseInfoRequest != null ? JsonHelper.ToJson(shopBaseInfoRequest) : ""));
                return response;
            }
            try
            {
                //Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})修改楼盘详细信息(PutBuildingBaseInfo)：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n(buildingId){buildingId},(shopId){shopId}," + (shopBaseInfoRequest != null ? JsonHelper.ToJson(shopBaseInfoRequest) : ""));
                if (await _shopBaseInfoManager.ShopsIsExist(new ShopsIsExistRequest
                {
                    Id = shopId,
                    BuildingId = buildingId,
                    BuildingNo = shopBaseInfoRequest.BuildingNo,
                    FloorNo = shopBaseInfoRequest.FloorNo,
                    Number = shopBaseInfoRequest.Number
                }, HttpContext.RequestAborted))
                {
                    response.Code = ResponseCodeDefines.ObjectAlreadyExists;
                    response.Message = "已存在相同的商铺";
                    return response;
                }
                await _shopBaseInfoManager.SaveAsync(user, buildingId, shopBaseInfoRequest, HttpContext.RequestAborted);
                //var dictionaryGroup = await _shopBaseInfoManager.FindByIdAsync(buildingId, HttpContext.RequestAborted);
                //if (dictionaryGroup == null)
                //{
                //    await _shopBaseInfoManager.CreateAsync(shopBaseInfoRequest, HttpContext.RequestAborted);
                //}
                //await _shopBaseInfoManager.UpdateAsync(shopBaseInfoRequest, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})修改楼盘详细信息(PutBuildingBaseInfo)请求失败：\r\n{e.ToString()}，\r\n请求参数为：\r\n(buildingId){buildingId},(shopId){shopId}," + (shopBaseInfoRequest != null ? JsonHelper.ToJson(shopBaseInfoRequest) : ""));

            }
            return response;
        }

    }
}
