using AuthorizationCenter.Models;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AuthorizationCenter.Stores
{
    public interface IUserRoleStore
    {
        IQueryable<UserRole> UserRoles { get; set; }
        //Task CreateListAsync(List<UserRole> userRoles);
        IQueryable<string> GetRoleIdsAsync(string userId, CancellationToken cancellationToken);
        IQueryable<string> GetRoleIdsAsync(List<string> userIds, CancellationToken cancellationToken);
        IQueryable<string> GetUserIdsAsync(string roleId, CancellationToken cancellationToken);
        IQueryable<string> GetUserIdsAsync(List<string> roleIds, CancellationToken cancellationToken);
    }
}
