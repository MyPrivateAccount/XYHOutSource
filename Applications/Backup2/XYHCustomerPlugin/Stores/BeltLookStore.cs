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
    public class BeltLookStore : IBeltLookStore
    {
        //Db
        protected CustomerDbContext Context { get; }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="customerDbContext">Context</param>
        public BeltLookStore(CustomerDbContext customerDbContext)
        {
            Context = customerDbContext;
        }

        public IQueryable<BeltLook> BeltLookAll()
        {
            return Context.BeltLooks;
        }

        /// <summary>
        /// 根据某一成员获取一条带看信息
        /// </summary>
        /// <typeparam name="TResult">返回值</typeparam>
        /// <param name="query">参数</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public Task<TResult> GetAsync<TResult>(Func<IQueryable<BeltLook>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.BeltLooks.AsNoTracking()).SingleOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// 根据某一成员获取列表带看信息
        /// </summary>
        /// <typeparam name="TResult">返回值</typeparam>
        /// <param name="query">参数</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<BeltLook>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.BeltLooks.AsNoTracking()).ToListAsync(cancellationToken);
        }

        /// <summary>
        /// 新增带看信息
        /// </summary>
        /// <param name="beltLook">实体</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public async Task<BeltLook> CreateAsync(BeltLook beltLook, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (beltLook == null)
            {
                throw new ArgumentNullException(nameof(beltLook));
            }
            Context.Add(beltLook);

            //增加用户带看次数
            var customerinfo = await Context.CustomerInfos.FindAsync(beltLook.CustomerId);
            customerinfo.BeltNum = customerinfo.BeltNum + 1;
            Context.Attach(customerinfo);
            Context.Update(customerinfo);

            await Context.SaveChangesAsync(cancellationToken);
            return beltLook;
        }

        /// <summary>
        /// 批量新增带看信息
        /// </summary>
        /// <param name="beltLooks">实体</param>
        /// <param name="customertransactionss"></param>
        /// <param name="customertransactionstollowups"></param>
        /// <param name="customerFollowUps"></param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public async Task<List<BeltLook>> CreateListAsync(List<BeltLook> beltLooks,
            List<CustomerTransactions> customertransactionss,
            List<CustomerTransactionsFollowUp> customertransactionstollowups,
            List<CustomerFollowUp> customerFollowUps,
            CancellationToken cancellationToken = default(CancellationToken))
        {
            if (beltLooks == null)
            {
                throw new ArgumentNullException(nameof(beltLooks));
            }
            if (beltLooks != null)
            {
                var customerinfos = new List<CustomerInfo>();
                foreach (var item in beltLooks)
                {
                    //增加用户带看次数
                    var customerinfo = Context.CustomerInfos.Find(item.CustomerId);

                    customerinfo.BeltNum = customerinfo.BeltNum + 1;

                    item.BeltInfo = customerinfo.BeltNum == 1 ? "首看" : "复看";

                    customerinfo.RateProgress = customerinfo.BeltNum == 1 ? RateProgress.Look : RateProgress.SencondLook;

                    item.CompleteBeltNum = customerinfo.BeltNum;

                    customerinfos.Add(customerinfo);
                }
                Context.AddRange(beltLooks);

                Context.AttachRange(customerinfos);
                Context.UpdateRange(customerinfos);
            }
            if (customertransactionss != null)
            {
                Context.AttachRange(customertransactionss);
                Context.UpdateRange(customertransactionss);
            }
            if (customertransactionstollowups != null)
                Context.AddRange(customertransactionstollowups);
            if (customerFollowUps != null)
                Context.AddRange(customerFollowUps);

            await Context.SaveChangesAsync(cancellationToken);
            return beltLooks;
        }

        /// <summary>
        /// 再次带看
        /// </summary>
        /// <param name="customertransactions"></param>
        /// <param name="customertransactionstollowup"></param>
        /// <param name="customerFollowUp"></param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public async Task CreateAgainBeltLookAsync(CustomerTransactions customertransactions, CustomerTransactionsFollowUp customertransactionstollowup, CustomerFollowUp customerFollowUp, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (customertransactions == null)
            {
                throw new ArgumentNullException(nameof(customertransactions));
            }
            if (customertransactions != null)
            {
                Context.Add(customertransactions);
            }
            if (customertransactionstollowup != null)
                Context.Add(customertransactionstollowup);
            if (customerFollowUp != null)
                Context.Add(customerFollowUp);

            await Context.SaveChangesAsync(cancellationToken);
        }


        /// <summary>
        /// 修改带看信息
        /// </summary>
        /// <param name="beltLook"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task UpdateAsync(BeltLook beltLook, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (beltLook == null)
            {
                throw new ArgumentNullException(nameof(beltLook));
            }
            Context.Attach(beltLook);
            Context.Update(beltLook);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { }
        }

        /// <summary>
        /// 批量修改带看信息(一般修改删除状态)
        /// </summary>
        /// <param name="beltLook"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task UpdateListAsync(List<BeltLook> beltLooks, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (beltLooks == null)
            {
                throw new ArgumentNullException(nameof(beltLooks));
            }
            Context.AttachRange(beltLooks);
            Context.UpdateRange(beltLooks);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { }
        }

        /// <summary>
        /// 删除带看
        /// </summary>
        /// <param name="user">登陆用户基本信息</param>
        /// <param name="beltLook">带看实体</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public async Task DeleteAsync(SimpleUser user, BeltLook beltLook, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (beltLook == null)
            {
                throw new ArgumentNullException(nameof(beltLook));
            }
            //删除基本信息
            beltLook.DeleteTime = DateTime.Now;
            beltLook.DeleteUser = user.Id;
            beltLook.IsDeleted = true;
            Context.Attach(beltLook);
            var entry = Context.Entry(beltLook);
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
    }
}
