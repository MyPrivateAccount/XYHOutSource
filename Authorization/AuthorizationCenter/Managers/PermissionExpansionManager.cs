using AuthorizationCenter.Dto;
using AuthorizationCenter.Models;
using AuthorizationCenter.Stores;
using AuthorizationCenter.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AuthorizationCenter.Managers
{
    public class PermissionExpansionManager
    {
        public PermissionExpansionManager(IPermissionExpansionStore permissionExpansionStore,
            IPermissionItemStore permissionItemStore,
            IRolePermissionStore rolePermissionStore,
            UserStore<Users> userStore,
            IPermissionOrganizationStore permissionOrganizationStore,
            IOrganizationExpansionStore organizationExpansionStore,
            IRoleStore<Roles> roleStore
            )
        {
            Store = permissionExpansionStore ?? throw new ArgumentNullException(nameof(permissionExpansionStore));
            _permissionItemStore = permissionItemStore ?? throw new ArgumentNullException(nameof(permissionItemStore));
            _rolePermissionStore = rolePermissionStore ?? throw new ArgumentNullException(nameof(rolePermissionStore));
            _userStore = userStore ?? throw new ArgumentNullException(nameof(userStore));
            _roleStore = roleStore ?? throw new ArgumentNullException(nameof(roleStore));
            _permissionOrganizationStore = permissionOrganizationStore ?? throw new ArgumentNullException(nameof(permissionOrganizationStore));
            _organizationExpansionStore = organizationExpansionStore ?? throw new ArgumentNullException(nameof(organizationExpansionStore));
        }

        protected IPermissionExpansionStore Store { get; }
        protected IPermissionItemStore _permissionItemStore { get; }
        protected IRolePermissionStore _rolePermissionStore { get; }
        protected UserStore<Users> _userStore { get; }
        protected IRoleStore<Roles> _roleStore { get; }
        protected IPermissionOrganizationStore _permissionOrganizationStore { get; }
        protected IOrganizationExpansionStore _organizationExpansionStore { get; }


        public virtual async Task Expansion()
        {
            var oldData = await Store.ListAsync(a => a.Where(b => true));
            if (oldData?.Count > 0)
            {
                Store.DeleteListAsync(oldData).Wait();
            }
            List<OrganizationExpansion> list = new List<OrganizationExpansion>();
            var result = await GetAllSonExpansion();
            await Store.CreateListAsync(result);
        }


        public virtual async Task<bool> HavePermission(string userId, string permissionId)
        {
            var permissions = await Store.ListAsync(a => a.Where(b => b.UserId == userId && b.PermissionId == permissionId));
            if (permissions?.Count > 0)
            {
                return true;
            }
            return false;
        }

        public virtual async Task<List<HavePermissionResponse>> GetEachPermission(string userId, List<string> permissionIds)
        {
            List<HavePermissionResponse> list = new List<HavePermissionResponse>();
            var permissions = await Store.ListAsync(a => a.Where(b => b.UserId == userId && permissionIds.Contains(b.PermissionId)));
            if (permissions?.Count > 0)
            {
                foreach (var item in permissionIds)
                {
                    if (permissions.Any(a => a.PermissionId == item))
                    {
                        list.Add(new HavePermissionResponse { PermissionItem = item, IsHave = true });
                    }
                    else
                    {
                        list.Add(new HavePermissionResponse { PermissionItem = item, IsHave = false });
                    }
                }
                return list;
            }
            foreach (var item in permissionIds)
            {
                list.Add(new HavePermissionResponse { PermissionItem = item, IsHave = false });
            }
            return list;
        }


        public virtual async Task<bool> HavePermission(string userId, List<string> permissionIds)
        {
            List<HavePermissionResponse> list = new List<HavePermissionResponse>();
            var permissions = await Store.ListAsync(a => a.Where(b => b.UserId == userId && permissionIds.Contains(b.PermissionId)));
            if (permissions?.Count > 0)
            {
                foreach (var item in permissionIds)
                {
                    if (!permissions.Any(a => a.PermissionId == item))
                    {
                        return false;
                    }
                }
                return true;
            }
            return false;
        }




        public virtual async Task<bool> HavePermission(string userId, string permissionId, string organizationId)
        {
            var permissions = await Store.ListAsync(a => a.Where(b => b.UserId == userId && b.PermissionId == permissionId));
            if (permissions?.Count == 0)
            {
                return false;
            }
            var Ids = permissions.Select(a => a.OrganizationId);
            var organizationIds = (await _organizationExpansionStore.ListAsync(a => a.Where(b => Ids.Contains(b.OrganizationId)))).Select(o => o.SonId).Distinct().ToList();
            organizationIds.AddRange(Ids);
            if (organizationIds.Contains(organizationId))
            {
                return true;
            }
            return false;
        }

        public virtual async Task<List<string>> GetOrganizationOfPermission(string userId, string permissionId)
        {
            List<string> list = new List<string>();
            var permissions = await Store.ListAsync(a => a.Where(b => b.UserId == userId && b.PermissionId == permissionId));
            if (permissions?.Count == 0)
            {
                return list;
            }
            var Ids = permissions.Select(a => a.OrganizationId);
            var organizationIds = (await _organizationExpansionStore.ListAsync(a => a.Where(b => Ids.Contains(b.OrganizationId)))).Select(o => o.SonId).Distinct().ToList();
            organizationIds.AddRange(Ids);
            return organizationIds;
        }

        public virtual async Task<List<string>> GetApplicationOfPermission(string userId)
        {
            List<string> list = new List<string>();
            var permissions = await Store.GetApplicationOfPermission(userId);
            if (permissions?.Count == 0)
            {
                return list;
            }
            return permissions;
        }

        public virtual async Task AddUserInRolesAsync(List<string> roleIds, string userName)
        {
            //获取用户
            var user = await _userStore.FindByNameAsync(userName);
            //先删除已经存在的数据
            await RemoveUserFromRoleAsync(roleIds, user.Id);
            //获取角色权限表中传入角色所有数据项
            var rolePermissions = await _rolePermissionStore.ListAsync(a => a.Where(b => roleIds.Contains(b.RoleId)), CancellationToken.None);

            List<PermissionExpansion> list = new List<PermissionExpansion>();
            foreach (var p in rolePermissions)
            {
                var item = await _permissionItemStore.GetAsync(a => a.Where(b => b.Id == p.PermissionId), CancellationToken.None);
                if (item == null)
                {
                    await _rolePermissionStore.DeleteByPermissionIdAsync(p.PermissionId, CancellationToken.None);
                    continue;
                }
                var role = _roleStore.FindByIdAsync(p.RoleId, CancellationToken.None).Result;
                if (role == null)
                {
                    await _rolePermissionStore.DeleteByRoleIdAsync(p.RoleId, CancellationToken.None);
                    continue;
                }
                var organization = await _permissionOrganizationStore.ListAsync(a => a.Where(b => b.OrganizationScope == p.OrganizationScope), CancellationToken.None);
                foreach (var o in organization)
                {
                    //本组织
                    if (o.OrganizationId == OrganizationScopeDefines.Department)
                    {
                        list.Add(new PermissionExpansion()
                        {
                            OrganizationId = user.OrganizationId,
                            ApplicationId = item.ApplicationId,
                            PermissionId = p.PermissionId,
                            UserId = user.Id,
                            UpdateTime = DateTime.Now
                        });
                    }//分公司
                    else if (o.OrganizationId == OrganizationScopeDefines.Filiale)
                    {
                        list.Add(new PermissionExpansion()
                        {
                            OrganizationId = user.FilialeId,
                            ApplicationId = item.ApplicationId,
                            PermissionId = p.PermissionId,
                            UserId = user.Id,
                            UpdateTime = DateTime.Now
                        });
                    }//全部
                    else if (o.OrganizationId == OrganizationScopeDefines.All)
                    {
                        list.Add(new PermissionExpansion()
                        {
                            OrganizationId = "0",
                            ApplicationId = item.ApplicationId,
                            PermissionId = p.PermissionId,
                            UserId = user.Id,
                            UpdateTime = DateTime.Now
                        });
                    }
                    else
                    {
                        //其他直接赋值
                        list.Add(new PermissionExpansion()
                        {
                            OrganizationId = o.OrganizationId,
                            ApplicationId = item.ApplicationId,
                            PermissionId = p.PermissionId,
                            UserId = user.Id,
                            UpdateTime = DateTime.Now
                        });
                    }
                }
            }
            if (list.Count > 0)
            {
                await Store.CreateListAsync(list.Distinct(new ExpansionEqComparer()).ToList());
            }
        }


        public virtual async Task RemoveUserFromRoleAsync(List<string> roleIds, string userid)
        {
            var rolePermissions = await _rolePermissionStore.ListAsync(a => a.Where(b => roleIds.Contains(b.RoleId)), CancellationToken.None);
            var permissions = await Store.ListAsync(a => a.Where(b => b.UserId == userid && rolePermissions.Select(x => x.PermissionId).Contains(b.PermissionId)));
            await Store.DeleteListAsync(permissions);
        }

        //public virtual async Task UpdatePermissionAsync(string roleName, List<ApplicationPermissionModel> permissionModelList)
        //{
        //    RemoveRoleAsync(roleName).GetAwaiter().GetResult();
        //    var ulist = await _userStore.GetUsersInRoleAsync(roleName);
        //    List<PermissionExpansion> list = new List<PermissionExpansion>();
        //    foreach (var l in permissionModelList)
        //    {
        //        foreach (var p in l.Permissions)
        //        {
        //            foreach (var u in ulist)
        //            {
        //                //var department = await _organizationExpansionStore.ListAsync(a => a.Where(b => b.OrganizationId == u.OrganizationId));
        //                //var filiale = await _organizationExpansionStore.ListAsync(a => a.Where(b => b.OrganizationId == u.FilialeId));
        //                //department.Add(new OrganizationExpansion() { OrganizationId = u.OrganizationId, SonId = u.OrganizationId });
        //                //filiale.Add(new OrganizationExpansion() { OrganizationId = u.FilialeId, SonId = u.FilialeId });
        //                foreach (var o in p.Organizations)
        //                {
        //                    if (o.OrganizationId == OrganizationScopeDefines.Department)
        //                    {
        //                        //foreach (var d in department)
        //                        //{
        //                        list.Add(new PermissionExpansion()
        //                        {
        //                            //OrganizationId = d.SonId,
        //                            OrganizationId = u.OrganizationId,
        //                            ApplicationId = l.ApplicationId,
        //                            PermissionId = p.PermissionId,
        //                            UserId = u.Id,
        //                            UpdateTime = DateTime.Now
        //                        });
        //                        //}
        //                    }
        //                    else if (o.OrganizationId == OrganizationScopeDefines.Filiale)
        //                    {
        //                        //foreach (var f in filiale)
        //                        //{
        //                        list.Add(new PermissionExpansion()
        //                        {
        //                            //OrganizationId = f.SonId,
        //                            OrganizationId = u.FilialeId,
        //                            ApplicationId = l.ApplicationId,
        //                            PermissionId = p.PermissionId,
        //                            UserId = u.Id,
        //                            UpdateTime = DateTime.Now
        //                        });
        //                        //}
        //                    }
        //                    else if (o.OrganizationId == OrganizationScopeDefines.All)
        //                    {
        //                        //var all = await _organizationExpansionStore.ListAsync(a => a.Where(b => b.OrganizationId == "0"));
        //                        //foreach (var a in all)
        //                        //{
        //                        list.Add(new PermissionExpansion()
        //                        {
        //                            OrganizationId = "0",
        //                            ApplicationId = l.ApplicationId,
        //                            PermissionId = p.PermissionId,
        //                            UserId = u.Id,
        //                            UpdateTime = DateTime.Now
        //                        });
        //                        //}
        //                    }
        //                    else
        //                    {
        //                        list.Add(new PermissionExpansion()
        //                        {
        //                            OrganizationId = o.OrganizationId,
        //                            ApplicationId = l.ApplicationId,
        //                            PermissionId = p.PermissionId,
        //                            UserId = u.Id,
        //                            UpdateTime = DateTime.Now
        //                        });
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    if (list.Count > 0)
        //    {
        //        await Store.CreateListAsync(list.Distinct(new ExpansionEqComparer()).ToList());
        //    }
        //}

        public virtual async Task UpdatePermissionAsync(string roleName, List<RolePermission> rolePermissionList)
        {
            RemoveRoleAsync(roleName).GetAwaiter().GetResult();
            var ulist = await _userStore.GetUsersInRoleAsync(roleName);
            var pIds = rolePermissionList.Select(s => s.PermissionId);
            var pScopes = rolePermissionList.Select(s => s.OrganizationScope);
            var permissionItem = await _permissionItemStore.ListAsync(a => a.Where(b => pIds.Contains(b.Id)), CancellationToken.None);
            var organizations = await _permissionOrganizationStore.ListAsync(a => a.Where(b => pScopes.Contains(b.OrganizationScope)), CancellationToken.None);

            List<PermissionExpansion> list = new List<PermissionExpansion>();
            foreach (var p in rolePermissionList)
            {
                var role = _roleStore.FindByIdAsync(p.RoleId, CancellationToken.None).Result;
                if (role == null)
                {
                    await _rolePermissionStore.DeleteByRoleIdAsync(p.RoleId, CancellationToken.None);
                    continue;
                }
                //var ulist = await _userStore.GetUsersInRoleAsync(role.Name);
                foreach (var u in ulist)
                {
                    var organization = organizations.Where(a => a.OrganizationScope == p.OrganizationScope);
                    foreach (var o in organization)
                    {
                        var item = permissionItem.Where(a => a.Id == p.PermissionId).FirstOrDefault();
                        if (item == null) { continue; }
                        if (o.OrganizationId == OrganizationScopeDefines.Department)
                        {
                            list.Add(new PermissionExpansion()
                            {
                                OrganizationId = u.OrganizationId,
                                ApplicationId = item.ApplicationId,
                                PermissionId = p.PermissionId,
                                UserId = u.Id,
                                UpdateTime = DateTime.Now
                            });
                        }
                        else if (o.OrganizationId == OrganizationScopeDefines.Filiale)
                        {
                            list.Add(new PermissionExpansion()
                            {
                                OrganizationId = u.FilialeId,
                                ApplicationId = item.ApplicationId,
                                PermissionId = p.PermissionId,
                                UserId = u.Id,
                                UpdateTime = DateTime.Now
                            });
                        }
                        else if (o.OrganizationId == OrganizationScopeDefines.All)
                        {
                            list.Add(new PermissionExpansion()
                            {
                                OrganizationId = "0",
                                ApplicationId = item.ApplicationId,
                                PermissionId = p.PermissionId,
                                UserId = u.Id,
                                UpdateTime = DateTime.Now
                            });
                        }
                        else
                        {
                            list.Add(new PermissionExpansion()
                            {
                                OrganizationId = o.OrganizationId,
                                ApplicationId = item.ApplicationId,
                                PermissionId = p.PermissionId,
                                UserId = u.Id,
                                UpdateTime = DateTime.Now
                            });
                        }
                    }
                }
            }
            if (list.Count > 0)
            {
                await Store.CreateListAsync(list.Distinct(new ExpansionEqComparer()).ToList());
            }
        }


        public virtual async Task RemoveRoleAsync(string roleName)
        {
            var users = await _userStore.GetUsersInRoleAsync(roleName);
            var ids = users.Select(x => x.Id);
            var permissions = await Store.ListAsync(a => a.Where(b => ids.Contains(b.UserId)));
            await Store.DeleteListAsync(permissions);
        }

        public virtual async Task RemoveUserAsync(string userId)
        {
            var permissions = await Store.ListAsync(a => a.Where(b => b.UserId == userId));
            await Store.DeleteListAsync(permissions);
        }

        public virtual async Task RemoveUsersAsync(List<string> userIds)
        {
            var permissions = await Store.ListAsync(a => a.Where(b => userIds.Contains(b.UserId)));
            await Store.DeleteListAsync(permissions);
        }

        public virtual async Task RemovePermissionAsync(string permissionId)
        {
            var permissions = await Store.ListAsync(a => a.Where(b => b.PermissionId == permissionId));
            await Store.DeleteListAsync(permissions);
        }
        public virtual async Task RemovePermissionsAsync(List<string> permissionIds)
        {
            var permissions = await Store.ListAsync(a => a.Where(b => permissionIds.Contains(b.PermissionId)));
            await Store.DeleteListAsync(permissions);
        }

        public virtual async Task RemoveOrganizationAsync(string organizationId)
        {
            var permissions = await Store.ListAsync(a => a.Where(b => b.OrganizationId == organizationId));
            if (permissions?.Count > 0)
            {
                await Store.DeleteListAsync(permissions);
            }
        }

        public virtual async Task RemoveOrganizationsAsync(List<string> organizationIds)
        {
            var permissions = await Store.ListAsync(a => a.Where(b => organizationIds.Contains(b.OrganizationId)));
            if (permissions?.Count > 0)
            {
                await Store.DeleteListAsync(permissions);
            }
        }

        /// <summary>
        /// 权限表全表展开
        /// </summary>
        /// <returns></returns>
        private async Task<List<PermissionExpansion>> GetAllSonExpansion()
        {
            //获取整个角色权限表
            var permissions = await _rolePermissionStore.ListAsync(a => a.Where(b => true), CancellationToken.None);
            //获取整个权限项表
            var permissionItem = await _permissionItemStore.ListAsync(a => a.Where(b => true), CancellationToken.None);
            //获取整个权限项组织表
            var organizations = await _permissionOrganizationStore.ListAsync(a => a.Where(b => true), CancellationToken.None);

            List<PermissionExpansion> list = new List<PermissionExpansion>();
            //遍历角色权限项表
            foreach (var p in permissions)
            {
                //判断角色权限项中的权限项是否存在，若不存在，则清理角色权限数据项，跳过此次执行
                var item = permissionItem.Where(a => a.Id == p.PermissionId).FirstOrDefault();
                if (item == null)
                {
                    await _rolePermissionStore.DeleteByPermissionIdAsync(p.PermissionId, CancellationToken.None);
                    continue;
                }
                //获取表中的角色信息
                var role = _roleStore.FindByIdAsync(p.RoleId, CancellationToken.None).Result;
                if (role == null)
                {
                    //如果未在角色表中找到角色信息，则删除该条角色权限项目记录，跳过此次执行
                    await _rolePermissionStore.DeleteByRoleIdAsync(p.RoleId, CancellationToken.None);
                    continue;
                }
                //获取拥有该角色的所有用户
                var ulist = await _userStore.GetUsersInRoleAsync(role.Name);
                foreach (var u in ulist)
                {
                    //获取最外层循环的角色权限项对应的所有组织
                    var organization = organizations.Where(a => a.OrganizationScope == p.OrganizationScope);
                    foreach (var o in organization)
                    {
                        //本组织
                        if (o.OrganizationId == OrganizationScopeDefines.Department)
                        {
                            //添加用户的组织
                            list.Add(new PermissionExpansion()
                            {
                                OrganizationId = u.OrganizationId,
                                ApplicationId = item.ApplicationId,
                                PermissionId = p.PermissionId,
                                UserId = u.Id,
                                UpdateTime = DateTime.Now
                            });
                        }
                        //分公司
                        else if (o.OrganizationId == OrganizationScopeDefines.Filiale)
                        {
                            //添加用户的分公司
                            list.Add(new PermissionExpansion()
                            {
                                OrganizationId = u.FilialeId,
                                ApplicationId = item.ApplicationId,
                                PermissionId = p.PermissionId,
                                UserId = u.Id,
                                UpdateTime = DateTime.Now
                            });
                        }
                        //全部
                        else if (o.OrganizationId == OrganizationScopeDefines.All)
                        {
                            //直接添加顶级
                            list.Add(new PermissionExpansion()
                            {
                                OrganizationId = "0",
                                ApplicationId = item.ApplicationId,
                                PermissionId = p.PermissionId,
                                UserId = u.Id,
                                UpdateTime = DateTime.Now
                            });
                        }
                        else
                        {
                            //添加选择的具体组织
                            list.Add(new PermissionExpansion()
                            {
                                OrganizationId = o.OrganizationId,
                                ApplicationId = item.ApplicationId,
                                PermissionId = p.PermissionId,
                                UserId = u.Id,
                                UpdateTime = DateTime.Now
                            });
                        }
                    }
                }
            }
            return list.Distinct(new ExpansionEqComparer()).ToList();
        }
    }

    public sealed class ExpansionEqComparer : IEqualityComparer<PermissionExpansion>
    {
        public bool Equals(PermissionExpansion x, PermissionExpansion y)
        {
            if (x.UserId == y.UserId && x.PermissionId == y.PermissionId && x.OrganizationId == y.OrganizationId)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public int GetHashCode(PermissionExpansion obj)
        {
            return (obj.UserId + obj.PermissionId + obj.OrganizationId).GetHashCode();
        }
    }


}
