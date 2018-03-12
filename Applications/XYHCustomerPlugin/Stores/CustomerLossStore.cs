using ApplicationCore.Dto;
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
    public class CustomerLossStore : ICustomerLossStore
    {
        public CustomerLossStore(CustomerDbContext customerDbContext)
        {
            Context = customerDbContext;
            CustomerLosss = Context.CustomerLosss;

        }

        protected CustomerDbContext Context { get; }

        public IQueryable<CustomerLoss> CustomerLosss { get; set; }


        public async Task<CustomerLoss> CreateAsync(CustomerLoss customerLoss, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (customerLoss == null)
            {
                throw new ArgumentNullException(nameof(customerLoss));
            }
            Context.Add(customerLoss);
            if (customerLoss.CustomerInfo != null)
            {
                Context.Attach(customerLoss.CustomerInfo);
                Context.Update(customerLoss.CustomerInfo);

                //插入跟进信息
                var followup = new CustomerFollowUp
                {
                    Id = Guid.NewGuid().ToString(),
                    CustomerId = customerLoss.CustomerInfo.Id,
                    TypeId = CustomerFollowUpType.Loss,
                    UserId = customerLoss.LossUserId,
                    DepartmentId = customerLoss.LossDepartmentId,
                    FollowUpTime = DateTime.Now,
                    TrueName = customerLoss.CustomerInfo.CustomerName,
                    FollowUpContents = "拉无效",
                    CustomerNo = customerLoss.CustomerInfo.CustomerNo,
                    IsRealFollow = false,
                    CreateTime = DateTime.Now,
                    CreateUser = customerLoss.LossUserId

                };
                Context.Add(followup);
            }

            

            


            await Context.SaveChangesAsync(cancellationToken);
            return customerLoss;
        }


        public async Task DeleteAsync(CustomerLoss customerLoss, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (customerLoss == null)
            {
                throw new ArgumentNullException(nameof(customerLoss));
            }
            Context.Remove(customerLoss);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public async Task DeleteListAsync(List<CustomerLoss> customerLossList, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (customerLossList == null)
            {
                throw new ArgumentNullException(nameof(customerLossList));
            }
            Context.RemoveRange(customerLossList);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public IQueryable<CustomerLoss> SimpleQuery()
        {
            var query = from cl in Context.CustomerLosss.AsNoTracking()
                        join ci in Context.CustomerInfos.AsNoTracking() on cl.CustomerId equals ci.Id into ci1
                        from customer in ci1.DefaultIfEmpty()
                            //user
                        join cu in Context.Users.AsNoTracking() on cl.LossUserId equals cu.Id into cu1
                        from cuser in cu1.DefaultIfEmpty()

                        select new CustomerLoss()
                        {
                            Id = cl.Id,
                            LossTime = cl.LossTime,
                            CustomerId = cl.CustomerId,
                            HouseTypeId = cl.HouseTypeId,
                            LossDepartmentId = cl.LossDepartmentId,
                            LossRemark = cl.LossRemark,
                            LossTypeId = cl.LossTypeId,
                            LossUserId = cuser.Id,
                            CustomerInfo = new CustomerInfo
                            {
                                CustomerName = customer.CustomerName
                            },
                            LossUser = new ApplicationCore.Models.Users
                            {
                                Id = cuser.Id,
                                OrganizationId = cuser.OrganizationId,
                                UserName = cuser.UserName,
                                TrueName = cuser.TrueName
                            }
                        };
            return query;
        }

        public async Task<bool> ActivateLossUser(UserInfo user, string Id, bool isDeleteOldData, CancellationToken cancellationToken = default(CancellationToken))
        {
            var customerLoss = await Context.CustomerLosss.AsNoTracking().Where(a => a.CustomerId == Id && !a.IsDeleted).FirstOrDefaultAsync(cancellationToken);
            if (isDeleteOldData)
            {

                if (customerLoss == null)
                {
                    return false;
                }
                var customer = await Context.CustomerInfos.AsNoTracking().Where(a => a.Id == customerLoss.CustomerId).FirstOrDefaultAsync(cancellationToken);
                if (customer == null)
                {
                    return false;
                }
                var customerdemand = await Context.CustomerDemands.AsNoTracking().Where(a => a.CustomerId == customerLoss.CustomerId).FirstOrDefaultAsync(cancellationToken);
                if (customerdemand == null)
                {
                    return false;
                }

                var oldcustomer = customer;
                oldcustomer.IsDeleted = true;
                oldcustomer.DeleteTime = DateTime.Now;
                oldcustomer.DeleteUser = user.Id;

                var newcustomerinfo = await Context.CustomerInfos.AsNoTracking().Where(a => a.Id == customerLoss.CustomerId).FirstOrDefaultAsync(cancellationToken); ;
                newcustomerinfo.Id = Guid.NewGuid().ToString();
                newcustomerinfo.IsDeleted = false;
                newcustomerinfo.CustomerStatus = CustomerStatus.ExistingCustomers;
                newcustomerinfo.FollowupTime = DateTime.Now;
                newcustomerinfo.SourceId = customer.Id;

                customerdemand.CustomerId = newcustomerinfo.Id;
                customerdemand.IsDeleted = false;

                customerLoss.IsDeleted = true;
                customerLoss.DeleteTime = DateTime.Now;
                customerLoss.DeleteUserId = user.Id;

                Context.Add(newcustomerinfo);
                Context.Attach(oldcustomer);
                Context.Update(oldcustomer);
                Context.Attach(customerdemand);
                Context.Update(customerdemand);
                Context.Attach(customerLoss);
                Context.Update(customerLoss);

                await Context.SaveChangesAsync(cancellationToken);
            }
            else
            {
                if (customerLoss == null)
                {
                    return false;
                }
                var customer = await Context.CustomerInfos.AsNoTracking().Where(a => a.Id == customerLoss.CustomerId).FirstOrDefaultAsync(cancellationToken);
                if (customer == null)
                {
                    return false;
                }
                var customerdemand = await Context.CustomerDemands.AsNoTracking().Where(a => a.CustomerId == customerLoss.CustomerId).FirstOrDefaultAsync(cancellationToken);
                if (customerdemand == null)
                {
                    return false;
                }
                customer.IsDeleted = false;
                customer.CustomerStatus = CustomerStatus.ExistingCustomers;
                customer.FollowUpNum = customer.FollowUpNum + 1;
                customer.FollowupTime = DateTime.Now;

                customerdemand.IsDeleted = false;

                customerLoss.IsDeleted = true;
                customerLoss.DeleteTime = DateTime.Now;
                customerLoss.DeleteUserId = user.Id;

                Context.Attach(customer);
                Context.Attach(customerdemand);
                Context.Attach(customerLoss);
                Context.Update(customer);
                Context.Update(customerdemand);
                Context.Update(customerLoss);





                //插入跟进信息
                var followup = new CustomerFollowUp
                {
                    Id = Guid.NewGuid().ToString(),
                    CustomerId = customer.Id,
                    TypeId = CustomerFollowUpType.Activation,
                    UserId = user.Id,
                    DepartmentId = user.OrganizationId,
                    FollowUpTime = DateTime.Now,
                    TrueName = customer.CustomerName,
                    FollowUpContents = "激活用户",
                    CustomerNo = customer.CustomerNo,
                    IsRealFollow = false,
                    CreateTime = DateTime.Now,
                    CreateUser = user.Id

                };
                Context.Add(followup);

                await Context.SaveChangesAsync(cancellationToken);
            }
            return true;
        }


        public Task<TResult> GetAsync<TResult>(Func<IQueryable<CustomerLoss>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.CustomerLosss.AsNoTracking()).SingleOrDefaultAsync(cancellationToken);
        }


        public Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<CustomerLoss>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.CustomerLosss.AsNoTracking()).ToListAsync(cancellationToken);
        }

        public async Task UpdateAsync(CustomerLoss customerLoss, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (customerLoss == null)
            {
                throw new ArgumentNullException(nameof(customerLoss));
            }
            Context.Attach(customerLoss);
            Context.Update(customerLoss);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException)
            {
                throw;
            }
        }

        public async Task UpdateListAsync(List<CustomerLoss> customerLossList, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (customerLossList == null)
            {
                throw new ArgumentNullException(nameof(customerLossList));
            }
            Context.AttachRange(customerLossList);
            Context.UpdateRange(customerLossList);
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
