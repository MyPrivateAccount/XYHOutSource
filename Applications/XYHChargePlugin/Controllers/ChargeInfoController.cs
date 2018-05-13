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
        private readonly XYH.Core.Log.ILogger Logger = LoggerManager.GetLogger("XYHHumaninfo");
        private readonly ChargeManager _chargeManager;
        private readonly RestClient _restClient;
        protected PermissionExpansionManager _permissionExpansionManager { get; }

        public ChargeInfoController(RestClient rsc, ChargeManager charge, PermissionExpansionManager per)
        {
            _permissionExpansionManager = per;
            _restClient = rsc;
            _chargeManager = charge;
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

        [HttpGet("searchchargeinfo")]
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

        [HttpPost("addcharge")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<List<ChargeInfoResponse>>> AddHumanInfo(UserInfo User, [FromBody]ContentRequest request)
        {
            var Response = new ResponseMessage<List<ChargeInfoResponse>>();
            try
            {
                string modifyid = Guid.NewGuid().ToString();

                GatewayInterface.Dto.ExamineSubmitRequest exarequest = new GatewayInterface.Dto.ExamineSubmitRequest();
                exarequest.ContentId = request.ChargeInfo.ID;
                exarequest.ContentType = "ChargeCommit";
                exarequest.ContentName = $"addcharge {request.ChargeInfo.Name}";
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



        #region File
        [HttpPost("files/callback")]
        //[TypeFilter(typeof(CheckPermission), Arguments = new object[] { "FileCallBack" })]
        public async Task<ResponseMessage> FileCallback(string userId, [FromBody] List<FileInfoCallbackRequest> fileInfoCallbackRequestList)
        {
            Logger.Trace($"上传文件信息回调(FileCallback)请求日志,请求参数为：\r\n" + (fileInfoCallbackRequestList != null ? JsonHelper.ToJson(fileInfoCallbackRequestList) : ""));
            ResponseMessage response = new ResponseMessage();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Warn($"上传文件信息回调(FileCallback)模型验证失败：\r\n{response.Message ?? ""},请求参数为：\r\n" + (fileInfoCallbackRequestList != null ? JsonHelper.ToJson(fileInfoCallbackRequestList) : ""));
                return response;
            }
            try
            {
                await _chargeManager.CreateFilelistAsync(userId, fileInfoCallbackRequestList);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"上传文件信息回调(FileCallback)模型验证失败：\r\n{e.ToString()},请求参数为：\r\n" + (fileInfoCallbackRequestList != null ? JsonHelper.ToJson(fileInfoCallbackRequestList) : ""));
            }
            return response;
        }

        [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
        [HttpPost("{dest}/uploadmore/{receiptid}/")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "FileUpload" })]
        public async Task<ResponseMessage<string>> UploadFiles(UserInfo user, [FromBody]List<FileInfoRequest> fileInfoRequests, [FromRoute]string dest, [FromRoute]string receiptid)
        {
            ResponseMessage<string> response = new ResponseMessage<string>();
            if (fileInfoRequests == null)
            {
                response.Code = ResponseCodeDefines.ArgumentNullError;
                response.Message = "请求参数错误";
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})批量上传文件信息(UploadFiles)模型验证失败：\r\n{response.Message ?? ""},请求参数为：\r\n,(dest){dest ?? ""}," + (fileInfoRequests != null ? JsonHelper.ToJson(fileInfoRequests) : ""));
                return response;
            }

            try
            {
                foreach (var item in fileInfoRequests)
                {
                    NameValueCollection nameValueCollection = new NameValueCollection();
                    var nwf = CreateNwf(user, "", item);

                    nameValueCollection.Add("appToken", "app:nwf");
                    Logger.Info("nwf协议");
                    string response2 = await _restClient.Post(ApplicationContext.Current.NWFUrl, nwf, "POST", nameValueCollection);
                    Logger.Info("返回：\r\n{0}", response2);
                }

                await _chargeManager.CreateFileScopeAsync(user.Id, receiptid, fileInfoRequests, HttpContext.RequestAborted);
                response.Message = "update ok";
            }
            catch (Exception e)
            {

                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"上传文件信息回调(FileCallback)模型验证失败：\r\n{e.ToString()},请求参数为：\r\n" + (fileInfoRequests != null ? JsonHelper.ToJson(fileInfoRequests) : ""));
            }


            return response;
        }

        private NWF CreateNwf(UserInfo user, string source, FileInfoRequest fileInfoRequest)
        {
            NWF nwf = new NWF();
            var bodyinfo = new BodyInfoType();
            var header = new HeaderType();
            bodyinfo.FileInfo = new List<FileInfoType>();

            nwf.BodyInfo = bodyinfo;
            nwf.Header = header;


            header.TaskGuid = "";
            header.ContentGuid = fileInfoRequest.SourceId;
            header.Action = "ImageProcess";
            header.SourceSystem = source;

            header.ExtraAttribute = new List<AttributeType>();
            header.ExtraAttribute.Add(new AttributeType() { Name = "UserID", Value = user.Id });
            header.ExtraAttribute.Add(new AttributeType() { Name = "SubSystem", Value = "chargeinfo" });
            bodyinfo.Priority = 0;
            bodyinfo.TaskName = fileInfoRequest.Name;
            if (String.IsNullOrEmpty(bodyinfo.TaskName))
            {
                bodyinfo.TaskName = $"{user.UserName}-{source ?? ""}";
            }

            var extra = new List<AttributeType>();
            extra.Add(new AttributeType { Name = "WXAppID", Value = fileInfoRequest.AppId });
            extra.Add(new AttributeType { Name = "From", Value = fileInfoRequest.From });
            extra.Add(new AttributeType { Name = "Source", Value = fileInfoRequest.Source });
            extra.Add(new AttributeType { Name = "Name", Value = fileInfoRequest.Name });
            extra.Add(new AttributeType { Name = "FileExt", Value = fileInfoRequest.FileExt });
            bodyinfo.ExtraAttribute = extra;

            FileInfoType fileInfoType = new FileInfoType();
            fileInfoType.FilePath = fileInfoRequest.WXPath;
            fileInfoType.FileExt = fileInfoRequest.FileExt;
            fileInfoType.FileGuid = fileInfoRequest.FileGuid;
            fileInfoType.QualityType = 0;
            fileInfoType.FileTypeId = "ROW";
            fileInfoType.ExtraAttribute = new List<AttributeType>();

            nwf.BodyInfo.FileInfo.Add(fileInfoType);
            return nwf;
        }
        #endregion


    }
}
