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
    public class HumanInfoLeaveManager
    {
        public HumanInfoLeaveManager(IHumanInfoLeaveStore humanInfoLeaveStore, IMapper mapper)
        {
            Store = humanInfoLeaveStore;
            _mapper = mapper;
        }

        protected IHumanInfoLeaveStore Store { get; }
        protected IMapper _mapper { get; }


        public async Task<HumanInfoLeaveResponse> FindByIdAsync(string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var humanInfoLeave = await Store.GetAsync(a => a.Where(b => b.Id == id), cancellationToken);
            return _mapper.Map<HumanInfoLeaveResponse>(humanInfoLeave);
        }


        public async Task<HumanInfoLeaveResponse> CreateAsync(UserInfo user, HumanInfoLeaveRequest humanInfoLeaveRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (humanInfoLeaveRequest == null)
            {
                throw new ArgumentNullException(nameof(humanInfoLeaveRequest));
            }
            return _mapper.Map<HumanInfoLeaveResponse>(await Store.CreateAsync(user, _mapper.Map<HumanInfoLeave>(humanInfoLeaveRequest), cancellationToken));
        }


        public async Task<HumanInfoLeaveResponse> UpdateAsync(UserInfo user, string id, HumanInfoLeaveRequest humanInfoLeaveRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (humanInfoLeaveRequest == null)
            {
                throw new ArgumentNullException(nameof(humanInfoLeaveRequest));
            }
            var humanInfoLeave = _mapper.Map<HumanInfoLeave>(humanInfoLeaveRequest);
            humanInfoLeave.Id = id;
            return _mapper.Map<HumanInfoLeaveResponse>(await Store.UpdateAsync(user, humanInfoLeave, cancellationToken));
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
