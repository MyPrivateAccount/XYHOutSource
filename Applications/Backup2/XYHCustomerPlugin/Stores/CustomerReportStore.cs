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
    public class CustomerReportStore : ICustomerReportStore
    {
        //Db
        protected CustomerDbContext Context { get; }


        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="customerDbContext">Context</param>
        public CustomerReportStore(CustomerDbContext customerDbContext)
        {
            Context = customerDbContext;
        }

        //获取所有报备信息
        public IQueryable<CustomerReport> CustomerReportAll()
        {
            //var query = from b in Context.CustomerReports.AsNoTracking()
            //            join basic1 in Context.CustomerDemands.AsNoTracking() on b.Id equals basic1.CustomerId into basic2
            //            from basic in basic2.DefaultIfEmpty()

            //                //所属人及部门
            //            join cu in Context.Users.AsNoTracking() on b.UserId equals cu.Id
            //            join ru1a in Context.Organizations.AsNoTracking() on b.DepartmentId equals ru1a.Id into ru1b
            //            from ru1 in ru1b.DefaultIfEmpty()
            //            select new CustomerReport()
            //            {
            //                Id = b.Id,
            //                UserName = cu.TrueName,
            //                DepartmentName = ru1.OrganizationName,
            //                CustomerDemand = basic,
            //                Mark=b.Mark
            //            };

            return Context.CustomerReports;
        }

        /// <summary>
        /// 根据某一成员获取一条报备信息
        /// </summary>
        /// <typeparam name="TResult">返回值</typeparam>
        /// <param name="query">参数</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public Task<TResult> GetAsync<TResult>(Func<IQueryable<CustomerReport>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.CustomerReports.AsNoTracking()).SingleOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// 根据某一成员获取列表报备信息
        /// </summary>
        /// <typeparam name="TResult">返回值</typeparam>
        /// <param name="query">参数</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<CustomerReport>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.CustomerReports.AsNoTracking()).ToListAsync(cancellationToken);
        }

        /// <summary>
        /// 新增报备信息
        /// </summary>
        /// <param name="customerreport">实体</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public async Task<CustomerReport> CreateAsync(CustomerReport customerreport, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (customerreport == null)
            {
                throw new ArgumentNullException(nameof(customerreport));
            }
            Context.Add(customerreport);

            Context.Add(new CustomerFollowUp
            {
                Id = Guid.NewGuid().ToString(),
                CustomerId = customerreport.CustomerId,
                TypeId = CustomerFollowUpType.CustomerReported,
                UserId = customerreport.UserId,
                DepartmentId = customerreport.DepartmentId,
                FollowUpTime = customerreport.CreateTime,
                TrueName = customerreport.CustomerInfo.CustomerName,
                FollowUpContents = "带看报备",
                CustomerNo = customerreport.CustomerInfo.CustomerNo,
                IsRealFollow = false,
                CreateTime = DateTime.Now,
                CreateUser = customerreport.UserId

            });

            await Context.SaveChangesAsync(cancellationToken);
            return customerreport;
        }

        /// <summary>
        /// 删除报备
        /// </summary>
        /// <param name="user">登陆用户基本信息</param>
        /// <param name="customerreport">报备实体</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public async Task DeleteAsync(SimpleUser user, CustomerReport customerreport, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (customerreport == null)
            {
                throw new ArgumentNullException(nameof(customerreport));
            }
            //删除基本信息
            customerreport.DeleteTime = DateTime.Now;
            customerreport.DeleteUser = user.Id;
            customerreport.IsDeleted = true;
            Context.Attach(customerreport);
            var entry = Context.Entry(customerreport);
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
        /// <param name="customerreportList">集合</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public async Task DeleteListAsync(List<CustomerReport> customerreportList, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (customerreportList == null)
            {
                throw new ArgumentNullException(nameof(customerreportList));
            }
            Context.RemoveRange(customerreportList);
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
        /// 修改报备信息
        /// </summary>
        /// <param name="customerreport"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task UpdateAsync(CustomerReport customerreport, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (customerreport == null)
            {
                throw new ArgumentNullException(nameof(customerreport));
            }
            Context.Attach(customerreport);
            Context.Update(customerreport);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { }
        }

        /// <summary>
        /// 批量修改报备信息(一般修改删除状态)
        /// </summary>
        /// <param name="customerreports"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task UpdateListAsync(List<CustomerReport> customerreports, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (customerreports == null)
            {
                throw new ArgumentNullException(nameof(customerreports));
            }
            Context.AttachRange(customerreports);
            Context.UpdateRange(customerreports);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { }
        }
    }
}
