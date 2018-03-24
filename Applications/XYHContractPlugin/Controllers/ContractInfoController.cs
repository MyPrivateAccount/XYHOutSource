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
using ContractContentInfoRequest = XYHContractPlugin.Dto.Response.ContractContentResponse;
using ContractAnnexInfoRequest = XYHContractPlugin.Dto.Response.ContractAnnexResponse;
using AspNet.Security.OAuth.Validation;

namespace XYHContractPlugin.Controllers
{
    [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
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
        public async Task<ResponseMessage<List<int>>> GetTestInfo(UserInfo user)//[FromRoute]string testinfo
        {
            var Response = new ResponseMessage<List<int>>();
            //if (string.IsNullOrEmpty(testinfo))
            //{
            //    Response.Code = ResponseCodeDefines.ModelStateInvalid;
            //    Response.Message = "请求参数不正确";
            //}
            try
            {
                return Response;
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

        [HttpGet("{getcontractbyid}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<ContractContentResponse>> GetContractByid(UserInfo user, [FromRoute] string contractId)
        {
            var Response = new ResponseMessage<ContractContentResponse>();
            if (string.IsNullOrEmpty(contractId))
            {
                Response.Code = ResponseCodeDefines.ModelStateInvalid;
                Response.Message = "请求参数不正确";
                Logger.Error("error GetContractByid");
                return Response;
            }
            try
            {
                Response.Extension = await _contractInfoManager.GetAllinfoByIdAsync(contractId, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                Response.Code = ResponseCodeDefines.ServiceError;
                Response.Message = "服务器错误：" + e.ToString();
                Logger.Error("error");
            }
            return Response;
        }

        [HttpGet("{getallcontractbyuser}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<List<ContractContentResponse>>> GetAllContractByUser(UserInfo user, [FromRoute] string contractId)
        {
            //if (user.Id == null)
            //{
            //    {
            //        user.Id = "66df64cb-67c5-4645-904f-704ff92b3e81";
            //        user.UserName = "wqtest";
            //        user.KeyWord = "";
            //        user.OrganizationId = "270";
            //        user.PhoneNumber = "18122132334";
            //    };
            //}

            var Response = new ResponseMessage<List<ContractContentResponse>>();
            if (string.IsNullOrEmpty(contractId))
            {
                Response.Code = ResponseCodeDefines.ModelStateInvalid;
                Response.Message = "请求参数不正确";
                Logger.Error("error GetContractByid");
                return Response;
            }
            try
            {
                Response.Extension = await _contractInfoManager.GetAllListinfoByUserIdAsync(user.Id, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                Response.Code = ResponseCodeDefines.ServiceError;
                Response.Message = "服务器错误：" + e.ToString();
                Logger.Error("error");
            }
            return Response;
        }

        [HttpPost("discardcontract")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<ContractContentResponse>> DiscardContractByid(UserInfo user, [FromRoute] string contractId)
        {
            var Response = new ResponseMessage<ContractContentResponse>();
            if (string.IsNullOrEmpty(contractId))
            {
                Response.Code = ResponseCodeDefines.ModelStateInvalid;
                Response.Message = "请求参数不正确";
                Logger.Error("error GetContractByid");
                return Response;
            }
            try
            {
                await _contractInfoManager.DiscardAsync(user, contractId, HttpContext.RequestAborted);
                Response.Message = $"discard {contractId} sucess";
            }
            catch (Exception e)
            {
                Response.Code = ResponseCodeDefines.ServiceError;
                Response.Message = "服务器错误：" + e.ToString();
                Logger.Error("error");
            }
            return Response;
        }

        [HttpGet("getmodifyhistorybyid/{contractId}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<List<ContractModifyResponse>>> GetHistoryByid(UserInfo user, [FromRoute] string contractId)
        {
            var Response = new ResponseMessage<List<ContractModifyResponse>>();
            if (string.IsNullOrEmpty(contractId))
            {
                Response.Code = ResponseCodeDefines.ModelStateInvalid;
                Response.Message = "请求参数不正确";
                Logger.Error("error GetContractByid");
                return Response;
            }
            try
            {
                Response.Extension = await _contractInfoManager.GetAllModifyInfo(contractId, HttpContext.RequestAborted);
                Response.Message = $"getmodifyhistorybyid {contractId} sucess";
            }
            catch (Exception e)
            {
                Response.Code = ResponseCodeDefines.ServiceError;
                Response.Message = "服务器错误：" + e.ToString();
                Logger.Error("error");
            }
            return Response;
        }

        [HttpPost("addsimplecontract")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<ContractInfoResponse>> AddSimpleContract(UserInfo User, [FromBody]ContractContentInfoRequest request)
        {
            Logger.Trace($"用户{User?.UserName ?? ""}({User?.Id ?? ""})保存合同基础信息(PutBuildingBaseInfo)：\r\n请求参数为：\r\n" + (request != null ? JsonHelper.ToJson(request) : ""));

            //if (User.Id == null)
            //{
            //    {
            //        User.Id = "66df64cb-67c5-4645-904f-704ff92b3e81";
            //        User.UserName = "wqtest";
            //        User.KeyWord = "";
            //        User.OrganizationId = "270";
            //        User.PhoneNumber = "18122132334";
            //    };
            //}

            var response = new ResponseMessage<ContractInfoResponse>();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                return response;
            }

            try
            {
                //写发送成功后的表
                response.Extension = await _contractInfoManager.AddContractAsync(User, request, HttpContext.RequestAborted);
                response.Message = "add simple ok";
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{User?.UserName ?? ""}({User?.Id ?? ""})合同动态提交审核(UpdateRecordSubmit)报错：\r\n{e.ToString()},\r\n请求参数为：\r\n" + (request != null ? JsonHelper.ToJson(request) : ""));
            }

            return response;
        }

        [HttpPost("modifysimplecontract")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<string>> ModifySimpleContract(UserInfo User, [FromBody]ContractContentInfoRequest request)
        {
            Logger.Trace($"用户{User?.UserName ?? ""}({User?.Id ?? ""})保存合同基础信息(PutBuildingBaseInfo)：\r\n请求参数为：\r\n" + (request != null ? JsonHelper.ToJson(request) : ""));

            var response = new ResponseMessage<string>();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                return response;
            }

            try
            {
                //写发送成功后的表
                var guid = await _contractInfoManager.ModifyContractBeforCheckAsync(User, request, HttpContext.RequestAborted);

                GatewayInterface.Dto.ExamineSubmitRequest exarequest = new GatewayInterface.Dto.ExamineSubmitRequest();
                exarequest.ContentId = request.BaseInfo.ID;
                exarequest.ContentType = "Contract";
                exarequest.ContentName = "Modify";
                exarequest.SubmitDefineId = guid;
                exarequest.Source = "";
                exarequest.CallbackUrl = ApplicationContext.Current.UpdateExamineCallbackUrl;
                exarequest.Action = "TEST"/*"TEST" exarequest.ContentType*/;
                exarequest.TaskName = $"{User.UserName}修改合同{exarequest.ContentName}的动态{exarequest.ContentType}";

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

                response.Extension = guid;
                response.Message = "modify simple ok";
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{User?.UserName ?? ""}({User?.Id ?? ""})合同动态提交审核(UpdateRecordSubmit)报错：\r\n{e.ToString()},\r\n请求参数为：\r\n" + (request != null ? JsonHelper.ToJson(request) : ""));
            }

            return response;
        }

        [HttpPost("checksimplecontract")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<bool>> CheckSimpleContract(UserInfo User, [FromBody]ContractCheckInfoRequest request)
        {
            Logger.Trace($"用户{User?.UserName ?? ""}({User?.Id ?? ""})审核合同基础信息(PutBuildingBaseInfo)：\r\n请求参数为：\r\n" + (request != null ? JsonHelper.ToJson(request) : ""));

            if (User.Id == null)
            {
                {
                    User.Id = "66df64cb-67c5-4645-904f-704ff92b3e81";
                    User.UserName = "wqtest";
                    User.KeyWord = "";
                    User.OrganizationId = "270";
                    User.PhoneNumber = "18122132334";
                };
            }

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
                exarequest.ContentId = request.ContractID;
                exarequest.ContentType = "Contract";
                exarequest.ContentName = request.CheckName;
                exarequest.SubmitDefineId = request.ModifyID;
                exarequest.Source = "";
                exarequest.CallbackUrl = ApplicationContext.Current.UpdateExamineCallbackUrl;
                exarequest.Action = request.Action/*"TEST" exarequest.ContentType*/;
                exarequest.TaskName = $"{User.UserName}提交合同{exarequest.ContentName}的动态{exarequest.ContentType}";

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
                await _contractInfoManager.SubmitAsync(request, ExamineStatusEnum.Auditing, HttpContext.RequestAborted);
                response.Message = $"check {request.CheckName} sucess";
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{User?.UserName ?? ""}({User?.Id ?? ""})合同动态提交审核(UpdateRecordSubmit)报错：\r\n{e.ToString()},\r\n请求参数为：\r\n" + (request != null ? JsonHelper.ToJson(request) : ""));
            }

            return response;
        }

        [HttpGet("getcontractcurcheck/{contractId}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<ContractModifyResponse>> GetContractCurCheck(UserInfo user, [FromRoute] string contractId)
        {
            var Response = new ResponseMessage<ContractModifyResponse>();
            if (string.IsNullOrEmpty(contractId))
            {
                Response.Code = ResponseCodeDefines.ModelStateInvalid;
                Response.Message = "请求参数不正确";
                Logger.Error("error GetContractByid");
                return Response;
            }
            try
            {
                Response.Extension = await _contractInfoManager.CurrentModifyByContractIdAsync(contractId, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                Response.Code = ResponseCodeDefines.ServiceError;
                Response.Message = "服务器错误：" + e.ToString();
                Logger.Error("error");
            }
            return Response;
        }

        #region//测试接口
        [HttpPost("addcontract")]//
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<bool>> AddContract(UserInfo User, [FromBody]ContractInfoRequest request)
        {
            Logger.Trace($"用户{User?.UserName ?? ""}({User?.Id ?? ""})保存合同基础信息(PutBuildingBaseInfo)：\r\n请求参数为：\r\n" + (request != null ? JsonHelper.ToJson(request) : ""));

            if (User.Id == null)
            {
                {
                    User.Id = "66df64cb-67c5-4645-904f-704ff92b3e81";
                    User.UserName = "wqtest";
                    User.KeyWord = "";
                    User.OrganizationId = "270";
                    User.PhoneNumber = "18122132334";
                };

            }

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
                exarequest.SubmitDefineId = Guid.NewGuid().ToString();
                exarequest.Source = "";
                exarequest.CallbackUrl = ApplicationContext.Current.UpdateExamineCallbackUrl;
                exarequest.Action = "TEST"/*exarequest.ContentType*/;
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
                await _contractInfoManager.CreateAsync(User,request, exarequest.SubmitDefineId, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{User?.UserName ?? ""}({User?.Id ?? ""})合同动态提交审核(UpdateRecordSubmit)报错：\r\n{e.ToString()},\r\n请求参数为：\r\n" + (request != null ? JsonHelper.ToJson(request) : ""));
            }

            return response;
        }

        #endregion
        [HttpPost("audit/updatecontractcallback")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage> UpdateRecordContractCallback([FromBody] ExamineResponse examineResponse)
        {
            Logger.Warn($" 加推、报备规则、佣金方案、楼栋批次、优惠政策审核回调接口(UpdateRecordSubmitCallback)：\r\n请求参数为：\r\n" + (examineResponse != null ? JsonHelper.ToJson(examineResponse) : ""));

            ResponseMessage response = new ResponseMessage();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Warn($"房源动态审核回调(UpdateRecordSubmitCallback)报错：\r\n{response.Message ?? ""},\r\n请求参数为：\r\n" + (examineResponse != null ? JsonHelper.ToJson(examineResponse) : ""));

                return response;
            }
            try
            {
                //await _updateRecordManager.UpdateRecordSubmitCallback(examineResponse);

            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"房源动态审核回调(UpdateRecordSubmitCallback)报错：\r\n{response.Message ?? ""},\r\n请求参数为：\r\n" + (examineResponse != null ? JsonHelper.ToJson(examineResponse) : ""));
            }
            return response;
        }

        [HttpPost("audit/submitcontractcallback")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<ExamineStatusEnum>> SubmitContractCallback([FromBody] ExamineResponse examineResponse)
        {
            Logger.Trace($"合同提交审核中心回调(SubmitContractCallback)：\r\n请求参数为：\r\n" + (examineResponse != null ? JsonHelper.ToJson(examineResponse) : ""));

            ResponseMessage<ExamineStatusEnum> response = new ResponseMessage<ExamineStatusEnum>();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Error($"合同提交审核中心回调(SubmitContractCallback)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (examineResponse != null ? JsonHelper.ToJson(examineResponse) : ""));
                return response;
            }
            try
            {
                var building = await _contractInfoManager.FindByIdAsync(examineResponse.ContentId, HttpContext.RequestAborted);
                if (building == null)
                {
                    response.Code = ResponseCodeDefines.NotFound;
                    response.Message = "合同不存在：" + examineResponse.ContentId;
                    Logger.Error($"合同提交审核中心回调(SubmitBuildingCallback)失败：合同不存在，\r\n请求参数为：\r\n" + (examineResponse != null ? JsonHelper.ToJson(examineResponse) : ""));
                    return response;
                }
                if (examineResponse.ExamineStatus == ExamineStatus.Examined)
                {
                    await _contractInfoManager.SubmitAsync(examineResponse.SubmitDefineId, ExamineStatusEnum.Approved);
                    response.Extension = ExamineStatusEnum.Approved;
                }
                else if (examineResponse.ExamineStatus == ExamineStatus.Reject)
                {
                    await _contractInfoManager.SubmitAsync(examineResponse.SubmitDefineId, ExamineStatusEnum.Reject);
                    response.Extension = ExamineStatusEnum.Reject;
                }
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"合同提交审核中心回调(SubmitBuildingCallback)报错：\r\n{e.ToString()}，\r\n请求参数为：\r\n" + examineResponse != null ? JsonHelper.ToJson(examineResponse) : "");
            }
            return response;
        }

    }
}
