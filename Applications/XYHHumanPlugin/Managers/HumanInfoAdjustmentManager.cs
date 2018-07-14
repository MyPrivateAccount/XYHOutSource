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
    public class HumanInfoAdjustmentManager
    {
        public HumanInfoAdjustmentManager(IHumanInfoAdjustmentStore humanInfoAdjustmentStore,
            IHumanInfoStore humanInfoStore,
            IHumanInfoChangeStore humanInfoChangeStore,
            PermissionExpansionManager permissionExpansionManager,
            IMapper mapper, RestClient restClient)
        {
            Store = humanInfoAdjustmentStore;
            _permissionExpansionManager = permissionExpansionManager;
            _humanInfoStore = humanInfoStore;
            _restClient = restClient;
            _humanInfoChangeStore = humanInfoChangeStore;
            _mapper = mapper;
        }

        protected IHumanInfoAdjustmentStore Store { get; }
        protected PermissionExpansionManager _permissionExpansionManager;
        protected IHumanInfoStore _humanInfoStore;
        protected IMapper _mapper { get; }
        protected RestClient _restClient { get; }
        protected IHumanInfoChangeStore _humanInfoChangeStore { get; }
        private readonly ILogger Logger = LoggerManager.GetLogger("HumanInfoAdjustmentManager");


        public async Task<ResponseMessage<HumanInfoAdjustmentResponse>> FindByIdAsync(UserInfo user, string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            ResponseMessage<HumanInfoAdjustmentResponse> response = new ResponseMessage<HumanInfoAdjustmentResponse>();
            var humanInfoAdjustment = await Store.SimpleQuery().SingleOrDefaultAsync(a => a.Id == id, cancellationToken);
            if (humanInfoAdjustment == null)
            {
                response.Code = ResponseCodeDefines.NotFound;
                response.Message = "没有找到";
                return response;
            }
            var org = await _permissionExpansionManager.GetOrganizationOfPermission(user.Id, "HumanAdjustment");
            if (org == null || org.Count == 0 || !org.Contains(humanInfoAdjustment.DepartmentId) || !org.Contains(humanInfoAdjustment.OrganizationId))
            {
                response.Code = ResponseCodeDefines.NotAllow;
                response.Message = "没有权限";
                return response;
            }
            response.Extension = _mapper.Map<HumanInfoAdjustmentResponse>(humanInfoAdjustment);
            return response;
        }


        public async Task<ResponseMessage<HumanInfoAdjustmentResponse>> CreateAsync(UserInfo user, HumanInfoAdjustmentRequest humanInfoAdjustmentRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            ResponseMessage<HumanInfoAdjustmentResponse> response = new ResponseMessage<HumanInfoAdjustmentResponse>();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (humanInfoAdjustmentRequest == null)
            {
                throw new ArgumentNullException(nameof(humanInfoAdjustmentRequest));
            }
            var humaninfo = await _humanInfoStore.GetAsync(a => a.Where(b => b.Id == humanInfoAdjustmentRequest.HumanId && !b.IsDeleted), cancellationToken);
            if (humaninfo == null)
            {
                response.Code = ResponseCodeDefines.NotFound;
                response.Message = "操作的人事信息未找到";
                return response;
            }
            var org = await _permissionExpansionManager.GetOrganizationOfPermission(user.Id, "HumanAdjustment");
            if (org == null || org.Count == 0 || !org.Contains(humanInfoAdjustmentRequest.DepartmentId) || !org.Contains(humaninfo.DepartmentId))
            {
                response.Code = ResponseCodeDefines.NotAllow;
                response.Message = "没有权限";
                return response;
            }

            if (string.IsNullOrEmpty(humanInfoAdjustmentRequest.Id))
            {
                humanInfoAdjustmentRequest.Id = Guid.NewGuid().ToString();
            }
            var gatwayurl = ApplicationContext.Current.AppGatewayUrl.EndsWith("/") ? ApplicationContext.Current.AppGatewayUrl.TrimEnd('/') : ApplicationContext.Current.AppGatewayUrl;
            GatewayInterface.Dto.ExamineSubmitRequest examineSubmitRequest = new GatewayInterface.Dto.ExamineSubmitRequest();
            examineSubmitRequest.ContentId = humanInfoAdjustmentRequest.Id;
            examineSubmitRequest.ContentType = "HumanAdjustment";
            examineSubmitRequest.ContentName = humaninfo.Name;
            examineSubmitRequest.Content = "新增员工人事调动信息";
            examineSubmitRequest.Source = user.FilialeName;
            examineSubmitRequest.SubmitDefineId = humanInfoAdjustmentRequest.Id;
            examineSubmitRequest.CallbackUrl = gatwayurl + "/api/humanadjustment/humanadjustmentcallback";
            examineSubmitRequest.StepCallbackUrl = gatwayurl + "/api/humanadjustment/humanadjustmentstepcallback";
            examineSubmitRequest.Action = "HumanAdjustment";
            examineSubmitRequest.TaskName = $"新增员工人事调动信息:{humaninfo.Name}";
            examineSubmitRequest.Desc = $"新增员工人事调动信息";

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
                Logger.Info($"新增员工人事调动信息提交审核失败：" + response2.Message);
                return response;
            }


            response.Extension = _mapper.Map<HumanInfoAdjustmentResponse>(await Store.CreateAsync(user, _mapper.Map<HumanInfoAdjustment>(humanInfoAdjustmentRequest), cancellationToken));
            return response;
        }


        public async Task<ResponseMessage<HumanInfoAdjustmentResponse>> UpdateAsync(UserInfo user, string id, HumanInfoAdjustmentRequest humanInfoAdjustmentRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            ResponseMessage<HumanInfoAdjustmentResponse> response = new ResponseMessage<HumanInfoAdjustmentResponse>();
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (humanInfoAdjustmentRequest == null)
            {
                throw new ArgumentNullException(nameof(humanInfoAdjustmentRequest));
            }
            var humaninfo = await _humanInfoStore.GetAsync(a => a.Where(b => b.Id == humanInfoAdjustmentRequest.HumanId && !b.IsDeleted), cancellationToken);
            if (humaninfo == null)
            {
                response.Code = ResponseCodeDefines.NotFound;
                response.Message = "操作的人事信息未找到";
                return response;
            }
            var org = await _permissionExpansionManager.GetOrganizationOfPermission(user.Id, "HumanAdjustment");
            if (org == null || org.Count == 0 || !org.Contains(humanInfoAdjustmentRequest.DepartmentId) || !org.Contains(humaninfo.DepartmentId))
            {
                response.Code = ResponseCodeDefines.NotAllow;
                response.Message = "没有权限";
                return response;
            }
            var humanInfoAdjustment = _mapper.Map<HumanInfoAdjustment>(humanInfoAdjustmentRequest);
            humanInfoAdjustment.Id = id;
            response.Extension = _mapper.Map<HumanInfoAdjustmentResponse>(await Store.UpdateAsync(user, humanInfoAdjustment, cancellationToken));
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
            UserInfo userInfo = new UserInfo
            {
                Id = model.CreateUser
            };
            var human = await _humanInfoStore.GetAsync(a => a.Where(b => b.Id == model.HumanId));
            if (human == null)
            {
                throw new Exception("未找到操作的人事信息");
            }
            human.Position = model.Position;
            human.DepartmentId = model.DepartmentId;
            human.UpdateTime = DateTime.Now;
            human.UpdateUser = model.CreateUser;

            HumanSocialSecurity humanSocialSecurity = new HumanSocialSecurity
            {
                EmploymentInjuryInsurance = model.EmploymentInjuryInsurance,
                HousingProvidentFundAccount = model.HousingProvidentFundAccount,
                InsuredAddress = model.InsuredAddress,
                MedicalInsuranceAccount = model.MedicalInsuranceAccount,
                SocialSecurityAccount = model.SocialSecurityAccount,
                EndowmentInsurance = model.EndowmentInsurance,
                HousingProvidentFund = model.HousingProvidentFund,
                Id = model.HumanId,
                InsuredTime = model.InsuredTime,
                IsGiveUp = model.IsGiveUp,
                IsHave = model.IsHave,
                IsSignCommitment = model.IsSignCommitment,
                MaternityInsurance = model.MaternityInsurance,
                MedicalInsurance = model.MedicalInsurance,
                UnemploymentInsurance = model.UnemploymentInsurance
            };


            HumanSalaryStructure humanSalaryStructure = new HumanSalaryStructure
            {
                BaseWages = model.BaseWages,
                CommunicationAllowance = model.CommunicationAllowance,
                OtherAllowance = model.OtherAllowance,
                TrafficAllowance = model.TrafficAllowance,
                GrossPay = model.GrossPay,
                Id = model.HumanId,
                PostWages = model.PostWages,
                ProbationaryPay = model.ProbationaryPay
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

            await _humanInfoStore.UpdateAsync(userInfo, human);
            await _humanInfoStore.UpdateHumanSalaryStructureAsync(userInfo, humanSalaryStructure);
            await _humanInfoStore.UpdateHumanSocialSecurityAsync(userInfo, humanSocialSecurity);
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
            var old = await Store.SimpleQuery().Where(a => a.Id == id).SingleOrDefaultAsync(cancellationToken);
            if (old == null)
            {
                response.Code = ResponseCodeDefines.NotFound;
                response.Message = "删除的对象不存在";
                return response;
            }
            var org = await _permissionExpansionManager.GetOrganizationOfPermission(user.Id, "HumanAdjustment");
            if (org == null || org.Count == 0 || !org.Contains(old.DepartmentId) || !org.Contains(old.OrganizationId))
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
