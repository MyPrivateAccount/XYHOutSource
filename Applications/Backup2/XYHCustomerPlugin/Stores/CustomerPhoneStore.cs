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
    /// 用户电话
    /// </summary>
    public class CustomerPhoneStore : ICustomerPhoneStore
    {
        ///Db
        protected CustomerDbContext Context { get; }


        public IQueryable<CustomerPhone> CustomerPhones { get; set; }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="customerDbContext">Context</param>
        public CustomerPhoneStore(CustomerDbContext customerDbContext)
        {
            Context = customerDbContext;
            CustomerPhones = Context.CustomerPhones;
        }

        ///获取所有电话信息
        public IQueryable<CustomerPhone> CustomerPhoneAll()
        {
            var query = from b in Context.CustomerPhones.AsNoTracking()

                        join cu in Context.CustomerInfos.AsNoTracking() on b.CustomerId equals cu.Id
                        select new CustomerPhone()
                        {
                            Id = b.Id,
                            Phone = b.Phone,
                            IsMain = b.IsMain,
                            CustomerId = b.CustomerId,
                            IsDeleted = b.IsDeleted,
                            CreateTime = b.CreateTime,
                            CreateUser = b.CreateUser,
                            UserId = cu.UserId,
                            DepartmentId = cu.DepartmentId,
                            DeleteTime = b.DeleteTime
                        };

            return query;
        }

        /// <summary>
        /// 根据某一成员获取一条客户电话信息
        /// </summary>
        /// <typeparam name="TResult">返回值</typeparam>
        /// <param name="query">参数</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public Task<TResult> GetAsync<TResult>(Func<IQueryable<CustomerPhone>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.CustomerPhones.AsNoTracking()).SingleOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// 根据某一成员获取列表客户电话信息
        /// </summary>
        /// <typeparam name="TResult">返回值</typeparam>
        /// <param name="query">参数</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<CustomerPhone>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.CustomerPhones.AsNoTracking()).ToListAsync(cancellationToken);
        }

        /// <summary>
        /// 新增客户电话信息
        /// </summary>
        /// <param name="customerphone">实体</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public async Task<CustomerPhone> CreateAsync(CustomerPhone customerphone, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (customerphone == null)
            {
                throw new ArgumentNullException(nameof(customerphone));
            }
            Context.Add(customerphone);
            await Context.SaveChangesAsync(cancellationToken);
            return customerphone;
        }

        /// <summary>
        /// 删除客户电话
        /// </summary>
        /// <param name="user">登陆用户基本信息</param>
        /// <param name="customerphone">客户电话实体</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public async Task DeleteAsync(SimpleUser user, CustomerPhone customerphone, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (customerphone == null)
            {
                throw new ArgumentNullException(nameof(customerphone));
            }
            //删除基本信息
            customerphone.DeleteTime = DateTime.Now;
            customerphone.DeleteUser = user.Id;
            customerphone.IsDeleted = true;
            Context.Attach(customerphone);
            var entry = Context.Entry(customerphone);
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
        /// <param name="customerphoneList">集合</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public async Task DeleteListAsync(List<CustomerPhone> customerphoneList, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (customerphoneList == null)
            {
                throw new ArgumentNullException(nameof(customerphoneList));
            }
            Context.RemoveRange(customerphoneList);
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
        /// 修改客户电话信息
        /// </summary>
        /// <param name="customerphone"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task UpdateAsync(CustomerPhone customerphone, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (customerphone == null)
            {
                throw new ArgumentNullException(nameof(customerphone));
            }
            Context.Attach(customerphone);
            Context.Update(customerphone);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { }
        }

        /// <summary>
        /// 批量修改客户电话信息(一般修改删除状态)
        /// </summary>
        /// <param name="customerphones"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task UpdateListAsync(List<CustomerPhone> customerphones, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (customerphones == null)
            {
                throw new ArgumentNullException(nameof(customerphones));
            }
            Context.AttachRange(customerphones);
            Context.UpdateRange(customerphones);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { }
        }
    }
}
