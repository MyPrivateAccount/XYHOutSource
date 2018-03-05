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
    [Route("api/shopsleaseinfos")]
    public class ShopLeaseInfoController : Controller
    {

        private readonly ShopLeaseInfoManager _shopLeaseInfoManager;
        private readonly ILogger Logger = LoggerManager.GetLogger("ShopLeaseInfo");

        public ShopLeaseInfoController(ShopLeaseInfoManager shopLeaseInfoManager)
        {
            _shopLeaseInfoManager = shopLeaseInfoManager;
        }

        /// <summary>
        /// 根据商铺ID获取商铺租赁
        /// </summary>
        /// <param name="user"></param>
        /// <param name="shopsId"></param>
        /// <returns></returns>
        [HttpGet("{shopsId}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "ShopsRetrieve" })]
        public async Task<ResponseMessage<ShopLeaseInfoResponse>> GetShopLeaseInfo(UserInfo user,[FromRoute] string shopsId)
        {
            ResponseMessage<ShopLeaseInfoResponse> response = new ResponseMessage<ShopLeaseInfoResponse>();
            if (string.IsNullOrEmpty(shopsId))
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = "传入值不符合规则";
                return response;
            }
            try
            {
                response.Extension = await _shopLeaseInfoManager.FindByIdAsync(shopsId, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})根据商铺ID获取商铺租赁(GetShopLeaseInfo)报错：\r\n{e.ToString()}，\r\n请求参数为：\r\n(shopsId){shopsId ?? ""}");
            }
            return response;
        }

        //[HttpPost]
        //[TypeFilter(typeof(CheckPermission), Arguments = new object[] { "ShopsCreate" })]
        //public async Task<ResponseMessage<ShopLeaseInfoResponse>> PostShopLeaseInfo([FromBody]ShopLeaseInfoRequest shopLeaseInfoRequest)
        //{
        //    ResponseMessage<ShopLeaseInfoResponse> response = new ResponseMessage<ShopLeaseInfoResponse>();
        //    if (!ModelState.IsValid)
        //    {
        //        response.Code = ResponseCodeDefines.ModelStateInvalid;
        //        return response;
        //    }
        //    response.Extension = await _shopLeaseInfoManager.CreateAsync(shopLeaseInfoRequest, HttpContext.RequestAborted);
        //    return response;
        //}

        /// <summary>
        /// 修改楼盘租赁信息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="buildingId"></param>
        /// <param name="shopsId"></param>
        /// <param name="shopLeaseInfoRequest"></param>
        /// <returns></returns>
        [HttpPut("{buildingId}/{shopsId}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "ShopsUpdate" })]
        public async Task<ResponseMessage> PutShopLeaseInfo(UserInfo user, [FromRoute] string buildingId, [FromRoute] string shopsId, [FromBody] ShopLeaseInfoRequest shopLeaseInfoRequest)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})修改楼盘租赁信息(PutShopLeaseInfo)：\r\n请求参数为：\r\n" + (shopLeaseInfoRequest != null ? JsonHelper.ToJson(shopLeaseInfoRequest) : ""));

            ResponseMessage response = new ResponseMessage();
            if (!ModelState.IsValid || shopLeaseInfoRequest.Id != shopsId)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})修改楼盘租赁信息(PutShopLeaseInfo)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (shopLeaseInfoRequest != null ? JsonHelper.ToJson(shopLeaseInfoRequest) : ""));
                return response;
            }
            try
            {
                await _shopLeaseInfoManager.SaveAsync(user, buildingId, shopLeaseInfoRequest, HttpContext.RequestAborted);
                //var dictionaryGroup = await _shopLeaseInfoManager.FindByIdAsync(shopsId, HttpContext.RequestAborted);
                //if (dictionaryGroup == null)
                //{
                //    await _shopLeaseInfoManager.CreateAsync(shopLeaseInfoRequest, HttpContext.RequestAborted);
                //}
                //await _shopLeaseInfoManager.UpdateAsync(shopLeaseInfoRequest, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})修改楼盘租赁信息(PutShopLeaseInfo)请求失败：\r\n{e.ToString()}，\r\n请求参数为：\r\n" + (shopLeaseInfoRequest != null ? JsonHelper.ToJson(shopLeaseInfoRequest) : ""));
            }
            return response;
        }
    }
}
