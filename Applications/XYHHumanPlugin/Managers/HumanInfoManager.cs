using ApplicationCore;
using ApplicationCore.Dto;
using ApplicationCore.Managers;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XYH.Core.Log;
using XYHHumanPlugin.Dto.Request;
using XYHHumanPlugin.Dto.Response;
using XYHHumanPlugin.Models;
using XYHHumanPlugin.Stores;

namespace XYHHumanPlugin.Managers
{
    public class HumanInfoManager
    {
        public HumanInfoManager(IHumanInfoStore humanInfoStore,
            IMapper mapper,
            PermissionExpansionManager permissionExpansionManager,
            RestClient restClient)
        {
            Store = humanInfoStore;
            _mapper = mapper;
            _restClient = restClient;
            _permissionExpansionManager = permissionExpansionManager;
        }

        protected IHumanInfoStore Store { get; }
        protected IMapper _mapper { get; }
        protected RestClient _restClient { get; }
        protected PermissionExpansionManager _permissionExpansionManager { get; }

        private readonly ILogger Logger = LoggerManager.GetLogger("HumanInfoManager");

        public async Task<ResponseMessage<HumanInfoResponse>> GetHumanInfoAsync(UserInfo user, string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            ResponseMessage<HumanInfoResponse> response = new ResponseMessage<HumanInfoResponse>();

            var q = await Store.GetDetailQuery().Where(a => !a.IsDeleted && a.Id == id).SingleOrDefaultAsync(cancellationToken);

            response.Extension = _mapper.Map<HumanInfoResponse>(q);

            return response;
        }

        public async Task<ResponseMessage<HumanInfoResponse>> SaveHumanInfo(UserInfo user, HumanInfoRequest humanInfoRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            ResponseMessage<HumanInfoResponse> response = new ResponseMessage<HumanInfoResponse>();
            if (user == null || humanInfoRequest == null)
            {
                throw new ArgumentNullException(nameof(UserInfo) + nameof(HumanInfoRequest));
            }
            //判断是否新增，则走审核流程
            if (!string.IsNullOrEmpty(humanInfoRequest.Id))
            {
                var result = await GetHumanInfoAsync(user, humanInfoRequest.Id);
                if (result?.Extension != null && (result.Extension?.ExamineStatus != ExamineStatusEnum.UnSubmit || result.Extension?.ExamineStatus != ExamineStatusEnum.Reject) && !result.Extension.IsDeleted)
                {

                    var gatwayurl = ApplicationContext.Current.AppGatewayUrl.EndsWith("/") ? ApplicationContext.Current.AppGatewayUrl.TrimEnd('/') : ApplicationContext.Current.AppGatewayUrl;
                    GatewayInterface.Dto.ExamineSubmitRequest examineSubmitRequest = new GatewayInterface.Dto.ExamineSubmitRequest();
                    examineSubmitRequest.ContentId = !string.IsNullOrEmpty(humanInfoRequest.Id) ? humanInfoRequest.Id : "";
                    examineSubmitRequest.ContentType = "HumanInfo";
                    examineSubmitRequest.ContentName = humanInfoRequest.Name;
                    examineSubmitRequest.Content = "新增员工人事信息";
                    examineSubmitRequest.Source = user.FilialeName;
                    examineSubmitRequest.CallbackUrl = gatwayurl + "/api/humaninfo/humaninfocallback";
                    examineSubmitRequest.StepCallbackUrl = gatwayurl + "/api/humaninfo/shoponsitestepcallback";
                    examineSubmitRequest.Action = "HumanInfo";
                    examineSubmitRequest.TaskName = $"新增员工人事信息:{humanInfoRequest.Name}";
                    examineSubmitRequest.Desc = $"新增员工人事信息";

                    GatewayInterface.Dto.UserInfo userInfo = new GatewayInterface.Dto.UserInfo()
                    {
                        Id = user.Id,
                        KeyWord = user.KeyWord,
                        OrganizationId = user.OrganizationId,
                        OrganizationName = user.OrganizationName,
                        UserName = user.UserName
                    };
                    examineSubmitRequest.UserInfo = userInfo;

                    string tokenUrl = $"{ApplicationContext.Current.AuthUrl}/connect/token";
                    string examineCenterUrl = $"{ApplicationContext.Current.ExamineCenterUrl}";
                    Logger.Info($"新增员工人事信息提交审核，\r\ntokenUrl:{tokenUrl ?? ""},\r\nexamineCenterUrl:{examineCenterUrl ?? ""},\r\nexamineSubmitRequest:" + (examineSubmitRequest != null ? JsonHelper.ToJson(examineSubmitRequest) : ""));
                    var tokenManager = new TokenManager(tokenUrl, ApplicationContext.Current.ClientID, ApplicationContext.Current.ClientSecret);
                    var response2 = await tokenManager.Execute(async (token) =>
                    {
                        return await _restClient.PostWithToken<ResponseMessage>(examineCenterUrl, examineSubmitRequest, token);
                    });
                    if (response2.Code != ResponseCodeDefines.SuccessCode)
                    {
                        response.Code = ResponseCodeDefines.ServiceError;
                        response.Message = "向审核中心发起审核请求失败：" + response2.Message;
                        Logger.Info($"新增员工人事信息提交审核失败：" + response2.Message);
                        return response;
                    }
                }
            }
            response.Extension = _mapper.Map<HumanInfoResponse>(await Store.SaveAsync(user, _mapper.Map<HumanInfo>(humanInfoRequest), cancellationToken));
            return response;
        }

        /// <summary>
        /// 更新审核状态
        /// </summary>
        /// <param name="humanId"></param>
        /// <param name="status"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task UpdateExamineStatus(string humanId, ExamineStatusEnum status, CancellationToken cancellationToken = default(CancellationToken))
        {
            await Store.UpdateExamineStatus(humanId, status, cancellationToken);
        }




        public async Task<PagingResponseMessage<HumanInfoSearchResponse>> SearchHumanInfo(UserInfo user, HumanInfoSearchCondition condition, CancellationToken cancellationToken = default(CancellationToken))
        {
            PagingResponseMessage<HumanInfoSearchResponse> pagingResponse = new PagingResponseMessage<HumanInfoSearchResponse>();

            var q = Store.GetQuery().Where(a => !a.IsDeleted);

            if (!string.IsNullOrEmpty(condition?.KeyWord))
            {
                q = q.Where(a => a.Name.Contains(condition.KeyWord) || a.UserID.Contains(condition.KeyWord) || a.IDCard.Contains(condition.KeyWord));
            }
            if (condition?.StaffStatuses?.Count > 0)
            {
                q = q.Where(a => condition.StaffStatuses.Contains(a.StaffStatus));
            }
            if (condition?.BirthdayStart != null)
            {
                q = q.Where(a => condition.BirthdayStart.Value <= a.Birthday);
            }
            if (condition?.BirthdayEnd != null)
            {
                q = q.Where(a => a.Birthday <= condition.BirthdayStart.Value);
            }
            if (!string.IsNullOrEmpty(condition?.DepartmentId))
            {
                q = q.Where(a => a.DepartmentId == condition.DepartmentId);
            }
            pagingResponse.TotalCount = await q.CountAsync(cancellationToken);
            var qlist = await q.Skip(condition.PageIndex * condition.PageSize).Take(condition.PageSize).ToListAsync(cancellationToken);

            var resulte = qlist.Select(a => new HumanInfoSearchResponse
            {
                Id = a.Id,
                BaseWages = a.HumanSalaryStructure.BaseWages,
                BecomeTime = a.BecomeTime,
                CreateTime = a.CreateTime,
                CreateUser = a.CreateUser,
                Name = a.Name,
                DepartmentId = a.DepartmentId,
                OrganizationFullName = a.OrganizationExpansion.FullName,
                Desc = a.Desc,
                EntryTime = a.EntryTime,
                IDCard = a.IDCard,
                Position = a.Position,
                Sex = a.Sex,
                StaffStatus = a.StaffStatus,
                UserID = a.UserID,
                IsSignContracInfo = a.HumanContractInfo.ContractSignDate != null ? true : false,
                IsHaveSocialSecurity = a.HumanSocialSecurity.IsHave
            });
            pagingResponse.PageIndex = condition.PageIndex;
            pagingResponse.PageSize = condition.PageSize;
            pagingResponse.Extension = resulte.ToList();
            return pagingResponse;
        }
    }
}
