using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using AutoMapper;
using XYHContractPlugin.Models;
using XYHContractPlugin.Stores;
using XYHContractPlugin.Dto.Response;
using System.Threading;
using System.Linq;
using ApplicationCore.Dto;
using ApplicationCore.Models;
using ContractInfoRequest = XYHContractPlugin.Dto.Response.ContractInfoResponse;

namespace XYHContractPlugin.Managers
{
    public class ContractInfoManager
    {
        public ContractInfoManager(IContractInfoStore contractStore, IMapper mapper)
        {
            Store = contractStore ?? throw new ArgumentNullException(nameof(contractStore));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }
        protected IContractInfoStore Store { get; }
        protected IMapper _mapper { get; }

        public virtual async Task<ContractInfoResponse> CreateAsync(ContractInfoRequest buildingBaseInfoRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (buildingBaseInfoRequest == null)
            {
                throw new ArgumentNullException(nameof(buildingBaseInfoRequest));
            }
            var baseinfo = await Store.CreateAsync(_mapper.Map<ContractInfo>(buildingBaseInfoRequest), cancellationToken);
            return _mapper.Map<ContractInfoResponse>(baseinfo);
        }

        public virtual async Task<ContractInfoResponse> FindByIdAsync(string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var baseinfo = await Store.GetAsync(a => a.Where(b => b.ID == id), cancellationToken);
            if (baseinfo.IsDelete)
            {
                throw new ArgumentNullException("已被删除");
            }
            return _mapper.Map<ContractInfoResponse>(baseinfo);
        }

        public virtual async Task UpdateAsync(ContractInfoRequest buildingBaseInfoRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (buildingBaseInfoRequest == null)
            {
                throw new ArgumentNullException(nameof(buildingBaseInfoRequest));
            }
            await Store.UpdateAsync(_mapper.Map<ContractInfo>(buildingBaseInfoRequest), cancellationToken);
        }


        public virtual async Task SaveAsync(UserInfo user, ContractInfoRequest buildingBaseInfoRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (buildingBaseInfoRequest == null)
            {
                throw new ArgumentNullException(nameof(buildingBaseInfoRequest));
            }


            await Store.SaveAsync(_mapper.Map<SimpleUser>(user), _mapper.Map<ContractInfo>(buildingBaseInfoRequest), cancellationToken);
        }
    }
}
