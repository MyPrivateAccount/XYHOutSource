using ApplicationCore;
using ApplicationCore.Dto;
using ApplicationCore.Filters;
using ApplicationCore.Managers;
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
using XYHCustomerPlugin.Dto;
using XYHCustomerPlugin.Dto.Common;
using XYHCustomerPlugin.Dto.Request;
using XYHCustomerPlugin.Dto.Response;
using XYHCustomerPlugin.Managers;

namespace XYHCustomerPlugin.Controllers
{
    //[Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/dealfiles")]
    public class FileInfoController : Controller
    {
        private readonly PermissionExpansionManager _permissionExpansionManager;
        private readonly FileInfoManager _fileInfoManager;
        private readonly CustomerInfoManager _icustomerInfoManager;
        private readonly FileScopeManager _fileScopeManager;
        private readonly CustomerDealManager _customerDealManager;
        private readonly RestClient _restClient;
        private readonly IMapper _mapper;
        private readonly ILogger Logger = LoggerManager.GetLogger("DealFileInfoController");

        public FileInfoController(FileInfoManager fileInfoManager,
            CustomerInfoManager customerInfoManager,
            FileScopeManager fileScopeManager,
            CustomerDealManager customerDealManager,
            RestClient restClient,
            IMapper mapper,
             PermissionExpansionManager permissionExpansionManager)
        {
            _permissionExpansionManager = permissionExpansionManager ?? throw new ArgumentNullException(nameof(permissionExpansionManager));
            _fileInfoManager = fileInfoManager;
            _icustomerInfoManager = customerInfoManager;
            _fileScopeManager = fileScopeManager;
            _customerDealManager = customerDealManager;
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
        /// <param name="Id"></param>
        /// <returns></returns>
        // dest --> shops or deal
        [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
        [HttpPost("{dest}/upload/{Id}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "FileUpload" })]
        public async Task<ResponseMessage> UploadFile(UserInfo user, [FromBody]DealFileInfoRequest fileInfoRequest, [FromQuery]string source, [FromRoute]string dest, [FromRoute]string Id)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})上传文件信息(UploadFile)：请求参数为：\r\n(source){source},(dest){dest},(Id){Id}," + (fileInfoRequest != null ? JsonHelper.ToJson(fileInfoRequest) : ""));

            ResponseMessage response = new ResponseMessage();
            if (fileInfoRequest == null)
            {
                response.Code = ResponseCodeDefines.ArgumentNullError;
                response.Message = ModelState.GetAllErrors();
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})上传文件信息(UploadFile)模型验证失败：\r\n{response.Message ?? ""},请求参数为：\r\n(source){source},(dest){dest},(Id){Id}," + (fileInfoRequest != null ? JsonHelper.ToJson(fileInfoRequest) : ""));
                return response;
            }
            try
            {
                //   await _fileScopeManager.CreateAsync(userId, fileInfoRequest, HttpContext.RequestAborted);
                await _fileScopeManager.CreateAsync(user, dest, Id, fileInfoRequest, HttpContext.RequestAborted);

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
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})上传文件信息(UploadFile)报错：\r\n{e.ToString()},请求参数为：\r\n(source){source ?? ""},(dest){dest ?? ""},(Id){Id ?? ""}," + (fileInfoRequest != null ? JsonHelper.ToJson(fileInfoRequest) : ""));
            }
            return response;
        }

        /// <summary>
        /// 上传文件信息回调
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="fileInfoCallbackRequestList"></param>
        /// <returns></returns>
        [HttpPost("~/api/xyhcustomer/files/callback")]
        //[TypeFilter(typeof(CheckPermission), Arguments = new object[] { "FileCallBack" })]
        public async Task<ResponseMessage> FileCallback([FromBody] List<DealFileInfoCallbackRequest> fileInfoCallbackRequestList)
        {
            Logger.Trace($"上传文件信息回调(FileCallback)：请求参数为：\r\n" + (fileInfoCallbackRequestList != null ? JsonHelper.ToJson(fileInfoCallbackRequestList) : ""));

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
                    var customerfilescope = await _fileScopeManager.FindByCustomerFileGuidAsync(fileinfo.FileGuid);
                    //if (customerfilescope != null)
                    //{
                    //    var customer = await _icustomerInfoManager.FindByIdAsync(userId, customerfilescope.CustomerId, HttpContext.RequestAborted);
                    //    if (string.IsNullOrEmpty(customer.HeadImg))
                    //    {
                    //        customer.HeadImg = fileinfo.FilePath;
                    //        await _icustomerInfoManager.UpdateAsync(userId, _mapper.Map<CustomerInfoCreateRequest>(customer), HttpContext.RequestAborted);
                    //    }
                    //}
                }
                await _fileInfoManager.CreateListAsync("", fileInfoCallbackRequestList, HttpContext.RequestAborted);
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
        /// 批量上传文件
        /// </summary>
        /// <param name="User"></param>
        /// <param name="fileInfoRequests"></param>
        /// <param name="source"></param>
        /// <param name="dest"></param>
        /// <param name="Id"></param>
        /// <returns></returns>
        // dest --> shops or deal
        [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
        [HttpPost("{dest}/uploadmore/{Id}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "FileUpload" })]
        public async Task<ResponseMessage> UploadFiles(UserInfo User, [FromBody]List<DealFileInfoRequest> fileInfoRequests, [FromQuery]string source, [FromRoute]string dest, [FromRoute]string Id)
        {
            Logger.Trace($"用户{User?.UserName ?? ""}({User?.Id ?? ""})批量上传文件信息(UploadFiles)：请求参数为：\r\n(source){source ?? ""},(dest){dest ?? ""},(Id){Id ?? ""}," + (fileInfoRequests != null ? JsonHelper.ToJson(fileInfoRequests) : ""));

            ResponseMessage response = new ResponseMessage();
            if (fileInfoRequests == null || fileInfoRequests.Count == 0)
            {
                response.Code = ResponseCodeDefines.ArgumentNullError;
                response.Message = "请求参数错误";
                Logger.Warn($"用户{User?.UserName ?? ""}({User?.Id ?? ""})批量上传文件信息(UploadFiles)模型验证失败：\r\n{response.Message ?? ""},请求参数为：\r\n(source){source ?? ""},(dest){dest ?? ""},(Id){Id ?? ""}," + (fileInfoRequests != null ? JsonHelper.ToJson(fileInfoRequests) : ""));
                return response;
            }

            foreach (var item in fileInfoRequests)
            {
                try
                {
                    //   await _fileScopeManager.CreateAsync(userId, fileInfoRequest, HttpContext.RequestAborted);
                    await _fileScopeManager.CreateAsync(User, dest, Id, item, HttpContext.RequestAborted);

                    NameValueCollection nameValueCollection = new NameValueCollection();
                    nameValueCollection.Add("appToken", "app:nwf");
                    var nwf = CreateNwf(User, source, item);
                    Logger.Info("nwf协议：\r\n{0}", JsonHelper.ToJson(nwf));
                    string response2 = await _restClient.Post(ApplicationContext.Current.NWFUrl, nwf, "POST", nameValueCollection);
                    Logger.Info("返回：\r\n{0}", response2);
                }
                catch (Exception e)
                {
                    response.Code = ResponseCodeDefines.PartialFailure;
                    response.Message += $"文件：{item.FileGuid}处理出错，错误信息：{e.ToString()}。\r\n";
                    Logger.Error($"用户{User?.UserName ?? ""}({User?.Id ?? ""})批量上传文件信息(UploadFiles)报错：\r\n{e.ToString()},请求参数为：\r\n(source){source ?? ""},(dest){dest ?? ""},(Id){Id ?? ""}," + (fileInfoRequests != null ? JsonHelper.ToJson(fileInfoRequests) : ""));
                }
            }
            return response;
        }


        [HttpPost("deletedealfile/{dealId}")]
        [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "DeleteDealFile" })]
        public async Task<ResponseMessage> DeleteDealFiles(UserInfo user, string dealId, [FromBody] List<string> fileGuids)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})删除商铺文件(DeleteShopsFiles)：请求参数为：\r\n(dealId){dealId ?? ""}," + (fileGuids != null ? JsonHelper.ToJson(fileGuids) : ""));

            ResponseMessage response = new ResponseMessage();
            try
            {
                await _fileScopeManager.DeleteDealFileListAsync(user.Id, dealId, fileGuids, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})删除商铺文件(DeleteShopsFiles)报错：\r\n{e.ToString()},请求参数为：\r\n(dealId){dealId ?? ""}," + (fileGuids != null ? JsonHelper.ToJson(fileGuids) : ""));
            }
            return response;
        }

        [HttpPost("deletecustomerfile/{customerId}")]
        [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "DeleteCustomerFile" })]
        public async Task<ResponseMessage> DeleteBuildingFiles(UserInfo user, string customerId, [FromBody] List<string> fileGuids)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})删除楼盘文件(DeleteBuildingFiles)：请求参数为：\r\n(customerId){customerId ?? ""}" + (fileGuids != null ? JsonHelper.ToJson(fileGuids) : ""));

            ResponseMessage response = new ResponseMessage();
            try
            {
                await _fileScopeManager.DeleteCustomerFileListAsync(user.Id, customerId, fileGuids, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})删除楼盘文件(DeleteBuildingFiles)报错：\r\n{e.ToString()},请求参数为：\r\n(customerId){customerId ?? ""}" + (fileGuids != null ? JsonHelper.ToJson(fileGuids) : ""));
            }
            return response;
        }


        [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
        [HttpPut("{dest}/iconupdate/{customerId}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "CustomerUpdate,ShopsUpdate" })]
        public async Task<ResponseMessage> UploadIcon(UserInfo User, [FromBody]string iconPath, [FromRoute]string dest, [FromRoute]string Id)
        {
            Logger.Trace($"用户{User?.UserName ?? ""}({User?.Id ?? ""})更新缩略图(UploadIcon)：请求参数为：\r\n(iconPath){iconPath ?? ""},(dest){dest ?? ""},(Id){Id ?? ""}");

            ResponseMessage response = new ResponseMessage();
            if (string.IsNullOrEmpty(iconPath) || string.IsNullOrEmpty(dest) || string.IsNullOrEmpty(Id))
            {
                response.Code = ResponseCodeDefines.ArgumentNullError;
                response.Message = "参数不能为空";
                Logger.Error($"用户{User?.UserName ?? ""}({User?.Id ?? ""})更新缩略图(UploadIcon)参数验证错误：参数不能为空,请求参数为：\r\n(iconPath){iconPath ?? ""},(dest){dest ?? ""},(Id){Id ?? ""}");
                return response;
            }
            try
            {
                if (dest == "customer")
                {
                    var customer = await _icustomerInfoManager.FindByIdAsync(User.Id, Id, HttpContext.RequestAborted);
                    if (customer != null)
                    {
                        customer.HeadImg = iconPath;
                        await _icustomerInfoManager.UpdateAsync(User.Id, _mapper.Map<CustomerInfoCreateRequest>(customer), HttpContext.RequestAborted);
                    }
                }
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{User?.UserName ?? ""}({User?.Id ?? ""})更新缩略图(UploadIcon)报错：\r\n{e.ToString()},请求参数为：\r\n(iconPath){iconPath ?? ""},(dest){dest ?? ""},(Id){Id ?? ""}");
            }

            return response;
        }

        private NWF CreateNwf(UserInfo user, string source, DealFileInfoRequest fileInfoRequest)
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
            header.ExtraAttribute.Add(new AttributeType() { Name = "SubSystem", Value = "xyhcustomer" });

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
