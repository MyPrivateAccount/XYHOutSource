

using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using XYHCustomerPlugin.Models;

namespace XYHCustomerPlugin.Stores
{
    public class CustomerDemandStore : ICustomerDemandStore
    {
        //Db
        protected CustomerDbContext Context { get; }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="customerDbContext">Context</param>
        public CustomerDemandStore(CustomerDbContext customerDbContext)
        {
            Context = customerDbContext;
        }

        /// <summary>
        /// 根据某一成员获取一条客源需求信息
        /// </summary>
        /// <typeparam name="TResult">返回值</typeparam>
        /// <param name="query">参数</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public Task<TResult> GetAsync<TResult>(Func<IQueryable<CustomerDemand>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.CustomerDemands.AsNoTracking()).SingleOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// 根据某一成员获取列表客源需求
        /// </summary>
        /// <typeparam name="TResult">返回值</typeparam>
        /// <param name="query">参数</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<CustomerDemand>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.CustomerDemands.AsNoTracking()).ToListAsync(cancellationToken);
        }

        /// <summary>
        /// 新增客源需求信息
        /// </summary>
        /// <param name="customerDemand">实体</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public async Task<CustomerDemand> CreateAsync(CustomerDemand customerDemand, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (customerDemand == null)
            {
                throw new ArgumentNullException(nameof(customerDemand));
            }
            Context.Add(customerDemand);
            await Context.SaveChangesAsync(cancellationToken);
            return customerDemand;
        }

        /// <summary>
        /// 修改客源需求信息
        /// </summary>
        /// <param name="customerDemand"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task UpdateAsync(CustomerDemand customerDemand, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (customerDemand == null)
            {
                throw new ArgumentNullException(nameof(customerDemand));
            }
            Context.Attach(customerDemand);
            Context.Update(customerDemand);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { }
        }

        /// <summary>
        /// 批量修改客源需求信息(一般修改删除状态)
        /// </summary>
        /// <param name="customerDemands"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task UpdateListAsync(List<CustomerDemand> customerDemands, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (customerDemands == null)
            {
                throw new ArgumentNullException(nameof(customerDemands));
            }
            Context.AttachRange(customerDemands);
            Context.UpdateRange(customerDemands);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { }
        }
    }
}
