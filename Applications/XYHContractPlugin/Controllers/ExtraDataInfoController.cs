using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using ApplicationCore;
using ApplicationCore.Dto;
using ApplicationCore.Filters;
using GatewayInterface;
using XYH.Core.Log;
using XYHContractPlugin.Dto.Response;
using XYHContractPlugin.Managers;
using AspNet.Security.OAuth.Validation;
using XYHContractPlugin.Models;
using System.Linq;
using XYHContractPlugin.Dto.Request;
using System.Collections.Specialized;
using XYHContractPlugin.Dto;
using CompanyAInfoRequest = XYHContractPlugin.Dto.Response.CompanyAInfoResponse;

namespace XYHContractPlugin.Controllers
{
    [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/extraInfo")]
    public class ExtraDataInfoController : Controller
    {
        private readonly ILogger Logger = LoggerManager.GetLogger("ExtraDataInfo");
        private readonly ExtraInfoDataManager _extraDataInfoManager;
    
        private readonly RestClient _restClient;

        public ExtraDataInfoController(ExtraInfoDataManager extraDataInfoManager, FileScopeManager fim, RestClient rsc)
        {
            _extraDataInfoManager = extraDataInfoManager;
            _restClient = rsc;
        }
        [HttpPost("addcompanyainfo")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<CompanyAInfoResponse>> AddCompanyAInfo(UserInfo User, [FromBody]CompanyAInfoRequest request)
        {
            Logger.Trace($"用户{User?.UserName ?? ""}({User?.Id ?? ""})添加甲方公司信息(AddCompanyAInfo)：\r\n请求参数为：\r\n" + (request != null ? JsonHelper.ToJson(request) : ""));

            var response = new ResponseMessage<CompanyAInfoResponse>();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                return response;
            }

            try
            {
 
                //写发送成功后的表
              
                response.Extension = await _extraDataInfoManager.CreateAsync(User,request, HttpContext.RequestAborted);
                response.Message = "添加成功!";
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{User?.UserName ?? ""}({User?.Id ?? ""})添加甲方公司信息报错：\r\n{e.ToString()},\r\n请求参数为：\r\n" + (request != null ? JsonHelper.ToJson(request) : ""));
            }

            return response;
        }

        [HttpPost("modifycompanyainfo/{id}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<bool>> ModifyCompanyAInfo(UserInfo User, [FromBody]CompanyAInfoRequest request, [FromRoute]string id)
        {
            Logger.Trace($"用户{User?.UserName ?? ""}({User?.Id ?? ""})修改甲方公司信息(ModifyCompanyAInfo)：\r\n请求参数为：\r\n" + (request != null ? JsonHelper.ToJson(request) : ""));

            var response = new ResponseMessage<bool>();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                return response;
            }

            
            try
            {

//                 var companyAInfo = await _extraDataInfoManager.GetCompanyAInfoAsync(id, HttpContext.RequestAborted);
//                 if(companyAInfo == null)
//                 {
//                     response.Code = ResponseCodeDefines.ArgumentNullError;
//                     response.Message = "该甲方公司不存在";
//                     return response;
//                 }

                await _extraDataInfoManager.ModifyAsync(request, HttpContext.RequestAborted);
                response.Extension = true;
                response.Message = "添加成功!";
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{User?.UserName ?? ""}({User?.Id ?? ""})修改甲方公司信息报错：\r\n{e.ToString()},\r\n请求参数为：\r\n" + (request != null ? JsonHelper.ToJson(request) : ""));
            }

            return response;
        }

        [HttpPost("deletecompanyainfo/{id}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<bool>> DeleteCompanyAInfo(UserInfo User, [FromRoute]string id)
        {
            Logger.Trace($"用户{User?.UserName ?? ""}({User?.Id ?? ""})添加甲方公司信息(DeleteCompanyAInfo)：\r\n请求参数为：\r\n" + (id != null ? id : ""));

            var response = new ResponseMessage<bool>();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                return response;
            }


            try
            {

                var companyAInfo = await _extraDataInfoManager.GetCompanyAInfoAsync(id, HttpContext.RequestAborted);
                if(companyAInfo == null)
                {
                    response.Code = ResponseCodeDefines.ArgumentNullError;
                    response.Message = "该甲方公司不存在";
                    return response;
                }

                await _extraDataInfoManager.DeleteAsync(User, companyAInfo, HttpContext.RequestAborted);
                response.Extension = true;
                response.Message = "添加成功!";
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{User?.UserName ?? ""}({User?.Id ?? ""})添加甲方公司信息报错：\r\n{e.ToString()},\r\n请求参数为：\r\n" + (id != null ? id : ""));
            }

            return response;
        }
        [HttpPost("searchCompanyA")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "CustomerInfoRelationHourse" })]
        public async Task<CompanyASearchResponse<CompanyAInfoResponse>> Search(UserInfo user, [FromBody]CompanyASearchCondition condition)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})查询条件(Search)：\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));

            var pagingResponse = new CompanyASearchResponse<CompanyAInfoResponse>();
            if (!ModelState.IsValid)
            {
                pagingResponse.Code = ResponseCodeDefines.ModelStateInvalid;
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})查询条件(Search)模型验证失败：\r\n{pagingResponse.Message ?? ""}，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
                return pagingResponse;
            }
            try
            {
                pagingResponse = await _extraDataInfoManager.Search(user, condition, HttpContext.RequestAborted);

            }
            catch (Exception e)
            {
                pagingResponse.Code = ResponseCodeDefines.ServiceError;
                pagingResponse.Message = "服务器错误:" + e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})查询条件(Search)请求失败：\r\n{pagingResponse.Message ?? ""}，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
            }
            return pagingResponse;
        }
    }


}
