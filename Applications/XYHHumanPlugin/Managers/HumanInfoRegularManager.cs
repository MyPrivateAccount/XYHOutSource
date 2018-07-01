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
    public class HumanInfoRegularManager
    {
        public HumanInfoRegularManager(IHumanInfoRegularStore humanInfoRegularStore, IMapper mapper)
        {
            Store = humanInfoRegularStore;
            _mapper = mapper;
        }

        protected IHumanInfoRegularStore Store { get; }
        protected IMapper _mapper { get; }


        public async Task<HumanInfoRegularResponse> FindByIdAsync(string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var humanInfoRegular = await Store.GetAsync(a => a.Where(b => b.Id == id), cancellationToken);
            return _mapper.Map<HumanInfoRegularResponse>(humanInfoRegular);
        }


        public async Task<HumanInfoRegularResponse> CreateAsync(UserInfo user, HumanInfoRegularRequest humanInfoRegularRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (humanInfoRegularRequest == null)
            {
                throw new ArgumentNullException(nameof(humanInfoRegularRequest));
            }
            return _mapper.Map<HumanInfoRegularResponse>(await Store.CreateAsync(user, _mapper.Map<HumanInfoRegular>(humanInfoRegularRequest), cancellationToken));
        }


        public async Task<HumanInfoRegularResponse> UpdateAsync(UserInfo user, string id, HumanInfoRegularRequest humanInfoRegularRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (humanInfoRegularRequest == null)
            {
                throw new ArgumentNullException(nameof(humanInfoRegularRequest));
            }
            var humanInfoRegular = _mapper.Map<HumanInfoRegular>(humanInfoRegularRequest);
            humanInfoRegular.Id = id;
            return _mapper.Map<HumanInfoRegularResponse>(await Store.UpdateAsync(user, humanInfoRegular, cancellationToken));
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
