using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XYHBaseDataPlugin.Models;

namespace XYHBaseDataPlugin.Stores
{
    public class UserTypeValueStore: IUserTypeValueStore
    {
        //Db
        protected BaseDataDbContext Context { get; }


        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="customerDbContext">Context</param>
        public UserTypeValueStore(BaseDataDbContext shopsDbContext)
        {
            Context = shopsDbContext;
        }

        //获取所有用户定义类型信息
        public IQueryable<UserTypeValue> UserTypeValueAll()
        {
            return Context.UserTypeValues;
        }

        /// <summary>
        /// 根据某一成员获取一条用户定义类型信息
        /// </summary>
        /// <typeparam name="TResult">返回值</typeparam>
        /// <param name="query">参数</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public Task<TResult> GetAsync<TResult>(Func<IQueryable<UserTypeValue>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.UserTypeValues.AsNoTracking()).SingleOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// 根据某一成员获取列表用户定义类型信息
        /// </summary>
        /// <typeparam name="TResult">返回值</typeparam>
        /// <param name="query">参数</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<UserTypeValue>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.UserTypeValues.AsNoTracking()).ToListAsync(cancellationToken);
        }

        /// <summary>
        /// 新增用户定义类型信息
        /// </summary>
        /// <param name="usertypevalue">实体</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public async Task<UserTypeValue> CreateAsync(UserTypeValue usertypevalue, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (usertypevalue == null)
            {
                throw new ArgumentNullException(nameof(usertypevalue));
            }
            Context.Add(usertypevalue);
            await Context.SaveChangesAsync(cancellationToken);
            return usertypevalue;
        }

        /// <summary>
        /// 从数据库中删除List
        /// </summary>
        /// <param name="usertypevalueList">集合</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public async Task DeleteListAsync(List<UserTypeValue> usertypevalueList, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (usertypevalueList == null)
            {
                throw new ArgumentNullException(nameof(usertypevalueList));
            }
            Context.RemoveRange(usertypevalueList);
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
        /// 修改用户定义类型信息
        /// </summary>
        /// <param name="usertypevalue"></param>
        /// <param name="cancellationToken"></param>
        /// <returns></returns>
        public async Task UpdateAsync(UserTypeValue usertypevalue, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (usertypevalue == null)
            {
                throw new ArgumentNullException(nameof(usertypevalue));
            }
            Context.Attach(usertypevalue);
            Context.Update(usertypevalue);
            try
            {
                await Context.SaveChangesAsync(cancellationToken);
            }
            catch (DbUpdateConcurrencyException) { }
        }
    }
}
