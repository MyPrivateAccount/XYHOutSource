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
    public class HumanInfoRegularManager
    {
        public HumanInfoRegularManager(IHumanInfoRegularStore humanInfoRegularStore,
                                    PermissionExpansionManager permissionExpansionManager,
                                    IHumanInfoStore humanInfoStore,
                                    IHumanInfoChangeStore humanInfoChangeStore,
                                    IMapper mapper, RestClient restClient)
        {
            Store = humanInfoRegularStore;
            _permissionExpansionManager = permissionExpansionManager;
            _humanInfoStore = humanInfoStore;
            _restClient = restClient;
            _humanInfoChangeStore = humanInfoChangeStore;
            _mapper = mapper;
        }

        protected IHumanInfoRegularStore Store { get; }
        protected PermissionExpansionManager _permissionExpansionManager;
        protected IHumanInfoStore _humanInfoStore;
        protected IMapper _mapper { get; }
        protected IHumanInfoChangeStore _humanInfoChangeStore { get; }

        protected RestClient _restClient { get; }
        private readonly ILogger Logger = LoggerManager.GetLogger("HumanInfoRegularManager");


        public async Task<ResponseMessage<HumanInfoRegularResponse>> FindByIdAsync(UserInfo user, string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            ResponseMessage<HumanInfoRegularResponse> response = new ResponseMessage<HumanInfoRegularResponse>();
            var humanInfoRegular = await Store.SimpleQuery().SingleOrDefaultAsync(a => a.Id == id, cancellationToken);
            if (humanInfoRegular == null)
            {
                response.Code = ResponseCodeDefines.NotFound;
                response.Message = "没有找到";
                return response;
            }
            var org = await _permissionExpansionManager.GetOrganizationOfPermission(user.Id, "HumanRegular");
            if (org == null || org.Count == 0 || !org.Contains(humanInfoRegular.OrganizationId))
            {
                response.Code = ResponseCodeDefines.NotAllow;
                response.Message = "没有权限";
                return response;
            }
            response.Extension = _mapper.Map<HumanInfoRegularResponse>(humanInfoRegular);
            return response;
        }


        public async Task<ResponseMessage<HumanInfoRegularResponse>> CreateAsync(UserInfo user, HumanInfoRegularRequest humanInfoRegularRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            ResponseMessage<HumanInfoRegularResponse> response = new ResponseMessage<HumanInfoRegularResponse>();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (humanInfoRegularRequest == null)
            {
                throw new ArgumentNullException(nameof(humanInfoRegularRequest));
            }
            var humaninfo = await _humanInfoStore.GetAsync(a => a.Where(b => b.Id == humanInfoRegularRequest.HumanId && !b.IsDeleted), cancellationToken);
            if (humaninfo == null)
            {
                response.Code = ResponseCodeDefines.NotFound;
                response.Message = "操作的人事信息未找到";
                return response;
            }
            var org = await _permissionExpansionManager.GetOrganizationOfPermission(user.Id, "HumanRegular");
            if (org == null || org.Count == 0 || !org.Contains(humaninfo.DepartmentId))
            {
                response.Code = ResponseCodeDefines.NotAllow;
                response.Message = "没有权限";
                return response;
            }
            if (string.IsNullOrEmpty(humanInfoRegularRequest.Id))
            {
                humanInfoRegularRequest.Id = Guid.NewGuid().ToString();
            }
            var gatwayurl = ApplicationContext.Current.AppGatewayUrl.EndsWith("/") ? ApplicationContext.Current.AppGatewayUrl.TrimEnd('/') : ApplicationContext.Current.AppGatewayUrl;
            GatewayInterface.Dto.ExamineSubmitRequest examineSubmitRequest = new GatewayInterface.Dto.ExamineSubmitRequest();
            examineSubmitRequest.ContentId = humanInfoRegularRequest.Id;
            examineSubmitRequest.ContentType = "HumanRegular";
            examineSubmitRequest.ContentName = humaninfo.Name;
            examineSubmitRequest.Content = "新增员工人事转正信息";
            examineSubmitRequest.Source = user.FilialeName;
            examineSubmitRequest.SubmitDefineId = humanInfoRegularRequest.Id;
            examineSubmitRequest.CallbackUrl = gatwayurl + "/api/humanregular/humanregularcallback";
            examineSubmitRequest.StepCallbackUrl = gatwayurl + "/api/humanregular/humanregularstepcallback";
            examineSubmitRequest.Action = "HumanRegular";
            examineSubmitRequest.TaskName = $"新增员工人事转正信息:{humaninfo.Name}";
            examineSubmitRequest.Desc = $"新增员工人事转正信息";

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
            Logger.Info($"新增员工人事转正信息提交审核，\r\ntokenUrl:{tokenUrl ?? ""},\r\nexamineCenterUrl:{examineCenterUrl ?? ""},\r\nexamineSubmitRequest:" + (examineSubmitRequest != null ? JsonHelper.ToJson(examineSubmitRequest) : ""));
            var tokenManager = new TokenManager(tokenUrl, ApplicationContext.Current.ClientID, ApplicationContext.Current.ClientSecret);
            var response2 = await tokenManager.Execute(async (token) =>
            {
                return await _restClient.PostWithToken<ResponseMessage>(examineCenterUrl, examineSubmitRequest, token);
            });
            if (response2.Code != ResponseCodeDefines.SuccessCode)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = "向审核中心发起审核请求失败：" + response2.Message;
                Logger.Info($"新增员工人事转正信息提交审核失败：" + response2.Message);
                return response;
            }

            response.Extension = _mapper.Map<HumanInfoRegularResponse>(await Store.CreateAsync(user, _mapper.Map<HumanInfoRegular>(humanInfoRegularRequest), cancellationToken));
            return response;
        }

        /// <summary>
        /// 更新审核状态
        /// </summary>
        /// <param name="humanId"></param>
        /// <param name="status"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task UpdateExamineStatus(string id, ExamineStatusEnum status, CancellationToken cancellationToken = default(CancellationToken))
        {
            var model = await Store.GetAsync(a => a.Where(b => b.Id == id));
            if (model == null)
            {
                throw new Exception("未找到更新对象");
            }
            var human = await _humanInfoStore.GetAsync(a => a.Where(b => b.Id == model.HumanId));
            if (human == null)
            {
                throw new Exception("未找到操作的人事信息");
            }

            UserInfo userInfo = new UserInfo
            {
                Id = model.CreateUser
            };
            HumanInfoChange humanInfoChange = new HumanInfoChange
            {
                ChangeContent = "",
                ChangeId = model.Id,
                ChangeReason = "",
                ChangeTime = DateTime.Now,
                ChangeType = HumanChangeType.Regular,
                CreateTime = DateTime.Now,
                CreateUser = model.CreateUser,
                Id = Guid.NewGuid().ToString(),
                IsDeleted = false,
                HumanId = model.HumanId,
                UserId = model.CreateUser
            };
            human.StaffStatus = StaffStatus.Regular;

            await _humanInfoStore.UpdateAsync(userInfo, human);
            await _humanInfoChangeStore.CreateAsync(userInfo, humanInfoChange);
            await Store.UpdateExamineStatus(id, status, cancellationToken);
        }


        public async Task<ResponseMessage<HumanInfoRegularResponse>> UpdateAsync(UserInfo user, string id, HumanInfoRegularRequest humanInfoRegularRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            ResponseMessage<HumanInfoRegularResponse> response = new ResponseMessage<HumanInfoRegularResponse>();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (humanInfoRegularRequest == null)
            {
                throw new ArgumentNullException(nameof(humanInfoRegularRequest));
            }
            var humaninfo = await _humanInfoStore.GetAsync(a => a.Where(b => b.Id == humanInfoRegularRequest.HumanId && !b.IsDeleted), cancellationToken);
            if (humaninfo == null)
            {
                response.Code = ResponseCodeDefines.NotFound;
                response.Message = "操作的人事信息未找到";
                return response;
            }
            var org = await _permissionExpansionManager.GetOrganizationOfPermission(user.Id, "HumanRegular");
            if (org == null || org.Count == 0 || !org.Contains(humaninfo.DepartmentId))
            {
                response.Code = ResponseCodeDefines.NotAllow;
                response.Message = "没有权限";
                return response;
            }
            var humanInfoRegular = _mapper.Map<HumanInfoRegular>(humanInfoRegularRequest);
            humanInfoRegular.Id = id;
            response.Extension = _mapper.Map<HumanInfoRegularResponse>(await Store.UpdateAsync(user, humanInfoRegular, cancellationToken));
            return response;
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
            var org = await _permissionExpansionManager.GetOrganizationOfPermission(user.Id, "HumanRegular");
            if (org == null || org.Count == 0 || !org.Contains(old.OrganizationId))
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
