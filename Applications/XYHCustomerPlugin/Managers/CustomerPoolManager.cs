using ApplicationCore;
using ApplicationCore.Dto;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
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
    public class CustomerPoolManager
    {
        public CustomerPoolManager(ICustomerPoolStore customerPoolStore,
            IMapper mapper)
        {
            Store = customerPoolStore ?? throw new ArgumentNullException(nameof(customerPoolStore));
            _mapper = mapper ?? throw new ArgumentNullException(nameof(mapper));
        }

        protected ICustomerPoolStore Store { get; }

        protected IMapper _mapper { get; }



        public async Task<PagingResponseMessage<CustomerPoolResponse>> FindByPoolIdAsync(string poolId, CustomerPoolCondition condition, CancellationToken cancellationToken = default(CancellationToken))
        {
            PagingResponseMessage<CustomerPoolResponse> pagingResponse = new PagingResponseMessage<CustomerPoolResponse>();
            if (condition == null)
            {
                pagingResponse.Code = ResponseCodeDefines.ArgumentNullError;
                return pagingResponse;
            }
            var q = Store.GetQuery(poolId);
            if (!string.IsNullOrEmpty(condition.KeyWords))
            {
                q = q.Where(a => a.CustomerName.Contains(condition.KeyWords));
            }
            pagingResponse.TotalCount = await q.CountAsync(cancellationToken);
            var qlist = await q.Skip(condition.PageIndex * condition.PageSize).Take(condition.PageSize).ToListAsync(cancellationToken);
            pagingResponse.PageIndex = condition.PageIndex;
            pagingResponse.PageSize = condition.PageSize;
            pagingResponse.Extension = qlist.ToList();
            return pagingResponse;
        }

        /// <summary>
        /// 公客池移动
        /// </summary>
        /// <param name="customerChangePoolRequest"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task ChangePool(UserInfo user, CustomerChangePoolRequest customerChangePoolRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (customerChangePoolRequest == null)
            {
                throw new ArgumentNullException(nameof(CustomerChangePoolRequest));
            }
            var list = await Store.ListAsync(a => a.Where(b => customerChangePoolRequest.CustomerIds.Contains(b.CustomerId)));


            var mig = new List<MigrationPoolHistory>();
            for (int i = 0; i < list.Count; i++)
            {
                list[i].DepartmentId = customerChangePoolRequest.NewDepartmentId;
                list[i].UpdateUser = user.Id;
                list[i].UpdateTime = DateTime.Now;


                mig.Add(new MigrationPoolHistory
                {
                    CustomerId = list[i].CustomerId,
                    MigrationTime = DateTime.Now,
                    OriginalDepartment = list[i].DepartmentId,
                    TargetDepartment = customerChangePoolRequest.NewDepartmentId
                });
            }
            await Store.UpdateListAsync(list, mig, cancellationToken);
        }

        /// <summary>
        /// 加入公客池
        /// </summary>
        /// <param name="customerPoolJoinRequest"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task JoinCustomer(UserInfo userInfo, CustomerPoolJoinRequest customerPoolJoinRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (customerPoolJoinRequest == null)
            {
                throw new ArgumentNullException(nameof(CustomerPoolJoinRequest));
            }
            await Store.JoinCustomerAsync(userInfo, customerPoolJoinRequest, cancellationToken);
        }

        /// <summary>
        /// 认领客户
        /// </summary>
        /// <returns></returns>
        public async Task ClaimCustomer(UserInfo userInfo, CustomerPoolClaimRequest customerPoolClaimRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (customerPoolClaimRequest == null)
            {
                throw new ArgumentNullException(nameof(CustomerPoolClaimRequest));
            }
            await Store.ClaimCustomerAsync(userInfo, customerPoolClaimRequest, cancellationToken);
        }

    }
}
