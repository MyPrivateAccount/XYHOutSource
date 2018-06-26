using ApplicationCore.Dto;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XYHHumanPlugin.Dto.Request;
using XYHHumanPlugin.Dto.Response;
using XYHHumanPlugin.Models;
using XYHHumanPlugin.Stores;
using RewardPunishmentRequest = XYHHumanPlugin.Dto.Response.RewardPunishmentResponse;

namespace XYHHumanPlugin.Managers
{
    public class RewardPunishmentManager
    {
        public RewardPunishmentManager(IHumanManageStore stor, IMapper mapper)
        {
            _Store = stor;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        protected IHumanManageStore _Store { get; }
        protected IMapper _mapper { get; }

        public virtual async Task AddRPInfo(RewardPunishmentRequest itm, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (itm == null)
            {
                throw new ArgumentNullException(nameof(itm));
            }

            //await _Store.AddRPInfoeAsync(_mapper.Map<List<RewardPunishmentInfo>>(itm), cancellationToken);
        }

        public virtual async Task DeleteRPInfo(string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(id))
            {
                throw new ArgumentNullException(nameof(id));
            }

            //await _Store.DeleteRPInfoAsync(new RewardPunishmentInfo() { ID = id }, cancellationToken);
        }

        public virtual async Task<HumanSearchResponse<RewardPunishmentResponse>> SearchAttendenceInfo(UserInfo user, RewardPunishmentSearchRequest condition, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition));
            }

            var Response = new HumanSearchResponse<RewardPunishmentResponse>();
            var sql = @"SELECT a.* from XYH_HU_REWARDPUNISHMENT as a where";

            string connectstr = " ";
            if (!string.IsNullOrEmpty(condition?.KeyWord))
            {
                sql += connectstr + @"LOCATE('" + condition.KeyWord + "', a.`Name`)>0";
                connectstr = " and ";
            }
            else
            {
                sql += connectstr + @"a.`ID`!=''";
                connectstr = " and ";
            }

            if (condition?.CreateDate != null)
            {
                sql += connectstr + @"(a.`WorkDate`='" + condition.CreateDate + "'";
                connectstr = " and ";
            }

            try
            {
                var query = _Store.DapperSelect<RewardPunishmentInfo>(sql).ToList();

                Response.ValidityContractCount = query.Count;
                Response.TotalCount = query.Count;

                List<RewardPunishmentInfo> result = new List<RewardPunishmentInfo>();
                var begin = (condition.pageIndex) * condition.pageSize;
                var end = (begin + condition.pageSize) > query.Count ? query.Count : (begin + condition.pageSize);

                for (; begin < end; begin++)
                {
                    result.Add(query.ElementAt(begin));
                }

                Response.PageIndex = condition.pageIndex;
                Response.PageSize = condition.pageSize;
                Response.Extension = _mapper.Map<List<RewardPunishmentResponse>>(result);
            }
            catch (Exception e)
            {

                throw;
            }

            return Response;
        }
    }
}
