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
    [Route("api/shopsfacilities")]
    public class ShopFacilitiesController : Controller
    {
        private readonly ShopFacilitiesManager _shopFacilitiesManager;
        private readonly ILogger Logger = LoggerManager.GetLogger("ShopFacilities");

        public ShopFacilitiesController(ShopFacilitiesManager shopFacilitiesManager)
        {
            _shopFacilitiesManager = shopFacilitiesManager;
        }

        /// <summary>
        /// 根据商铺ID获取商铺设施
        /// </summary>
        /// <param name="user"></param>
        /// <param name="shopsId"></param>
        /// <returns></returns>
        [HttpGet("{shopsId}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "ShopsRetrieve" })]
        public async Task<ResponseMessage<ShopFacilitiesResponse>> GetShopsFacilities(UserInfo user, [FromRoute] string shopsId)
        {
            ResponseMessage<ShopFacilitiesResponse> response = new ResponseMessage<ShopFacilitiesResponse>();
            if (string.IsNullOrEmpty(shopsId))
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = "传入值不符合规则";
                return response;
            }
            try
            {
                response.Extension = await _shopFacilitiesManager.FindByIdAsync(shopsId, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})根据商铺ID获取商铺设施(GetShopsFacilities)报错：\r\n{e.ToString()}，\r\n请求参数为：\r\n(shopsId){shopsId ?? ""}");
            }
            return response;
        }

        //[HttpPost]
        //[TypeFilter(typeof(CheckPermission), Arguments = new object[] { "ShopsCreate" })]
        //public async Task<ResponseMessage<ShopFacilitiesResponse>> PostShopsFacilities([FromBody]ShopFacilitiesRequest shopFacilitiesRequest)
        //{
        //    ResponseMessage<ShopFacilitiesResponse> response = new ResponseMessage<ShopFacilitiesResponse>();
        //    if (!ModelState.IsValid)
        //    {
        //        response.Code = ResponseCodeDefines.ModelStateInvalid;
        //        return response;
        //    }
        //    response.Extension = await _shopFacilitiesManager.CreateAsync(shopFacilitiesRequest, HttpContext.RequestAborted);
        //    return response;
        //}

        /// <summary>
        /// 修改楼盘设施信息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="buildingId"></param>
        /// <param name="shopId"></param>
        /// <param name="shopFacilitiesRequest"></param>
        /// <returns></returns>
        [HttpPut("{buildingId}/{shopId}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "ShopsUpdate" })]
        public async Task<ResponseMessage> PutShopsFacilities(UserInfo user, [FromRoute] string buildingId, [FromRoute]  string shopId, [FromBody] ShopFacilitiesRequest shopFacilitiesRequest)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})修改楼盘设施信息(PutShopsFacilities)：\r\n请求参数为：\r\n" + (shopFacilitiesRequest != null ? JsonHelper.ToJson(shopFacilitiesRequest) : ""));

            ResponseMessage response = new ResponseMessage();
            if (!ModelState.IsValid || shopFacilitiesRequest.Id != shopId)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})修改楼盘设施信息(PutShopsFacilities)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (shopFacilitiesRequest != null ? JsonHelper.ToJson(shopFacilitiesRequest) : ""));
                return response;
            }
            try
            {
                await _shopFacilitiesManager.SaveAsync(user, buildingId, shopFacilitiesRequest, HttpContext.RequestAborted);
                //var dictionaryGroup = await _shopFacilitiesManager.FindByIdAsync(buildingId, HttpContext.RequestAborted);
                //if (dictionaryGroup == null)
                //{
                //    await _shopFacilitiesManager.CreateAsync(shopFacilitiesRequest, HttpContext.RequestAborted);
                //}
                //await _shopFacilitiesManager.UpdateAsync(shopFacilitiesRequest, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})修改楼盘设施信息(PutShopsFacilities)请求失败：\r\n{e.ToString()}，\r\n请求参数为：\r\n" + (shopFacilitiesRequest != null ? JsonHelper.ToJson(shopFacilitiesRequest) : ""));

            }
            return response;
        }
    }
}
