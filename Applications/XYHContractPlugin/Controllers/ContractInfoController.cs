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
using ContractComplementRequest = XYHContractPlugin.Dto.Response.ContractComplementResponse;
using AspNet.Security.OAuth.Validation;
using XYHContractPlugin.Models;
using System.Linq;
using XYHContractPlugin.Dto.Request;
using System.Collections.Specialized;
using XYHContractPlugin.Dto;

namespace XYHContractPlugin.Controllers
{
    [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/contractinfo")]
    public class ContractInfoController : Controller
    {
        private readonly ILogger Logger = LoggerManager.GetLogger("Contractinfo");
        private readonly ContractInfoManager _contractInfoManager;
        private readonly FileScopeManager _fileScopeManager;
        private readonly RestClient _restClient;

        public ContractInfoController(ContractInfoManager contractManager, FileScopeManager fim, RestClient rsc)
        {
            _fileScopeManager = fim;
            _contractInfoManager = contractManager;
            _restClient = rsc;
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

        [HttpGet("{contractId}")]
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
                var ret = await _contractInfoManager.GetAllinfoByIdAsync2(user,contractId, HttpContext.RequestAborted);
                Response.Extension = ret;

                foreach (var item in ret.Modifyinfo)
                {
                    if (item.ID == ret.BaseInfo.CurrentModify)
                    {
                        ret.BaseInfo.ExamineStatus = (int)item.ExamineStatus;
                        break;
                    }
                }

                foreach (var itm in ret.AnnexInfo)
                {
                    int annextype = int.Parse(itm.Group);
                    if (annextype == 1)
                    {
                        ret.BaseInfo.IsSubmmitContractScan = true;
                    }
                    else if (annextype == 2)
                    {
                        ret.BaseInfo.IsSubmmitContractAnnex = true;
                    }
                    else if (annextype == 3)
                    {
                        ret.BaseInfo.IsSubmmitRelation = true;
                    }
                    else if (annextype == 4)
                    {
                        ret.BaseInfo.IsSubmmitNet = true;
                    }
                }

                if (!string.IsNullOrEmpty(Response.Extension.BaseInfo.CreateUser))
                {
                    var resp = await _restClient.Get<ResponseMessage<string>>($"http://localhost:5000/api/user/{Response.Extension.BaseInfo.CreateUser}", null);
                    Response.Extension.BaseInfo.CreateUserName = resp.Extension;
                }

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

                    Response.Extension.FileList = fileItems;
                }
            }
            catch (Exception e)
            {
                Response.Code = ResponseCodeDefines.ServiceError;
                Response.Message = "服务器错误：" + e.ToString();
                Logger.Error("error");
            }
            return Response;
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

        [HttpGet("allcontractinfo")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<List<ContractContentResponse>>> GetAllContractByUser(UserInfo user)
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

        [HttpGet("currentmodifybyid/{contractId}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<ContractModifyResponse>> GetCurrentModifyByid(UserInfo user, [FromRoute] string contractId)
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
                Response.Extension = await _contractInfoManager.GetCurrentModifyInfo(contractId, HttpContext.RequestAborted);
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

        [HttpGet("modifyhistorybyid/{contractId}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<List<ContractModifyResponse>>> GetModifyHistoryByid(UserInfo user, [FromRoute] string contractId)
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
                if (!string.IsNullOrEmpty(User.OrganizationId))
                {
                    var resp = await _restClient.Get<ResponseMessage<string>>($"http://localhost:5000/api/Organization/{User.OrganizationId}", null);
                    request.BaseInfo.CreateDepartment = resp.Extension;
                }

                string strModifyGuid = "";
                if (request.Modifyinfo != null && request.Modifyinfo.Count > 0)
                {
                    strModifyGuid = request.Modifyinfo.ElementAt(0).ID;
                }
                else
                {
                    strModifyGuid = Guid.NewGuid().ToString();
                    request.Modifyinfo = new List<ContractModifyResponse>();
                    request.Modifyinfo.Add(new ContractModifyResponse { ID= strModifyGuid });
                }

                
                GatewayInterface.Dto.ExamineSubmitRequest exarequest = new GatewayInterface.Dto.ExamineSubmitRequest();
                exarequest.ContentId = request.BaseInfo.ID;
                exarequest.ContentType = "ContractCommit";
                exarequest.ContentName = "AddContract";
                exarequest.SubmitDefineId = strModifyGuid;
                exarequest.Source = "";
                exarequest.CallbackUrl = ApplicationContext.Current.UpdateExamineCallbackUrl;
                exarequest.Action = "TEST";/* exarequest.ContentType*/;
                exarequest.TaskName = $"{User.UserName}添加合同{exarequest.ContentName}的动态{exarequest.ContentType}";

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
                request.BaseInfo.Num = await _contractInfoManager.GetContractNum(request.BaseInfo.ID, HttpContext.RequestAborted);
                response.Extension = await _contractInfoManager.AddContractAsync(User, request, "TEST", HttpContext.RequestAborted);
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

        [HttpPost("addcomplement/{contract}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<bool>> AddComplementContract(UserInfo User, [FromBody]List<ContractComplementRequest> request, [FromRoute]string contract)
        {
            Logger.Trace($"用户{User?.UserName ?? ""}({User?.Id ?? ""})添加补充协议基础信息(PutBuildingBaseInfo)：\r\n请求参数为：\r\n" + (request != null ? JsonHelper.ToJson(request) : ""));

            var response = new ResponseMessage<bool>();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                return response;
            }

            try
            {
                string strModifyGuid = Guid.NewGuid().ToString();

                GatewayInterface.Dto.ExamineSubmitRequest exarequest = new GatewayInterface.Dto.ExamineSubmitRequest();
                exarequest.ContentId = contract;
                exarequest.ContentType = "ContractCommit";
                exarequest.ContentName = "AddComplement";
                exarequest.SubmitDefineId = strModifyGuid;
                exarequest.Source = "";
                exarequest.CallbackUrl = ApplicationContext.Current.UpdateExamineCallbackUrl;
                exarequest.Action = "TEST";/* exarequest.ContentType*/;
                exarequest.TaskName = $"{User.UserName}添加合同补充协议{exarequest.ContentName}的动态{exarequest.ContentType}";

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
                //response.Extension = await _contractInfoManager.AddComplementAsync(User, contract, strModifyGuid, "TEST", request, HttpContext.RequestAborted);
                await _contractInfoManager.CreateComplementModifyAsync(User, contract, strModifyGuid, "TEST", JsonHelper.ToJson(User), JsonHelper.ToJson(request),HttpContext.RequestAborted);
                response.Message = "addcomplement ok";
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{User?.UserName ?? ""}({User?.Id ?? ""})合同动态提交审核(UpdateRecordSubmit)报错：\r\n{e.ToString()},\r\n请求参数为：\r\n" + (request != null ? JsonHelper.ToJson(request) : ""));
            }

            return response;
        }

        [HttpPost("autocomplement/{contract}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<bool>> AutoComplementContract(UserInfo User, [FromBody]List<ContractComplementRequest> request, [FromRoute]string contract)
        {
            Logger.Trace($"用户{User?.UserName ?? ""}({User?.Id ?? ""})添加补充协议基础信息(PutBuildingBaseInfo)：\r\n请求参数为：\r\n" + (request != null ? JsonHelper.ToJson(request) : ""));

            var response = new ResponseMessage<bool>();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                return response;
            }

            try
            {
                string strModifyGuid = Guid.NewGuid().ToString();

                GatewayInterface.Dto.ExamineSubmitRequest exarequest = new GatewayInterface.Dto.ExamineSubmitRequest();
                exarequest.ContentId = contract;
                exarequest.ContentType = "ContractCommit";
                exarequest.ContentName = "AddComplement";
                exarequest.SubmitDefineId = strModifyGuid;
                exarequest.Source = "";
                exarequest.CallbackUrl = ApplicationContext.Current.UpdateExamineCallbackUrl;
                exarequest.Action = "TEST";/* exarequest.ContentType*/;
                exarequest.TaskName = $"{User.UserName}更新合同补充协议{exarequest.ContentName}的动态{exarequest.ContentType}";

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
                response.Extension = await _contractInfoManager.AutoUpdateComplementAsync(User, contract, "TEST", strModifyGuid, request, HttpContext.RequestAborted);
                response.Message = "addcomplement ok";
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Error($"用户{User?.UserName ?? ""}({User?.Id ?? ""})合同动态提交审核(UpdateRecordSubmit)报错：\r\n{e.ToString()},\r\n请求参数为：\r\n" + (request != null ? JsonHelper.ToJson(request) : ""));
            }

            return response;
        }

        [HttpPost("modifycomplement/{contract}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<bool>> ModifyComplementContract(UserInfo User, [FromBody]List<ContractComplementRequest> request, [FromRoute]string contract)
        {
            Logger.Trace($"用户{User?.UserName ?? ""}({User?.Id ?? ""})修改补充协议基础信息(PutBuildingBaseInfo)：\r\n请求参数为：\r\n" + (request != null ? JsonHelper.ToJson(request) : ""));

            var response = new ResponseMessage<bool>();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                response.Message = ModelState.GetAllErrors();
                return response;
            }

            try
            {
                string strModifyGuid = Guid.NewGuid().ToString();

                GatewayInterface.Dto.ExamineSubmitRequest exarequest = new GatewayInterface.Dto.ExamineSubmitRequest();
                exarequest.ContentId = contract;
                exarequest.ContentType = "ContractCommit";
                exarequest.ContentName = "ModifyComplement";
                exarequest.SubmitDefineId = strModifyGuid;
                exarequest.Source = "";
                exarequest.CallbackUrl = ApplicationContext.Current.UpdateExamineCallbackUrl;
                exarequest.Action = "TEST";/* exarequest.ContentType*/;
                exarequest.TaskName = $"{User.UserName}修改合同补充协议{exarequest.ContentName}的动态{exarequest.ContentType}";

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
                response.Extension = await _contractInfoManager.ModifyComplementAsync(User, contract, strModifyGuid, "TEST", request, HttpContext.RequestAborted);
                response.Message = "addcomplement ok";
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
                var guid = Guid.NewGuid().ToString();

                //审核提交
                GatewayInterface.Dto.ExamineSubmitRequest exarequest = new GatewayInterface.Dto.ExamineSubmitRequest();
                exarequest.ContentId = request.BaseInfo.ID;
                exarequest.ContentType = "ContractCommit";
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

                Logger.Trace($"{exarequest.ContentId}发生审核成功");
                request.BaseInfo.Num = string.IsNullOrEmpty(request.BaseInfo.Num) ? await _contractInfoManager.GetContractNum(request.BaseInfo.ID, HttpContext.RequestAborted) : request.BaseInfo.Num;
                await _contractInfoManager.ModifyContractBeforCheckAsync(User,  request, guid, "TEST", HttpContext.RequestAborted);

                
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
                exarequest.ContentType = "ContractCommit";
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

        [HttpGet("contractcurcheck/{contractId}")]
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
                await _contractInfoManager.CreateAsync(User,request, exarequest.SubmitDefineId, "TEST",HttpContext.RequestAborted);
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
        public async Task<ResponseMessage> SubmitContractCallback([FromBody] ExamineResponse examineResponse)
        {
            Logger.Trace($"合同提交审核中心回调(SubmitContractCallback)：\r\n请求参数为：\r\n" + (examineResponse != null ? JsonHelper.ToJson(examineResponse) : ""));

            ResponseMessage response = new ResponseMessage();

            if (examineResponse == null)
            {

                response.Code = ResponseCodeDefines.ModelStateInvalid;
                Logger.Trace($"合同提交审核中心回调(SubmitContractCallback)模型验证失败：\r\n{response.Message ?? ""}，\r\n请求参数为：\r\n" + (examineResponse != null ? JsonHelper.ToJson(examineResponse) : ""));
                return response;
            }
            try
            {
                var building = await _contractInfoManager.FindByIdAsync(examineResponse.ContentId);

                response.Code = ResponseCodeDefines.SuccessCode;
                if (building == null)
                {
                    response.Code = ResponseCodeDefines.NotFound;
                    response.Message = "合同不存在：" + examineResponse.ContentId;
                    Logger.Trace($"合同提交审核中心回调(SubmitBuildingCallback)失败：合同不存在，\r\n请求参数为：\r\n" + (examineResponse != null ? JsonHelper.ToJson(examineResponse) : ""));
                    return response;
                }

                if (examineResponse.ExamineStatus == ExamineStatus.Examined)
                {
                    var modifyre = await _contractInfoManager.OperModifyInfoAsync(examineResponse.SubmitDefineId, examineResponse.ContentId, ExamineStatusEnum.Approved);

                    if (modifyre.Type == ContractInfoManager.ModifyContract)
                    {
                        await _contractInfoManager.ModifyContractAfterCheckAsync(modifyre.ID, modifyre.ContractID, modifyre.Ext1, ExamineStatusEnum.Approved);
                    }
                    else if (modifyre.Type == ContractInfoManager.AddAnnexContract)
                    {
                        List<FileInfoRequest> fileInfoRequests = JsonHelper.ToObject<List<FileInfoRequest>>(modifyre.Ext1);
                        List<NWF> listnf = JsonHelper.ToObject<List<NWF>>(modifyre.Ext2);
                        UserInfo user = JsonHelper.ToObject<UserInfo>(modifyre.Ext3);

                        int nindex = 0;
                        foreach (var item in fileInfoRequests)
                        {
                            try
                            {
                                NameValueCollection nameValueCollection = new NameValueCollection();
                                nameValueCollection.Add("appToken", "app:nwf");
                                Logger.Info("nwf协议");
                                string response2 = await _restClient.Post(ApplicationContext.Current.NWFUrl, listnf.ElementAt(nindex++), "POST", nameValueCollection);
                                Logger.Info("返回：\r\n{0}", response2);

                                await _fileScopeManager.CreateAsync(user, modifyre.Ext4, modifyre.ContractID, modifyre.ID, item);

                                response.Message = response2;
                            }
                            catch (Exception e)
                            {
                                response.Code = ResponseCodeDefines.PartialFailure;
                                response.Message += $"文件：{item.FileGuid}处理出错，错误信息：{e.ToString()}。\r\n";
                                Logger.Error($"用户{user?.UserName ?? ""}({user?.Id ?? ""})批量上传文件信息(UploadFiles)报错：\r\n{e.ToString()},请求参数为：\r\n,(dest){modifyre.Ext4 ?? ""},(contractId){ modifyre.ContractID ?? ""}," + (fileInfoRequests != null ? JsonHelper.ToJson(fileInfoRequests) : ""));
                            }
                        }
                    }
                    else if (modifyre.Type == ContractInfoManager.UpdateComplementContract)
                    {
                        UserInfo User = JsonHelper.ToObject<UserInfo>(modifyre.Ext1);
                        List<ContractComplementRequest> request = JsonHelper.ToObject<List<ContractComplementRequest>>(modifyre.Ext2);
                        await _contractInfoManager.AddComplementAsync(User, modifyre.ContractID, modifyre.ID, "TEST", request);
                    }
                }
                else if (examineResponse.ExamineStatus == ExamineStatus.Reject)
                {
                    await _contractInfoManager.SubmitAsync(examineResponse.SubmitDefineId, ExamineStatusEnum.Reject);
                }
            }
            catch (Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Trace($"合同提交审核中心回调(SubmitBuildingCallback)报错：\r\n{e.ToString()}，\r\n请求参数为：\r\n" + examineResponse != null ? JsonHelper.ToJson(examineResponse) : "");
            }
            return response;
        }
        [HttpGet("getFollowHistory/{contractId}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "" })]
        public async Task<ResponseMessage<List<ContractInfoResponse>>> GetFollowHistory(UserInfo user, string contractId)
        {
            ResponseMessage<List<ContractInfoResponse>> response = new ResponseMessage<List<ContractInfoResponse>>();
            
            try
            {
                Logger.Trace($"获取合同续签记录(GetFollowHistory)：\r\n请求参数为：\r\n" + contractId);
                if (string.IsNullOrEmpty(contractId))
                {
                    response.Code = ResponseCodeDefines.ModelStateInvalid;
                    response.Message = "请求参数不正确";
                    Logger.Error("error GetContractByid");
                    return response;
                }

                response.Extension = await _contractInfoManager.GetFollowHistory(user, contractId, HttpContext.RequestAborted);
                response.Code = "0";
            }
            catch(Exception e)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = e.ToString();
                Logger.Trace($"获取合同续签记录(GetFollowHistory)：\r\n请求参数为：\r\n" + contractId);
            }
            return response;
        }
    }
}
