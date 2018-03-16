using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XYHCustomerPlugin.Dto.Request;
using XYHCustomerPlugin.Models;

namespace XYHCustomerPlugin.Stores
{
    public class CustomerDealStore : ICustomerDealStore
    {
        ///Db
        protected CustomerDbContext Context { get; }


        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="customerDbContext">Context</param>
        public CustomerDealStore(CustomerDbContext customerDbContext)
        {
            Context = customerDbContext;
        }

        ///获取所有成交信息
        public IQueryable<CustomerDeal> CustomerDealAll()
        {
            var query = from b in Context.CustomerDeals.AsNoTracking()


                        join customer1 in Context.CustomerInfos.AsNoTracking() on b.Customer equals customer1.Id into customer2
                        from customer in customer2.DefaultIfEmpty()

                        join user1 in Context.Users.AsNoTracking() on b.Salesman equals user1.Id into user2
                        from user in user2.DefaultIfEmpty()

                        select new CustomerDeal()
                        {
                            Id = b.Id,
                            Address = b.Address,
                            Commission = b.Commission,
                            CreateTime = b.CreateTime,
                            Customer = b.Customer,
                            FlowId = b.FlowId,
                            Idcard = b.Idcard,
                            IsDeleted = b.IsDeleted,
                            ExamineStatus = b.ExamineStatus,
                            Mobile = b.Mobile,
                            ProjectId = b.ProjectId,
                            Proprietor = b.Proprietor,
                            Salesman = b.Salesman,
                            Seller = b.Seller,
                            SellerType = b.SellerType,
                            ShopId = b.ShopId,
                            TotalPrice = b.TotalPrice,
                            UserId = b.UserId,
                            CustomerName = customer.CustomerName,
                            CustomerPhone = customer.MainPhone,
                            UserName = user.TrueName,
                            UserPhone = user.PhoneNumber,
                            DepartmentId = user.OrganizationId,
                            Mark = b.Mark,
                            BuildingName = b.BuildingName,
                            ShopName = b.ShopName,
                            DealFileInfos = from f1 in Context.FileInfos.AsNoTracking()
                                            join file in Context.DealFileScopes.AsNoTracking() on f1.FileGuid equals file.FileGuid
                                            where !file.IsDeleted && file.DealId == b.Id
                                            select new CustomerDealFileInfo
                                            {
                                                FileGuid = f1.FileGuid,
                                                IsDeleted = f1.IsDeleted,
                                                ProofType = f1.ProofType,
                                                FileExt = f1.FileExt,
                                                Ext1 = f1.Ext1,
                                                Ext2 = f1.Ext2,
                                                Height = f1.Height,
                                                Name = f1.Name,
                                                Size = f1.Size,
                                                Summary = f1.Summary,
                                                Type = f1.Type,
                                                Uri = f1.Uri,
                                                Width = f1.Width
                                            },
                        };

            return query;
        }

        ///获取所有自售跟进信息
        public IQueryable<CustomerDeal> CustomerDealGetMy()
        {
            var query = from b in Context.CustomerDeals.AsNoTracking()

                        join tran in Context.CustomerTransactions.AsNoTracking() on b.FlowId equals tran.Id
                        join customer1 in Context.CustomerInfos.AsNoTracking() on b.Customer equals customer1.Id into customer2
                        from customer in customer2.DefaultIfEmpty()

                        join user1 in Context.Users.AsNoTracking() on b.Salesman equals user1.Id into user2
                        from user in user2.DefaultIfEmpty()

                        select new CustomerDeal()
                        {
                            Id = b.Id,
                            Address = b.Address,
                            Commission = b.Commission,
                            CreateTime = b.CreateTime,
                            Customer = b.Customer,
                            FlowId = b.FlowId,
                            Idcard = b.Idcard,
                            IsDeleted = b.IsDeleted,
                            ExamineStatus = b.ExamineStatus,
                            Mobile = b.Mobile,
                            ProjectId = b.ProjectId,
                            Proprietor = b.Proprietor,
                            Salesman = b.Salesman,
                            Seller = b.Seller,
                            SellerType = b.SellerType,
                            ShopId = b.ShopId,
                            TotalPrice = b.TotalPrice,
                            UserId = b.UserId,
                            CustomerName = customer.CustomerName,
                            CustomerPhone = customer.MainPhone,
                            UserName = user.TrueName,
                            UserPhone = user.PhoneNumber,
                            DepartmentId = user.OrganizationId,
                            Mark = b.Mark,
                            BuildingName = b.BuildingName,
                            ShopName = b.ShopName,

                        };

            return query;
        }


        /// <summary>
        /// 新增客户成交信息
        /// </summary>
        /// <param name="customerdeal">实体</param>
        /// <param name="customerInfo"></param>
        /// <param name="customerTransactionsFollowUp"></param>
        /// <param name="customerTransactions"></param>
        /// <param name="customerFollowUp"></param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public async Task<CustomerDeal> SubmitCreateAsync(CustomerDeal customerdeal, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (customerdeal == null)
            {
                throw new ArgumentNullException(nameof(customerdeal));
            }
            try
            {
                Context.Add(customerdeal);
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            return customerdeal;
        }



        /// <summary>
        /// 新增客户成交信息
        /// </summary>
        /// <param name="customerdeal">实体</param>
        /// <param name="customerInfo"></param>
        /// <param name="customerTransactionsFollowUp"></param>
        /// <param name="customerTransactions"></param>
        /// <param name="customerFollowUp"></param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public async Task<CustomerDeal> CreateAsync(CustomerDeal customerdeal, CustomerInfo customerInfo, CustomerTransactionsFollowUp customerTransactionsFollowUp, CustomerTransactions customerTransactions, CustomerFollowUp customerFollowUp, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (customerdeal == null)
            {
                throw new ArgumentNullException(nameof(customerdeal));
            }
            Context.Attach(customerdeal);
            Context.Update(customerdeal);
            if (customerInfo != null)
            {
                Context.Attach(customerInfo);
                Context.Update(customerInfo);
            }
            if (customerTransactions != null)
            {
                Context.Attach(customerTransactions);
                Context.Update(customerTransactions);
                Context.Add(customerTransactionsFollowUp);
            }
            if (customerFollowUp != null)
                Context.Add(customerFollowUp);
            await Context.SaveChangesAsync(cancellationToken);
            return customerdeal;
        }

        public Task<TResult> GetAsync<TResult>(Func<IQueryable<CustomerDeal>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.CustomerDeals.AsNoTracking()).SingleOrDefaultAsync(cancellationToken);
        }

        public Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<CustomerDeal>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.CustomerDeals.AsNoTracking()).ToListAsync(cancellationToken);
        }

        public async Task UpdateAsync(CustomerDeal customerDeal, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (customerDeal == null)
            {
                throw new ArgumentNullException(nameof(customerDeal));
            }
            Context.Attach(customerDeal);
            Context.Update(customerDeal);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { throw; }
        }
    }
}
