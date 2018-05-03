using ApplicationCore;
using ApplicationCore.Dto;
using ApplicationCore.Filters;
using AspNet.Security.OAuth.Validation;
using AutoMapper;
using GatewayInterface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XYH.Core.Log;
using XYHContractPlugin.Dto;
using XYHContractPlugin.Dto.Request;
using XYHContractPlugin.Dto.Response;
using XYHContractPlugin.Managers;
using XYHContractPlugin.Models;

namespace XYHContractPlugin.Controllers
{
    //[Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/contractfiles")]
    public class ContractFileController : Controller
    {
        private readonly FileInfoManager _fileInfoManager;
        private readonly FileScopeManager _fileScopeManager;
        private readonly ContractInfoManager _contractInfoManager;
        private readonly RestClient _restClient;
        private readonly ILogger Logger = LoggerManager.GetLogger("XYHContractFileInfo");

        public ContractFileController(
            FileInfoManager fim,
            FileScopeManager fsm,
            ContractInfoManager cim,
            RestClient rsc)
        {
            _fileInfoManager = fim;
            _fileScopeManager = fsm;
            _contractInfoManager = cim;
            _restClient = rsc;
        }

        /// <summary>
        /// 批量上传文件
        /// </summary>
        /// <param name="user"></param>
        /// <param name="fileInfoRequests"></param>
        /// <param name="source"></param>
        /// <param name="contractId"></param>
        /// <returns></returns>
        // dest --> shops or building
        [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
        [HttpPost("{dest}/uploadmore/{contractId}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "FileUpload" })]
        public async Task<ResponseMessage> UploadFiles(UserInfo user, [FromBody]List<FileInfoRequest> fileInfoRequests, [FromQuery]string source, [FromRoute]string dest, [FromRoute]string contractId)
        {
            ResponseMessage response = new ResponseMessage();
            if (fileInfoRequests == null || fileInfoRequests.Count == 0)
            {
                response.Code = ResponseCodeDefines.ArgumentNullError;
                response.Message = "请求参数错误";
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})批量上传文件信息(UploadFiles)模型验证失败：\r\n{response.Message ?? ""},请求参数为：\r\n(source){source ?? ""},(dest){dest ?? ""},(contractId){contractId ?? ""}," + (fileInfoRequests != null ? JsonHelper.ToJson(fileInfoRequests) : ""));
                return response;
            }
            var info = await _contractInfoManager.FindByIdAsync(contractId, HttpContext.RequestAborted);
            if (info == null)
            {
                response.Code = ResponseCodeDefines.NotFound;
                response.Message = "合同不存在：" + contractId;
                Logger.Error($"合同文件上传失败：合同不存在，\r\n请求参数为：\r\n" + (fileInfoRequests != null ? JsonHelper.ToJson(fileInfoRequests) : ""));
                return response;
            }

            string strModifyGuid = Guid.NewGuid().ToString();

            try
            {
                GatewayInterface.Dto.ExamineSubmitRequest exarequest = new GatewayInterface.Dto.ExamineSubmitRequest();
                exarequest.ContentId = contractId;
                exarequest.ContentType = "ContractCommit";
                exarequest.ContentName = "UploadFiles";
                exarequest.SubmitDefineId = strModifyGuid;
                exarequest.Source = "";
                exarequest.CallbackUrl = ApplicationContext.Current.UpdateExamineCallbackUrl;
                exarequest.Action = "TEST";/* exarequest.ContentType*/;
                exarequest.TaskName = $"{user.UserName}添加合同附件{exarequest.ContentName}的动态{exarequest.ContentType}";

                GatewayInterface.Dto.UserInfo userinfo = new GatewayInterface.Dto.UserInfo()
                {
                    Id = user.Id,
                    KeyWord = user.KeyWord,
                    OrganizationId = user.OrganizationId,
                    OrganizationName = user.OrganizationName,
                    UserName = user.UserName
                };

                var examineInterface = ApplicationContext.Current.Provider.GetRequiredService<IExamineInterface>();
                var reponse = await examineInterface.Submit(userinfo, exarequest);
                if (reponse.Code != ResponseCodeDefines.SuccessCode)
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Message = "向审核中心发起审核请求失败：" + reponse.Message;
                    return response;
                }

                response.Message = "发起审核成功";

                List<NWF> nf = new List<NWF>();
                foreach (var item in fileInfoRequests)
                {
                    nf.Add(CreateNwf(user, source, item));
                }

                string st = JsonHelper.ToJson(nf);
                Logger.Trace($"all lenght {st.Length}");
                await _fileScopeManager.CreateModifyAsync(user, contractId, strModifyGuid, "TEST", JsonHelper.ToJson(fileInfoRequests),
                    st, JsonHelper.ToJson(user), dest, HttpContext.RequestAborted);//添加修改历史

                response.Message = "添加附件修改成功";
            }
            catch (Exception e)
            {

                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"上传文件信息回调(FileCallback)模型验证失败：\r\n{e.ToString()},请求参数为：\r\n" + (fileInfoRequests != null ? JsonHelper.ToJson(fileInfoRequests) : ""));
            }
            

            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="fileInfoCallbackRequestList"></param>
        /// <returns></returns>
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
                await _fileInfoManager.CreateListAsync(userId, fileInfoCallbackRequestList, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"上传文件信息回调(FileCallback)模型验证失败：\r\n{e.ToString()},请求参数为：\r\n" + (fileInfoCallbackRequestList != null ? JsonHelper.ToJson(fileInfoCallbackRequestList) : ""));
            }
            return response;
        }

        /// <summary>
        /// 删除合同文件
        /// </summary>
        /// <param name="user"></param>
        /// <param name="buildingId"></param>
        /// <param name="fileGuids"></param>
        /// <returns></returns>
        [HttpPost("deletecontractfile/{contractId}")]
        [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "DeleteContractFile" })]
        public async Task<ResponseMessage> DeleteContractFiles(UserInfo user, string contractId, [FromBody] List<string> fileGuids)
        {
            ResponseMessage response = new ResponseMessage();
            Logger.Trace($"deletecontractfile {fileGuids.Count}");
            try
            {
                await _fileScopeManager.DeleteContractFileListAsync(user.Id, contractId, fileGuids, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})删除合同附件文件(DeleteBuildingFiles)报错：\r\n{e.ToString()},请求参数为：\r\n(contractId){contractId ?? ""}" + (fileGuids != null ? JsonHelper.ToJson(fileGuids) : ""));
            }
            return response;
        }

        /// <summary>
        /// 获取合同文件
        /// </summary>
        /// <param name="user"></param>
        /// <param name="contractId"></param>
        /// <returns></returns>
        [HttpGet("GetFileListByContractId/{contractId}")]
        [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
        public async Task<ResponseMessage<List<FileItemResponse>>> GetFileListByContractId(UserInfo user, string contractId)
        {
            ResponseMessage<List<FileItemResponse>> response = new ResponseMessage<List<FileItemResponse>>();
            if(contractId == "")
            {
                response.Code = ResponseCodeDefines.ArgumentNullError;
                response.Message = "合同参数为空";
                return response;
            }
            try
            {
                List<FileInfo> fileInfos = new List<FileInfo>();
                List<FileItemResponse> fileItems = new List<FileItemResponse>();
                fileInfos = await _fileScopeManager.FindByContractIdAsync(user.Id, contractId);
                if (fileInfos.Count() > 0)
                {
                    var f = fileInfos.Select(a => a.FileGuid).Distinct();
                    foreach (var item in f)
                    {
                        var f1 = fileInfos.Where(a => a.Type != "Attachment" && a.FileGuid == item).ToList();
                        if (f1?.Count > 0)
                        {
                            fileItems.Add(ConvertToFileItem(item, f1));
                        }
                    }
                }
                response.Extension = fileItems;
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})根据合同Id获取合同信息(GetFileListByContractId)报错：\r\n{e.ToString()}，\r\n请求参数为：\r\n(contractId){contractId ?? ""}");
            }
            return response;
        }

        private FileItemResponse ConvertToFileItem(string fileGuid, List<FileInfo> fl)
        {
            FileItemResponse fi = new FileItemResponse();
            fi.FileGuid = fileGuid;
            fi.Group = fl.FirstOrDefault()?.Group;
            fi.Icon = fl.FirstOrDefault(x => x.Type == "ICON")?.Uri;
            fi.Original = fl.FirstOrDefault(x => x.Type == "ORIGINAL")?.Uri;
            fi.Medium = fl.FirstOrDefault(x => x.Type == "MEDIUM")?.Uri;
            fi.Small = fl.FirstOrDefault(x => x.Type == "SMALL")?.Uri;

            string fr = ApplicationCore.ApplicationContext.Current.FileServerRoot;
            fr = (fr ?? "").TrimEnd('/');
            if (!String.IsNullOrEmpty(fi.Icon))
            {
                fi.Icon = fr + "/" + fi.Icon.TrimStart('/');
            }
            if (!String.IsNullOrEmpty(fi.Original))
            {
                fi.Original = fr + "/" + fi.Original.TrimStart('/');
            }
            if (!String.IsNullOrEmpty(fi.Medium))
            {
                fi.Medium = fr + "/" + fi.Medium.TrimStart('/');
            }
            if (!String.IsNullOrEmpty(fi.Small))
            {
                fi.Small = fr + "/" + fi.Small.TrimStart('/');
            }
            return fi;
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
            header.ExtraAttribute.Add(new AttributeType() { Name = "SubSystem", Value = "contractfiles" });
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

    
    }
}
