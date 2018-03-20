using ApplicationCore;
using ApplicationCore.Dto;
using ApplicationCore.Filters;
using AspNet.Security.OAuth.Validation;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
using XYHShopsPlugin.Managers;

namespace XYHContractPlugin.Controllers
{
    [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/contractfiles")]
    class ContractFileController : Controller
    {
        private readonly FileInfoManager _fileInfoManager;
        private readonly FileScopeManager _fileScopeManager;
        private readonly ContractInfoManager _contractInfoManager;
        private readonly RestClient _restClient;
        private readonly IMapper _mapper;
        private readonly ILogger Logger = LoggerManager.GetLogger("XYHContractFileInfo");
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
            var building = await _contractInfoManager.FindByIdAsync(contractId, HttpContext.RequestAborted);
            if (building == null)
            {
                response.Code = ResponseCodeDefines.NotFound;
                response.Message = "合同不存在：" + contractId;
                Logger.Error($"合同文件上传失败：合同不存在，\r\n请求参数为：\r\n" + (fileInfoRequests != null ? JsonHelper.ToJson(fileInfoRequests) : ""));
                return response;
            }
            foreach (var item in fileInfoRequests)
            {
                try
                {
                    await _fileScopeManager.CreateAsync(user, dest, contractId, item, HttpContext.RequestAborted);

                    NameValueCollection nameValueCollection = new NameValueCollection();
                    nameValueCollection.Add("appToken", "app:nwf");
                    var nwf = CreateNwf(user, source, item);
                    Logger.Info("nwf协议：\r\n{0}", JsonHelper.ToJson(nwf));
                    string response2 = await _restClient.Post(ApplicationContext.Current.NWFUrl, nwf, "POST", nameValueCollection);
                    Logger.Info("返回：\r\n{0}", response2);
                }
                catch (Exception e)
                {
                    response.Code = ResponseCodeDefines.PartialFailure;
                    response.Message += $"文件：{item.FileGuid}处理出错，错误信息：{e.ToString()}。\r\n";
                    Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})批量上传文件信息(UploadFiles)报错：\r\n{e.ToString()},请求参数为：\r\n(source){source ?? ""},(dest){dest ?? ""},(contractId){contractId ?? ""}," + (fileInfoRequests != null ? JsonHelper.ToJson(fileInfoRequests) : ""));
                }
            }
            return response;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="fileInfoCallbackRequestList"></param>
        /// <returns></returns>
        [HttpPost("callback")]
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
            try
            {
                await _fileScopeManager.DeleteContractFileListAsync(user.Id, contractId, fileGuids, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})删除楼盘文件(DeleteBuildingFiles)报错：\r\n{e.ToString()},请求参数为：\r\n(contractId){contractId ?? ""}" + (fileGuids != null ? JsonHelper.ToJson(fileGuids) : ""));
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
            header.ExtraAttribute.Add(new AttributeType() { Name = "SubSystem", Value = "contract" });
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
