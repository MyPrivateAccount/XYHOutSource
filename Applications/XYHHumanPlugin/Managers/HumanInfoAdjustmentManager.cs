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

namespace XYHHumanPlugin.Managers
{
    public class HumanInfoAdjustmentManager
    {
        public HumanInfoAdjustmentManager(IHumanInfoAdjustmentStore humanInfoAdjustmentStore, IMapper mapper)
        {
            Store = humanInfoAdjustmentStore;
            _mapper = mapper;
        }

        protected IHumanInfoAdjustmentStore Store { get; }
        protected IMapper _mapper { get; }


        public async Task<HumanInfoAdjustmentResponse> FindByIdAsync(string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var humanInfoAdjustment = await Store.GetAsync(a => a.Where(b => b.Id == id), cancellationToken);
            return _mapper.Map<HumanInfoAdjustmentResponse>(humanInfoAdjustment);
        }


        public async Task<HumanInfoAdjustmentResponse> CreateAsync(UserInfo user, HumanInfoAdjustmentRequest humanInfoAdjustmentRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (humanInfoAdjustmentRequest == null)
            {
                throw new ArgumentNullException(nameof(humanInfoAdjustmentRequest));
            }
            return _mapper.Map<HumanInfoAdjustmentResponse>(await Store.CreateAsync(user, _mapper.Map<HumanInfoAdjustment>(humanInfoAdjustmentRequest), cancellationToken));
        }


        public async Task<HumanInfoAdjustmentResponse> UpdateAsync(UserInfo user, string id, HumanInfoAdjustmentRequest humanInfoAdjustmentRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (humanInfoAdjustmentRequest == null)
            {
                throw new ArgumentNullException(nameof(humanInfoAdjustmentRequest));
            }
            var humanInfoAdjustment = _mapper.Map<HumanInfoAdjustment>(humanInfoAdjustmentRequest);
            humanInfoAdjustment.Id = id;
            return _mapper.Map<HumanInfoAdjustmentResponse>(await Store.UpdateAsync(user, humanInfoAdjustment, cancellationToken));
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
            var old = await Store.GetAsync(a => a.Where(b => b.Id == id));
            if (old == null)
            {
                throw new Exception("删除的对象不存在");
            }
            await Store.DeleteAsync(user, old, cancellationToken);
        }
    }
}
