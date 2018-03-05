using AuthorizationCenter.Dto;
using AuthorizationCenter.Models;
using AuthorizationCenter.Stores;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AuthorizationCenter.Managers
{
    public class UserLoginLogManager
    {

        public UserLoginLogManager(IUserLoginLogStore userLoginLogStore)
        {
            Store = userLoginLogStore ?? throw new ArgumentNullException(nameof(userLoginLogStore));
        }

        protected IUserLoginLogStore Store { get; }


        public virtual async Task<UserLoginLog> CreateAsync(UserLoginLog userLoginLog, CancellationToken cancellationToken)
        {
            if (userLoginLog == null)
            {
                throw new ArgumentNullException(nameof(userLoginLog));
            }
            return await Store.CreateAsync(userLoginLog, cancellationToken);
        }

        public virtual Task<List<UserLoginLog>> FindByUserIdAsync(string userId, CancellationToken cancellationToken)
        {
            return Store.ListAsync(a => a.Where(b => b.UserId == userId), cancellationToken);
        }


        public virtual async Task<PagingResponseMessage<UserLoginLog>> Search(UserLoginLogSearchCondition condition, CancellationToken cancellationToken)
        {
            PagingResponseMessage<UserLoginLog> pagingResponse = new PagingResponseMessage<UserLoginLog>();
            if (condition == null)
            {
                throw new ArgumentNullException(nameof(condition));
            }
            var query = Store.UserLoginLogs;
            if (condition?.OrganizationIds?.Count > 0)
            {
                query = query.Where(a => condition.OrganizationIds.Contains(a.OrganizationId));
            }
            if (!string.IsNullOrEmpty(condition.KeyWord))
            {
                query = query.Where(a => a.UserName.Contains(condition.KeyWord) || a.TrueName.Contains(condition.KeyWord));
            }
            if (condition?.UserIds?.Count > 0)
            {
                query = query.Where(a => condition.UserIds.Contains(a.UserId));
            }
            if (condition?.StartTime != null)
            {
                query = query.Where(a => a.LoginTime >= condition.StartTime.Value);
            }
            if (condition?.EndTime != null)
            {
                query = query.Where(a => a.LoginTime <= condition.EndTime.Value);
            }
            pagingResponse.TotalCount = await query.CountAsync();
            var resulte = await query.OrderByDescending(a => a.LoginTime).Skip(condition.PageIndex * condition.PageSize).Take(condition.PageSize).ToListAsync(cancellationToken);
            pagingResponse.PageIndex = condition.PageIndex;
            pagingResponse.PageSize = condition.PageSize;
            pagingResponse.Extension = resulte;
            return pagingResponse;
        }

    }
}
