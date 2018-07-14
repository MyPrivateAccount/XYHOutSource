using ApplicationCore;
using ApplicationCore.Dto;
using ApplicationCore.Managers;
using AutoMapper;
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
    public class HumanPositionManager
    {
        public HumanPositionManager(IHumanPositionStore humanPositionStore,
            PermissionExpansionManager permissionExpansionManager,
            IMapper mapper, RestClient restClient)
        {
            Store = humanPositionStore;
            _permissionExpansionManager = permissionExpansionManager;
            _restClient = restClient;
            _mapper = mapper;
        }

        protected IHumanPositionStore Store { get; }
        protected PermissionExpansionManager _permissionExpansionManager;
        protected IMapper _mapper { get; }
        protected RestClient _restClient { get; }
        private readonly ILogger Logger = LoggerManager.GetLogger("HumanPositionManager");


        public async Task<ResponseMessage<HumanPositionResponse>> FindByIdAsync(UserInfo user, string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            ResponseMessage<HumanPositionResponse> response = new ResponseMessage<HumanPositionResponse>();
            var humanPosition = await Store.GetAsync(a => a.Where(b => b.Id == id));
            if (humanPosition == null)
            {
                response.Code = ResponseCodeDefines.NotFound;
                response.Message = "没有找到";
                return response;
            }
            var org = await _permissionExpansionManager.GetOrganizationOfPermission(user.Id, "HumanPosition");
            if (org == null || org.Count == 0 || !org.Contains(humanPosition.DepartmentId))
            {
                response.Code = ResponseCodeDefines.NotAllow;
                response.Message = "没有权限";
                return response;
            }
            response.Extension = _mapper.Map<HumanPositionResponse>(humanPosition);
            return response;
        }


        public async Task<ResponseMessage<HumanPositionResponse>> CreateAsync(UserInfo user, HumanPositionRequest humanPositionRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            ResponseMessage<HumanPositionResponse> response = new ResponseMessage<HumanPositionResponse>();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (humanPositionRequest == null)
            {
                throw new ArgumentNullException(nameof(humanPositionRequest));
            }
            var org = await _permissionExpansionManager.GetOrganizationOfPermission(user.Id, "HumanPosition");
            if (org == null || org.Count == 0 || !org.Contains(humanPositionRequest.DepartmentId))
            {
                response.Code = ResponseCodeDefines.NotAllow;
                response.Message = "没有权限";
                return response;
            }
            if (string.IsNullOrEmpty(humanPositionRequest.Id))
            {
                humanPositionRequest.Id = Guid.NewGuid().ToString();
            }
            var gatwayurl = ApplicationContext.Current.AppGatewayUrl.EndsWith("/") ? ApplicationContext.Current.AppGatewayUrl.TrimEnd('/') : ApplicationContext.Current.AppGatewayUrl;
            GatewayInterface.Dto.ExamineSubmitRequest examineSubmitRequest = new GatewayInterface.Dto.ExamineSubmitRequest();
            examineSubmitRequest.ContentId = humanPositionRequest.Id;
            examineSubmitRequest.ContentType = "HumanPosition";
            examineSubmitRequest.ContentName = humanPositionRequest.Name;
            examineSubmitRequest.Content = "新增职位信息";
            examineSubmitRequest.Source = user.FilialeName;
            examineSubmitRequest.SubmitDefineId = humanPositionRequest.Id;
            examineSubmitRequest.CallbackUrl = gatwayurl + "/api/humanposition/humanpositioncallback";
            examineSubmitRequest.StepCallbackUrl = gatwayurl + "/api/humanposition/humanpositionstepcallback";
            examineSubmitRequest.Action = "HumanPosition";
            examineSubmitRequest.TaskName = $"新增职位信息:{humanPositionRequest.Name}";
            examineSubmitRequest.Desc = $"新增职位信息";

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
            Logger.Info($"新增员工人事调动信息提交审核，\r\ntokenUrl:{tokenUrl ?? ""},\r\nexamineCenterUrl:{examineCenterUrl ?? ""},\r\nexamineSubmitRequest:" + (examineSubmitRequest != null ? JsonHelper.ToJson(examineSubmitRequest) : ""));
            var tokenManager = new TokenManager(tokenUrl, ApplicationContext.Current.ClientID, ApplicationContext.Current.ClientSecret);
            var response2 = await tokenManager.Execute(async (token) =>
            {
                return await _restClient.PostWithToken<ResponseMessage>(examineCenterUrl, examineSubmitRequest, token);
            });
            if (response2.Code != ResponseCodeDefines.SuccessCode)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = "向审核中心发起审核请求失败：" + response2.Message;
                Logger.Info($"新增职位信息提交审核失败：" + response2.Message);
                return response;
            }

            response.Extension = _mapper.Map<HumanPositionResponse>(await Store.CreateAsync(user, _mapper.Map<HumanPosition>(humanPositionRequest), cancellationToken));
            return response;
        }


        public async Task<ResponseMessage<HumanPositionResponse>> UpdateAsync(UserInfo user, string id, HumanPositionRequest humanPositionRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            ResponseMessage<HumanPositionResponse> response = new ResponseMessage<HumanPositionResponse>();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (humanPositionRequest == null)
            {
                throw new ArgumentNullException(nameof(humanPositionRequest));
            }
            var org = await _permissionExpansionManager.GetOrganizationOfPermission(user.Id, "HumanPosition");
            if (org == null || org.Count == 0 || !org.Contains(humanPositionRequest.DepartmentId))
            {
                response.Code = ResponseCodeDefines.NotAllow;
                response.Message = "没有权限";
                return response;
            }
            var humanPosition = _mapper.Map<HumanPosition>(humanPositionRequest);
            humanPosition.Id = id;
            response.Extension = _mapper.Map<HumanPositionResponse>(await Store.UpdateAsync(user, humanPosition, cancellationToken));

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

        public async Task<ResponseMessage> DeleteAsync(UserInfo user, string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            ResponseMessage response = new ResponseMessage();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }
            var old = await Store.GetAsync(a => a.Where(b => b.Id == id));
            if (old == null)
            {
                throw new Exception("删除的对象不存在");
            }
            var org = await _permissionExpansionManager.GetOrganizationOfPermission(user.Id, "HumanPosition");
            if (org == null || org.Count == 0 || !org.Contains(old.DepartmentId))
            {
                response.Code = ResponseCodeDefines.NotAllow;
                response.Message = "没有权限";
                return response;
            }
            await Store.DeleteAsync(user, old, cancellationToken);
            return response;
        }
    }
}
