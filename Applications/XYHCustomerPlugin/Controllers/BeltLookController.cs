using ApplicationCore;
using ApplicationCore.Dto;
using ApplicationCore.Filters;
using ApplicationCore.Managers;
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
using XYHCustomerPlugin.Dto.Request;
using XYHCustomerPlugin.Dto.Response;
using XYHCustomerPlugin.Managers;

namespace XYHCustomerPlugin.Controllers
{
    [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/beltLook")]
    public class BeltLookController : Controller
    {
        #region 成员
        private readonly PermissionExpansionManager _permissionExpansionManager;
        private readonly BeltLookManager _beltLookManager;
        private readonly IMapper _mapper;
        private readonly ILogger Logger = LoggerManager.GetLogger("BeltLook");

        #endregion

        /// <summary>
        /// 带看
        /// </summary>
        public BeltLookController(BeltLookManager beltLookManager, IMapper mapper, PermissionExpansionManager permissionExpansionManager)
        {
            _permissionExpansionManager = permissionExpansionManager ?? throw new ArgumentNullException(nameof(permissionExpansionManager));
            _beltLookManager = beltLookManager;
            _mapper = mapper;
        }

        /// <summary>
        /// 根据带看Id查询信息
        /// </summary>
        /// <param name="userid"></param>
        /// <param name="beltLookId"></param>
        /// <returns></returns>
        [HttpGet("{beltLookId}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "BeltLookById" })]
        public async Task<ResponseMessage<BeltLookResponse>> GetBeltLook(UserInfo user, [FromRoute] string beltLookId)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})发起请求根据带看Id查询信息(GetBeltLook)：\r\n请求参数为：\r\n(beltLookId){beltLookId ?? ""}");
            var response = new ResponseMessage<BeltLookResponse>();
            if (string.IsNullOrEmpty(beltLookId))
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                return response;
            }
            try
            {
                response.Extension = await _beltLookManager.FindByIdAsync(beltLookId, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})根据带看Id查询信息(GetBeltLook)报错：\r\n{e.ToString()}，\r\n请求参数为：\r\n(beltLookId){beltLookId ?? ""}");
            }
            return response;
        }

        /// <summary>
        /// 查询我的带看信息
        /// </summary>
        /// <returns></returns>
        [HttpPost("[action]")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "CustomerInfoByMy" })]
        public async Task<PagingResponseMessage<BeltLookResponse>> GetMyCustomerInfo(UserInfo user, [FromBody]CustomerPageCondition pageCondition)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})发起请求查询我的带看信息(GetMyCustomerInfo)：\r\n请求参数为：\r\n" + (pageCondition != null ? JsonHelper.ToJson(pageCondition) : ""));

            var response = new PagingResponseMessage<BeltLookResponse>();
            if (pageCondition == null)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                return response;
            }
            try
            {
                response = await _beltLookManager.FindBeltLookByMyAsync(user.Id, pageCondition, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})查询我的带看信息(GetMyCustomerInfo)请求失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (pageCondition != null ? JsonHelper.ToJson(pageCondition) : ""));
            }
            return response;
        }

        /// <summary>
        /// 新增带看
        /// </summary>
        /// <param name="user">用户Id</param>
        /// <param name="beltLookRequest">增加实体</param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "BeltLookAdd" })]
        public async Task<ResponseMessage<BeltLookResponse>> PostBeltLook(UserInfo user, [FromBody]BeltLookRequest beltLookRequest)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})发起请求新增带看(PostBeltLook)：\r\n请求参数为：\r\n" + (beltLookRequest != null ? JsonHelper.ToJson(beltLookRequest) : ""));

            var response = new ResponseMessage<BeltLookResponse>();
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
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})新增带看(PostBeltLook)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (beltLookRequest != null ? JsonHelper.ToJson(beltLookRequest) : ""));
                return response;
            }
            try
            {
                response.Extension = await _beltLookManager.CreateAsync(user, beltLookRequest, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})新增带看(PostBeltLook)请求失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (beltLookRequest != null ? JsonHelper.ToJson(beltLookRequest) : ""));
            }
            return response;
        }

        /// <summary>
        /// 删除带看
        /// </summary>
        /// <param name="user">删除用户</param>
        /// <param name="beltLookId"></param>
        /// <returns></returns>
        [HttpDelete("{beltLookId}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "BeltLookDelete" })]
        public async Task<ResponseMessage> DeleteBeltLook(UserInfo user, [FromRoute] string beltLookId)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})发起请求删除带看(DeleteBeltLook)：\r\n请求参数为：\r\n(beltLookId){beltLookId ?? ""}");

            ResponseMessage response = new ResponseMessage();
            if (string.IsNullOrEmpty(beltLookId))
            {
                response.Code = ResponseCodeDefines.ArgumentNullError;
                response.Message = "参数出错";
                return response;
            }
            try
            {
                await _beltLookManager.DeleteAsync(user, beltLookId, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})删除带看(DeleteBeltLook)报错：\r\n{e.ToString()}，\r\n请求参数为：\r\n(beltLookId){beltLookId ?? ""}");
            }
            return response;
        }

        /// <summary>
        /// 修改带看信息
        /// </summary>
        /// <param name="user">用户Id</param>
        /// <param name="beltLookRequest">修改实体</param>
        /// <returns></returns>
        [HttpPut]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "BeltLookUpdate" })]
        public async Task<ResponseMessage> PutBeltLook(UserInfo user, [FromBody]BeltLookRequest beltLookRequest)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})发起请求修改带看信息(PutBeltLook)：\r\n请求参数为：\r\n" + (beltLookRequest != null ? JsonHelper.ToJson(beltLookRequest) : ""));

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
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})修改带看信息(PutBeltLook)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (beltLookRequest != null ? JsonHelper.ToJson(beltLookRequest) : ""));
                return response;
            }
            try
            {
                var dictionaryGroup = await _beltLookManager.FindByIdAsync(beltLookRequest.Id, HttpContext.RequestAborted);
                if (dictionaryGroup == null)
                {
                    await _beltLookManager.CreateAsync(user, beltLookRequest, HttpContext.RequestAborted);
                }
                await _beltLookManager.UpdateAsync(user.Id, beltLookRequest, HttpContext.RequestAborted);
                response.Code = ResponseCodeDefines.SuccessCode;
                response.Message = "修改成功";
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})修改带看信息(PutBeltLook)请求失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (beltLookRequest != null ? JsonHelper.ToJson(beltLookRequest) : ""));
            }
            return response;
        }

        /// <summary>
        /// 根据条件获取我的带看信息
        /// </summary>
        /// <param name="user">用户Id</param>
        /// <param name="condition">客源idList</param>
        /// <returns></returns>
        [HttpPost("[action]")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "BeltLookSreach" })]
        public async Task<PagingResponseMessage<BeltLookResponse>> GetMyBeltLook(UserInfo user, [FromBody] MyBeltLookCondition condition)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})发起请求根据条件获取我的带看信息(GetMyBeltLook)：\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));

            var response = new PagingResponseMessage<BeltLookResponse>();
            if (condition == null)
            {
                response.Code = ResponseCodeDefines.ArgumentNullError;
                response.Message = "参数出错";
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})根据条件获取我的带看信息(GetMyBeltLook)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
                return response;
            }
            try
            {
                response = await _beltLookManager.SelectMyCustomer(user.Id, condition, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})根据条件获取我的带看信息(GetMyBeltLook)请求失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
            }
            return response;
        }

        /// <summary>
        /// 查询带看信息
        /// </summary>
        /// <param name="user">用户Id</param>
        /// <param name="searchcondition">修改实体</param>
        /// <returns></returns>
        [HttpPost("[action]")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "BeltLookSelectMy" })]
        public async Task<PagingResponseMessage<BeltLookResponse>> Search(UserInfo  user, [FromBody]BeltLookSearchRequest searchcondition)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})发起请求查询带看信息(Search)：\r\n请求参数为：\r\n" + (searchcondition != null ? JsonHelper.ToJson(searchcondition) : ""));

            var response = new PagingResponseMessage<BeltLookResponse>();
            response.Code = ResponseCodeDefines.NotFound;
            response.Message = "未查询到信息";
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
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})查询带看信息(Search)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (searchcondition != null ? JsonHelper.ToJson(searchcondition) : ""));
                return response;
            }
            try
            {
                response = await _beltLookManager.Search(user.Id, searchcondition, HttpContext.RequestAborted);
                response.Code = ResponseCodeDefines.SuccessCode;
                response.Message = "查询成功";
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = "服务器错误：" + e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})查询带看信息(Search)请求失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (searchcondition != null ? JsonHelper.ToJson(searchcondition) : ""));
            }
            return response;
        }
    }
}
