using ApplicationCore;
using ApplicationCore.Dto;
using ApplicationCore.Filters;
using AspNet.Security.OAuth.Validation;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XYH.Core.Log;
using XYHShopsPlugin.Dto;
using XYHShopsPlugin.Managers;

namespace XYHShopsPlugin.Controllers
{
    //[Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/files")]
    public class FileInfoController : Controller
    {
        private readonly FileInfoManager _fileInfoManager;
        private readonly FileScopeManager _fileScopeManager;
        private readonly BuildingsManager _buildingsManager;
        private readonly ShopsManager _shopsManager;
        private readonly BuildingNoticeManager _buildingNoticeManager;
        private readonly RestClient _restClient;
        private readonly IMapper _mapper;
        private readonly UpdateRecordManager _updateRecordManager;
        private readonly ILogger Logger = LoggerManager.GetLogger("XYHShopsFileInfo");

        public FileInfoController(FileInfoManager fileInfoManager,
            FileScopeManager fileScopeManager,
            BuildingsManager buildingsManager,
            UpdateRecordManager updateRecordManager,
            BuildingNoticeManager buildingNoticeManager,
            ShopsManager shopsManager,
            RestClient restClient,
            IMapper mapper)
        {
            _fileInfoManager = fileInfoManager;
            _fileScopeManager = fileScopeManager;
            _buildingsManager = buildingsManager;
            _updateRecordManager = updateRecordManager;
            _buildingNoticeManager = buildingNoticeManager;
            _shopsManager = shopsManager;
            _restClient = restClient;
            _mapper = mapper;
        }

        /// <summary>
        /// 上传文件信息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="fileInfoRequest"></param>
        /// <param name="source"></param>
        /// <param name="dest"></param>
        /// <param name="buildingId"></param>
        /// <returns></returns>
        // dest --> shops or building
        [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
        [HttpPost("{dest}/upload/{buildingId}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "FileUpload" })]
        public async Task<ResponseMessage> UploadFile(UserInfo user, [FromBody]FileInfoRequest fileInfoRequest, [FromQuery]string source, [FromRoute]string dest, [FromRoute]string buildingId)
        {
            ResponseMessage response = new ResponseMessage();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ArgumentNullError;
                response.Message = ModelState.GetAllErrors();
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})上传文件信息(UploadFile)模型验证失败：\r\n{response.Message ?? ""},请求参数为：\r\n(source){source},(dest){dest},(buildingId){buildingId}," + (fileInfoRequest != null ? JsonHelper.ToJson(fileInfoRequest) : ""));
                return response;
            }
            try
            {
                await _fileScopeManager.CreateAsync(user, dest, buildingId, fileInfoRequest, HttpContext.RequestAborted);

                NameValueCollection nameValueCollection = new NameValueCollection();
                nameValueCollection.Add("appToken", "app:nwf");
                var nwf = CreateNwf(user, source, fileInfoRequest);
                Logger.Info("nwf协议：\r\n{0}", JsonHelper.ToJson(nwf));
                string response2 = await _restClient.Post(ApplicationContext.Current.NWFUrl, nwf, "POST", nameValueCollection);
                Logger.Info("返回：\r\n{0}", response2);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})上传文件信息(UploadFile)报错：\r\n{e.ToString()},请求参数为：\r\n(source){source ?? ""},(dest){dest ?? ""},(buildingId){buildingId ?? ""}," + (fileInfoRequest != null ? JsonHelper.ToJson(fileInfoRequest) : ""));
            }
            return response;
        }

        /// <summary>
        /// 批量上传文件
        /// </summary>
        /// <param name="user"></param>
        /// <param name="fileInfoRequests"></param>
        /// <param name="source"></param>
        /// <param name="dest"></param>
        /// <param name="buildingId"></param>
        /// <returns></returns>
        // dest --> shops or building
        [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
        [HttpPost("{dest}/uploadmore/{buildingId}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "FileUpload" })]
        public async Task<ResponseMessage> UploadFiles(UserInfo user, [FromBody]List<FileInfoRequest> fileInfoRequests, [FromQuery]string source, [FromRoute]string dest, [FromRoute]string buildingId)
        {
            ResponseMessage response = new ResponseMessage();
            if (fileInfoRequests == null || fileInfoRequests.Count == 0)
            {
                response.Code = ResponseCodeDefines.ArgumentNullError;
                response.Message = "请求参数错误";
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})批量上传文件信息(UploadFiles)模型验证失败：\r\n{response.Message ?? ""},请求参数为：\r\n(source){source ?? ""},(dest){dest ?? ""},(buildingId){buildingId ?? ""}," + (fileInfoRequests != null ? JsonHelper.ToJson(fileInfoRequests) : ""));
                return response;
            }
            foreach (var item in fileInfoRequests)
            {
                try
                {
                    await _fileScopeManager.CreateAsync(user, dest, buildingId, item, HttpContext.RequestAborted);

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
                    Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})批量上传文件信息(UploadFiles)报错：\r\n{e.ToString()},请求参数为：\r\n(source){source ?? ""},(dest){dest ?? ""},(buildingId){buildingId ?? ""}," + (fileInfoRequests != null ? JsonHelper.ToJson(fileInfoRequests) : ""));
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
                var fileinfo = fileInfoCallbackRequestList.Where(a => a.Type == "ICON").FirstOrDefault();
                if (fileinfo != null)
                {
                    var buildingfilescope = await _fileScopeManager.FindByBuildingFileGuidAsync(userId, fileinfo.FileGuid);
                    if (buildingfilescope != null)
                    {
                        var building = await _buildingsManager.FindByIdAsync(userId, buildingfilescope.BuildingId);
                        if (string.IsNullOrEmpty(building.Icon))
                        {
                            building.Icon = fileinfo.FilePath;
                            await _buildingsManager.UpdateAsync(userId, buildingfilescope.BuildingId, _mapper.Map<BuildingRequest>(building));
                        }
                    }
                    var shopfilescope = await _fileScopeManager.FindByShopsFileGuidAsync(userId, fileinfo.FileGuid);
                    if (shopfilescope != null)
                    {
                        var shops = await _shopsManager.FindByIdAsync(userId, shopfilescope.ShopsId);
                        if (string.IsNullOrEmpty(shops.Icon))
                        {
                            shops.Icon = fileinfo.FilePath;
                            await _shopsManager.UpdateAsync(userId, shopfilescope.ShopsId, _mapper.Map<ShopsRequest>(shops));
                        }
                    }
                }
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
        /// 删除商铺文件
        /// </summary>
        /// <param name="user"></param>
        /// <param name="shopsId"></param>
        /// <param name="fileGuids"></param>
        /// <returns></returns>
        [HttpPost("deleteshopsfile/{shopsId}")]
        [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "DeleteShopsFile" })]
        public async Task<ResponseMessage> DeleteShopsFiles(UserInfo user, string shopsId, [FromBody] List<string> fileGuids)
        {
            ResponseMessage response = new ResponseMessage();
            try
            {
                await _fileScopeManager.DeleteShopsFileListAsync(user.Id, shopsId, fileGuids, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})删除商铺文件(DeleteShopsFiles)报错：\r\n{e.ToString()},请求参数为：\r\n(shopsId){shopsId ?? ""}," + (fileGuids != null ? JsonHelper.ToJson(fileGuids) : ""));
            }
            return response;
        }
        /// <summary>
        /// 删除楼盘文件
        /// </summary>
        /// <param name="user"></param>
        /// <param name="buildingId"></param>
        /// <param name="fileGuids"></param>
        /// <returns></returns>
        [HttpPost("deletebuildingfile/{buildingId}")]
        [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "DeleteBuildingFile" })]
        public async Task<ResponseMessage> DeleteBuildingFiles(UserInfo user, string buildingId, [FromBody] List<string> fileGuids)
        {
            ResponseMessage response = new ResponseMessage();
            try
            {
                await _fileScopeManager.DeleteBuildingFileListAsync(user.Id, buildingId, fileGuids, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})删除楼盘文件(DeleteBuildingFiles)报错：\r\n{e.ToString()},请求参数为：\r\n(buildingId){buildingId ?? ""}" + (fileGuids != null ? JsonHelper.ToJson(fileGuids) : ""));
            }
            return response;
        }

        /// <summary>
        /// 更新缩略图
        /// </summary>
        /// <param name="user"></param>
        /// <param name="iconPath"></param>
        /// <param name="dest"></param>
        /// <param name="buildingId"></param>
        /// <returns></returns>
        [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
        [HttpPut("{dest}/iconupdate/{buildingId}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "BuildingUpdate,ShopsUpdate" })]
        public async Task<ResponseMessage> UploadIcon(UserInfo user, [FromBody]string iconPath, [FromRoute]string dest, [FromRoute]string buildingId)
        {
            ResponseMessage response = new ResponseMessage();
            if (string.IsNullOrEmpty(iconPath) || string.IsNullOrEmpty(dest) || string.IsNullOrEmpty(buildingId))
            {
                response.Code = ResponseCodeDefines.ArgumentNullError;
                response.Message = "参数不能为空";
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})更新缩略图(UploadIcon)参数验证错误：参数不能为空,请求参数为：\r\n(iconPath){iconPath ?? ""},(dest){dest ?? ""},(buildingId){buildingId ?? ""}");
                return response;
            }
            try
            {
                if (dest == "building")
                {
                    var building = await _buildingsManager.FindByIdAsync(user.Id, buildingId);
                    if (building != null)
                    {
                        building.Icon = iconPath;
                        await _buildingsManager.UpdateAsync(user.Id, buildingId, _mapper.Map<BuildingRequest>(building));
                    }
                }
                else if (dest == "shops")
                {
                    var shops = await _shopsManager.FindByIdAsync(user.Id, buildingId);
                    if (shops != null)
                    {
                        shops.Icon = iconPath;
                        await _shopsManager.UpdateAsync(user.Id, buildingId, _mapper.Map<ShopsRequest>(shops));
                    }
                }
                else if (dest == "updaterecord")
                {
                    var records = await _updateRecordManager.FindByIdAsync(buildingId);
                    if (records != null)
                    {
                        records.Icon = iconPath;
                        await _shopsManager.UpdateAsync(user.Id, buildingId, _mapper.Map<ShopsRequest>(records));
                    }
                }
                else if (dest == "buildingnotice")
                {
                    var notice = await _buildingNoticeManager.FindByIdAsync(buildingId);
                    if (notice != null)
                    {
                        notice.Icon = iconPath;
                        await _buildingNoticeManager.UpdateAsync(buildingId, _mapper.Map<BuildingNoticeRequest>(notice));
                    }
                }
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})更新缩略图(UploadIcon)报错：\r\n{e.ToString()},请求参数为：\r\n(iconPath){iconPath ?? ""},(dest){dest ?? ""},(buildingId){buildingId ?? ""}");
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
