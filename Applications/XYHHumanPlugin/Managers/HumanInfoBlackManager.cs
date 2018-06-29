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
