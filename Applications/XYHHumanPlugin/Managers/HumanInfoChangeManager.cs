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
    /// <summary>
    /// 
    /// </summary>
    public class HumanInfoChangeManager
    {
        public HumanInfoChangeManager(IHumanInfoChangeStore humanInfoChangeStore, IMapper mapper)
        {
            Store = humanInfoChangeStore;
            _mapper = mapper;
        }

        protected IHumanInfoChangeStore Store { get; }
        protected IMapper _mapper { get; }


        public async Task<HumanInfoChangeResponse> FindByIdAsync(string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var humanInfoChange = await Store.ListAsync(a => a.Where(b => b.Id == id), cancellationToken);
            return _mapper.Map<HumanInfoChangeResponse>(humanInfoChange);
        }


        public async Task<HumanInfoChangeResponse> CreateAsync(UserInfo user, HumanInfoChangeRequest humanInfoChangeRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (humanInfoChangeRequest == null)
            {
                throw new ArgumentNullException(nameof(humanInfoChangeRequest));
            }
            return _mapper.Map<HumanInfoChangeResponse>(await Store.CreateAsync(user, _mapper.Map<HumanInfoChange>(humanInfoChangeRequest), cancellationToken));
        }


        public async Task<HumanInfoChangeResponse> UpdateAsync(UserInfo user, string id, HumanInfoChangeRequest humanInfoChangeRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (humanInfoChangeRequest == null)
            {
                throw new ArgumentNullException(nameof(humanInfoChangeRequest));
            }
            var humanInfoChange = _mapper.Map<HumanInfoChange>(humanInfoChangeRequest);
            humanInfoChange.Id = id;
            return _mapper.Map<HumanInfoChangeResponse>(await Store.UpdateAsync(user, humanInfoChange, cancellationToken));
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
