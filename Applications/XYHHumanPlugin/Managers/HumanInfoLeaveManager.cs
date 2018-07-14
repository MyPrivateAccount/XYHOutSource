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
    public class HumanInfoLeaveManager
    {
        public HumanInfoLeaveManager(IHumanInfoLeaveStore humanInfoLeaveStore,
            IHumanInfoStore humanInfoStore,
            IHumanInfoChangeStore humanInfoChangeStore,
            PermissionExpansionManager permissionExpansionManager,
            IMapper mapper, RestClient restClient)
        {
            Store = humanInfoLeaveStore;
            _permissionExpansionManager = permissionExpansionManager;
            _humanInfoStore = humanInfoStore;
            _restClient = restClient;
            _humanInfoChangeStore = humanInfoChangeStore;
            _mapper = mapper;
        }

        protected IHumanInfoLeaveStore Store { get; }
        protected PermissionExpansionManager _permissionExpansionManager;
        protected IHumanInfoStore _humanInfoStore;
        protected IMapper _mapper { get; }
        protected RestClient _restClient { get; }
        protected IHumanInfoChangeStore _humanInfoChangeStore { get; }

        private readonly ILogger Logger = LoggerManager.GetLogger("HumanInfoLeaveManager");


        public async Task<ResponseMessage<HumanInfoLeaveResponse>> FindByIdAsync(UserInfo user, string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            ResponseMessage<HumanInfoLeaveResponse> response = new ResponseMessage<HumanInfoLeaveResponse>();
            var humanInfoLeave = await Store.SimpleQuery().SingleOrDefaultAsync(a => a.Id == id, cancellationToken);
            if (humanInfoLeave == null)
            {
                response.Code = ResponseCodeDefines.NotFound;
                response.Message = "没有找到";
                return response;
            }
            var org = await _permissionExpansionManager.GetOrganizationOfPermission(user.Id, "HumanLeave");
            if (org == null || org.Count == 0 || !org.Contains(humanInfoLeave.OrganizationId))
            {
                response.Code = ResponseCodeDefines.NotAllow;
                response.Message = "没有权限";
                return response;
            }
            response.Extension = _mapper.Map<HumanInfoLeaveResponse>(humanInfoLeave);
            return response;
        }


        public async Task<ResponseMessage<HumanInfoLeaveResponse>> CreateAsync(UserInfo user, HumanInfoLeaveRequest humanInfoLeaveRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            ResponseMessage<HumanInfoLeaveResponse> response = new ResponseMessage<HumanInfoLeaveResponse>();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (humanInfoLeaveRequest == null)
            {
                throw new ArgumentNullException(nameof(humanInfoLeaveRequest));
            }
            var humaninfo = await _humanInfoStore.GetAsync(a => a.Where(b => b.Id == humanInfoLeaveRequest.HumanId && !b.IsDeleted), cancellationToken);
            if (humaninfo == null)
            {
                response.Code = ResponseCodeDefines.NotFound;
                response.Message = "操作的人事信息未找到";
                return response;
            }
            var org = await _permissionExpansionManager.GetOrganizationOfPermission(user.Id, "HumanLeave");
            if (org == null || org.Count == 0 || !org.Contains(humaninfo.DepartmentId))
            {
                response.Code = ResponseCodeDefines.NotAllow;
                response.Message = "没有权限";
                return response;
            }
            if (string.IsNullOrEmpty(humanInfoLeaveRequest.Id))
            {
                humanInfoLeaveRequest.Id = Guid.NewGuid().ToString();
            }
            var gatwayurl = ApplicationContext.Current.AppGatewayUrl.EndsWith("/") ? ApplicationContext.Current.AppGatewayUrl.TrimEnd('/') : ApplicationContext.Current.AppGatewayUrl;
            GatewayInterface.Dto.ExamineSubmitRequest examineSubmitRequest = new GatewayInterface.Dto.ExamineSubmitRequest();
            examineSubmitRequest.ContentId = humanInfoLeaveRequest.Id;
            examineSubmitRequest.ContentType = "HumanLeave";
            examineSubmitRequest.ContentName = humaninfo.Name;
            examineSubmitRequest.Content = "新增员工人事离职信息";
            examineSubmitRequest.Source = user.FilialeName;
            examineSubmitRequest.SubmitDefineId = humanInfoLeaveRequest.Id;
            examineSubmitRequest.CallbackUrl = gatwayurl + "/api/humanleave/humanleavecallback";
            examineSubmitRequest.StepCallbackUrl = gatwayurl + "/api/humanleave/humanleavestepcallback";
            examineSubmitRequest.Action = "HumanLeave";
            examineSubmitRequest.TaskName = $"新增员工人事离职信息:{humaninfo.Name}";
            examineSubmitRequest.Desc = $"新增员工人事离职调动信息";

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
            Logger.Info($"新增员工人事离职信息提交审核，\r\ntokenUrl:{tokenUrl ?? ""},\r\nexamineCenterUrl:{examineCenterUrl ?? ""},\r\nexamineSubmitRequest:" + (examineSubmitRequest != null ? JsonHelper.ToJson(examineSubmitRequest) : ""));
            var tokenManager = new TokenManager(tokenUrl, ApplicationContext.Current.ClientID, ApplicationContext.Current.ClientSecret);
            var response2 = await tokenManager.Execute(async (token) =>
            {
                return await _restClient.PostWithToken<ResponseMessage>(examineCenterUrl, examineSubmitRequest, token);
            });
            if (response2.Code != ResponseCodeDefines.SuccessCode)
            {
                response.Code = ResponseCodeDefines.ServiceError;
                response.Message = "向审核中心发起审核请求失败：" + response2.Message;
                Logger.Info($"新增员工人事离职信息提交审核失败：" + response2.Message);
                return response;
            }
            response.Extension = _mapper.Map<HumanInfoLeaveResponse>(await Store.CreateAsync(user, _mapper.Map<HumanInfoLeave>(humanInfoLeaveRequest), cancellationToken));
            return response;
        }


        public async Task<ResponseMessage<HumanInfoLeaveResponse>> UpdateAsync(UserInfo user, string id, HumanInfoLeaveRequest humanInfoLeaveRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            ResponseMessage<HumanInfoLeaveResponse> response = new ResponseMessage<HumanInfoLeaveResponse>();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (humanInfoLeaveRequest == null)
            {
                throw new ArgumentNullException(nameof(humanInfoLeaveRequest));
            }
            var humaninfo = await _humanInfoStore.GetAsync(a => a.Where(b => b.Id == humanInfoLeaveRequest.HumanId && !b.IsDeleted), cancellationToken);
            if (humaninfo == null)
            {
                response.Code = ResponseCodeDefines.NotFound;
                response.Message = "操作的人事信息未找到";
                return response;
            }
            var org = await _permissionExpansionManager.GetOrganizationOfPermission(user.Id, "HumanLeave");
            if (org == null || org.Count == 0 || !org.Contains(humaninfo.DepartmentId))
            {
                response.Code = ResponseCodeDefines.NotAllow;
                response.Message = "没有权限";
                return response;
            }
            var humanInfoLeave = _mapper.Map<HumanInfoLeave>(humanInfoLeaveRequest);
            humanInfoLeave.Id = id;
            response.Extension = _mapper.Map<HumanInfoLeaveResponse>(await Store.UpdateAsync(user, humanInfoLeave, cancellationToken));

            return response;
        }

        /// <summary>
        /// 更新审核状态
        /// </summary>
        /// <param name="id"></param>
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
                ChangeType = HumanChangeType.Adjustment,
                CreateTime = DateTime.Now,
                CreateUser = model.CreateUser,
                Id = Guid.NewGuid().ToString(),
                IsDeleted = false,
                HumanId = model.HumanId,
                UserId = model.CreateUser
            };
            human.StaffStatus = StaffStatus.Leave;

            await _humanInfoStore.UpdateAsync(userInfo, human);
            await _humanInfoChangeStore.CreateAsync(userInfo, humanInfoChange);
            await Store.UpdateExamineStatus(id, status, cancellationToken);
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
            var humaninfo = await _humanInfoStore.GetAsync(a => a.Where(b => b.Id == old.HumanId && !b.IsDeleted), cancellationToken);
            if (humaninfo == null)
            {
                response.Code = ResponseCodeDefines.NotFound;
                response.Message = "操作的人事信息未找到";
                return response;
            }
            var org = await _permissionExpansionManager.GetOrganizationOfPermission(user.Id, "HumanLeave");
            if (org == null || org.Count == 0 || !org.Contains(humaninfo.DepartmentId))
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
