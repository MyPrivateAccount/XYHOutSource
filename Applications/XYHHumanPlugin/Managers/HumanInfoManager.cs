using ApplicationCore;
using ApplicationCore.Dto;
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
        public HumanInfoManager(IHumanInfoStore humanInfoStore, IMapper mapper, RestClient restClient)
        {
            Store = humanInfoStore;
            _mapper = mapper;
            _restClient = restClient;
        }

        protected IHumanInfoStore Store { get; }
        protected IMapper _mapper { get; }
        protected RestClient _restClient { get; }

        private readonly ILogger Logger = LoggerManager.GetLogger("HumanInfoManager");


        public async Task<ResponseMessage<HumanInfoResponse>> SaveHumanInfo(UserInfo user, HumanInfoRequest humanInfoRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            ResponseMessage<HumanInfoResponse> response = new ResponseMessage<HumanInfoResponse>();
            if (user == null || humanInfoRequest == null)
            {
                throw new ArgumentNullException(nameof(UserInfo) + nameof(HumanInfoRequest));
            }
            response.Extension = _mapper.Map<HumanInfoResponse>(await Store.SaveAsync(user, _mapper.Map<HumanInfo>(humanInfoRequest), cancellationToken));
            return response;
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
                IsHaveSocialSecurity = a.HumanSocialSecurity.IsGiveUp ? false : true
            });
            pagingResponse.PageIndex = condition.PageIndex;
            pagingResponse.PageSize = condition.PageSize;
            pagingResponse.Extension = resulte.ToList();
            return pagingResponse;
        }
    }
}
