using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XYHCustomerPlugin.Models;

namespace XYHCustomerPlugin.Stores
{
    public class CustomerFollowUpStore : ICustomerFollowUpStore
    {
        ///Db
        protected CustomerDbContext Context { get; }


        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="customerDbContext">Context</param>
        public CustomerFollowUpStore(CustomerDbContext customerDbContext)
        {
            Context = customerDbContext;
        }

        ///获取所有跟进信息
        public IQueryable<CustomerFollowUp> CustomerFollowUpAll()
        {
            var query = from b in Context.CustomerFollowUps.AsNoTracking()
                        select new CustomerFollowUp()
                        {
                        };

            return query;
        }

        /// <summary>
        /// 根据某一成员获取一条客户跟进信息
        /// </summary>
        /// <typeparam name="TResult">返回值</typeparam>
        /// <param name="query">参数</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public Task<TResult> GetAsync<TResult>(Func<IQueryable<CustomerFollowUp>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.CustomerFollowUps.AsNoTracking()).SingleOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// 根据某一成员获取列表客户跟进信息
        /// </summary>
        /// <typeparam name="TResult">返回值</typeparam>
        /// <param name="query">参数</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<CustomerFollowUp>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.CustomerFollowUps.AsNoTracking()).ToListAsync(cancellationToken);
        }

        /// <summary>
        /// 新增客户跟进信息
        /// </summary>
        /// <param name="customerfollowup">实体</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public async Task<CustomerFollowUp> CreateAsync(CustomerFollowUp customerfollowup, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (customerfollowup == null)
            {
                throw new ArgumentNullException(nameof(customerfollowup));
            }

            var costomerinfo = await Context.CustomerInfos.FindAsync(customerfollowup.CustomerId);
            costomerinfo.FollowUpNum++;
            costomerinfo.FollowupTime = customerfollowup.FollowUpTime;


            var customerdemand = Context.CustomerDemands.Where(x => x.CustomerId == costomerinfo.Id).FirstOrDefault();
            if (customerfollowup.Importance != null)
            {
                customerdemand.Importance = customerfollowup.Importance;
                customerdemand.DemandLevel = customerfollowup.DemandLevel;
                Context.Attach(customerdemand);
                Context.Update(customerdemand);
            }
            else
            {
                customerfollowup.Importance = customerdemand.Importance;
                customerfollowup.DemandLevel = customerfollowup.DemandLevel;
            }

            Context.Attach(costomerinfo);
            Context.Update(costomerinfo);

            Context.Add(customerfollowup);


            await Context.SaveChangesAsync(cancellationToken);
            return customerfollowup;
        }

       

        /// <summary>
        /// 删除客户跟进
        /// </summary>
        /// <param name="user">登陆用户基本信息</param>
        /// <param name="customerfollowup">客户跟进实体</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public async Task DeleteAsync(SimpleUser user, CustomerFollowUp customerfollowup, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (customerfollowup == null)
            {
                throw new ArgumentNullException(nameof(customerfollowup));
            }
            //删除基本信息
            customerfollowup.DeleteTime = DateTime.Now;
            customerfollowup.DeleteUser = user.Id;
            customerfollowup.IsDeleted = true;
            Context.Attach(customerfollowup);
            var entry = Context.Entry(customerfollowup);
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
        /// <param name="customerfollowupList">集合</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public async Task DeleteListAsync(List<CustomerFollowUp> customerfollowupList, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (customerfollowupList == null)
            {
                throw new ArgumentNullException(nameof(customerfollowupList));
            }
            Context.RemoveRange(customerfollowupList);
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
        /// 修改客户跟进信息
        /// </summary>
        /// <param name="customerfollowup"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task UpdateAsync(CustomerFollowUp customerfollowup, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (customerfollowup == null)
            {
                throw new ArgumentNullException(nameof(customerfollowup));
            }
            Context.Attach(customerfollowup);
            Context.Update(customerfollowup);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { }
        }

        /// <summary>
        /// 批量修改客户跟进信息(一般修改删除状态)
        /// </summary>
        /// <param name="customerfollowups"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task UpdateListAsync(List<CustomerFollowUp> customerfollowups, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (customerfollowups == null)
            {
                throw new ArgumentNullException(nameof(customerfollowups));
            }
            Context.AttachRange(customerfollowups);
            Context.UpdateRange(customerfollowups);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { }
        }
    }
}
