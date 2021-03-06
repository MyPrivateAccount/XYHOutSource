﻿using System;
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
using XYHHumanPlugin.Managers;
using System.Collections.Specialized;
using XYHHumanPlugin.Dto.Common;
using GatewayInterface;
using Microsoft.Extensions.DependencyInjection;
using SocialInsuranceRequest = XYHHumanPlugin.Dto.Response.SocialInsuranceResponse;
using LeaveInfoRequest = XYHHumanPlugin.Dto.Response.LeaveInfoResponse;
using ChangeInfoRequest = XYHHumanPlugin.Dto.Response.ChangeInfoResponse;
using ApplicationCore.Managers;
using XYHHumanPlugin.Models;
using GatewayInterface.Dto.Response;

namespace XYHHumanPlugin.Controllers
{
    [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/humaninfo")]
    public class HumanInfoController : Controller
    {
        private readonly ILogger Logger = LoggerManager.GetLogger("XYHHumaninfo");
        private readonly HumanManager _humanManage;
        private readonly HumanInfoManager _humanInfoManager;
        private readonly RestClient _restClient;
        private string _lastDate;
        private int _lastNumber;

        //一共俩份
        //private const int CreateNOModifyType = 0;//无
        //private const int CreateHumanModifyType = 1;//未入
        //private const int EntryHumanModifyType = 3;//入职
        //private const int BecomeHumanModifyType = 5;//转正
        //private const int LeaveHumanModifyType = 7;//离职


        public HumanInfoController(RestClient rsc, HumanInfoManager humanInfoManager, HumanManager human)
        {
            _humanManage = human;
            _humanInfoManager = humanInfoManager;
            _lastNumber = 0;
            _restClient = rsc;
        }

        /// <summary>
        /// 保存员工人事信息，如果已存在则更新
        /// </summary>
        /// <param name="user"></param>
        /// <param name="humanInfoRequest"></param>
        /// <returns></returns>
        [HttpPut]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<HumanInfoResponse>> SaveHumanInfo(UserInfo user, [FromBody]HumanInfoRequest humanInfoRequest)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})保存员工人事信息，如果已存在则更新(SaveHumanInfo)，请求体为：\r\n" + (humanInfoRequest != null ? JsonHelper.ToJson(humanInfoRequest) : ""));

            ResponseMessage<HumanInfoResponse> response = new ResponseMessage<HumanInfoResponse>();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})保存员工人事信息，如果已存在则更新(SaveHumanInfo)模型验证失败：{response.Message}请求体为：\r\n" + (humanInfoRequest != null ? JsonHelper.ToJson(humanInfoRequest) : ""));
                return response;
            }
            try
            {
                return await _humanInfoManager.SaveHumanInfo(user, humanInfoRequest, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.Message;
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})根据报告Id获取成交报告信息(GetReport)失败：{response.Message}请求体为：\r\n" + (humanInfoRequest != null ? JsonHelper.ToJson(humanInfoRequest) : ""));
            }
            return response;
        }



        /// <summary>
        /// 通过Id获取人事信息
        /// </summary>
        /// <param name="user"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<HumanInfoResponse>> GetHumanInfo(UserInfo user, [FromRoute]string id)
        {
            Logger.Trace($"用户{user?.UserName ?? ""}({user?.Id ?? ""})根据Id获取员工人事信息(GetHumanInfo)，请求体为：id:{id ?? ""}");

            ResponseMessage<HumanInfoResponse> response = new ResponseMessage<HumanInfoResponse>();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})根据Id获取员工人事信息(GetHumanInfo)模型验证失败：{response.Message}请求体为：id:{id ?? ""}");
                return response;
            }
            try
            {
                return await _humanInfoManager.GetHumanInfoAsync(user, id, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.Message;
                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})根据Id获取员工人事信息(GetHumanInfo)失败：{response.Message}请求体为：id:{id ?? ""}");
            }
            return response;
        }

        [HttpPost("search")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<PagingResponseMessage<HumanInfoSearchResponse>> Search(UserInfo User, [FromBody]HumanInfoSearchCondition condition)
        {
            Logger.Trace($"用户{User?.UserName ?? ""}({User?.Id ?? ""})查询人事信息(Search)，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
            var pagingResponse = new PagingResponseMessage<HumanInfoSearchResponse>();
            if (!ModelState.IsValid)
            {
                pagingResponse.Code = ResponseCodeDefines.ModelStateInvalid;
                pagingResponse.Message = ModelState.GetAllErrors();
                Logger.Warn($"用户{User?.UserName ?? ""}({User?.Id ?? ""})查询人事信息(Search)模型验证失败：\r\n{pagingResponse.Message ?? ""}，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
                return pagingResponse;
            }
            try
            {
                //if (await _permissionExpansionManager.HavePermission(User.Id, "SEARCH_CONTRACT"))
                //{
                pagingResponse = await _humanInfoManager.SearchHumanInfo(User, condition, HttpContext.RequestAborted);
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
                Logger.Error($"用户{User?.UserName ?? ""}({User?.Id ?? ""})查询人事信息(Search)请求失败：\r\n{pagingResponse.Message ?? ""}，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
            }
            return pagingResponse;
        }

        /// <summary>
        /// 员工转正
        /// </summary>
        /// <param name="user"></param>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpPost("regular")]
        public async Task<ResponseMessage> HumanRegular(UserInfo user, HumanInfoRegularRequest humanRegularRequest)
        {
            ResponseMessage response = new ResponseMessage();




            return response;
        }



        /// <summary>
        /// 员工异动调薪
        /// </summary>
        /// <param name="user"></param>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpPost("adjustment")]
        public async Task<ResponseMessage> HumanAdjustment(UserInfo user, HumanInfoAdjustmentRequest humanAdjustmentRequest)
        {
            ResponseMessage response = new ResponseMessage();




            return response;
        }



        /// <summary>
        /// 员工离职
        /// </summary>
        /// <param name="user"></param>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpPost("leave")]
        public async Task<ResponseMessage> HumanLeave(UserInfo user, HumanInfoLeaveRequest humanLeaveRequest)
        {
            ResponseMessage response = new ResponseMessage();




            return response;
        }


        /// <summary>
        /// 员工兼职
        /// </summary>
        /// <param name="user"></param>
        /// <param name=""></param>
        /// <returns></returns>
        [HttpPost("parttime")]
        public async Task<ResponseMessage> HumanParttime(UserInfo user, HumanInfoPartPositionRequest humanParttimeRequest)
        {
            ResponseMessage response = new ResponseMessage();




            return response;
        }



        [HttpGet("humanformdata")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<List<HumanInfoFormResponse>>> GetHumanFormData(UserInfo User)
        {
            var Response = new ResponseMessage<List<HumanInfoFormResponse>>();
            if (!ModelState.IsValid)
            {
                Response.Code = ResponseCodeDefines.ModelStateInvalid;
                Logger.Warn($"用户{User?.UserName ?? ""}({User?.Id ?? ""})获取报表信息(PostCustomerListSaleMan)模型验证失败：\r\n{Response.Message ?? ""}，\r\n请求参数为：\r\n");
                return Response;
            }

            try
            {
                //Response.Extension = await _monthManage.GetMonthForm();
            }
            catch (Exception e)
            {
                Response.Code = ResponseCodeDefines.ServiceError;
                Response.Message = "服务器错误：" + e.ToString();
                Logger.Error("error");
            }

            return Response;
        }

        [HttpPost("becomehuman")]//转正
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage> BecomeHumanInfo(UserInfo User, [FromBody]SocialInsuranceRequest condition)
        {
            var pagingResponse = new ResponseMessage();
            if (!ModelState.IsValid)
            {
                pagingResponse.Code = ResponseCodeDefines.ModelStateInvalid;
                Logger.Warn($"用户{User?.UserName ?? ""}({User?.Id ?? ""})转正人事条件(PostCustomerListSaleMan)模型验证失败：\r\n{pagingResponse.Message ?? ""}，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
                return pagingResponse;
            }

            try
            {
                string modifyid = Guid.NewGuid().ToString();

                GatewayInterface.Dto.ExamineSubmitRequest exarequest = new GatewayInterface.Dto.ExamineSubmitRequest();
                exarequest.ContentId = condition.ID;
                exarequest.ContentType = "HumanCommit";
                exarequest.ContentName = $"beconmehuman {condition.IDCard}";
                exarequest.SubmitDefineId = modifyid;
                exarequest.Source = "";
                //exarequest.CallbackUrl = ApplicationContext.Current.UpdateExamineCallbackUrl;
                exarequest.Action = "TEST"/*exarequest.ContentType*/;
                exarequest.TaskName = $"{User.UserName}提交转正请求{exarequest.ContentName}的动态{exarequest.ContentType}"; ;
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
                    pagingResponse.Code = ResponseCodeDefines.ServiceError;
                    pagingResponse.Message = "向审核中心发起审核请求失败：" + reponse.Message;
                    return pagingResponse;
                }

                await _humanManage.PreBecomeHuman(User, modifyid, condition, "TEST", HttpContext.RequestAborted);
                //await _humanManage.BecomeHuman(condition, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                pagingResponse.Code = ResponseCodeDefines.ServiceError;
                pagingResponse.Message = "服务器错误:" + e.ToString();
                Logger.Error($"用户{User?.UserName ?? ""}({User?.Id ?? ""})转正条件(PostCustomerListSaleMan)请求失败：\r\n{pagingResponse.Message ?? ""}，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
            }
            return pagingResponse;
        }

        [HttpPost("leavehuman")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage> LeaveHumanInfo(UserInfo User, [FromBody]LeaveInfoRequest condition)
        {
            var pagingResponse = new ResponseMessage();
            if (!ModelState.IsValid)
            {
                pagingResponse.Code = ResponseCodeDefines.ModelStateInvalid;
                Logger.Warn($"用户{User?.UserName ?? ""}({User?.Id ?? ""})人事离职条件(PostCustomerListSaleMan)模型验证失败：\r\n{pagingResponse.Message ?? ""}，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
                return pagingResponse;
            }

            try
            {
                string modifyid = Guid.NewGuid().ToString();

                GatewayInterface.Dto.ExamineSubmitRequest exarequest = new GatewayInterface.Dto.ExamineSubmitRequest();
                exarequest.ContentId = condition.ID;
                exarequest.ContentType = "HumanCommit";
                exarequest.ContentName = $"leavehuman {condition.IDCard}";
                exarequest.SubmitDefineId = modifyid;
                exarequest.Source = "";
                //exarequest.CallbackUrl = ApplicationContext.Current.UpdateExamineCallbackUrl;
                exarequest.Action = "TEST"/*exarequest.ContentType*/;
                exarequest.TaskName = $"{User.UserName}提交离职请求{exarequest.ContentName}的动态{exarequest.ContentType}"; ;
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
                    pagingResponse.Code = ResponseCodeDefines.ServiceError;
                    pagingResponse.Message = "向审核中心发起审核请求失败：" + reponse.Message;
                    return pagingResponse;
                }

                await _humanManage.PreLeaveHuman(User, modifyid, condition, "TEST", HttpContext.RequestAborted);
                //await _humanManage.LeaveHuman(condition, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                pagingResponse.Code = ResponseCodeDefines.ServiceError;
                pagingResponse.Message = "服务器错误:" + e.ToString();
                Logger.Error($"用户{User?.UserName ?? ""}({User?.Id ?? ""})员工离职条件(PostCustomerListSaleMan)请求失败：\r\n{pagingResponse.Message ?? ""}，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
            }
            return pagingResponse;
        }

        [HttpPost("changehuman")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage> ChangeHumanInfo(UserInfo User, [FromBody]ChangeInfoRequest condition)
        {
            var pagingResponse = new ResponseMessage();
            if (!ModelState.IsValid)
            {
                pagingResponse.Code = ResponseCodeDefines.ModelStateInvalid;
                Logger.Warn($"用户{User?.UserName ?? ""}({User?.Id ?? ""})人事异动条件(PostCustomerListSaleMan)模型验证失败：\r\n{pagingResponse.Message ?? ""}，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
                return pagingResponse;
            }

            try
            {
                string modifyid = Guid.NewGuid().ToString();

                GatewayInterface.Dto.ExamineSubmitRequest exarequest = new GatewayInterface.Dto.ExamineSubmitRequest();
                exarequest.ContentId = condition.ID;
                exarequest.ContentType = "HumanCommit";
                exarequest.ContentName = $"changehuman {condition.IDCard}";
                exarequest.SubmitDefineId = modifyid;
                exarequest.Source = "";
                //exarequest.CallbackUrl = ApplicationContext.Current.UpdateExamineCallbackUrl;
                exarequest.Action = "TEST"/*exarequest.ContentType*/;
                exarequest.TaskName = $"{User.UserName}提交离职请求{exarequest.ContentName}的动态{exarequest.ContentType}"; ;
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
                    pagingResponse.Code = ResponseCodeDefines.ServiceError;
                    pagingResponse.Message = "向审核中心发起审核请求失败：" + reponse.Message;
                    return pagingResponse;
                }
                await _humanManage.PreChangeHuman(User, modifyid, condition, "TEST", HttpContext.RequestAborted);
                //await _humanManage.ChangeHuman(condition, HttpContext.RequestAborted);
            }
            catch (Exception e)
            {
                pagingResponse.Code = ResponseCodeDefines.ServiceError;
                pagingResponse.Message = "服务器错误:" + e.ToString();
                Logger.Error($"用户{User?.UserName ?? ""}({User?.Id ?? ""})员工异动条件(PostCustomerListSaleMan)请求失败：\r\n{pagingResponse.Message ?? ""}，\r\n请求参数为：\r\n" + (condition != null ? JsonHelper.ToJson(condition) : ""));
            }
            return pagingResponse;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="User"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        [HttpPost]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<List<HumanInfoResponse1>>> CreateHumanInfo(UserInfo User, [FromBody]HumanInfoRequest humanInfoRequest)
        {
            var Response = new ResponseMessage<List<HumanInfoResponse1>>();
            try
            {
                string modifyid = Guid.NewGuid().ToString();

                GatewayInterface.Dto.ExamineSubmitRequest exarequest = new GatewayInterface.Dto.ExamineSubmitRequest();
                //exarequest.ContentId = condition.humaninfo.ID;
                exarequest.ContentType = "HumanCommit";
                //exarequest.ContentName = $"addhuman {condition.humaninfo.Name}";
                exarequest.SubmitDefineId = modifyid;
                exarequest.Source = "";
                //exarequest.CallbackUrl = ApplicationContext.Current.UpdateExamineCallbackUrl;
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

                //if (condition.fileinfo != null)
                //{
                //    NameValueCollection nameValueCollection = new NameValueCollection();
                //    var nwf = CreateNwf(User, "humaninfo", condition.fileinfo);

                //    nameValueCollection.Add("appToken", "app:nwf");
                //    Logger.Info("nwf协议");
                //    string response2 = await _restClient.Post(ApplicationContext.Current.NWFUrl, nwf, "POST", nameValueCollection);
                //    Logger.Info("返回：\r\n{0}", response2);

                //    await _humanManage.CreateFileScopeAsync(User.Id, condition.humaninfo.ID, condition.fileinfo, HttpContext.RequestAborted);
                //}

                //await _humanManage.AddHuman(User, condition.humaninfo, modifyid, "TEST", HttpContext.RequestAborted);
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
                if (td.Month.ToString() + td.Day.ToString() == _lastDate)
                {
                    _lastNumber++;
                }
                else
                {
                    _lastNumber = 0;
                    _lastDate = td.Month.ToString() + td.Day.ToString();
                }
                Response.Extension = $"XYH-{td.Year.ToString()}{td.Month.ToString()}{td.Day.ToString()}{string.Format("{0:D3}", _lastNumber)}";
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
            header.ExtraAttribute.Add(new AttributeType() { Name = "SubSystem", Value = "humanfile" });
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

        [HttpGet("simpleSearch")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<PagingResponseMessage<HumanInfoResponse>> SimpleSearch(UserInfo User, string permissionId, string keyword, string branchId, int pageSize, int pageIndex)
        {
            var r = new PagingResponseMessage<HumanInfoResponse>();
            try
            {
                r = await _humanManage.SimpleSearch(User, permissionId, keyword, branchId, pageSize, pageIndex);
            }
            catch (Exception e)
            {
                r.Code = ResponseCodeDefines.ServiceError;
                r.Message = "服务器错误：" + e.Message;
                Logger.Error("error");
            }
            return r;
        }

        [HttpPost("hulistbyorg")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<List<HumanInfoResponse>>> SimpleSearch(UserInfo User, [FromBody]List<string> lst)
        {
            var r = new ResponseMessage<List<HumanInfoResponse>>();
            try
            {
                //r.Extension = await _humanManage.GethumanlistByorgid(lst);
                //await _humanManage.SimpleSearch(User, permissionId, keyword,branchId, pageSize, pageIndex);
            }
            catch (Exception e)
            {
                r.Code = ResponseCodeDefines.ServiceError;
                r.Message = "服务器错误：" + e.Message;
                Logger.Error("error");
            }
            return r;
        }



        /// <summary>
        /// 新增人事信息回调
        /// </summary>
        /// <param name="examineResponse"></param>
        /// <returns></returns>
        [HttpPost("humaninfocallback")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage> HumanInfoCreateCallback([FromBody] ExamineResponse examineResponse)
        {
            Logger.Trace($"新增人事信息回调(HumanInfoCreateCallback)：\r\n请求参数为：\r\n" + (examineResponse != null ? JsonHelper.ToJson(examineResponse) : ""));

            ResponseMessage response = new ResponseMessage();

            if (examineResponse == null)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                Logger.Trace($"新增人事信息回调(HumanInfoCreateCallback)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (examineResponse != null ? JsonHelper.ToJson(examineResponse) : ""));
                return response;
            }
            try
            {
                response.Code = ResponseCodeDefines.SuccessCode;
                if (examineResponse.ExamineStatus == ExamineStatus.Examined)
                {
                    await _humanInfoManager.UpdateExamineStatus(examineResponse.SubmitDefineId, ExamineStatusEnum.Approved);
                }
                else if (examineResponse.ExamineStatus == ExamineStatus.Reject)
                {
                    await _humanInfoManager.UpdateExamineStatus(examineResponse.SubmitDefineId, ExamineStatusEnum.Reject);
                }
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Trace($"新增人事信息回调(HumanInfoCreateCallback)报错：\r\n{e.ToString()}，\r\n请求参数为：\r\n" + examineResponse != null ? JsonHelper.ToJson(examineResponse) : "");
            }
            return response;
        }



        /// <summary>
        /// 新增人事信息步骤回调
        /// </summary>
        /// <param name="examineResponse"></param>
        /// <returns></returns>
        [HttpPost("humaninfostepcallback")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage> HumanInfoCreateStepCallback([FromBody] ExamineStepResponse examineStepResponse)
        {
            Logger.Trace($"新增人事信息步骤回调(HumanInfoCreateStepCallback)：\r\n请求参数为：\r\n" + (examineStepResponse != null ? JsonHelper.ToJson(examineStepResponse) : ""));

            ResponseMessage response = new ResponseMessage();
            return response;
        }
    }
}
