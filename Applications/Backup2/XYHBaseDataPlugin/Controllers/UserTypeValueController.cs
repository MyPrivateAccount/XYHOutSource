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
using XYHBaseDataPlugin.Dto.Request;
using XYHBaseDataPlugin.Dto.Response;
using XYHBaseDataPlugin.Managers;

namespace XYHBaseDataPlugin.Controllers
{
    [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/userTypeValue")]
    public class UserTypeValueController : Controller
    {
        #region 成员

        private readonly UserTypeValueManager _userTypeValueManager;
        private readonly IMapper _mapper;
        private readonly ILogger Logger = LoggerManager.GetLogger("UserTypeValue");
        #endregion

        /// <summary>
        /// 用户自定义类型信息
        /// </summary>
        public UserTypeValueController(UserTypeValueManager userTypeValueManager, IMapper mapper)
        {
            _userTypeValueManager = userTypeValueManager;
            _mapper = mapper;
        }

        /// <summary>
        /// 获取用户自定义数据
        /// </summary>
        /// <param name="user"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        [HttpGet("{type}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "UserTypeValueByType" })]
        public async Task<ResponseMessage<List<UserTypeValueResponse>>> GetUserTypeValueList(UserInfo user, [FromRoute]string type)
        {
            var Response = new ResponseMessage<List<UserTypeValueResponse>>();
            if (string.IsNullOrEmpty(type))
            {
                Response.Code = ResponseCodeDefines.ModelStateInvalid;
                Response.Message = "请求参数不正确";
            }
            try
            {
                Response.Extension = await _userTypeValueManager.FindByTypeAsync(user.Id, type, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                Response.Code = ResponseCodeDefines.ServiceError;
                Response.Message = "服务器错误：" + e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})获取用户自定义数据(GetUserTypeValueList)报错：\r\n{e.ToString()}，\r\n请求参数为：\r\n(type){type ?? ""}");
            }
            return Response;
        }

        /// <summary>
        /// 获取用户自定义数据信息
        /// </summary>
        /// <param name="user">用户Id</param>
        /// <returns></returns>
        [HttpGet]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "UserTypeValueSelect" })]
        public async Task<ResponseMessage<List<UserTypeValueResponse>>> GetUserTypeValue(UserInfo user)
        {
            var response = new ResponseMessage<List<UserTypeValueResponse>>();
            try
            {
                response.Extension = await _userTypeValueManager.FindByUserIdAsync(user.Id, HttpContext.RequestAborted);
                response.Code = ResponseCodeDefines.SuccessCode;
                response.Message = "查询成功";
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})获取用户自定义数据信息(GetUserTypeValue)报错：\r\n{e.ToString()}");
            }
            return response;
        }

        /// <summary>
        /// 修改用户自定义数据信息
        /// </summary>
        /// <param name="user">用户Id</param>
        /// <param name="userTypeValueRequest">修改实体</param>
        /// <returns></returns>
        [HttpPut]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "UserTypeValueUpdate" })]
        public async Task<ResponseMessage> PutUserTypeValue(UserInfo user, [FromBody]UserTypeValueRequest userTypeValueRequest)
        {
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
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})修改用户自定义数据信息(PutUserTypeValue)模型验证失败：\r\n{response.Message ?? ""}，\r\n" + (userTypeValueRequest != null ? JsonHelper.ToJson(userTypeValueRequest) : ""));
                return response;
            }
            try
            {
                var dictionaryGroup = await _userTypeValueManager.FindByUserIdAndTypeAsync(user.Id, userTypeValueRequest.Type, HttpContext.RequestAborted);
                if (dictionaryGroup == null)
                {
                    await _userTypeValueManager.CreateAsync(user, userTypeValueRequest, HttpContext.RequestAborted);
                }
                await _userTypeValueManager.UpdateAsync(user, userTypeValueRequest, HttpContext.RequestAborted);
                response.Code = ResponseCodeDefines.SuccessCode;
                response.Message = "修改成功";
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})修改用户自定义数据信息(PutUserTypeValue)请求失败：\r\n{response.Message ?? ""}，\r\n" + (userTypeValueRequest != null ? JsonHelper.ToJson(userTypeValueRequest) : ""));
            }
            return response;
        }

        /// <summary>
        /// 批量删除
        /// </summary>
        /// <param name="user">用户Id</param>
        /// <param name="userTypeValueIds">用户自定义数据idList</param>
        /// <returns></returns>
        [HttpDelete("delete")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "UserTypeValueDeletes" })]
        public async Task<ResponseMessage> DeleteUserTypeValueItems(UserInfo user, [FromBody] List<string> userTypeValueIds)
        {
            ResponseMessage response = new ResponseMessage();
            if (userTypeValueIds == null)
            {
                response.Code = ResponseCodeDefines.ArgumentNullError;
                response.Message = "参数出错";
                return response;
            }
            try
            {

                await _userTypeValueManager.DeleteListAsync(user.Id, userTypeValueIds, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})批量删除(DeleteUserTypeValueItems)请求失败：\r\n{response.Message ?? ""}，\r\n" + (userTypeValueIds != null ? JsonHelper.ToJson(userTypeValueIds) : ""));
            }
            return response;
        }
    }
}
