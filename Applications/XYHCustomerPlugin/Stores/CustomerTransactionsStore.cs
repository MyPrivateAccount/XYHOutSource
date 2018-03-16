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
    /// <summary>
    /// 实现接口
    /// </summary>
    public class CustomerTransactionsStore : ICustomerTransactionsStore
    {
        ///Db
        protected CustomerDbContext Context { get; }

        public IQueryable<CustomerTransactions> CustomerTransactions { get; set; }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="customerDbContext">Context</param>
        public CustomerTransactionsStore(CustomerDbContext customerDbContext)
        {
            Context = customerDbContext;
            CustomerTransactions = Context.CustomerTransactions;
        }

        ///获取所有成交信息
        public IQueryable<CustomerTransactions> CustomerTransactionsAll()
        {
            var query = from b in Context.CustomerTransactions.AsNoTracking()
                            //业务员
                        join cu in Context.Users.AsNoTracking() on b.UserId equals cu.Id into cu1
                        from hh in cu1.DefaultIfEmpty()
                            //    //客户
                        join customer in Context.CustomerInfos.AsNoTracking() on b.CustomerId equals customer.Id into customer1
                        from customer2 in customer1


                        where !customer2.IsDeleted


                        select new CustomerTransactions()
                        {
                            Id = b.Id,
                            IsDeleted = b.IsDeleted,
                            CustomerId = b.CustomerId,
                            SignTime = b.SignTime,
                            TransactionsStatus = b.TransactionsStatus,
                            BuildingId = b.BuildingId,
                            ShopsId = b.ShopsId,
                            BuildingName = b.BuildingName,
                            ShopsName = b.ShopsName,
                            UserTrueName = hh.TrueName,
                            UserPhone = hh.PhoneNumber,
                            CustomerName = customer2.CustomerName,
                            MainPhone = customer2.MainPhone,
                            IsSellIntention = customer2.IsSellIntention,
                            UserId = b.UserId,
                            DepartmentId = b.DepartmentId,
                            BeltLookId = b.BeltLookId,
                            BeltLookTime = b.BeltLookTime,
                            ReportTime = b.ReportTime,
                            ExpectedBeltTime = b.ExpectedBeltTime,
                            FollowUpTime = b.FollowUpTime,
                            CreateTime = b.CreateTime,
                            Phones = from p in Context.CustomerPhones.AsNoTracking()
                                     where b.CustomerId == p.CustomerId && !p.IsDeleted
                                     select p.Phone,
                            CustomerTransactionsFollowUps = from fu in Context.CustomerTransactionsFollowUps.AsNoTracking()
                                                            where fu.CustomerTransactionsId == b.Id && !fu.IsDeleted
                                                            orderby fu.CreateTime descending
                                                            select new CustomerTransactionsFollowUp
                                                            {
                                                                Id = fu.Id,
                                                                Contents = fu.Contents,
                                                                IsDeleted = fu.IsDeleted,
                                                                CustomerTransactionsId = fu.CustomerTransactionsId,
                                                                MarkTime = fu.MarkTime,
                                                                CreateTime = fu.CreateTime,
                                                                TransactionsStatus = fu.TransactionsStatus
                                                            }
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
        public Task<TResult> GetAsync<TResult>(Func<IQueryable<CustomerTransactions>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.CustomerTransactions.AsNoTracking()).SingleOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// 根据某一成员获取列表客户成交信息
        /// </summary>
        /// <typeparam name="TResult">返回值</typeparam>
        /// <param name="query">参数</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<CustomerTransactions>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.CustomerTransactions.AsNoTracking()).ToListAsync(cancellationToken);
        }

        /// <summary>
        /// 新增客户成交信息
        /// </summary>
        /// <param name="customertransactions">实体</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public async Task<CustomerTransactions> CreateAsync(CustomerTransactions customertransactions, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (customertransactions == null)
            {
                throw new ArgumentNullException(nameof(customertransactions));
            }
            Context.Add(customertransactions);

            if (customertransactions.CustomerTransactionsFollowUps != null)
            {
                Context.AddRange(customertransactions.CustomerTransactionsFollowUps);
            }

            await Context.SaveChangesAsync(cancellationToken);
            return customertransactions;
        }

        /// <summary>
        /// 报备新增客户跟进信息
        /// </summary>
        /// <param name="customerfollowups"></param>
        /// <param name="customerInfo"></param>
        /// <param name="customerTransactions"></param>
        /// <param name="customerTransactionsup"></param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public async Task CreateAndUpdateUserInfoAsync(List<CustomerFollowUp> customerfollowups,
            List<CustomerInfo> customerInfo,
            List<CustomerTransactions> customerTransactions,
            List<CustomerTransactions> customerTransactionsup,
            CancellationToken cancellationToken = default(CancellationToken))
        {

            Context.AttachRange(customerInfo);
            Context.UpdateRange(customerInfo);

            if (customerTransactionsup != null)
            {
                Context.AttachRange(customerTransactionsup);
                Context.UpdateRange(customerTransactionsup);
            }

            if (customerTransactions != null)
            {
                Context.AddRange(customerTransactions);
            }
            Context.AddRange(customerfollowups);

            await Context.SaveChangesAsync(cancellationToken);
        }

        /// <summary>
        /// 删除客户成交
        /// </summary>
        /// <param name="user">登陆用户基本信息</param>
        /// <param name="customertransactions">客户成交实体</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public async Task DeleteAsync(SimpleUser user, CustomerTransactions customertransactions, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (customertransactions == null)
            {
                throw new ArgumentNullException(nameof(customertransactions));
            }
            //删除基本信息
            customertransactions.DeleteTime = DateTime.Now;
            customertransactions.DeleteUser = user.Id;
            customertransactions.IsDeleted = true;
            Context.Attach(customertransactions);
            var entry = Context.Entry(customertransactions);
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
        /// <param name="customertransactionsList">集合</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public async Task DeleteListAsync(List<CustomerTransactions> customertransactionsList, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (customertransactionsList == null)
            {
                throw new ArgumentNullException(nameof(customertransactionsList));
            }
            Context.RemoveRange(customertransactionsList);
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
        /// <param name="customertransactions"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task UpdateAsync(CustomerTransactions customertransactions, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (customertransactions == null)
            {
                throw new ArgumentNullException(nameof(customertransactions));
            }
            Context.Attach(customertransactions);
            Context.Update(customertransactions);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { }
        }

        /// <summary>
        /// 修改客户成交信息
        /// </summary>
        /// <param name="customertransactions"></param>
        /// <param name="customerTransactionsFollowUp"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task UpdateAsync(CustomerTransactions customertransactions, CustomerTransactionsFollowUp customerTransactionsFollowUp, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (customertransactions == null)
            {
                throw new ArgumentNullException(nameof(customertransactions));
            }
            Context.Attach(customertransactions);
            Context.Update(customertransactions);
            if (customerTransactionsFollowUp != null)
            {
                Context.Add(customerTransactionsFollowUp);
            }
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { }
        }

        /// <summary>
        /// 批量修改客户成交信息(一般修改删除状态)
        /// </summary>
        /// <param name="customertransactionss"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task UpdateListAsync(List<CustomerTransactions> customertransactionss, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (customertransactionss == null)
            {
                throw new ArgumentNullException(nameof(customertransactionss));
            }
            Context.AttachRange(customertransactionss);
            Context.UpdateRange(customertransactionss);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { }
        }
    }
}
