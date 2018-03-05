using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AuthorizationCenter.Models;
using Microsoft.EntityFrameworkCore;

namespace AuthorizationCenter.Stores
{


    public class UserExtensionsStore<TContext> : IUserExtensionsStore where TContext : ApplicationDbContext
    {
        public UserExtensionsStore(TContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
        }

        protected virtual TContext Context { get; }

        public async Task<bool> DeleteUserParamaters(List<UserExtensions> parameters, CancellationToken cancellationToken)
        {
            if(parameters!=null && parameters.Count > 0)
            {
                Context.UserExtensions.AttachRange(parameters);
                Context.RemoveRange(parameters);
                await Context.SaveChangesAsync(cancellationToken);
            }
            return true;
        }

        public async Task<List<UserExtensions>> GetUserParameters(string userId, List<string> parameterNames, CancellationToken cancellationToken)
        {
            var q = from p in Context.UserExtensions.AsNoTracking()
                    where p.UserId == userId
                    select p;
            if(parameterNames!=null && parameterNames.Count > 0)
            {
                q = q.Where(p => parameterNames.Contains(p.ParName));
            }

            return await q.ToListAsync(cancellationToken);
        }

        public async Task<bool> SaveUserParamaters(List<UserExtensions> parameters, CancellationToken cancellationToken)
        {
            if (parameters == null || parameters.Count == 0)
                return true;

            foreach (UserExtensions par in parameters)
            {
                if (Context.UserExtensions.Any(x => x.UserId == par.UserId && x.ParName == par.ParName))
                {
                    //存在，更新
                    par.UpdateDate = DateTime.Now;
                    Context.Attach(par);

                    var entry = Context.Entry(par);
                    entry.Property(x => x.ParValue).IsModified = true;
                    entry.Property(x => x.ParValue2).IsModified = true;
                    entry.Property(x => x.UpdateDate).IsModified = true;
                }
                else
                {
                    //新增
                    par.CreateDate = DateTime.Now;
                    Context.UserExtensions.Add(par);
                }
            }


            await Context.SaveChangesAsync(cancellationToken);

            return true;

        }

        public async Task<List<UserExtensions>> GetUserParameters(List<string> userIds, string parameterNames, CancellationToken cancellationToken)
        {
            var q = from p in Context.UserExtensions.AsNoTracking()
                    where p.ParName == parameterNames
                    select p;
            if( userIds!=null && userIds.Count > 0)
            {
                q = q.Where(p => userIds.Contains(p.UserId));
            }

            return await q.ToListAsync(cancellationToken);
        }
    }
}
