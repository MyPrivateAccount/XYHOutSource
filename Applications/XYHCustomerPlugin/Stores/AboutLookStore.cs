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
    public class AboutLookStore : IAboutLookStore
    {
        //Db
        protected CustomerDbContext Context { get; }

        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="customerDbContext">Context</param>
        public AboutLookStore(CustomerDbContext customerDbContext)
        {
            Context = customerDbContext;
        }

        public IQueryable<AboutLook> AboutLookAll()
        {
            return Context.AboutLooks;
        }

        /// <summary>
        /// 根据某一成员获取一条约看信息
        /// </summary>
        /// <typeparam name="TResult">返回值</typeparam>
        /// <param name="query">参数</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public Task<TResult> GetAsync<TResult>(Func<IQueryable<AboutLook>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.AboutLooks.AsNoTracking()).SingleOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// 根据某一成员获取列表约看信息
        /// </summary>
        /// <typeparam name="TResult">返回值</typeparam>
        /// <param name="query">参数</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<AboutLook>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.AboutLooks.AsNoTracking()).ToListAsync(cancellationToken);
        }

        /// <summary>
        /// 新增带看信息
        /// </summary>
        /// <param name="aboutLook">实体</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public async Task<AboutLook> CreateAsync(AboutLook aboutLook, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (aboutLook == null)
            {
                throw new ArgumentNullException(nameof(aboutLook));
            }
            Context.Add(aboutLook);
            await Context.SaveChangesAsync(cancellationToken);
            return aboutLook;
        }

        /// <summary>
        /// 修改约看信息
        /// </summary>
        /// <param name="aboutLook"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task UpdateAsync(AboutLook aboutLook, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (aboutLook == null)
            {
                throw new ArgumentNullException(nameof(aboutLook));
            }
            Context.Attach(aboutLook);
            Context.Update(aboutLook);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { }
        }

        /// <summary>
        /// 批量修改约看信息(一般修改删除状态)
        /// </summary>
        /// <param name="aboutLook"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task UpdateListAsync(List<AboutLook> aboutLooks, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (aboutLooks == null)
            {
                throw new ArgumentNullException(nameof(aboutLooks));
            }
            Context.AttachRange(aboutLooks);
            Context.UpdateRange(aboutLooks);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { }
        }

        /// <summary>
        /// 删除约看
        /// </summary>
        /// <param name="user">登陆用户基本信息</param>
        /// <param name="aboutLook">约看实体</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public async Task DeleteAsync(SimpleUser user, AboutLook aboutLook, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (user == null)
            {
                throw new ArgumentNullException(nameof(user));
            }

            if (aboutLook == null)
            {
                throw new ArgumentNullException(nameof(aboutLook));
            }
            //删除基本信息
            aboutLook.DeleteTime = DateTime.Now;
            aboutLook.DeleteUser = user.Id;
            aboutLook.IsDeleted = true;
            Context.Attach(aboutLook);
            var entry = Context.Entry(aboutLook);
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
