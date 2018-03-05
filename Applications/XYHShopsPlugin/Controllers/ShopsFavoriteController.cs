using ApplicationCore;
using ApplicationCore.Dto;
using ApplicationCore.Filters;
using AspNet.Security.OAuth.Validation;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XYH.Core.Log;
using XYHShopsPlugin.Dto;
using XYHShopsPlugin.Dto.Request;
using XYHShopsPlugin.Dto.Response;
using XYHShopsPlugin.Managers;
using XYHShopsPlugin.Models;

namespace XYHShopsPlugin.Controllers
{
    [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/shopsFavorite")]
    public class ShopsFavoriteController : Controller
    {
        #region 成员

        private readonly ShopsFavoriteManager _shopsFavoriteManager;
        private readonly IMapper _mapper;
        private readonly ILogger Logger = LoggerManager.GetLogger("ShopsFavorite");

        #endregion

        /// <summary>
        /// 商铺收藏信息
        /// </summary>
        public ShopsFavoriteController(ShopsFavoriteManager shopsFavoriteManager, IMapper mapper)
        {
            _shopsFavoriteManager = shopsFavoriteManager;
            _mapper = mapper;
        }

        /// <summary>
        /// 获取我收藏的商铺
        /// </summary>
        /// <param name="user"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpPost("[action]")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "BuildingRetrieve" })]
        public async Task<PagingResponseMessage<ShopsFavoriteResponse>> GetMyShopsFavoriteList(UserInfo user, [FromBody]PageCondition condition)
        {
            PagingResponseMessage<ShopsFavoriteResponse> pagingResponse = new PagingResponseMessage<ShopsFavoriteResponse>();
            if (!ModelState.IsValid)
            {
                pagingResponse.Code = ResponseCodeDefines.ModelStateInvalid;
                pagingResponse.Message = ModelState.GetAllErrors();
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})获取我收藏的商铺(GetMyShopsFavoriteList)模型验证失败：\r\n{pagingResponse.Message ?? ""}，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
                return pagingResponse;
            }
            try
            {
                return await _shopsFavoriteManager.FindMyShopsFavoriteAsync(user.Id, condition, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                pagingResponse.Code = ResponseCodeDefines.ServiceError;
                pagingResponse.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})获取我收藏的商铺(GetMyShopsFavoriteList)请求失败：\r\n{pagingResponse.Message ?? ""}，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
            }
            return pagingResponse;
        }

        /// <summary>
        /// 根据收藏Id查询信息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="shopsFavoriteId"></param>
        /// <returns></returns>
        [HttpGet("{shopsFavoriteId}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "ShopsFavoriteById" })]
        public async Task<ResponseMessage<ShopsFavoriteResponse>> GetShopsFavorite(UserInfo user, [FromRoute] string shopsFavoriteId)
        {
            var response = new ResponseMessage<ShopsFavoriteResponse>();
            if (string.IsNullOrEmpty(shopsFavoriteId))
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                return response;
            }
            try
            {
                response.Extension = await _shopsFavoriteManager.FindByIdAsync(shopsFavoriteId, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})根据收藏Id查询信息(GetShopsFavorite)报错：\r\n{e.ToString()}，\r\n请求参数为：\r\n(shopsFavoriteId){shopsFavoriteId ?? ""}");
            }
            return response;
        }

        /// <summary>
        /// 新增商铺收藏信息
        /// </summary>
        /// <param name="user">用户Id</param>
        /// <param name="shopsFavoriteRequest">增加实体</param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "ShopsFavoriteCreate" })]
        public async Task<ResponseMessage<ShopsFavoriteResponse>> PostShopsFavorite(UserInfo user, [FromBody]ShopsFavoriteRequest shopsFavoriteRequest)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})新增商铺收藏信息(PostShopsFavorite)：\r\n请求参数为：\r\n" + (shopsFavoriteRequest != null ? JsonHelper.ToJson(shopsFavoriteRequest) : ""));

            var response = new ResponseMessage<ShopsFavoriteResponse>();
            if (!ModelState.IsValid)
            {
                var error = "";
                var errors = ModelState.Values.ToList();
                foreach (var item in errors)
                {
                    foreach (var e in item.Errors)
                    {
                        error += e.ErrorMessage + "  ";
                    }
                }
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = error;
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})新增商铺收藏信息(PostShopsFavorite)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (shopsFavoriteRequest != null ? JsonHelper.ToJson(shopsFavoriteRequest) : ""));
                return response;
            }
            try
            {
                if (await _shopsFavoriteManager.FindByUserIdAndShopsIdAsync(user.Id, shopsFavoriteRequest.ShopsId) != null)
                {
                    response.Code = ResponseCodeDefines.NotAllow;
                    response.Message = "当前已收藏该商铺";
                    Logger.Info($"用户{user?.UserName ?? ""}({user?.Id ?? ""})新增商铺收藏信息(PostShopsFavorite)失败：该商铺已经被收藏,\r\n请求的参数为：\r\n" + (shopsFavoriteRequest != null ? JsonHelper.ToJson(shopsFavoriteRequest) : ""));
                    return response;
                }
                response.Extension = await _shopsFavoriteManager.CreateAsync(user, shopsFavoriteRequest, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})新增商铺收藏信息(PostShopsFavorite)请求失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (shopsFavoriteRequest != null ? JsonHelper.ToJson(shopsFavoriteRequest) : ""));
            }
            return response;
        }


        /// <summary>
        /// 修改商铺收藏信息
        /// </summary>
        /// <param name="user">用户Id</param>
        /// <param name="shopsFavoriteRequest">修改实体</param>
        /// <returns></returns>
        [HttpPut]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "ShopsFavoriteUpdate" })]
        public async Task<ResponseMessage> PutShopsFavorite(UserInfo user, [FromBody]ShopsFavoriteRequest shopsFavoriteRequest)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})修改商铺收藏信息(PutShopsFavorite)：\r\n请求参数为：\r\n" + (shopsFavoriteRequest != null ? JsonHelper.ToJson(shopsFavoriteRequest) : ""));

            var response = new ResponseMessage();
            if (!ModelState.IsValid)
            {
                var error = "";
                var errors = ModelState.Values.ToList();
                foreach (var item in errors)
                {
                    foreach (var e in item.Errors)
                    {
                        error += e.ErrorMessage + "  ";
                    }
                }
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = error;
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})修改商铺收藏信息(PutShopsFavorite)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (shopsFavoriteRequest != null ? JsonHelper.ToJson(shopsFavoriteRequest) : ""));
                return response;
            }
            try
            {
                var dictionaryGroup = await _shopsFavoriteManager.FindByIdAsync(shopsFavoriteRequest.Id, HttpContext.RequestAborted);
                if (dictionaryGroup == null)
                {
                    await _shopsFavoriteManager.CreateAsync(user, shopsFavoriteRequest, HttpContext.RequestAborted);
                }
                await _shopsFavoriteManager.UpdateAsync(user.Id, shopsFavoriteRequest, HttpContext.RequestAborted);
                response.Code = ResponseCodeDefines.SuccessCode;
                response.Message = "修改成功";
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})修改商铺收藏信息(PutShopsFavorite)请求失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (shopsFavoriteRequest != null ? JsonHelper.ToJson(shopsFavoriteRequest) : ""));
            }
            return response;
        }

        /// <summary>
        /// 删除单个商铺收藏
        /// </summary>
        /// <param name="user">删除用户</param>
        /// <param name="shopsId"></param>
        /// <returns></returns>
        [HttpDelete("{shopsId}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "FavoriteDelete" })]
        public async Task<ResponseMessage> DeleteShopsFavorite(UserInfo user, [FromRoute] string shopsId)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})删除单个商铺收藏(DeleteShopsFavorite)：\r\n请求参数为：\r\n(shopsId){shopsId ?? ""}");

            ResponseMessage response = new ResponseMessage();
            if (string.IsNullOrEmpty(shopsId))
            {
                response.Code = ResponseCodeDefines.ArgumentNullError;
                response.Message = "参数出错";
                return response;
            }
            try
            {
                await _shopsFavoriteManager.DeleteAsync(user, shopsId, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})删除单个商铺收藏(DeleteShopsFavorite)报错：\r\n{e.ToString()}，\r\n请求参数为：\r\n(shopsId){shopsId ?? ""}");
                return response;
            }
            return response;
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="user">用户Id</param>
        /// <param name="favoriteIds">商铺收藏idList</param>
        /// <returns></returns>
        [HttpPost("delete")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "FavoriteDeletes" })]
        public async Task<ResponseMessage> DeletePermissionItems(UserInfo user, [FromBody] List<string> favoriteIds)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})批量删除商铺收藏(DeletePermissionItems)：\r\n请求参数为：\r\n" + (favoriteIds != null ? JsonHelper.ToJson(favoriteIds) : ""));

            ResponseMessage response = new ResponseMessage();
            if (favoriteIds == null)
            {
                response.Code = ResponseCodeDefines.ArgumentNullError;
                response.Message = "参数出错";
                return response;
            }
            try
            {
                await _shopsFavoriteManager.DeleteListAsync(user.Id, favoriteIds, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})批量删除商铺收藏(DeletePermissionItems)报错：\r\n{e.ToString()}，\r\n请求参数为：\r\n" + (favoriteIds != null ? JsonHelper.ToJson(favoriteIds) : ""));
            }
            return response;
        }

    }
}
