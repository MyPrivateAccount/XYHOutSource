using ApplicationCore;
using ApplicationCore.Dto;
using ApplicationCore.Filters;
using ApplicationCore.Managers;
using ApplicationCore.Models;
using AspNet.Security.OAuth.Validation;
using AutoMapper;
using GatewayInterface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Text;
using System.Threading.Tasks;
using XYH.Core.Log;
using XYHShopsPlugin.Dto;
using XYHShopsPlugin.Dto.Response;
using XYHShopsPlugin.Managers;


namespace XYHShopsPlugin.Controllers
{
    [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/buildings")]
    public class BuildingsController : Controller
    {
        private readonly BuildingsManager _buildingsManager;
        private readonly IMapper _mapper;
        private readonly RestClient _restClient;
        private readonly PermissionExpansionManager _permissionExpansionManager;
        private readonly ILogger Logger = LoggerManager.GetLogger("Buildings");
        private readonly ILogger MessageLogger = LoggerManager.GetLogger("SendMessageServerError");

        public BuildingsController(BuildingsManager buildingsManager,
            RestClient restClient,
            PermissionExpansionManager permissionExpansionManager,
            IMapper mapper)
        {
            _buildingsManager = buildingsManager;
            _restClient = restClient;
            _mapper = mapper;
            _permissionExpansionManager = permissionExpansionManager;
        }

        /// <summary>
        /// 获取负责楼盘
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpGet]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<List<BuildingResponse>>> GetResponsibleBuildingList(UserInfo user)
        {
            ResponseMessage<List<BuildingResponse>> response = new ResponseMessage<List<BuildingResponse>>();
            BuildingSearchCondition condition = new BuildingSearchCondition()
            {
                ResidentUser = user.Id
            };
            try
            {
                response.Extension = await _buildingsManager.SimpleSearchOld(user, condition, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})获取负责楼盘(GetResponsibleBuildingList)报错：\r\n{e.ToString()}");
            }
            return response;
        }

        /// <summary>
        /// 获取负责楼盘管理驻场用户
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpGet("buildingsite")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<List<BuildingSiteResponse>>> GetBuildingSiteList(UserInfo user)
        {
            ResponseMessage<List<BuildingSiteResponse>> response = new ResponseMessage<List<BuildingSiteResponse>>();
            try
            {
                response.Extension = await _buildingsManager.SearchBulidingSite(user.Id, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})获取负责楼盘管理驻场用户(GetBuildingSiteList)报错：\r\n{e.ToString()}");
            }
            return response;
        }

        /// <summary>
        /// 获取负责驻场用户列表
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        [HttpGet("siteuserlist")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<List<UserResponse>>> GetSiteUserList(UserInfo user)
        {
            ResponseMessage<List<UserResponse>> response = new ResponseMessage<List<UserResponse>>();
            try
            {
                if (await _permissionExpansionManager.HavePermission(user.Id, "APPOINT_SCENE"))
                {
                    response.Extension = await _buildingsManager.InSiteList(user.Id, HttpContext.RequestAborted);
                }
                else
                {
                    response.Code = ResponseCodeDefines.NotAllow;
                    response.Message = "权限不足";
                    Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})获取负责驻场用户列表(GetSiteUserList)失败：没有权限");
                }
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})获取负责驻场用户列表(GetSiteUserList)报错：\r\n{e.ToString()}");
            }
            return response;
        }

        /// <summary>
        /// 获取楼盘列表
        /// </summary>
        /// <param name="user"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpPost("list")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "BuildingRetrieve" })]
        public async Task<PagingResponseMessage<BuildingSearchResponse>> GetBuildingList(UserInfo user, [FromBody]BuildingListSearchCondition condition)
        {
            PagingResponseMessage<BuildingSearchResponse> pagingResponse = new PagingResponseMessage<BuildingSearchResponse>();
            if (!ModelState.IsValid)
            {
                pagingResponse.Code = ResponseCodeDefines.ModelStateInvalid;
                pagingResponse.Message = ModelState.GetAllErrors();
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})获取楼盘列表(GetBuildingList)模型验证失败：\r\n{pagingResponse.Message ?? ""}，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
                return pagingResponse;
            }
            try
            {
                return await _buildingsManager.Search(user.Id, condition, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                pagingResponse.Code = ResponseCodeDefines.ServiceError;
                pagingResponse.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})获取楼盘列表(GetBuildingList)报错：{e.ToString()}，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
            }
            return pagingResponse;
        }

        /// <summary>
        /// 获取负责楼盘
        /// </summary>
        /// <param name="user"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpPost("[action]")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<List<BuildingResponse>>> PostResponsibleBuildingSreach(UserInfo user, [FromBody]BuildingSearchCondition condition)
        {
            ResponseMessage<List<BuildingResponse>> response = new ResponseMessage<List<BuildingResponse>>();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})获取负责楼盘(PostResponsibleBuildingSreach)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
                return response;
            }
            try
            {
                condition.ResidentUser = user.Id;
                response.Extension = await _buildingsManager.SimpleSearch(user, condition, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})获取负责楼盘(PostResponsibleBuildingSreach)模型验证失败：\r\n{e.ToString()}，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
            }
            return response;
        }

        /// <summary>
        /// 是否是驻场
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="buildingId"></param>
        /// <returns></returns>
        [HttpPost("IsResident/{buildingId}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<bool>> IsResidentUser(string userId, [FromRoute]string buildingId)
        {
            ResponseMessage<bool> response = new ResponseMessage<bool>();
            try
            {
                response.Extension = await _buildingsManager.IsResidentUser(userId, buildingId);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{userId ?? ""}是否是驻场(IsResidentUser)报错：\r\n{e.ToString()}，\r\n请求参数为：\r\n(buildingId){buildingId ?? ""}");
            }
            return response;
        }

        /// <summary>
        /// 是否为驻场经理
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="buildingId"></param>
        /// <returns></returns>
        [HttpPost("IsManagerSite/{buildingId}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<bool>> IsManagerSiteUser(string userId, string buildingId)
        {
            ResponseMessage<bool> response = new ResponseMessage<bool>();
            try
            {
                response.Extension = await _buildingsManager.IsManagerSiteUserAsync(userId, buildingId);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{userId ?? ""}是否是驻场经理(IsManagerSiteUser)报错：\r\n{e.ToString()}，\r\n请求参数为：\r\n(buildingId){buildingId ?? ""}");
            }
            return response;
        }

        /// <summary>
        /// 根据楼盘Id获取楼盘信息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="buildingId"></param>
        /// <returns></returns>
        [HttpGet("{buildingId}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "BuildingRetrieve" })]
        public async Task<ResponseMessage<BuildingResponse>> GetBuilding(UserInfo user, [FromRoute] string buildingId)
        {
            ResponseMessage<BuildingResponse> response = new ResponseMessage<BuildingResponse>();
            try
            {
                response.Extension = await _buildingsManager.FindByIdAsync(user.Id, buildingId, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})根据楼盘Id获取楼盘信息(GetBuilding)报错：\r\n{e.ToString()}，\r\n请求参数为：\r\n(buildingId){buildingId ?? ""}");
            }
            return response;
        }

        /// <summary>
        /// 保存楼盘信息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="buildingId"></param>
        /// <param name="buildingRequest"></param>
        /// <returns></returns>
        [HttpPut("{buildingId}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "BuildingUpdate" })]
        public async Task<ResponseMessage> PutBuildings(UserInfo user, [FromRoute] string buildingId, [FromBody] BuildingRequest buildingRequest)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})保存楼盘信息(PutBuildings)：\r\n请求参数为：\r\n" + buildingRequest != null ? JsonHelper.ToJson(buildingRequest) : "");

            ResponseMessage response = new ResponseMessage();
            if (!ModelState.IsValid || buildingRequest.Id != buildingId)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})保存楼盘信息(PutBuildings)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + buildingRequest != null ? JsonHelper.ToJson(buildingRequest) : "");
                return response;
            }
            try
            {
                var dictionaryGroup = await _buildingsManager.FindByIdAsync(user.Id, buildingId, HttpContext.RequestAborted);
                if (dictionaryGroup == null)
                {
                    await _buildingsManager.CreateAsync(user.Id, buildingRequest, HttpContext.RequestAborted);
                }
                else
                {
                    await _buildingsManager.UpdateAsync(user.Id, buildingId, buildingRequest, HttpContext.RequestAborted);
                }
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})保存楼盘信息(PutBuildings)报错：\r\n{e.ToString()}，\r\n请求参数为：\r\n" + buildingRequest != null ? JsonHelper.ToJson(buildingRequest) : "");
            }
            return response;
        }

        /// <summary>
        /// 指派驻场
        /// </summary>
        /// <param name="user"></param>
        /// <param name="buildingRequest"></param>
        /// <returns></returns>
        [HttpPut("saveonsite")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "BuildingOnSiteUpdate" })]
        public async Task<ResponseMessage> PutBuildingsOnSite(UserInfo user, [FromBody] BuildingsOnSiteRequest buildingsOnSiteRequest)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})指派驻场(PutBuildingsOnSite)：\r\n请求参数为：\r\n" + (buildingsOnSiteRequest != null ? JsonHelper.ToJson(buildingsOnSiteRequest) : ""));

            ResponseMessage response = new ResponseMessage();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})指派驻场(PutBuildingsOnSite)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (buildingsOnSiteRequest != null ? JsonHelper.ToJson(buildingsOnSiteRequest) : ""));
                return response;
            }
            try
            {
                if (!await _permissionExpansionManager.HavePermission(user.Id, "APPOINT_SCENE"))
                {
                    response.Code = ResponseCodeDefines.NotAllow;
                    response.Message = "权限不足";
                    Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})指派驻场(PutBuildingsOnSite)失败：没有权限");
                    return response;
                }
                else
                {
                    GatewayInterface.Dto.ExamineSubmitRequest examineSubmitRequest = new GatewayInterface.Dto.ExamineSubmitRequest();
                    examineSubmitRequest.ContentId = buildingsOnSiteRequest.Id;
                    examineSubmitRequest.ContentType = "BuildingsOnSite";
                    examineSubmitRequest.ContentName = buildingsOnSiteRequest.Name;
                    examineSubmitRequest.Content = JsonHelper.ToJson(buildingsOnSiteRequest);
                    examineSubmitRequest.Source = "";
                    examineSubmitRequest.CallbackUrl = "通过http回调时再设置回调地址";
                    examineSubmitRequest.Action = "BuildingsOnSite";
                    examineSubmitRequest.TaskName = $"指派驻场:{buildingsOnSiteRequest.Name}";
                    examineSubmitRequest.Desc = $"指派驻场{buildingsOnSiteRequest.ResidentUserName1},{buildingsOnSiteRequest.ResidentUserName2},{buildingsOnSiteRequest.ResidentUserName3}";
                    examineSubmitRequest.Ext1 = buildingsOnSiteRequest.ResidentUserName1;
                    examineSubmitRequest.Ext2 = buildingsOnSiteRequest.ResidentUserName2;
                    examineSubmitRequest.Ext3 = buildingsOnSiteRequest.ResidentUserName3;
                    examineSubmitRequest.Ext4 = buildingsOnSiteRequest.ResidentUserName4;

                    GatewayInterface.Dto.UserInfo userInfo = new GatewayInterface.Dto.UserInfo()
                    {
                        Id = user.Id,
                        KeyWord = user.KeyWord,
                        OrganizationId = user.OrganizationId,
                        OrganizationName = user.OrganizationName,
                        UserName = user.UserName
                    };

                    var _examineInterface = ApplicationContext.Current.Provider.GetRequiredService<IExamineInterface>();
                    var response2 = await _examineInterface.Submit(userInfo, examineSubmitRequest);
                    if (response2.Code != ResponseCodeDefines.SuccessCode)
                    {
                        response.Code = ResponseCodeDefines.ServiceError;
                        response.Message = "向审核中心发起审核请求失败：" + response2.Message;
                        return response;
                    }

                    //await _buildingsManager.SaveResidentUserAsync(user.Id, buildingsOnSiteRequest, HttpContext.RequestAborted);
                }
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})指派驻场(PutBuildingsOnSite)报错：\r\n{e.ToString()}，\r\n请求参数为：\r\n" + (buildingsOnSiteRequest != null ? JsonHelper.ToJson(buildingsOnSiteRequest) : ""));
            }
            return response;
        }


        /// <summary>
        /// 指派驻场回调(内部使用)
        /// </summary>
        /// <param name="examineResponse"></param>
        /// <returns></returns>
        [HttpPost("onsitecallback")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage> BuildingsOnSiteCallback([FromBody] ExamineResponse examineResponse)
        {
            Logger.Trace($"指派驻场回调(BuildingsOnSiteCallback)：\r\n请求参数为：\r\n" + (examineResponse != null ? JsonHelper.ToJson(examineResponse) : ""));

            ResponseMessage response = new ResponseMessage();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Warn("模型验证失败：\r\n{0}", response.Message ?? "");
                return response;
            }
            if (examineResponse.ContentType != "BuildingsOnSite")
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = "该回调实现不支持BuildingsOnSite的内容类型";
                Logger.Warn("指派驻场业务层回调失败：\r\n{0}", response.Message ?? "");
                return response;
            }
            try
            {
                var onsite = JsonHelper.ToObject<BuildingsOnSiteRequest>(examineResponse.Content);
                if (examineResponse.ExamineStatus == ExamineStatus.Examined)
                {
                    await _buildingsManager.SaveResidentUserAsync("", onsite);

                    //发送通知消息
                    SendMessageRequest sendMessageRequest = new SendMessageRequest();
                    sendMessageRequest.MessageTypeCode = "BuildingsOnSite";
                    MessageItem messageItem = new MessageItem();
                    messageItem.UserIds = new List<string>();
                    if (!string.IsNullOrEmpty(onsite.ResidentUser1))
                    {
                        messageItem.UserIds.Add(onsite.ResidentUser1);
                    }
                    if (!string.IsNullOrEmpty(onsite.ResidentUser2))
                    {
                        messageItem.UserIds.Add(onsite.ResidentUser2);
                    }
                    if (!string.IsNullOrEmpty(onsite.ResidentUser3))
                    {
                        messageItem.UserIds.Add(onsite.ResidentUser3);
                    }
                    if (!string.IsNullOrEmpty(onsite.ResidentUser4))
                    {
                        messageItem.UserIds.Add(onsite.ResidentUser4);
                    }
                    messageItem.MessageTypeItems = new List<TypeItem> {
                    new TypeItem { Key="NAME",Value=onsite.Name},
                    new TypeItem{ Key="TIME",Value=DateTime.Now.ToString("MM-dd hh:mm")}
                    };
                    sendMessageRequest.MessageList = new List<MessageItem> { messageItem };
                    try
                    {
                        MessageLogger.Info("发送通知消息协议：\r\n{0}", JsonHelper.ToJson(sendMessageRequest));
                        _restClient.Post(ApplicationContext.Current.MessageServerUrl, sendMessageRequest, "POST", new NameValueCollection());
                    }
                    catch (Exception e)
                    {
                        MessageLogger.Error("发送通知消息出错：\r\n{0}", e.ToString());
                    }
                }
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error("指派驻场回调失败：\r\n{0}", e.ToString());
            }
            return response;
        }









        /// <summary>
        /// 保存楼盘概况
        /// </summary>
        /// <param name="user"></param>
        /// <param name="buildingId"></param>
        /// <param name="buildingRequest"></param>
        /// <returns></returns>
        [HttpPut("summary/{buildingId}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "BuildingUpdate" })]
        public async Task<ResponseMessage> PutBuildingsSummary(UserInfo user, [FromRoute] string buildingId, [FromBody] BuildingRequest buildingRequest)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})保存楼盘概况(PutBuildingsSummary)：\r\n请求参数为：\r\n" + (buildingRequest != null ? JsonHelper.ToJson(buildingRequest) : ""));

            ResponseMessage response = new ResponseMessage();
            if (!ModelState.IsValid || buildingRequest.Id != buildingId)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})保存楼盘概况(PutBuildingsSummary)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (buildingRequest != null ? JsonHelper.ToJson(buildingRequest) : ""));
                return response;
            }
            try
            {
                await _buildingsManager.SaveSummaryAsync(user, buildingId, buildingRequest.Summary, buildingRequest.Source, buildingRequest.SourceId, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})保存楼盘概况(PutBuildingsSummary)报错：\r\n{e.ToString()}，\r\n请求参数为：\r\n" + (buildingRequest != null ? JsonHelper.ToJson(buildingRequest) : ""));
            }
            return response;
        }

        /// <summary>
        /// 保存楼盘佣金方案
        /// </summary>
        /// <param name="user"></param>
        /// <param name="buildingId"></param>
        /// <param name="buildingRequest"></param>
        /// <returns></returns>
        [HttpPut("commission/{buildingId}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "BuildingUpdate" })]
        public async Task<ResponseMessage> PutBuildingsCommission(UserInfo user, [FromRoute] string buildingId, [FromBody] BuildingRequest buildingRequest)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})保存楼盘佣金方案(PutBuildingsCommission)：\r\n请求参数为：\r\n" + (buildingRequest != null ? JsonHelper.ToJson(buildingRequest) : ""));

            ResponseMessage response = new ResponseMessage();
            if (!ModelState.IsValid || buildingRequest.Id != buildingId)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})保存楼盘佣金方案(PutBuildingsCommission)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (buildingRequest != null ? JsonHelper.ToJson(buildingRequest) : ""));
                return response;
            }
            try
            {
                await _buildingsManager.SaveCommissionAsync(user, buildingId, buildingRequest.CommissionPlan, buildingRequest.Source, buildingRequest.SourceId, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})保存楼盘佣金方案(PutBuildingsCommission)报错：\r\n{e.ToString()}，\r\n请求参数为：\r\n(buildingId){buildingId ?? ""}");
            }
            return response;
        }

        /// <summary>
        /// 楼盘提交审核
        /// </summary>
        /// <param name="user"></param>
        /// <param name="buildingId"></param>
        /// <returns></returns>
        [HttpPost("audit/submit/{buildingId}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "BuildingUpdate" })]
        public async Task<ResponseMessage<ExamineStatusEnum>> SubmitBuilding(UserInfo user, [FromRoute] string buildingId)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})楼盘提交审核(SubmitBuilding)：\r\n请求参数为：\r\n(buildingId){buildingId}");

            ResponseMessage<ExamineStatusEnum> response = new ResponseMessage<ExamineStatusEnum>();
            if (string.IsNullOrEmpty(buildingId))
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = "参数不能为空";
                Logger.Warn($"用户{user?.UserName ?? ""}({user?.Id ?? ""})楼盘提交审核(SubmitBuilding)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n(buildingId){buildingId}");
                return response;
            }
            try
            {
                var building = await _buildingsManager.FindByIdAsync(user.Id, buildingId);
                if (building == null)
                {
                    response.Code = ResponseCodeDefines.NotFound;
                    response.Message = "提交审核的楼盘不存在：" + buildingId;
                    Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})楼盘提交审核(SubmitBuilding)失败：提交审核的楼盘不存在，\r\n请求参数为：\r\n(buildingId){buildingId ?? ""}");
                    return response;
                }
                GatewayInterface.Dto.ExamineSubmitRequest examineSubmitRequest = new GatewayInterface.Dto.ExamineSubmitRequest();
                examineSubmitRequest.ContentId = buildingId;
                examineSubmitRequest.ContentType = "building";
                examineSubmitRequest.ContentName = building.BasicInfo.Name;
                examineSubmitRequest.Source = "";
                examineSubmitRequest.CallbackUrl = ApplicationContext.Current.BuildingExamineCallbackUrl;
                if (await _permissionExpansionManager.HavePermission(user.Id, "BuildingCreateQuick"))
                {
                    examineSubmitRequest.Action = "BuildingExaminePass";
                }
                else
                {
                    examineSubmitRequest.Action = "BuildingExamine";
                }
                examineSubmitRequest.TaskName = user.UserName + "提交的楼盘：" + buildingId;
                examineSubmitRequest.Ext1 = building.BasicInfo.Name;
                examineSubmitRequest.Ext2 = building.BasicInfo.AreaFullName;

                GatewayInterface.Dto.UserInfo userInfo = new GatewayInterface.Dto.UserInfo()
                {
                    Id = user.Id,
                    KeyWord = user.KeyWord,
                    OrganizationId = user.OrganizationId,
                    OrganizationName = user.OrganizationName,
                    UserName = user.UserName
                };

                var _examineInterface = ApplicationContext.Current.Provider.GetRequiredService<IExamineInterface>();
                var response2 = await _examineInterface.Submit(userInfo, examineSubmitRequest);
                if (response2.Code != ResponseCodeDefines.SuccessCode)
                {
                    response.Code = ResponseCodeDefines.ServiceError;
                    response.Message = "向审核中心发起审核请求失败：" + response2.Message;
                    Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})楼盘提交审核(SubmitBuilding)失败：\r\n向审核中心发起审核请求失败{response2.Message ?? ""}，\r\n请求参数为：\r\n(buildingId){buildingId ?? ""}");
                    return response;
                }
                await _buildingsManager.SubmitAsync(buildingId, Dto.ExamineStatusEnum.Auditing, HttpContext.RequestAborted);
                response.Extension = ExamineStatusEnum.Auditing;
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})楼盘提交审核(SubmitBuilding)报错：\r\n{e.ToString()}，\r\n请求参数为：\r\n(buildingId){buildingId ?? ""}");
            }
            return response;
        }


        /// <summary>
        /// 楼盘提交审核中心回调(内部使用)
        /// </summary>
        /// <param name="examineResponse"></param>
        /// <returns></returns>
        [HttpPost("audit/examinecallback")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<ExamineStatusEnum>> SubmitBuildingCallback([FromBody] ExamineResponse examineResponse)
        {
            Logger.Trace($"楼盘提交审核中心回调(SubmitBuildingCallback)：\r\n请求参数为：\r\n" + (examineResponse != null ? JsonHelper.ToJson(examineResponse) : ""));

            ResponseMessage<ExamineStatusEnum> response = new ResponseMessage<ExamineStatusEnum>();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Error($"楼盘提交审核中心回调(SubmitBuildingCallback)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (examineResponse != null ? JsonHelper.ToJson(examineResponse) : ""));
                return response;
            }
            try
            {
                var building = await _buildingsManager.FindByIdAsync("", examineResponse.ContentId);
                if (building == null)
                {
                    response.Code = ResponseCodeDefines.NotFound;
                    response.Message = "楼盘不存在：" + examineResponse.ContentId;
                    Logger.Error($"楼盘提交审核中心回调(SubmitBuildingCallback)失败：楼盘不存在，\r\n请求参数为：\r\n" + (examineResponse != null ? JsonHelper.ToJson(examineResponse) : ""));
                    return response;
                }
                if (examineResponse.ExamineStatus == ExamineStatus.Examined)
                {
                    await _buildingsManager.SubmitAsync(examineResponse.ContentId, Dto.ExamineStatusEnum.Approved);
                    response.Extension = ExamineStatusEnum.Approved;
                }
                else if (examineResponse.ExamineStatus == ExamineStatus.Reject)
                {
                    await _buildingsManager.SubmitAsync(examineResponse.ContentId, Dto.ExamineStatusEnum.Reject);
                    response.Extension = ExamineStatusEnum.Reject;
                }
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"楼盘提交审核中心回调(SubmitBuildingCallback)报错：\r\n{e.ToString()}，\r\n请求参数为：\r\n" + examineResponse != null ? JsonHelper.ToJson(examineResponse) : "");
            }
            return response;
        }

        /// <summary>
        /// 根据楼盘Id删除楼盘信息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="buildingId"></param>
        /// <returns></returns>
        [HttpDelete("{buildingId}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "BuildingDelete" })]
        public async Task<ResponseMessage> DeleteBuildings(UserInfo user, [FromRoute] string buildingId)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})根据楼盘Id删除楼盘信息(DeleteBuildings)：\r\n请求参数为：\r\n(buildingId){buildingId ?? ""}");

            ResponseMessage response = new ResponseMessage();
            try
            {
                await _buildingsManager.DeleteAsync(user, buildingId, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})根据楼盘Id删除楼盘信息(DeleteBuildings)报错：\r\n{e.ToString()}，\r\n请求参数为：\r\n(buildingId){buildingId ?? ""}");
            }
            return response;
        }

        /// <summary>
        /// 批量删除楼盘
        /// </summary>
        /// <param name="user"></param>
        /// <param name="groupIds"></param>
        /// <returns></returns>
        [HttpPost("delete")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "BuildingDelete" })]
        public async Task<ResponseMessage> DeleteBuildingsList(UserInfo user, [FromBody] List<string> groupIds)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})批量删除楼盘(DeleteBuildingsList)：\r\n请求参数为：\r\n" + (groupIds != null ? JsonHelper.ToJson(groupIds) : ""));

            ResponseMessage response = new ResponseMessage();
            try
            {
                await _buildingsManager.DeleteListAsync(user.Id, groupIds, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})批量删除楼盘(DeleteBuildingsList)报错：\r\n{e.ToString()}，\r\n请求参数为：\r\n" + (groupIds != null ? JsonHelper.ToJson(groupIds) : ""));
            }
            return response;
        }

    }
}
