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
    public class HumanInfoBlackManager
    {
        public HumanInfoBlackManager(IHumanInfoBlackStore humanInfoBlackStore, IMapper mapper, RestClient restClient)
        {
            Store = humanInfoBlackStore;
            _mapper = mapper;
            _restClient = restClient;
        }

        protected IHumanInfoBlackStore Store { get; }
        protected IMapper _mapper { get; }
        protected RestClient _restClient { get; }

        private readonly ILogger Logger = LoggerManager.GetLogger("HumanInfoManager");


        public async Task<ResponseMessage<HumanInfoBlackResponse>> SaveAsync(UserInfo user, HumanInfoBlackRequest humanInfoBlackRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            ResponseMessage<HumanInfoBlackResponse> response = new ResponseMessage<HumanInfoBlackResponse>();
            if (user == null || humanInfoBlackRequest == null)
            {
                throw new ArgumentNullException(nameof(UserInfo) + nameof(HumanInfoRequest));
            }


            if (string.IsNullOrEmpty(humanInfoBlackRequest.Id))
            {
                humanInfoBlackRequest.Id = Guid.NewGuid().ToString();
            }
            var gatwayurl = ApplicationContext.Current.AppGatewayUrl.EndsWith("/") ? ApplicationContext.Current.AppGatewayUrl.TrimEnd('/') : ApplicationContext.Current.AppGatewayUrl;
            GatewayInterface.Dto.ExamineSubmitRequest examineSubmitRequest = new GatewayInterface.Dto.ExamineSubmitRequest();
            examineSubmitRequest.ContentId = humanInfoBlackRequest.Id;
            examineSubmitRequest.ContentType = "HumanBlack";
            examineSubmitRequest.ContentName = humanInfoBlackRequest.Name;
            examineSubmitRequest.Content = "新增员工人事黑名单信息";
            examineSubmitRequest.Source = user.FilialeName;
            examineSubmitRequest.SubmitDefineId = humanInfoBlackRequest.Id;
            examineSubmitRequest.CallbackUrl = gatwayurl + "/api/humaninfoblack/humanblackcallback";
            examineSubmitRequest.StepCallbackUrl = gatwayurl + "/api/humaninfoblack/humanblackstepcallback";
            examineSubmitRequest.Action = "HumanBlack";
            examineSubmitRequest.TaskName = $"新增员工人事黑名单信息:{humanInfoBlackRequest.Name}";
            examineSubmitRequest.Desc = $"新增员工人事黑名单信息";

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
            Logger.Info($"新增员工人事黑名单信息提交审核，\r\ntokenUrl:{tokenUrl ?? ""},\r\nexamineCenterUrl:{examineCenterUrl ?? ""},\r\nexamineSubmitRequest:" + (examineSubmitRequest != null ? JsonHelper.ToJson(examineSubmitRequest) : ""));
            var tokenManager = new TokenManager(tokenUrl, ApplicationContext.Current.ClientID, ApplicationContext.Current.ClientSecret);
            var response2 = await tokenManager.Execute(async (token) =>
            {
                return await _restClient.PostWithToken<ResponseMessage>(examineCenterUrl, examineSubmitRequest, token);
            });
            if (response2.Code != ResponseCodeDefines.SuccessCode)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = "向审核中心发起审核请求失败：" + response2.Message;
                Logger.Info($"新增员工人事黑名单信息提交审核失败：" + response2.Message);
                return response;
            }


            response.Extension = _mapper.Map<HumanInfoBlackResponse>(await Store.SaveAsync(user, _mapper.Map<HumanInfoBlack>(humanInfoBlackRequest), cancellationToken));
            return response;
        }

        public async Task<PagingResponseMessage<HumanInfoBlackResponse>> SearchAsync(HumanInfoBlackSearchCondition condition, CancellationToken cancellationToken = default(CancellationToken))
        {
            PagingResponseMessage<HumanInfoBlackResponse> pagingResponse = new PagingResponseMessage<HumanInfoBlackResponse>();

            var q = Store.GetQuery().Where(a => !a.IsDeleted);

            if (!string.IsNullOrEmpty(condition?.KeyWord))
            {
                q = q.Where(a => a.Name.Contains(condition.KeyWord) || a.Phone.Contains(condition.KeyWord) || a.Reason.Contains(condition.KeyWord) || a.IDCard.Contains(condition.KeyWord));
            }
            pagingResponse.TotalCount = await q.CountAsync(cancellationToken);
            var qlist = await q.Skip(condition.PageIndex * condition.PageSize).Take(condition.PageSize).ToListAsync(cancellationToken);

            var resulte = qlist.Select(a => new HumanInfoBlackResponse
            {
                Id = a.Id,
                CreateTime = a.CreateTime,
                Email = a.Email,
                IDCard = a.IDCard,
                Name = a.Name,
                Phone = a.Phone,
                Reason = a.Reason,
                Sex = a.Sex,
                UserId = a.UserId
            });
            pagingResponse.PageIndex = condition.PageIndex;
            pagingResponse.PageSize = condition.PageSize;
            pagingResponse.Extension = resulte.ToList();
            return pagingResponse;
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

        public async Task DeleteAsync(UserInfo user, string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }
            var humanInfoBlack = await Store.GetAsync(a => a.Where(b => b.Id == id));
            if (humanInfoBlack == null)
            {
                throw new Exception("删除的黑名单用户不存在");
            }
            await Store.DeleteAsync(user, humanInfoBlack, cancellationToken);
        }

    }
}
