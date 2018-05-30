using ApplicationCore;
using ApplicationCore.Dto;
using ApplicationCore.Filters;
using AspNet.Security.OAuth.Validation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using XYH.Core.Log;
using System.Text;
using System.Collections.Specialized;
using System.Threading.Tasks;
using XYH.Core.Log;
using XYHChargePlugin.Dto.Request;
using XYHChargePlugin.Dto.Response;
using Microsoft.Extensions.DependencyInjection;
using ChargeInfoRequest = XYHChargePlugin.Dto.Response.ChargeInfoResponse;
using GatewayInterface;
using XYHChargePlugin.Dto.Common;
using XYHChargePlugin.Managers;
using ApplicationCore.Managers;

namespace XYHChargePlugin.Controllers
{
    [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/chargeinfo")]
    public class ChargeInfoController : Controller
    {
        private readonly XYH.Core.Log.ILogger Logger = LoggerManager.GetLogger("XYHChargeinfo");
        private readonly ChargeManager _chargeManager;
        private readonly RestClient _restClient;
        protected PermissionExpansionManager _permissionExpansionManager { get; }

        private static string _lastDate;
        private static int _lastNumber;

        public ChargeInfoController(RestClient rsc, ChargeManager charge, PermissionExpansionManager per)
        {
            _permissionExpansionManager = per;
            _restClient = rsc;
            _chargeManager = charge;

            _lastNumber = 1;
            _restClient = rsc;
        }

        [HttpGet("testinfo")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<List<int>>> GetTestInfo([FromRoute]string testinfo)
        {
            var Response = new ResponseMessage<List<int>>();
            if (string.IsNullOrEmpty(testinfo))
            {
                Response.Code = ResponseCodeDefines.ModelStateInvalid;
                Response.Message = "请求参数不正确";
            }
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

        [HttpGet("chargeid")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<string>> GetChargeID()
        {
            var Response = new ResponseMessage<string>();
            
            try
            {
                var td = DateTime.Now;
                if (td.Month.ToString() + td.Day.ToString() == _lastDate)
                {
                    _lastNumber++;
                }
                else
                {
                    _lastNumber = 0;
                    _lastDate = td.Month.ToString() + td.Day.ToString();
                }

                Response.Extension = $"FY-{td.Year.ToString()}{td.Month.ToString()}{td.Day.ToString()}-{_lastNumber}";
                Response.Message = $"getchargenumber sucess";
            }
            catch (Exception e)
            {
                Response.Code = ResponseCodeDefines.ServiceError;
                Response.Message = "服务器错误：" + e.ToString();
                Logger.Error("error");
            }
            return Response;
        }

        [HttpGet("chargedetail/{chargeid}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<ChargeDetailInfoResponse>> GetChargeDetail(UserInfo User, string chargeid)
        {
            var Response = new ResponseMessage<ChargeDetailInfoResponse>();

            if (!ModelState.IsValid)
            {
                Response.Code = ResponseCodeDefines.ModelStateInvalid;
                Logger.Warn($"用户{User?.UserName ?? ""}({User?.Id ?? ""})查询人事条件(PostCustomerListSaleMan)模型验证失败：\r\n{Response.Message ?? ""}，\r\n请求参数为：\r\n" + (chargeid != null ? JsonHelper.ToJson(chargeid) : ""));
                return Response;
            }

            try
            {
                Response.Extension = await _chargeManager.GetChargeDetailInfo(chargeid);
                //Response.Extension = $"FY-{td.Year.ToString()}{td.Month.ToString()}{td.Day.ToString()}-{_lastNumber}";
                Response.Message = $"getchargenumber sucess";
            }
            catch (Exception e)
            {
                Response.Code = ResponseCodeDefines.ServiceError;
                Response.Message = "服务器错误：" + e.ToString();
                Logger.Error("error");
            }
            return Response;
        }

        [HttpPost("searchchargeinfo")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<List<ChargeInfoResponse>>> SearchChargeInfo(UserInfo User, [FromBody]ChargeSearchRequest condition)
        {
            var pagingResponse = new ChargeSearchResponse<ChargeInfoResponse>();
            if (!ModelState.IsValid)
            {
                pagingResponse.Code = ResponseCodeDefines.ModelStateInvalid;
                Logger.Warn($"用户{User?.UserName ?? ""}({User?.Id ?? ""})查询人事条件(PostCustomerListSaleMan)模型验证失败：\r\n{pagingResponse.Message ?? ""}，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
                return pagingResponse;
            }

            try
            {
                if (await _permissionExpansionManager.HavePermission(User.Id, "SEARCH_CHARGE"))
                {
                    pagingResponse = await _chargeManager.SearchChargeInfo(User, condition, false, HttpContext.RequestAborted);
                }
                else
                {
                    pagingResponse = await _chargeManager.SearchChargeInfo(User, condition, true, HttpContext.RequestAborted);
                }
            }
            catch (Exception e)
            {
                pagingResponse.Code = ResponseCodeDefines.ServiceError;
                pagingResponse.Message = "服务器错误:" + e.ToString();
                Logger.Error($"用户{User?.UserName ?? ""}({User?.Id ?? ""})查询业务员条件(PostCustomerListSaleMan)请求失败：\r\n{pagingResponse.Message ?? ""}，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));

            }
            return pagingResponse;
        }

        [HttpPost("setlimit")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<List<ChargeInfoResponse>>> SetLimit(UserInfo User, [FromBody]LimitHumanRequest cost)
        {
            var pagingResponse = new ChargeSearchResponse<ChargeInfoResponse>();
            if (!ModelState.IsValid)
            {
                pagingResponse.Code = ResponseCodeDefines.ModelStateInvalid;
                Logger.Warn($"用户{User?.UserName ?? ""}({User?.Id ?? ""})费用设定条件(PostCustomerListSaleMan)模型验证失败：\r\n{pagingResponse.Message ?? ""}，\r\n请求参数为：\r\n" );
                return pagingResponse;
            }

            try
            {
                await _chargeManager.UpdateLimit(cost.ID, cost.CostLimit, HttpContext.RequestAborted);
                pagingResponse.Message = "SetLimit ok";
            }
            catch (Exception e)
            {
                pagingResponse.Code = ResponseCodeDefines.ServiceError;
                pagingResponse.Message = "服务器错误:" + e.ToString();
                Logger.Error($"用户{User?.UserName ?? ""}({User?.Id ?? ""})费用设定条件(PostCustomerListSaleMan)请求失败：\r\n{pagingResponse.Message ?? ""}，\r\n请求参数为：\r\n" );

            }
            return pagingResponse;
        }

        [HttpGet("limitinfo")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<LimitInfoResponse>> GetLimitChargeHumanLst(UserInfo User)
        {
            var Response = new ResponseMessage<LimitInfoResponse>();
            
            try
            {
                Response.Extension = await _chargeManager.GetLimitInfo(User.Id, HttpContext.RequestAborted);
                Response.Message = $"limitinfo sucess";
            }
            catch (Exception e)
            {
                Response.Code = ResponseCodeDefines.ServiceError;
                Response.Message = "服务器错误：" + e.ToString();
                Logger.Error("error");
            }
            return Response;
        }

        [HttpPost("setrecieptinfo")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<List<ChargeInfoResponse>>> PostRecieptInfo(UserInfo User, [FromBody]List<ReceiptInfoResponse> reciept)
        {
            var pagingResponse = new ChargeSearchResponse<ChargeInfoResponse>();
            if (!ModelState.IsValid)
            {
                pagingResponse.Code = ResponseCodeDefines.ModelStateInvalid;
                Logger.Warn($"用户{User?.UserName ?? ""}({User?.Id ?? ""})查询费用单条件(PostCustomerListSaleMan)模型验证失败：\r\n{pagingResponse.Message ?? ""}，\r\n请求参数为：\r\n" + (reciept != null ? JsonHelper.ToJson(reciept) : ""));
                return pagingResponse;
            }

            try
            {
                await _chargeManager.UpdateRecieptList(reciept, HttpContext.RequestAborted);
                pagingResponse.Message = "setrecieptinfo ok";
            }
            catch (Exception e)
            {
                pagingResponse.Code = ResponseCodeDefines.ServiceError;
                pagingResponse.Message = "服务器错误:" + e.ToString();
                Logger.Error($"用户{User?.UserName ?? ""}({User?.Id ?? ""})查询费用单条件(PostCustomerListSaleMan)请求失败：\r\n{pagingResponse.Message ?? ""}，\r\n请求参数为：\r\n" + (reciept != null ? JsonHelper.ToJson(reciept) : ""));

            }
            return pagingResponse;
        }

        [HttpPost("addcharge")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<List<ChargeInfoResponse>>> AddChargeInfo(UserInfo User, [FromBody]ContentRequest request)
        {
            var Response = new ResponseMessage<List<ChargeInfoResponse>>();
            if (!ModelState.IsValid)
            {
                Response.Code = ResponseCodeDefines.ModelStateInvalid;
                Logger.Warn($"用户{User?.UserName ?? ""}({User?.Id ?? ""})paymentcharge(PostCustomerListSaleMan)模型验证失败：\r\n{Response.Message ?? ""}，\r\n请求参数为：\r\n" + (request != null ? JsonHelper.ToJson(request) : ""));
                return Response;
            }

            try
            {
                string modifyid = Guid.NewGuid().ToString();

                GatewayInterface.Dto.ExamineSubmitRequest exarequest = new GatewayInterface.Dto.ExamineSubmitRequest();
                exarequest.ContentId = request.ChargeInfo.ID;
                exarequest.ContentType = "ChargeCommit";
                exarequest.ContentName = $"addcharge {request.ChargeInfo.ID}";
                exarequest.SubmitDefineId = modifyid;
                exarequest.Source = "";
                exarequest.CallbackUrl = ApplicationContext.Current.UpdateExamineCallbackUrl;
                exarequest.Action = "TEST"/*exarequest.ContentType*/;
                exarequest.TaskName = $"{User.UserName}提交费用请求{exarequest.ContentName}的动态{exarequest.ContentType}";
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
                    Response.Code = ResponseCodeDefines.ServiceError;
                    Response.Message = "向审核中心发起审核请求失败：" + reponse.Message;
                    return Response;
                }

                //await _humanManage.AddHuman(User, condition, modifyid, "TEST", HttpContext.RequestAborted);
                await _chargeManager.AddCharge(User, request, modifyid, "TEST", HttpContext.RequestAborted);

                Response.Message = $"addchargeinfo sucess";
            }
            catch (Exception e)
            {
                Response.Code = ResponseCodeDefines.ServiceError;
                Response.Message = "服务器错误：" + e.ToString();
                Logger.Error("error");
            }
            return Response;
        }

        [HttpPost("paymentcharge/{chargeid}/{department}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<string>> PaymentCharge(UserInfo User, [FromRoute]string chargeid, [FromRoute]string department)
        {
            var pagingResponse = new ResponseMessage<string>();
            if (!ModelState.IsValid)
            {
                pagingResponse.Code = ResponseCodeDefines.ModelStateInvalid;
                Logger.Warn($"用户{User?.UserName ?? ""}({User?.Id ?? ""})paymentcharge(PostCustomerListSaleMan)模型验证失败：\r\n{pagingResponse.Message ?? ""}，\r\n请求参数为：\r\n" + (chargeid != null ? JsonHelper.ToJson(chargeid) : ""));
                return pagingResponse;
            }

            try
            {
                await _chargeManager.UpdateChargePostTime(chargeid, department, HttpContext.RequestAborted);
                pagingResponse.Message = "updateposttime ok";
            }
            catch (Exception e)
            {
                pagingResponse.Code = ResponseCodeDefines.ServiceError;
                pagingResponse.Message = "服务器错误:" + e.ToString();
                Logger.Error($"用户{User?.UserName ?? ""}({User?.Id ?? ""})paymentcharge(PostCustomerListSaleMan)请求失败：\r\n{pagingResponse.Message ?? ""}，\r\n请求参数为：\r\n" + (chargeid != null ? JsonHelper.ToJson(chargeid) : ""));

            }
            return pagingResponse;
        }

        [HttpGet("getrecipt/{chargeid}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<List<ReceiptInfoResponse>>> GetRecipt(UserInfo User, [FromRoute]string chargeid)
        {
            var Response = new ResponseMessage<List<ReceiptInfoResponse>>();
            try
            {
                Response.Extension = await _chargeManager.GetRecieptbyID(User, chargeid);
                Response.Message = $"getrecipt sucess";
            }
            catch (Exception e)
            {
                Response.Code = ResponseCodeDefines.ServiceError;
                Response.Message = "服务器错误：" + e.ToString();
                Logger.Error("error");
            }
            return Response;
        }

        #region Flow
        [HttpPost("audit/updatechargecallback")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ApplicationCore.ResponseMessage> UpdateRecordChargeCallback([FromBody] ExamineResponse examineResponse)
        {
            Logger.Warn($"审核回调接口(UpdateRecordSubmitCallback)：\r\n请求参数为：\r\n" + (examineResponse != null ? JsonHelper.ToJson(examineResponse) : ""));

            ApplicationCore.ResponseMessage response = new ApplicationCore.ResponseMessage();
            if (!ModelState.IsValid)
            {
                response.Code = ApplicationCore.ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Warn($"费用动态审核回调(UpdateRecordSubmitCallback)报错：\r\n{response.Message ?? ""},\r\n请求参数为：\r\n" + (examineResponse != null ? JsonHelper.ToJson(examineResponse) : ""));
                return response;
            }

            try
            {
                //await _updateRecordManager.UpdateRecordSubmitCallback(examineResponse);

            }
            catch (Exception e)
            {
                response.Code = ApplicationCore.ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"费用动态审核回调(UpdateRecordSubmitCallback)报错：\r\n{response.Message ?? ""},\r\n请求参数为：\r\n" + (examineResponse != null ? JsonHelper.ToJson(examineResponse) : ""));
            }
            return response;
        }

        [HttpPost("audit/submitchargecallback")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ApplicationCore.ResponseMessage> SubmitChargeCallback([FromBody] ExamineResponse examineResponse)
        {
            Logger.Trace($"费用提交审核中心回调(SubmitContractCallback)：\r\n请求参数为：\r\n" + (examineResponse != null ? JsonHelper.ToJson(examineResponse) : ""));

            ApplicationCore.ResponseMessage response = new ApplicationCore.ResponseMessage();

            if (examineResponse == null)
            {
                response.Code = ApplicationCore.ResponseCodeDefines.ModelStateInvalid;
                Logger.Trace($"费用提交审核中心回调(SubmitContractCallback)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (examineResponse != null ? JsonHelper.ToJson(examineResponse) : ""));
                return response;
            }
            try
            {
                response.Code = ApplicationCore.ResponseCodeDefines.SuccessCode;

                if (examineResponse.ExamineStatus == ExamineStatus.Examined)
                {
                    await _chargeManager.SubmitAsync(examineResponse.SubmitDefineId, ExamineStatusEnum.Approved);
                }
                else if (examineResponse.ExamineStatus == ExamineStatus.Reject)
                {
                    await _chargeManager.SubmitAsync(examineResponse.SubmitDefineId, ExamineStatusEnum.Reject);
                }
            }
            catch (Exception e)
            {
                response.Code = ApplicationCore.ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Trace($"费用提交审核中心回调(SubmitBuildingCallback)报错：\r\n{e.ToString()}，\r\n请求参数为：\r\n" + examineResponse != null ? JsonHelper.ToJson(examineResponse) : "");
            }
            return response;
        }
        #endregion

        


    }
}
