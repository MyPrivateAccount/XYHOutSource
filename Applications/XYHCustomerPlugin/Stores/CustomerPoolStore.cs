using ApplicationCore.Dto;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XYHCustomerPlugin.Dto;
using XYHCustomerPlugin.Models;

namespace XYHCustomerPlugin.Stores
{
    public class CustomerPoolStore : ICustomerPoolStore
    {
        public CustomerPoolStore(CustomerDbContext customerDbContext)
        {
            Context = customerDbContext;
            CustomerPools = Context.CustomerPools;
        }

        protected CustomerDbContext Context { get; }

        public IQueryable<CustomerPool> CustomerPools { get; set; }

        public async Task<CustomerPool> CreateAsync(CustomerPool customerPool, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (customerPool == null)
            {
                throw new ArgumentNullException(nameof(customerPool));
            }
            Context.Add(customerPool);
            await Context.SaveChangesAsync(cancellationToken);
            return customerPool;
        }

        public async Task<List<CustomerPool>> CreateListAsync(List<CustomerPool> customerPools, List<MigrationPoolHistory> migrationPoolHistory, List<CustomerFollowUp> customerFollowUp, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (customerPools == null || migrationPoolHistory == null)
            {
                throw new ArgumentNullException(nameof(customerPools));
            }

            var ids = customerPools.Select(b => b.CustomerId);

            var customerInfos = Context.CustomerInfos.Where(a => ids.Contains(a.Id)).ToList();

            customerInfos = customerInfos.Select(x =>
            {
                x.UserId = "";
                x.DepartmentId = customerPools.FirstOrDefault(a => a.CustomerId == x.Id).DepartmentId;
                x.CustomerStatus = CustomerStatus.PoolCustomer;
                x.FollowupTime = DateTime.Now;
                return x;
            }).ToList();

            Context.AttachRange(customerInfos);
            Context.UpdateRange(customerInfos);


            Context.AddRange(customerPools);
            Context.AddRange(migrationPoolHistory);
            Context.AddRange(customerFollowUp);
            await Context.SaveChangesAsync(cancellationToken);
            return customerPools;
        }


        public async Task DeleteAsync(CustomerPool customerPool, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (customerPool == null)
            {
                throw new ArgumentNullException(nameof(customerPool));
            }
            Context.Remove(customerPool);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public async Task DeleteListAsync(List<CustomerPool> customerPoolList, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (customerPoolList == null)
            {
                throw new ArgumentNullException(nameof(customerPoolList));
            }
            Context.RemoveRange(customerPoolList);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public IQueryable<CustomerPoolResponse> GetQuery(string DepartmentId)
        {
            var query = from cp in Context.CustomerPools.AsNoTracking()
                        join c in Context.CustomerInfos.AsNoTracking() on cp.CustomerId equals c.Id
                        //所属人及部门
                        join cu1 in Context.Users.AsNoTracking() on c.UserId equals cu1.Id into cu2
                        from cu in cu2.DefaultIfEmpty()

                        join ru1a in Context.Organizations.AsNoTracking() on c.DepartmentId equals ru1a.Id into ru1b
                        from ru1 in ru1b.DefaultIfEmpty()
                        where cp.DepartmentId == DepartmentId
                        orderby cp.JoinDate descending
                        select new CustomerPoolResponse()
                        {
                            Mark = c.Mark,
                            CustomerName = c.CustomerName,
                            CustomerId = c.Id,
                            JoinDate = cp.JoinDate,
                            DepartmentId = DepartmentId,
                            MainPhone = c.MainPhone,
                            CreateTime = c.CreateTime,
                            FollowupTime = c.FollowupTime,
                            Sex = c.Sex
                        };
            return query;
        }


        public async Task JoinCustomerAsync(UserInfo user, CustomerPoolJoinRequest customerPoolJoinRequest, CancellationToken cancellationToken = default(CancellationToken))
        {
            var customerInfos = Context.CustomerInfos.Where(a => customerPoolJoinRequest.CustomerIds.Contains(a.Id)).ToList();
            foreach (var item in customerInfos)
            {
                item.UserId = "";
                item.DepartmentId = customerPoolJoinRequest.DepartmentId;
                var pool = new CustomerPool()
                {
                    CreateTime = DateTime.Now,
                    CreateUser = user.Id,
                    CustomerId = item.Id,
                    IsDeleted = false,
                    JoinDate = DateTime.Now,
                    DepartmentId = customerPoolJoinRequest.DepartmentId
                };
                Context.Update(item);
                Context.Add(pool);
            }
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }


        public async Task<bool> ClaimCustomerAsync(UserInfo user, CustomerPoolClaimRequest customerPoolClaimRequest, CancellationToken cancellationToken = default(CancellationToken))
        {

            var customerpools = Context.CustomerPools.Where(a => customerPoolClaimRequest.CustomerIds.Contains(a.CustomerId) && !a.IsDeleted).ToList();
            customerpools = customerpools.Select(x =>
            {
                x.IsDeleted = true;
                x.DeleteUser = user.Id;
                x.DeleteTime = DateTime.Now;
                return x;
            }).ToList();
            if (customerpools != null)
            {
                Context.UpdateRange(customerpools);
            }

            var customerInfos = Context.CustomerInfos.Where(a => customerPoolClaimRequest.CustomerIds.Contains(a.Id)).ToList();
            foreach (var item in customerInfos)
            {
                var customerdemand = await Context.CustomerDemands.AsNoTracking().Where(a => a.CustomerId == item.Id).FirstOrDefaultAsync(cancellationToken);
                if (customerdemand == null)
                {
                    return false;
                }

                item.UserId = user.Id;
                item.DepartmentId = user.OrganizationId;
                item.CustomerStatus = CustomerStatus.ExistingCustomers;

                customerdemand.UserId = user.Id;
                customerdemand.DepartmentId = user.OrganizationId;

                //var newcustomerinfo = JsonConvert.DeserializeObject<CustomerInfo>(JsonConvert.SerializeObject(item));
                //newcustomerinfo.Id = Guid.NewGuid().ToString();
                //newcustomerinfo.IsDeleted = false;
                //newcustomerinfo.UserId = user.Id;
                //newcustomerinfo.DepartmentId = user.OrganizationId;
                //newcustomerinfo.UniqueId = item.UniqueId;
                //newcustomerinfo.CustomerStatus = CustomerStatus.ExistingCustomers;
                //newcustomerinfo.SourceId = item.Id;

                //var newcustomerdemand = JsonConvert.DeserializeObject<CustomerDemand>(JsonConvert.SerializeObject(customerdemand));
                //customerdemand.Id = Guid.NewGuid().ToString();
                //customerdemand.CustomerId = newcustomerinfo.Id;
                //customerdemand.IsDeleted = false;
                //customerdemand.UserId = user.Id;
                //customerdemand.DepartmentId = user.OrganizationId;

                var customerfu = new CustomerFollowUp
                {
                    Id = Guid.NewGuid().ToString(),
                    CustomerId = item.Id,
                    TypeId = CustomerFollowUpType.ClaimCustomer,
                    UserId = item.UserId,
                    DepartmentId = item.DepartmentId,
                    FollowUpTime = DateTime.Now,
                    TrueName = item.CustomerName,
                    FollowUpContents = "认领客户",
                    CustomerNo = item.CustomerNo,
                    IsRealFollow = false,
                    CreateTime = DateTime.Now,
                    CreateUser = user.Id
                };

                if (item != null)
                    Context.Update(item);
                //Context.Add(newcustomerinfo);
                if (customerfu != null)
                    Context.Add(customerfu);
                if (customerdemand != null)
                    Context.Update(customerdemand);
            }
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
            return true;
        }


        public Task<TResult> GetAsync<TResult>(Func<IQueryable<CustomerPool>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.CustomerPools.AsNoTracking()).SingleOrDefaultAsync(cancellationToken);
        }


        public Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<CustomerPool>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.CustomerPools.AsNoTracking()).ToListAsync(cancellationToken);
        }

        public async Task UpdateAsync(CustomerPool customerPool, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (customerPool == null)
            {
                throw new ArgumentNullException(nameof(customerPool));
            }
            Context.Attach(customerPool);
            Context.Update(customerPool);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public async Task UpdateListAsync(List<CustomerPool> customerPoolList, List<MigrationPoolHistory> migrationPoolHistory, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (customerPoolList == null || migrationPoolHistory == null)
            {
                throw new ArgumentNullException(nameof(customerPoolList));
            }
            var ids = customerPoolList.Select(b => b.CustomerId);

            var customerInfos = Context.CustomerInfos.Where(a => ids.Contains(a.Id)).ToList();

            customerInfos = customerInfos.Select(x =>
            {
                x.UserId = "";
                x.DepartmentId = customerPoolList.FirstOrDefault(a => a.CustomerId == x.Id).DepartmentId;
                x.FollowupTime = DateTime.Now;
                return x;
            }).ToList();

            Context.AttachRange(customerInfos);
            Context.UpdateRange(customerInfos);

            Context.AttachRange(customerPoolList);
            Context.UpdateRange(customerPoolList);

            Context.AddRange(migrationPoolHistory);
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
