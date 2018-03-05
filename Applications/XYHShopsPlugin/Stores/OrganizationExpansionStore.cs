using ApplicationCore.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XYHShopsPlugin.Models;

namespace XYHShopsPlugin.Stores
{
    public class OrganizationExpansionStore : IOrganizationExpansionStore
    {
        //Db
        protected ShopsDbContext Context { get; }


        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="shopsDbContext">Context</param>
        public OrganizationExpansionStore(ShopsDbContext shopsDbContext)
        {
            Context = shopsDbContext;
        }

        /// <summary>
        /// 根据部门ID获取一条部门展开信息
        /// </summary>
        /// <param name="departmentid"></param>
        /// <returns></returns>
        public string GetFullName(string departmentid)
        {
            if (string.IsNullOrEmpty(departmentid))
            {
                return "未找到相应部门";
            }
            var response = Context.OrganizationExpansions.AsNoTracking().Where(x => x.SonId == departmentid && x.Type == "Region").SingleOrDefault();
            return response == null ? "未找到相应部门" : response.FullName;
        }

        /// <summary>
        /// 根据某一成员获取一条部门展开信息
        /// </summary>
        /// <typeparam name="TResult">返回值</typeparam>
        /// <param name="query">参数</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public Task<TResult> GetAsync<TResult>(Func<IQueryable<OrganizationExpansion>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.OrganizationExpansions.AsNoTracking()).SingleOrDefaultAsync(cancellationToken);
        }

        /// <summary>
        /// 根据某一成员获取列表部门展开信息
        /// </summary>
        /// <typeparam name="TResult">返回值</typeparam>
        /// <param name="query">参数</param>
        /// <param name="cancellationToken">验证</param>
        /// <returns></returns>
        public Task<List<TResult>> ListAsync<TResult>(Func<IQueryable<OrganizationExpansion>, IQueryable<TResult>> query, CancellationToken cancellationToken = default(CancellationToken))
        {
            if (query == null)
            {
                throw new ArgumentNullException(nameof(query));
            }
            return query.Invoke(Context.OrganizationExpansions.AsNoTracking()).ToListAsync(cancellationToken);
        }
    }
}
