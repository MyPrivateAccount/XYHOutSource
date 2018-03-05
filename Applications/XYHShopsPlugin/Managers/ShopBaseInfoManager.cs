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
    public class ShopBaseInfoManager
    {
        public ShopBaseInfoManager(IShopBaseInfoStore shopBaseInfoStore, IMapper mapper)
        {
            Store = shopBaseInfoStore ?? throw new ArgumentNullException(nameof(shopBaseInfoStore));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        protected IShopBaseInfoStore Store { get; }
        protected IMapper _mapper { get; }

        public virtual async Task<ShopBaseInfoResponse> CreateAsync(ShopBaseInfoRequest shopBaseInfoRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (shopBaseInfoRequest == null)
            {
                throw new ArgumentNullException(nameof(shopBaseInfoRequest));
            }
            var baseinfo = await Store.CreateAsync(_mapper.Map<ShopBaseInfo>(shopBaseInfoRequest), cancellationToken);
            return _mapper.Map<ShopBaseInfoResponse>(baseinfo);
        }

        public virtual async Task<ShopBaseInfoResponse> FindByIdAsync(string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var baseinfo = await Store.GetAsync(a => a.Where(b => b.Id == id), cancellationToken);
            return _mapper.Map<ShopBaseInfoResponse>(baseinfo);
        }

        public virtual async Task UpdateAsync(ShopBaseInfoRequest shopBaseInfoRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (shopBaseInfoRequest == null)
            {
                throw new ArgumentNullException(nameof(shopBaseInfoRequest));
            }
            await Store.UpdateAsync(_mapper.Map<ShopBaseInfo>(shopBaseInfoRequest), cancellationToken);
        }


        public async Task<bool> ShopsIsExist(ShopsIsExistRequest shopsIsExistRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            //var shops = await Store.GetAsync(a => a.Where(b => b.BuildingId == shopsIsExistRequest.BuildingId && b.BuildingNo == shopsIsExistRequest.BuildingNo && b.FloorNo == shopsIsExistRequest.FloorNo && b.Number == shopsIsExistRequest.Number ));
            //if (shops == null)
            //{
            //    return false;
            //}
            //return true;
            return await Store.CheckDuplicateShop(shopsIsExistRequest.BuildingId, shopsIsExistRequest.Id, shopsIsExistRequest.BuildingNo, shopsIsExistRequest.FloorNo, shopsIsExistRequest.Number, cancellationToken);
        }

        public virtual async Task SaveAsync(UserInfo user, string buildingId, ShopBaseInfoRequest shopBaseInfoRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }
            if (shopBaseInfoRequest == null)
            {
                throw new ArgumentNullException(nameof(shopBaseInfoRequest));
            }
            await Store.SaveAsync(_mapper.Map<SimpleUser>(user), buildingId, _mapper.Map<ShopBaseInfo>(shopBaseInfoRequest), cancellationToken);
        }
    }
}
