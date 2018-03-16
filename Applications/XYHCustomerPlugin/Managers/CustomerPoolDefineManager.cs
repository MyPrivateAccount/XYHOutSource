using ApplicationCore.Dto;
using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XYHCustomerPlugin.Dto;
using XYHCustomerPlugin.Models;
using XYHCustomerPlugin.Stores;

namespace XYHCustomerPlugin.Managers
{
    public class CustomerPoolDefineManager
    {

        public CustomerPoolDefineManager(ICustomerPoolDefineStore customerPoolDefineStore,
            IMapper mapper)
        {
            Store = customerPoolDefineStore ?? throw new ArgumentNullException(nameof(customerPoolDefineStore));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        protected ICustomerPoolDefineStore Store { get; }

        protected IMapper _mapper { get; }


        public virtual async Task<CustomerPoolDefineResponse> CreateAsync(UserInfo userInfo, CustomerPoolDefineRequest customerLossRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (customerLossRequest == null)
            {
                throw new ArgumentNullException(nameof(customerLossRequest));
            }
            var pool = _mapper.Map<CustomerPoolDefine>(customerLossRequest);
            pool.Id = Guid.NewGuid().ToString();
            pool.CreateUser = userInfo.Id;
            pool.CreateTime = DateTime.Now;

            return _mapper.Map<CustomerPoolDefineResponse>(await Store.CreateAsync(pool, cancellationToken));
        }


        public virtual async Task UpdateAsync(string userId, CustomerPoolDefineRequest customerLossRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (customerLossRequest == null)
            {
                throw new ArgumentNullException(nameof(customerLossRequest));
            }
            var pooldefine = await Store.GetAsync(a => a.Where(b => b.Id == customerLossRequest.Id), cancellationToken);
            var customer = _mapper.Map<CustomerPoolDefine>(customerLossRequest);
            customer.CreateTime = pooldefine.CreateTime;
            customer.CreateUser = pooldefine.CreateUser;
            customer.DepartmentId = pooldefine.DepartmentId;
            customer.UpdateUser = userId;
            customer.UpdateTime = DateTime.Now;
            await Store.UpdateAsync(customer, cancellationToken);
        }



        public virtual async Task DeleteListAsync(string userId, List<string> ids, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (ids == null)
            {
                throw new ArgumentNullException(nameof(ids));
            }
            var list = await Store.ListAsync(a => a.Where(b => ids.Contains(b.Id)));

            for (int i = 0; i < list.Count; i++)
            {
                list[i].DeleteUser = userId;
                list[i].IsDeleted = true;
                list[i].DeleteTime = DateTime.Now;
            }
            await Store.UpdateListAsync(list, cancellationToken);
        }

        public virtual async Task<List<CustomerPoolDefineResponse>> FindAllAsync(CancellationToken cancellationToken = default(CancellationToken))
        {
            var pooldefines = await Store.ListAsync(a => a.Where(b => !b.IsDeleted));

            return _mapper.Map<List<CustomerPoolDefineResponse>>(pooldefines);
        }

        public virtual async Task<CustomerPoolDefineResponse> FindByIdAsync(string id, CancellationToken cancellationToken = default(CancellationToken))
        {
            var pooldefine = await Store.GetAsync(a => a.Where(b => b.Id == id), cancellationToken);
            return _mapper.Map<CustomerPoolDefineResponse>(pooldefine);
        }

    }
}
