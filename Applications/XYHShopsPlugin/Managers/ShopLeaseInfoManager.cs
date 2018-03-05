using ApplicationCore.Dto;
using ApplicationCore.Models;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XYHShopsPlugin.Dto;
using XYHShopsPlugin.Models;
using XYHShopsPlugin.Stores;

namespace XYHShopsPlugin.Managers
{
    public class ShopLeaseInfoManager
    {
        public ShopLeaseInfoManager(IShopLeaseInfoStore shopLeaseInfoStore, IMapper mapper)
        {
            Store = shopLeaseInfoStore ?? throw new ArgumentNullException(nameof(shopLeaseInfoStore));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        protected IShopLeaseInfoStore Store { get; }
        protected IMapper _mapper { get; }

        public virtual async Task<ShopLeaseInfoResponse> CreateAsync(ShopLeaseInfoRequest shopLeaseInfoRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (shopLeaseInfoRequest == null)
            {
                throw new ArgumentNullException(nameof(shopLeaseInfoRequest));
            }
            var facilities = await Store.CreateAsync(_mapper.Map<ShopLeaseInfo>(shopLeaseInfoRequest), cancellationToken);
            return _mapper.Map<ShopLeaseInfoResponse>(facilities);
        }

        public virtual async Task<ShopLeaseInfoResponse> FindByIdAsync(string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var facilities = await Store.GetAsync(a => a.Where(b => b.Id == id), cancellationToken);
            return _mapper.Map<ShopLeaseInfoResponse>(facilities);
        }

        public virtual async Task UpdateAsync(ShopLeaseInfoRequest shopLeaseInfoRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (shopLeaseInfoRequest == null)
            {
                throw new ArgumentNullException(nameof(shopLeaseInfoRequest));
            }
            await Store.UpdateAsync(_mapper.Map<ShopLeaseInfo>(shopLeaseInfoRequest), cancellationToken);
        }

        public virtual async Task SaveAsync(UserInfo user, string buildingId, ShopLeaseInfoRequest shopLeaseInfoRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (shopLeaseInfoRequest == null)
            {
                throw new ArgumentNullException(nameof(shopLeaseInfoRequest));
            }
            await Store.SaveAsync(_mapper.Map<SimpleUser>(user), buildingId, _mapper.Map<ShopLeaseInfo>(shopLeaseInfoRequest), cancellationToken);
        }

    }
}
