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
    public class ShopFacilitiesManager
    {
        public ShopFacilitiesManager(IShopFacilitiesStore shopFacilitiesStore, IMapper mapper)
        {
            Store = shopFacilitiesStore ?? throw new ArgumentNullException(nameof(shopFacilitiesStore));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        protected IShopFacilitiesStore Store { get; }
        protected IMapper _mapper { get; }

        public virtual async Task<ShopFacilitiesResponse> CreateAsync(ShopFacilitiesRequest shopFacilitiesRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (shopFacilitiesRequest == null)
            {
                throw new ArgumentNullException(nameof(shopFacilitiesRequest));
            }
            var facilities = await Store.CreateAsync(_mapper.Map<ShopFacilities>(shopFacilitiesRequest), cancellationToken);
            return _mapper.Map<ShopFacilitiesResponse>(facilities);
        }

        public virtual async Task<ShopFacilitiesResponse> FindByIdAsync(string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var facilities = await Store.GetAsync(a => a.Where(b => b.Id == id), cancellationToken);
            return _mapper.Map<ShopFacilitiesResponse>(facilities);
        }

        public virtual async Task UpdateAsync(ShopFacilitiesRequest shopFacilitiesRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (shopFacilitiesRequest == null)
            {
                throw new ArgumentNullException(nameof(shopFacilitiesRequest));
            }
            await Store.UpdateAsync(_mapper.Map<ShopFacilities>(shopFacilitiesRequest), cancellationToken);
        }

        public virtual async Task SaveAsync(UserInfo user, string buildingId, ShopFacilitiesRequest shopFacilitiesRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (shopFacilitiesRequest == null)
            {
                throw new ArgumentNullException(nameof(shopFacilitiesRequest));
            }
            await Store.SaveAsync(_mapper.Map<SimpleUser>(user), buildingId, _mapper.Map<ShopFacilities>(shopFacilitiesRequest), cancellationToken);
        }
    }
}
