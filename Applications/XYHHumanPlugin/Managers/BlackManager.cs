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
using BlackInfoRequest = XYHHumanPlugin.Dto.Response.BlackInfoResponse;

namespace XYHHumanPlugin.Managers
{
    public class BlackManager
    {
        public BlackManager(IHumanManageStore stor, IMapper mapper)
        {
            _Store = stor;
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        protected IHumanManageStore _Store { get; }
        protected IMapper _mapper { get; }

        public virtual async Task SetBlack(BlackInfoRequest black, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (black == null)
            {
                throw new ArgumentNullException(nameof(black));
            }
            if (string.IsNullOrEmpty(black.IDCard))
            {
                throw new ArgumentNullException(nameof(black));
            }

            await _Store.SetBlackAsync(_mapper.Map<BlackInfo>(black), cancellationToken);
        }

        public virtual async Task DeleteBlack(BlackInfoRequest black, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (string.IsNullOrEmpty(black.IDCard))
            {
                throw new ArgumentNullException(nameof(black));
            }
            await _Store.DeleteBlackAsync(_mapper.Map<BlackInfo>(black), cancellationToken);
        }
        
        public virtual async Task<HumanSearchResponse<BlackInfoResponse>> SearchBlackInfo(UserInfo user, HumanSearchRequest condition, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition));
            }

            var Response = new HumanSearchResponse<BlackInfoResponse>();
            var sql = @"SELECT a.* from xyh_hu_blacklist as a where";

            string connectstr = " ";
            if (!string.IsNullOrEmpty(condition?.KeyWord))
            {
                sql += connectstr + @"LOCATE('" + condition.KeyWord + "', a.`Position`)>0";
                connectstr = " and ";
            }
            else if (condition?.KeyWord != null)
            {
                sql += connectstr + @"a.`ID`!=''";
                connectstr = " and ";
            }
            else if (condition?.KeyWord == null)
            {
                sql += connectstr + @"a.`ID`!=''";
                connectstr = " and ";
            }

            if (condition?.OrderRule == 0 || condition?.OrderRule == null)
            {
                sql += @" ORDER BY a.`Position`";
            }

            try
            {
                var query = _Store.DapperSelect<BlackInfo>(sql).ToList();
                Response.ValidityContractCount = query.Count;
                Response.TotalCount = query.Count;

                List<BlackInfo> result = new List<BlackInfo>();
                var begin = (condition.pageIndex) * condition.pageSize;
                var end = (begin + condition.pageSize) > query.Count ? query.Count : (begin + condition.pageSize);

                for (; begin < end; begin++)
                {
                    result.Add(query.ElementAt(begin));
                }
                Response.PageIndex = condition.pageIndex;
                Response.PageSize = condition.pageSize;
                Response.Extension = _mapper.Map<List<BlackInfoResponse>>(result);
            }
            catch (Exception e)
            {
                throw;
            }
            
            return Response;
        }
    }
}
