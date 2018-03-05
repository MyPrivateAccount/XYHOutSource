using AuthorizationCenter.Dto;
using AuthorizationCenter.Models;
using AuthorizationCenter.ViewModels;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AuthorizationCenter.Stores
{
    public class UserRoleStore<TContext> : IUserRoleStore where TContext : ApplicationDbContext
    {
        public UserRoleStore(TContext context)
        {
            Context = context ?? throw new ArgumentNullException(nameof(context));
            UserRoles = Context.UserRoles;
        }

        protected virtual TContext Context { get; }
        public IQueryable<UserRole> UserRoles { get; set; }


        //public async Task CreateListAsync(string userId, AddUserToRolesExtRequest roles)
        //{
        //    if (roles == null)
        //    {
        //        throw new ArgumentNullException(nameof(roles));
        //    }
        //    try
        //    {
        //        List<UserRole> ur = new List<UserRole>();
        //        List<RolePermission> rp = new List<RolePermission>();
        //        foreach (var item in roles.Roles.Distinct())
        //        {
        //            if (await Context.UserRoles.AnyAsync(a => a.UserId == userId && a.RoleId == item))
        //            {
        //                continue;
        //            }
        //            else
        //            {
        //                ur.Add(new UserRole { RoleId = item, UserId = userId, IsAgent = false });
        //            }
        //        }
        //        foreach (var item in roles.AgentRoles)
        //        {
        //            if (await Context.UserRoles.AnyAsync(a => a.UserId == userId && a.RoleId == item.Role))
        //            {
        //                continue;
        //            }
        //            else
        //            {
        //                ur.Add(new UserRole { RoleId = item.Role, UserId = userId, IsAgent = true });
        //            }
        //        }
        //        //foreach (var item in ur)
        //        //{}
        //        //移除老数据
        //        var rolePermissions = Context.RolePermissions.Where(a => ur.Select(b => b.RoleId).Contains(a.RoleId));
        //        var permissions = Context.PermissionExpansions.Where(a => a.UserId == userId && rolePermissions.Select(b => b.PermissionId).Contains(a.PermissionId));
        //        Context.RemoveRange(permissions);
                
        //        foreach (var item in rolePermissions)
        //        {
        //        }
        //        foreach (var item in rolePermissions)
        //        {
        //            var role = Context.Roles.FirstOrDefaultAsync(a => a.Id == item.RoleId, CancellationToken.None).Result;
        //            if (role == null)
        //            {
        //                var roleP = Context.RolePermissions.Where(a => a.RoleId == item.RoleId);
        //                Context.RemoveRange(roleP);
        //                continue;
        //            }
        //        }
        //        Context.AddRange(ur);
        //        await Context.SaveChangesAsync();
        //    }
        //    catch (DbUpdateConcurrencyException e)
        //    {
        //        throw;
        //    }
        //}



        //private async Task<List<OrganizationScopeModel>> OrganizationFilter(List<OrganizationScopeModel> newlist, List<OrganizationScopeModel> list)
        //{
        //    if (list?.Count > 0)
        //    {
        //        var sons = await _organizationExpansionManager.GetAllSonIdAsync(list[0].OrganizationId);
        //        var parents = await _organizationExpansionManager.GetAllParentIdAsync(list[0].OrganizationId);
        //        var listid = list.Select(a => a.OrganizationId);
        //        bool haveParent = parents?.Any(a => listid.Contains(a)) ?? false;
        //        bool havaSon = sons?.Any(a => listid.Contains(a)) ?? false;
        //        if (!havaSon && !haveParent)
        //        {
        //            newlist.Add(list[0]);
        //            list.RemoveAt(0);
        //        }
        //        else
        //        {
        //            if (havaSon)
        //            {
        //                list.RemoveAll(new Predicate<OrganizationScopeModel>(a => sons.Contains(a.OrganizationId)));
        //            }
        //            if (haveParent)
        //            {
        //                list.RemoveAt(0);
        //            }
        //        }
        //        return await OrganizationFilter(newlist, list);
        //    }
        //    else
        //    {
        //        return newlist;
        //    }
        //}


        public IQueryable<string> GetRoleIdsAsync(string userId, CancellationToken cancellationToken)
        {
            return Context.UserRoles.Where(a => a.UserId == userId).Select(b => b.RoleId);
        }

        public IQueryable<string> GetRoleIdsAsync(List<string> userIds, CancellationToken cancellationToken)
        {
            return Context.UserRoles.Where(a => userIds.Contains(a.UserId)).Select(b => b.RoleId);
        }

        public IQueryable<string> GetUserIdsAsync(string roleId, CancellationToken cancellationToken)
        {
            return Context.UserRoles.Where(a => a.RoleId == roleId).Select(b => b.RoleId);
        }
        public IQueryable<string> GetUserIdsAsync(List<string> roleIds, CancellationToken cancellationToken)
        {
            return Context.UserRoles.Where(a => roleIds.Contains(a.UserId)).Select(b => b.RoleId);
        }

    }
}
