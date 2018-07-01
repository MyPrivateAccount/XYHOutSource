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
    public class HumanInfoPartPositionManager
    {
        public HumanInfoPartPositionManager(IHumanInfoPartPositionStore humanInfoPartPostionStore, IMapper mapper)
        {
            Store = humanInfoPartPostionStore;
            _mapper = mapper;
        }

        protected IHumanInfoPartPositionStore Store { get; }
        protected IMapper _mapper { get; }


        public async Task<HumanInfoPartPositionResponse> FindByIdAsync(string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var humanInfoPartPostion = await Store.GetAsync(a => a.Where(b => b.Id == id), cancellationToken);
            return _mapper.Map<HumanInfoPartPositionResponse>(humanInfoPartPostion);
        }


        public async Task<HumanInfoPartPositionResponse> CreateAsync(UserInfo user, HumanInfoPartPositionRequest humanInfoPartPostionRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (humanInfoPartPostionRequest == null)
            {
                throw new ArgumentNullException(nameof(humanInfoPartPostionRequest));
            }
            return _mapper.Map<HumanInfoPartPositionResponse>(await Store.CreateAsync(user, _mapper.Map<HumanInfoPartPosition>(humanInfoPartPostionRequest), cancellationToken));
        }


        public async Task<HumanInfoPartPositionResponse> UpdateAsync(UserInfo user, string id, HumanInfoPartPositionRequest humanInfoPartPostionRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (humanInfoPartPostionRequest == null)
            {
                throw new ArgumentNullException(nameof(humanInfoPartPostionRequest));
            }
            var humanInfoPartPostion = _mapper.Map<HumanInfoPartPosition>(humanInfoPartPostionRequest);
            humanInfoPartPostion.Id = id;
            return _mapper.Map<HumanInfoPartPositionResponse>(await Store.UpdateAsync(user, humanInfoPartPostion, cancellationToken));
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
