using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using XYHCustomerPlugin.Models;

namespace XYHCustomerPlugin.Stores
{
    /// <summary>
    /// 实现接口
    /// </summary>
    public class CustomerTransactionsFollowUpStore : ICustomerTransactionsFollowUpStore
    {
        ///Db
        protected CustomerDbContext Context { get; }
        
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="customerDbContext">Context</param>
        public CustomerTransactionsFollowUpStore(CustomerDbContext customerDbContext)
        {
            Context = customerDbContext;
        }

        ///获取所有成交信息
        public IQueryable<CustomerTransactionsFollowUp> CustomerTransactionsFollowUpAll()
        {
            var query = from b in Context.CustomerTransactionsFollowUps.AsNoTracking()
                        select new CustomerTransactionsFollowUp()
                        {
                        };

            return query;
        }

        /// <summary>
        /// 根据某一成员获取一条客户成交信息
        /// </summary>
        /// <typeparam name="TResult">返回值</typeparam>
        /// <param name="query">参数</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public Task<TResult> GetAsync<TResult>(Func<IQueryable<CustomerTransactionsFollowUp>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.CustomerTransactionsFollowUps.AsNoTracking()).SingleOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// 根据某一成员获取列表客户成交信息
        /// </summary>
        /// <typeparam name="TResult">返回值</typeparam>
        /// <param name="query">参数</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<CustomerTransactionsFollowUp>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.CustomerTransactionsFollowUps.AsNoTracking()).ToListAsync(cancellationToken);
        }

        /// <summary>
        /// 新增客户成交信息
        /// </summary>
        /// <param name="customertransactionstollowup">实体</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public async Task<CustomerTransactionsFollowUp> CreateAsync(CustomerTransactionsFollowUp customertransactionstollowup, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (customertransactionstollowup == null)
            {
                throw new ArgumentNullException(nameof(customertransactionstollowup));
            }
            Context.Add(customertransactionstollowup);


            await Context.SaveChangesAsync(cancellationToken);
            return customertransactionstollowup;
        }

        /// <summary>
        /// 批量新增客户成交信息
        /// </summary>
        /// <param name="customertransactionstollowups">实体</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public async Task<List<CustomerTransactionsFollowUp>> CreateListAsync(List<CustomerTransactionsFollowUp> customertransactionstollowups, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (customertransactionstollowups == null)
            {
                throw new ArgumentNullException(nameof(customertransactionstollowups));
            }
            Context.AddRange(customertransactionstollowups);


            await Context.SaveChangesAsync(cancellationToken);
            return customertransactionstollowups;
        }

        /// <summary>
        /// 删除客户成交
        /// </summary>
        /// <param name="user">登陆用户基本信息</param>
        /// <param name="customertransactionstollowup">客户成交实体</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public async Task DeleteAsync(SimpleUser user, CustomerTransactionsFollowUp customertransactionstollowup, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (customertransactionstollowup == null)
            {
                throw new ArgumentNullException(nameof(customertransactionstollowup));
            }
            //删除基本信息
            customertransactionstollowup.DeleteTime = DateTime.Now;
            customertransactionstollowup.DeleteUser = user.Id;
            customertransactionstollowup.IsDeleted = true;
            Context.Attach(customertransactionstollowup);
            var entry = Context.Entry(customertransactionstollowup);
            entry.Property(x => x.IsDeleted).IsModified = true;
            entry.Property(x => x.DeleteUser).IsModified = true;
            entry.Property(x => x.DeleteTime).IsModified = true;
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        /// <summary>
        /// 从数据库中删除List
        /// </summary>
        /// <param name="customertransactionstollowupList">集合</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public async Task DeleteListAsync(List<CustomerTransactionsFollowUp> customertransactionstollowupList, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (customertransactionstollowupList == null)
            {
                throw new ArgumentNullException(nameof(customertransactionstollowupList));
            }
            Context.RemoveRange(customertransactionstollowupList);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        /// <summary>
        /// 修改客户成交信息
        /// </summary>
        /// <param name="customertransactionstollowup"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task UpdateAsync(CustomerTransactionsFollowUp customertransactionstollowup, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (customertransactionstollowup == null)
            {
                throw new ArgumentNullException(nameof(customertransactionstollowup));
            }
            Context.Attach(customertransactionstollowup);
            Context.Update(customertransactionstollowup);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { }
        }

        /// <summary>
        /// 批量修改客户成交信息(一般修改删除状态)
        /// </summary>
        /// <param name="customertransactionstollowups"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task UpdateListAsync(List<CustomerTransactionsFollowUp> customertransactionstollowups, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (customertransactionstollowups == null)
            {
                throw new ArgumentNullException(nameof(customertransactionstollowups));
            }
            Context.AttachRange(customertransactionstollowups);
            Context.UpdateRange(customertransactionstollowups);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { }
        }
    }
}
