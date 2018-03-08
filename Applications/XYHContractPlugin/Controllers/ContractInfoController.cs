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
using ContractInfoRequest = XYHContractPlugin.Dto.Response.ContractInfoResponse;

namespace XYHContractPlugin.Controllers
{
    //[Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/contractinfo")]
    public class ContractInfoController : Controller
    {
        private readonly ILogger Logger = LoggerManager.GetLogger("Contractinfo");
        private readonly ContractInfoManager _contractInfoManager;

        public ContractInfoController(ContractInfoManager contractManager)
        {
            _contractInfoManager = contractManager;
        }


        [HttpGet("testinfo")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<List<int>>> GetTestInfo()//[FromRoute]string testinfo
        {
            var Response = new ResponseMessage<List<int>>();
            //if (string.IsNullOrEmpty(testinfo))
            //{
            //    Response.Code = ResponseCodeDefines.ModelStateInvalid;
            //    Response.Message = "请求参数不正确";
            //}
            try
            {
                //Response.Extension = await _userTypeValueManager.FindByTypeAsync(user.Id, type, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                Response.Code = ResponseCodeDefines.ServiceError;
                Response.Message = "服务器错误：" + e.ToString();
                Logger.Error("error");
            }
            return Response;
        }

        [HttpGet("{contractid}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<ContractInfoResponse>> GetContractByid(UserInfo user, [FromRoute] string contractId)
        {
            var Response = new ResponseMessage<ContractInfoResponse>();
            if (string.IsNullOrEmpty(contractId))
            {
                Response.Code = ResponseCodeDefines.ModelStateInvalid;
                Response.Message = "请求参数不正确";
                Logger.Error("error GetContractByid");
                return Response;
            }
            try
            {
                Response.Extension = await _contractInfoManager.FindByIdAsync(contractId, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                Response.Code = ResponseCodeDefines.ServiceError;
                Response.Message = "服务器错误：" + e.ToString();
                Logger.Error("error");
            }
            return Response;
        }

        [HttpPost("addcontract")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<bool>> AddContract(UserInfo User, [FromBody]ContractInfoRequest request)
        {
            Logger.Trace($"用户{User?.UserName ?? ""}({User?.Id ?? ""})保存楼盘基础信息(PutBuildingBaseInfo)：\r\n请求参数为：\r\n" + (request != null ? JsonHelper.ToJson(request) : ""));

            var response = new ResponseMessage<bool>();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                return response;
            }

            try
            {
                GatewayInterface.Dto.ExamineSubmitRequest exarequest = new GatewayInterface.Dto.ExamineSubmitRequest();
                exarequest.ContentId = request.ID;
                exarequest.ContentType = "ContractCommit";
                exarequest.ContentName = request.Name;
                exarequest.SubmitDefineId = request.ID;
                exarequest.Source = "";
                exarequest.CallbackUrl = ApplicationContext.Current.UpdateExamineCallbackUrl;
                exarequest.Action = exarequest.ContentType;
                exarequest.TaskName = $"{User.UserName}提交合同{exarequest.ContentName}的动态{exarequest.ContentType}"; ;
                GatewayInterface.Dto.UserInfo userinfo = new GatewayInterface.Dto.UserInfo()
                {
                    Id = User.Id,
                    KeyWord = User.KeyWord,
                    OrganizationId = User.OrganizationId,
                    OrganizationName = User.OrganizationName,
                    UserName = User.UserName
                };

                var examineInterface = ApplicationContext.Current.Provider.GetRequiredService<IExamineInterface>();
                var reponse = await examineInterface.Submit(userinfo, exarequest);
                if (reponse.Code != ResponseCodeDefines.SuccessCode)
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Message = "向审核中心发起审核请求失败：" + reponse.Message;
                    return response;
                }

                //写发送成功后的表
                await _contractInfoManager.CreateAsync(request, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{User?.UserName ?? ""}({User?.Id ?? ""})合同动态提交审核(UpdateRecordSubmit)报错：\r\n{e.ToString()},\r\n请求参数为：\r\n" + (request != null ? JsonHelper.ToJson(request) : ""));
            }

            return response;
        }

    }
}
