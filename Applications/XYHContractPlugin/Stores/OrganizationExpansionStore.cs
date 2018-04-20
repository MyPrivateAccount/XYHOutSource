using ApplicationCore.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using XYH.Core.Log;
using XYHContractPlugin.Models;

namespace XYHContractPlugin.Stores
{
    public class OrganizationExpansionStore : IOrganizationExpansionStore
    {
        private ILogger Logger = LoggerManager.GetLogger("ContractOrganizationExpansionStore");
        //Db
        protected ContractDbContext Context { get; }


        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="ContractDbContext">Context</param>
        public OrganizationExpansionStore(ContractDbContext customerDbContext)
        {
            Context = customerDbContext;
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
            try
            {
                var response = Context.OrganizationExpansions.AsNoTracking().Where(x => (x.SonId == departmentid ) || (x.OrganizationId == departmentid )).FirstOrDefault();
                return response == null ? "未找到相应部门" : response.FullName;
            }
            catch (Exception e)
            {
                Logger.Error("获取部门名称失败:{0}\r\n{1}", departmentid, e.ToString());
                throw;
            }
        }


        /// <summary>
        /// 根据用户ID获取一条部门展开信息
        /// </summary>
        /// <param name="userid"></param>
        /// <returns></returns>
        public string GetFullNameByUserId(string userid)
        {
            if (string.IsNullOrEmpty(userid))
            {
                return "未找到相应部门";
            }
            var departmentid = Context.Users.FirstOrDefault(x => x.Id == userid).OrganizationId;
            var response = Context.OrganizationExpansions.AsNoTracking().Where(x => (x.SonId == departmentid && x.Type == "Region") || (x.OrganizationId == departmentid && x.Type == "Region")).FirstOrDefault();
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
