using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using AspNet.Security.OAuth.Validation;
using ApplicationCore;
using ApplicationCore.Dto;
using ApplicationCore.Filters;
using XYH.Core.Log;
using XYHHumanPlugin.Dto.Request;
using XYHHumanPlugin.Dto.Response;
using XYHHumanPlugin.Stores;
using HumanInfRequest = XYHHumanPlugin.Dto.Response.HumanInfoResponse;
using XYHHumanPlugin.Managers;
using System.Collections.Specialized;
using XYHHumanPlugin.Dto.Common;
using GatewayInterface;
using Microsoft.Extensions.DependencyInjection;

namespace XYHHumanPlugin.Controllers
{
    [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/humaninfo")]
   public class HumanInfoController : Controller
    {
        private readonly ILogger Logger = LoggerManager.GetLogger("XYHHumaninfo");
        private readonly HumanManager _humanManage;
        private readonly RestClient _restClient;
        private string _lastDate;
        private int  _lastNumber;

        HumanInfoController(HumanManager human, RestClient rsc)
        {
            _humanManage = human;
            _lastNumber = 0;
            _restClient = rsc;
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

        [HttpPost("searchhumaninfo")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<HumanSearchResponse<HumanInfoResponse>> SearchHumanInfo(UserInfo User, [FromBody]HumanSearchRequest condition)
        {
            var pagingResponse = new HumanSearchResponse<HumanInfoResponse>();
            if (!ModelState.IsValid)
            {
                pagingResponse.Code = ResponseCodeDefines.ModelStateInvalid;
                Logger.Warn($"用户{User?.UserName ?? ""}({User?.Id ?? ""})查询人事条件(PostCustomerListSaleMan)模型验证失败：\r\n{pagingResponse.Message ?? ""}，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
                return pagingResponse;
            }

            try
            {
                //if (await _permissionExpansionManager.HavePermission(User.Id, "SEARCH_CONTRACT"))
                //{
                    pagingResponse = await _humanManage.SearchHumanInfo(User, condition, HttpContext.RequestAborted);
                //}
                //else
                //{
                //    pagingResponse.Code = ResponseCodeDefines.NotAllow;
                //    pagingResponse.Message = "权限不足";
                //}

            }
            catch (Exception e)
            {
                pagingResponse.Code = ResponseCodeDefines.ServiceError;
                pagingResponse.Message = "服务器错误:" + e.ToString();
                Logger.Error($"用户{User?.UserName ?? ""}({User?.Id ?? ""})查询业务员条件(PostCustomerListSaleMan)请求失败：\r\n{pagingResponse.Message ?? ""}，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));

            }
            return pagingResponse;
        }
        
        [HttpPost("addhuman")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<List<HumanInfoResponse>>> AddHumanInfo(UserInfo User, [FromBody]HumanInfRequest condition)
        {
            var Response = new ResponseMessage<List<HumanInfoResponse>>();
            try
            {
                string modifyid = Guid.NewGuid().ToString();

                GatewayInterface.Dto.ExamineSubmitRequest exarequest = new GatewayInterface.Dto.ExamineSubmitRequest();
                exarequest.ContentId = condition.ID;
                exarequest.ContentType = "ContractCommit";
                exarequest.ContentName = $"addhuman {condition.Name}";
                exarequest.SubmitDefineId = modifyid;
                exarequest.Source = "";
                exarequest.CallbackUrl = ApplicationContext.Current.UpdateExamineCallbackUrl;
                exarequest.Action = "TEST"/*exarequest.ContentType*/;
                exarequest.TaskName = $"{User.UserName}提交入职请求{exarequest.ContentName}的动态{exarequest.ContentType}"; ;
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

                await _humanManage.AddHuman(User, condition, modifyid, "TEST", HttpContext.RequestAborted);
                Response.Message = $"addhumaninfo sucess";
            }
            catch (Exception e)
            {
                Response.Code = ResponseCodeDefines.ServiceError;
                Response.Message = "服务器错误：" + e.ToString();
                Logger.Error("error");
            }
            return Response;
        }

        [HttpGet("jobnumber")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<string>> GetJobNumber(UserInfo User)
        {
            var Response = new ResponseMessage<string>();
            try
            {
                var td = DateTime.Now;
                if (td.Month.ToString()+td.Day.ToString() == _lastDate)
                {
                    _lastNumber++;
                }
                else 
                {
                    _lastNumber = 0;
                    _lastDate = td.Month.ToString() + td.Day.ToString();
                }

                Response.Extension = $"XYH-{td.Year.ToString()}{td.Month.ToString()}{td.Day.ToString()}-{_lastNumber}";
                Response.Message = $"getjobnumber sucess";
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
        [HttpPost("audit/updatecontractcallback")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage> UpdateRecordContractCallback([FromBody] ExamineResponse examineResponse)
        {
            Logger.Warn($"审核回调接口(UpdateRecordSubmitCallback)：\r\n请求参数为：\r\n" + (examineResponse != null ? JsonHelper.ToJson(examineResponse) : ""));

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
        public async Task<ResponseMessage> SubmitContractCallback([FromBody] ExamineResponse examineResponse)
        {
            Logger.Trace($"人事提交审核中心回调(SubmitContractCallback)：\r\n请求参数为：\r\n" + (examineResponse != null ? JsonHelper.ToJson(examineResponse) : ""));

            ResponseMessage response = new ResponseMessage();

            if (examineResponse == null)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                Logger.Trace($"人事提交审核中心回调(SubmitContractCallback)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (examineResponse != null ? JsonHelper.ToJson(examineResponse) : ""));
                return response;
            }
            try
            {
                response.Code = ResponseCodeDefines.SuccessCode;

                if (examineResponse.ExamineStatus == ExamineStatus.Examined)
                {
                    await _humanManage.SubmitAsync(examineResponse.SubmitDefineId, ExamineStatusEnum.Approved);
                }
                else if (examineResponse.ExamineStatus == ExamineStatus.Reject)
                {
                    await _humanManage.SubmitAsync(examineResponse.SubmitDefineId, ExamineStatusEnum.Reject);
                }
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Trace($"人事提交审核中心回调(SubmitBuildingCallback)报错：\r\n{e.ToString()}，\r\n请求参数为：\r\n" + examineResponse != null ? JsonHelper.ToJson(examineResponse) : "");
            }
            return response;
        }
        #endregion

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
                await _humanManage.CreateFilelistAsync(userId, fileInfoCallbackRequestList);
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
        [HttpPost("{dest}/uploadmore/")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "FileUpload" })]
        public async Task<ResponseMessage<string>> UploadFiles(UserInfo user, [FromBody]FileInfoRequest fileInfoRequests, [FromQuery]string source, [FromRoute]string dest)
        {
            ResponseMessage<string> response = new ResponseMessage<string>();
            if (fileInfoRequests == null)
            {
                response.Code = ResponseCodeDefines.ArgumentNullError;
                response.Message = "请求参数错误";
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})批量上传文件信息(UploadFiles)模型验证失败：\r\n{response.Message ?? ""},请求参数为：\r\n(source){source ?? ""},(dest){dest ?? ""}," + (fileInfoRequests != null ? JsonHelper.ToJson(fileInfoRequests) : ""));
                return response;
            }

            try
            {
                NameValueCollection nameValueCollection = new NameValueCollection();
                var nwf = CreateNwf(user, source, fileInfoRequests);

                nameValueCollection.Add("appToken", "app:nwf");
                Logger.Info("nwf协议");
                string response2 = await _restClient.Post(ApplicationContext.Current.NWFUrl, nwf, "POST", nameValueCollection);
                Logger.Info("返回：\r\n{0}", response2);

                await _humanManage.CreateFileScopeAsync(user.Id, fileInfoRequests, HttpContext.RequestAborted);

                response.Message = response2;
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
            header.ExtraAttribute.Add(new AttributeType() { Name = "SubSystem", Value = "humaninfo" });
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
