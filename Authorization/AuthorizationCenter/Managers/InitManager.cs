using AuthorizationCenter.Dto;
using AuthorizationCenter.Models;
using AuthorizationCenter.Stores;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace AuthorizationCenter.Managers
{
    public class InitManager
    {
        //public InitManager()
        //{

        //}
        public InitManager(OrganizationsManager organizationsManager,
            ApplicationManager applicationManager,
            RoleApplicationManager roleApplicationManager,
            ExtendUserManager<Users> extendUserManager,
            RolePermissionManager rolePermissionManager,
            RoleManager<Roles> roleManager,
            PermissionItemManager permissionItemManager,
            PermissionOrganizationManager permissionOrganizationManager,
            PermissionExpansionManager permissionExpansionManager,
            OrganizationExpansionManager organizationExpansionManager
            )
        {
            _organizationsManager = organizationsManager ?? throw new ArgumentNullException(nameof(organizationsManager));
            _applicationManager = applicationManager ?? throw new ArgumentNullException(nameof(applicationManager));
            _roleApplicationManager = roleApplicationManager ?? throw new ArgumentNullException(nameof(roleApplicationManager));
            _extendUserManager = extendUserManager ?? throw new ArgumentNullException(nameof(extendUserManager));
            _rolePermissionManager = rolePermissionManager ?? throw new ArgumentNullException(nameof(rolePermissionManager));
            _roleManager = roleManager ?? throw new ArgumentNullException(nameof(roleManager));
            _permissionItemManager = permissionItemManager ?? throw new ArgumentNullException(nameof(permissionItemManager));
            _permissionOrganizationManager = permissionOrganizationManager ?? throw new ArgumentNullException(nameof(permissionOrganizationManager));
            _permissionExpansionManager = permissionExpansionManager ?? throw new ArgumentNullException(nameof(permissionExpansionManager));
            _organizationExpansionManager = organizationExpansionManager ?? throw new ArgumentNullException(nameof(organizationExpansionManager));
        }

        private OrganizationsManager _organizationsManager { get; }
        private ApplicationManager _applicationManager { get; }
        private RolePermissionManager _rolePermissionManager { get; }
        private RoleApplicationManager _roleApplicationManager { get; }
        private RoleManager<Roles> _roleManager { get; }
        private ExtendUserManager<Users> _extendUserManager { get; }
        private PermissionItemManager _permissionItemManager { get; }
        private PermissionOrganizationManager _permissionOrganizationManager { get; }
        private PermissionExpansionManager _permissionExpansionManager { get; }
        private OrganizationExpansionManager _organizationExpansionManager { get; }

        public async Task InitDate()
        {
            await AddDefaultAdminData();
            await _organizationExpansionManager.Expansion();
            await _permissionExpansionManager.Expansion();
        }

        protected async Task AddDefaultAdminData()
        {
            if (_extendUserManager.FindByNameAsync("admin").Result == null)
            {
                await _extendUserManager.CreateAsync(new Users
                {
                    OrganizationId = "0",
                    UserName = "admin",
                    Email = "chenrongku@163.com",
                    FilialeId = "0",
                    TrueName = "admin",
                }, "123456");
            }
            var user = _extendUserManager.FindByNameAsync("admin").Result;
            if (_roleManager.FindByNameAsync("admin").Result == null)
            {
                await _roleManager.CreateAsync(new Roles
                {
                    Name = "admin",
                    NormalizedName = "admin",
                    OrganizationId = "0"
                });
            }
            if (!_extendUserManager.IsInRoleAsync(user, "admin").Result)
            {
                await _extendUserManager.AddToRoleAsync(user, "admin");
            }
            var role = await _roleManager.FindByNameAsync("admin");
            var applications = await _applicationManager.ListAsync(a => a.Where(b => true), CancellationToken.None);
            var applicationIds = await _roleApplicationManager.FindApplicationIdsByRoleIdsAsync(new List<string> { role.Id }, CancellationToken.None);
            foreach (var item in applications)
            {
                if (!applicationIds.Contains(item.Id))
                {
                    await _roleApplicationManager.CreateAsync(new RoleApplication
                    {
                        ApplicationId = item.Id,
                        RoleId = role.Id
                    }, CancellationToken.None);
                }
            }

            string applicationId = _applicationManager.FindByClientIdAsync("privilegeManager", CancellationToken.None).Result.Id;

            #region 用户角色
            if (_permissionItemManager.FindByIdAsync("UserInfoRetrieve", CancellationToken.None).Result == null)
            {
                await _permissionItemManager.CreateAsync(new PermissionItem
                {
                    ApplicationId = applicationId,
                    Groups = "用户",
                    Id = "UserInfoRetrieve",
                    Name = "获取用户信息"
                }, CancellationToken.None);
            }
            if (_permissionItemManager.FindByIdAsync("UserInfoCreate", CancellationToken.None).Result == null)
            {
                await _permissionItemManager.CreateAsync(new PermissionItem
                {
                    ApplicationId = applicationId,
                    Groups = "用户",
                    Id = "UserInfoCreate",
                    Name = "创建用户"
                }, CancellationToken.None);
            }
            if (_permissionItemManager.FindByIdAsync("UserInfoUpdate", CancellationToken.None).Result == null)
            {
                await _permissionItemManager.CreateAsync(new PermissionItem
                {
                    ApplicationId = applicationId,
                    Groups = "用户",
                    Id = "UserInfoUpdate",
                    Name = "更新用户信息"
                }, CancellationToken.None);
            }
            if (_permissionItemManager.FindByIdAsync("UserInfoDelete", CancellationToken.None).Result == null)
            {
                await _permissionItemManager.CreateAsync(new PermissionItem
                {
                    ApplicationId = applicationId,
                    Groups = "用户",
                    Id = "UserInfoDelete",
                    Name = "删除用户信息"
                }, CancellationToken.None);
            }
            if (_permissionItemManager.FindByIdAsync("InitUserPassword", CancellationToken.None).Result == null)
            {
                await _permissionItemManager.CreateAsync(new PermissionItem
                {
                    ApplicationId = applicationId,
                    Groups = "用户",
                    Id = "InitUserPassword",
                    Name = "初始化用户密码"
                }, CancellationToken.None);
            }

            if (_permissionItemManager.FindByIdAsync("RoleRetrieve", CancellationToken.None).Result == null)
            {
                await _permissionItemManager.CreateAsync(new PermissionItem
                {
                    ApplicationId = applicationId,
                    Groups = "角色",
                    Id = "RoleRetrieve",
                    Name = "获取角色信息"
                }, CancellationToken.None);
            }
            if (_permissionItemManager.FindByIdAsync("GetRoleByRoleName", CancellationToken.None).Result == null)
            {
                await _permissionItemManager.CreateAsync(new PermissionItem
                {
                    ApplicationId = applicationId,
                    Groups = "角色",
                    Id = "GetRoleByRoleName",
                    Name = "通过角色名称获取角色信息"
                }, CancellationToken.None);
            }
            if (_permissionItemManager.FindByIdAsync("RoleUpdate", CancellationToken.None).Result == null)
            {
                await _permissionItemManager.CreateAsync(new PermissionItem
                {
                    ApplicationId = applicationId,
                    Groups = "角色",
                    Id = "RoleUpdate",
                    Name = "更新角色"
                }, CancellationToken.None);
            }
            if (_permissionItemManager.FindByIdAsync("RoleCreate", CancellationToken.None).Result == null)
            {
                await _permissionItemManager.CreateAsync(new PermissionItem
                {
                    ApplicationId = applicationId,
                    Groups = "角色",
                    Id = "RoleCreate",
                    Name = "创建角色"
                }, CancellationToken.None);
            }
            if (_permissionItemManager.FindByIdAsync("RoleDelete", CancellationToken.None).Result == null)
            {
                await _permissionItemManager.CreateAsync(new PermissionItem
                {
                    ApplicationId = applicationId,
                    Groups = "角色",
                    Id = "RoleDelete",
                    Name = "删除角色"
                }, CancellationToken.None);
            }
            if (_permissionItemManager.FindByIdAsync("GetUserInfoByRoleName", CancellationToken.None).Result == null)
            {
                await _permissionItemManager.CreateAsync(new PermissionItem
                {
                    ApplicationId = applicationId,
                    Groups = "用户角色",
                    Id = "GetUserInfoByRoleName",
                    Name = "通过角色名获取用户信息"
                }, CancellationToken.None);
            }
            if (_permissionItemManager.FindByIdAsync("GetRoleInfoByUserName", CancellationToken.None).Result == null)
            {
                await _permissionItemManager.CreateAsync(new PermissionItem
                {
                    ApplicationId = applicationId,
                    Groups = "用户角色",
                    Id = "GetRoleInfoByUserName",
                    Name = "通过用户获取角色信息"
                }, CancellationToken.None);
            }
            if (_permissionItemManager.FindByIdAsync("AddUserInRoles", CancellationToken.None).Result == null)
            {
                await _permissionItemManager.CreateAsync(new PermissionItem
                {
                    ApplicationId = applicationId,
                    Groups = "用户角色",
                    Id = "AddUserInRoles",
                    Name = "为用户赋予角色"
                }, CancellationToken.None);
            }
            if (_permissionItemManager.FindByIdAsync("RemoveUserFromRoles", CancellationToken.None).Result == null)
            {
                await _permissionItemManager.CreateAsync(new PermissionItem
                {
                    ApplicationId = applicationId,
                    Groups = "用户角色",
                    Id = "RemoveUserFromRoles",
                    Name = "移除用户角色"
                }, CancellationToken.None);
            }
            if (_permissionItemManager.FindByIdAsync("OnSiteRoles", CancellationToken.None).Result == null)
            {
                await _permissionItemManager.CreateAsync(new PermissionItem
                {
                    ApplicationId = applicationId,
                    Groups = "驻场角色",
                    Id = "OnSiteRoles",
                    Name = "驻场基本角色"
                }, CancellationToken.None);
            }
            #endregion
            #region 应用
            if (_permissionItemManager.FindByIdAsync("ApplicationRetrieve", CancellationToken.None).Result == null)
            {
                await _permissionItemManager.CreateAsync(new PermissionItem
                {
                    ApplicationId = applicationId,
                    Groups = "应用",
                    Id = "ApplicationRetrieve",
                    Name = "获取应用信息"
                }, CancellationToken.None);
            }
            if (_permissionItemManager.FindByIdAsync("ApplicationCreate", CancellationToken.None).Result == null)
            {
                await _permissionItemManager.CreateAsync(new PermissionItem
                {
                    ApplicationId = applicationId,
                    Groups = "应用",
                    Id = "ApplicationCreate",
                    Name = "创建应用"
                }, CancellationToken.None);
            }
            if (_permissionItemManager.FindByIdAsync("ApplicationUpdate", CancellationToken.None).Result == null)
            {
                await _permissionItemManager.CreateAsync(new PermissionItem
                {
                    ApplicationId = applicationId,
                    Groups = "应用",
                    Id = "ApplicationUpdate",
                    Name = "更新应用"
                }, CancellationToken.None);
            }
            if (_permissionItemManager.FindByIdAsync("ApplicationDelete", CancellationToken.None).Result == null)
            {
                await _permissionItemManager.CreateAsync(new PermissionItem
                {
                    ApplicationId = applicationId,
                    Groups = "应用",
                    Id = "ApplicationDelete",
                    Name = "删除应用"
                }, CancellationToken.None);
            }

            if (_permissionItemManager.FindByIdAsync("PermissionItemRetrieve", CancellationToken.None).Result == null)
            {
                await _permissionItemManager.CreateAsync(new PermissionItem
                {
                    ApplicationId = applicationId,
                    Groups = "权限项",
                    Id = "PermissionItemRetrieve",
                    Name = "获取权限项"
                }, CancellationToken.None);
            }
            if (_permissionItemManager.FindByIdAsync("PermissionItemUpdate", CancellationToken.None).Result == null)
            {
                await _permissionItemManager.CreateAsync(new PermissionItem
                {
                    ApplicationId = applicationId,
                    Groups = "权限项",
                    Id = "PermissionItemUpdate",
                    Name = "更新权限项"
                }, CancellationToken.None);
            }
            if (_permissionItemManager.FindByIdAsync("PermissionItemCreate", CancellationToken.None).Result == null)
            {
                await _permissionItemManager.CreateAsync(new PermissionItem
                {
                    ApplicationId = applicationId,
                    Groups = "权限项",
                    Id = "PermissionItemCreate",
                    Name = "创建权限项"
                }, CancellationToken.None);
            }
            if (_permissionItemManager.FindByIdAsync("PermissionItemDelete", CancellationToken.None).Result == null)
            {
                await _permissionItemManager.CreateAsync(new PermissionItem
                {
                    ApplicationId = applicationId,
                    Groups = "权限项",
                    Id = "PermissionItemDelete",
                    Name = "删除权限项"
                }, CancellationToken.None);
            }
            if (_permissionItemManager.FindByIdAsync("GetPermissionItemByRoleId", CancellationToken.None).Result == null)
            {

                await _permissionItemManager.CreateAsync(new PermissionItem
                {
                    ApplicationId = applicationId,
                    Groups = "角色权限",
                    Id = "GetPermissionItemByRoleId",
                    Name = "通过角色ID获取权限项"
                }, CancellationToken.None);
            }
            if (_permissionItemManager.FindByIdAsync("UpdatePermissionItemByRoleId", CancellationToken.None).Result == null)
            {
                await _permissionItemManager.CreateAsync(new PermissionItem
                {
                    ApplicationId = applicationId,
                    Groups = "角色权限",
                    Id = "UpdatePermissionItemByRoleId",
                    Name = "更新角色权限项"
                }, CancellationToken.None);
            }
            #endregion
            #region 组织
            if (_permissionItemManager.FindByIdAsync("OrganizationRetrieve", CancellationToken.None).Result == null)
            {
                await _permissionItemManager.CreateAsync(new PermissionItem
                {
                    ApplicationId = applicationId,
                    Groups = "组织",
                    Id = "OrganizationRetrieve",
                    Name = "获取组织信息"
                }, CancellationToken.None);
            }
            if (_permissionItemManager.FindByIdAsync("GetOrganizationsByParent", CancellationToken.None).Result == null)
            {
                await _permissionItemManager.CreateAsync(new PermissionItem
                {
                    ApplicationId = applicationId,
                    Groups = "组织",
                    Id = "GetOrganizationsByParent",
                    Name = "获取子级组织信息"
                }, CancellationToken.None);
            }
            if (_permissionItemManager.FindByIdAsync("GetAllOrganizationsByParent", CancellationToken.None).Result == null)
            {
                await _permissionItemManager.CreateAsync(new PermissionItem
                {
                    ApplicationId = applicationId,
                    Groups = "组织",
                    Id = "GetAllOrganizationsByParent",
                    Name = "获取所有子级组织信息"
                }, CancellationToken.None);
            }
            if (_permissionItemManager.FindByIdAsync("OrganizationUpdate", CancellationToken.None).Result == null)
            {
                await _permissionItemManager.CreateAsync(new PermissionItem
                {
                    ApplicationId = applicationId,
                    Groups = "组织",
                    Id = "OrganizationUpdate",
                    Name = "更新组织"
                }, CancellationToken.None);
            }
            if (_permissionItemManager.FindByIdAsync("OrganizationCreate", CancellationToken.None).Result == null)
            {
                await _permissionItemManager.CreateAsync(new PermissionItem
                {
                    ApplicationId = applicationId,
                    Groups = "组织",
                    Id = "OrganizationCreate",
                    Name = "创建组织"
                }, CancellationToken.None);
            }
            if (_permissionItemManager.FindByIdAsync("OrganizationDelete", CancellationToken.None).Result == null)
            {
                await _permissionItemManager.CreateAsync(new PermissionItem
                {
                    ApplicationId = applicationId,
                    Groups = "组织",
                    Id = "OrganizationDelete",
                    Name = "删除组织"
                }, CancellationToken.None);
            }
            #endregion
            #region 登录日志
            if (_permissionItemManager.FindByIdAsync("UserLoginLogRetrieve", CancellationToken.None).Result == null)
            {
                await _permissionItemManager.CreateAsync(new PermissionItem
                {
                    ApplicationId = applicationId,
                    Groups = "日志",
                    Id = "UserLoginLogRetrieve",
                    Name = "获取用户登录日志"
                }, CancellationToken.None);
            }
            #endregion

            #region 授权权限
            if (_permissionItemManager.FindByIdAsync("AuthorizationPermission", CancellationToken.None).Result == null)
            {
                await _permissionItemManager.CreateAsync(new PermissionItem
                {
                    ApplicationId = applicationId,
                    Groups = "授权",
                    Id = "AuthorizationPermission",
                    Name = "为角色授权"
                }, CancellationToken.None);
            }
            if (_permissionItemManager.FindByIdAsync("PublicRolePermission", CancellationToken.None).Result == null)
            {
                await _permissionItemManager.CreateAsync(new PermissionItem
                {
                    ApplicationId = applicationId,
                    Groups = "公共角色",
                    Id = "PublicRolePermission",
                    Name = "公共角色授权"
                }, CancellationToken.None);
            }
            if (_permissionItemManager.FindByIdAsync("PublicRoleOper", CancellationToken.None).Result == null)
            {
                await _permissionItemManager.CreateAsync(new PermissionItem
                {
                    ApplicationId = applicationId,
                    Groups = "公共角色",
                    Id = "PublicRoleOper",
                    Name = "公共角色操作"
                }, CancellationToken.None);
            }
            #endregion

            string baseapplicationId = _applicationManager.FindByClientIdAsync("xtwh", CancellationToken.None).Result.Id;

            #region 字典组
            if (_permissionItemManager.FindByIdAsync("DictionaryDefinesRetrieve", CancellationToken.None).Result == null)
            {
                await _permissionItemManager.CreateAsync(new PermissionItem
                {
                    ApplicationId = baseapplicationId,
                    Groups = "字典定义",
                    Id = "DictionaryDefinesRetrieve",
                    Name = "获取字典定义"
                }, CancellationToken.None);
            }
            if (_permissionItemManager.FindByIdAsync("DictionaryDefinesCreate", CancellationToken.None).Result == null)
            {
                await _permissionItemManager.CreateAsync(new PermissionItem
                {
                    ApplicationId = baseapplicationId,
                    Groups = "字典定义",
                    Id = "DictionaryDefinesCreate",
                    Name = "添加字典定义"
                }, CancellationToken.None);
            }
            if (_permissionItemManager.FindByIdAsync("DictionaryDefinesDelete", CancellationToken.None).Result == null)
            {
                await _permissionItemManager.CreateAsync(new PermissionItem
                {
                    ApplicationId = baseapplicationId,
                    Groups = "字典定义",
                    Id = "DictionaryDefinesDelete",
                    Name = "删除字典定义"
                }, CancellationToken.None);
            }

            if (_permissionItemManager.FindByIdAsync("DictionaryGroupsRetrieve", CancellationToken.None).Result == null)
            {
                await _permissionItemManager.CreateAsync(new PermissionItem
                {
                    ApplicationId = baseapplicationId,
                    Groups = "字典组",
                    Id = "DictionaryGroupsRetrieve",
                    Name = "获取字典组"
                }, CancellationToken.None);
            }
            if (_permissionItemManager.FindByIdAsync("DictionaryGroupsCreate", CancellationToken.None).Result == null)
            {
                await _permissionItemManager.CreateAsync(new PermissionItem
                {
                    ApplicationId = baseapplicationId,
                    Groups = "字典组",
                    Id = "DictionaryGroupsCreate",
                    Name = "添加字典组"
                }, CancellationToken.None);
            }
            if (_permissionItemManager.FindByIdAsync("DictionaryGroupsUpdate", CancellationToken.None).Result == null)
            {
                await _permissionItemManager.CreateAsync(new PermissionItem
                {
                    ApplicationId = baseapplicationId,
                    Groups = "字典组",
                    Id = "DictionaryGroupsUpdate",
                    Name = "修改字典组"
                }, CancellationToken.None);
            }
            if (_permissionItemManager.FindByIdAsync("DictionaryGroupsDelete", CancellationToken.None).Result == null)
            {
                await _permissionItemManager.CreateAsync(new PermissionItem
                {
                    ApplicationId = baseapplicationId,
                    Groups = "字典组",
                    Id = "DictionaryGroupsDelete",
                    Name = "删除字典组"
                }, CancellationToken.None);
            }
            #endregion

            string residentapplicationId = _applicationManager.FindByClientIdAsync("wx-zcgj", CancellationToken.None).Result.Id;

            #region 区域
            if (_permissionItemManager.FindByIdAsync("AreaDefineRetrieve", CancellationToken.None).Result == null)
            {
                await _permissionItemManager.CreateAsync(new PermissionItem
                {
                    ApplicationId = residentapplicationId,
                    Groups = "区域",
                    Id = "AreaDefineRetrieve",
                    Name = "获取区域"
                }, CancellationToken.None);
            }
            if (_permissionItemManager.FindByIdAsync("AreaDefineCreate", CancellationToken.None).Result == null)
            {
                await _permissionItemManager.CreateAsync(new PermissionItem
                {
                    ApplicationId = residentapplicationId,
                    Groups = "区域",
                    Id = "AreaDefineCreate",
                    Name = "添加区域"
                }, CancellationToken.None);
            }
            if (_permissionItemManager.FindByIdAsync("AreaDefineUpdate", CancellationToken.None).Result == null)
            {
                await _permissionItemManager.CreateAsync(new PermissionItem
                {
                    ApplicationId = residentapplicationId,
                    Groups = "区域",
                    Id = "AreaDefineUpdate",
                    Name = "修改区域"
                }, CancellationToken.None);
            }
            if (_permissionItemManager.FindByIdAsync("AreaDefineDelete", CancellationToken.None).Result == null)
            {
                await _permissionItemManager.CreateAsync(new PermissionItem
                {
                    ApplicationId = residentapplicationId,
                    Groups = "区域",
                    Id = "AreaDefineDelete",
                    Name = "删除区域"
                }, CancellationToken.None);
            }
            #endregion
            #region 房源
            if (_permissionItemManager.FindByIdAsync("BuildingRetrieve", CancellationToken.None).Result == null)
            {
                await _permissionItemManager.CreateAsync(new PermissionItem
                {
                    ApplicationId = residentapplicationId,
                    Groups = "楼盘",
                    Id = "BuildingRetrieve",
                    Name = "获取楼盘"
                }, CancellationToken.None);
            }
            if (_permissionItemManager.FindByIdAsync("BuildingCreate", CancellationToken.None).Result == null)
            {
                await _permissionItemManager.CreateAsync(new PermissionItem
                {
                    ApplicationId = residentapplicationId,
                    Groups = "楼盘",
                    Id = "BuildingCreate",
                    Name = "添加楼盘"
                }, CancellationToken.None);
            }
            if (_permissionItemManager.FindByIdAsync("BuildingUpdate", CancellationToken.None).Result == null)
            {
                await _permissionItemManager.CreateAsync(new PermissionItem
                {
                    ApplicationId = residentapplicationId,
                    Groups = "楼盘",
                    Id = "BuildingUpdate",
                    Name = "修改楼盘"
                }, CancellationToken.None);
            }
            if (_permissionItemManager.FindByIdAsync("BuildingDelete", CancellationToken.None).Result == null)
            {
                await _permissionItemManager.CreateAsync(new PermissionItem
                {
                    ApplicationId = residentapplicationId,
                    Groups = "楼盘",
                    Id = "BuildingDelete",
                    Name = "删除楼盘"
                }, CancellationToken.None);
            }
            if (_permissionItemManager.FindByIdAsync("ShopsRetrieve", CancellationToken.None).Result == null)
            {
                await _permissionItemManager.CreateAsync(new PermissionItem
                {
                    ApplicationId = residentapplicationId,
                    Groups = "商铺",
                    Id = "ShopsRetrieve",
                    Name = "获取商铺"
                }, CancellationToken.None);
            }
            if (_permissionItemManager.FindByIdAsync("ShopsCreate", CancellationToken.None).Result == null)
            {
                await _permissionItemManager.CreateAsync(new PermissionItem
                {
                    ApplicationId = residentapplicationId,
                    Groups = "商铺",
                    Id = "ShopsCreate",
                    Name = "添加商铺"
                }, CancellationToken.None);
            }
            if (_permissionItemManager.FindByIdAsync("ShopsUpdate", CancellationToken.None).Result == null)
            {
                await _permissionItemManager.CreateAsync(new PermissionItem
                {
                    ApplicationId = residentapplicationId,
                    Groups = "商铺",
                    Id = "ShopsUpdate",
                    Name = "修改商铺"
                }, CancellationToken.None);
            }
            if (_permissionItemManager.FindByIdAsync("ShopsDelete", CancellationToken.None).Result == null)
            {
                await _permissionItemManager.CreateAsync(new PermissionItem
                {
                    ApplicationId = residentapplicationId,
                    Groups = "商铺",
                    Id = "ShopsDelete",
                    Name = "删除商铺"
                }, CancellationToken.None);
            }
            if (_permissionItemManager.FindByIdAsync("ShopsDelete", CancellationToken.None).Result == null)
            {
                await _permissionItemManager.CreateAsync(new PermissionItem
                {
                    ApplicationId = residentapplicationId,
                    Groups = "楼盘查询",
                    Id = "MyManagerBuildings",
                    Name = "查询我管理的楼盘"
                }, CancellationToken.None);
            }

            if (_permissionItemManager.FindByIdAsync("StatisticalReports", CancellationToken.None).Result == null)
            {
                await _permissionItemManager.CreateAsync(new PermissionItem
                {
                    ApplicationId = residentapplicationId,
                    Groups = "统计报表",
                    Id = "StatisticalReports",
                    Name = "查询统计报表"
                }, CancellationToken.None);
            }


            #endregion
            #region 文件
            if (_permissionItemManager.FindByIdAsync("FileUpload", CancellationToken.None).Result == null)
            {
                await _permissionItemManager.CreateAsync(new PermissionItem
                {
                    ApplicationId = residentapplicationId,
                    Groups = "文件",
                    Id = "FileUpload",
                    Name = "文件上传"
                }, CancellationToken.None);
            }
            if (_permissionItemManager.FindByIdAsync("FileCallBack", CancellationToken.None).Result == null)
            {
                await _permissionItemManager.CreateAsync(new PermissionItem
                {
                    ApplicationId = residentapplicationId,
                    Groups = "商铺",
                    Id = "FileCallBack",
                    Name = "文件回调"
                }, CancellationToken.None);
            }
            if (_permissionItemManager.FindByIdAsync("DeleteShopsFile", CancellationToken.None).Result == null)
            {
                await _permissionItemManager.CreateAsync(new PermissionItem
                {
                    ApplicationId = residentapplicationId,
                    Groups = "文件",
                    Id = "DeleteShopsFile",
                    Name = "删除商铺文件"
                }, CancellationToken.None);
            }
            if (_permissionItemManager.FindByIdAsync("DeleteBuildingFile", CancellationToken.None).Result == null)
            {
                await _permissionItemManager.CreateAsync(new PermissionItem
                {
                    ApplicationId = residentapplicationId,
                    Groups = "文件",
                    Id = "DeleteBuildingFile",
                    Name = "删除楼盘文件"
                }, CancellationToken.None);
            }
            if (_permissionItemManager.FindByIdAsync("CutomerFileCallBack", CancellationToken.None).Result == null)
            {
                await _permissionItemManager.CreateAsync(new PermissionItem
                {
                    ApplicationId = residentapplicationId,
                    Groups = "客源",
                    Id = "CutomerFileCallBack",
                    Name = "文件回调"
                }, CancellationToken.None);
            }
            if (_permissionItemManager.FindByIdAsync("DeleteDealFile", CancellationToken.None).Result == null)
            {
                await _permissionItemManager.CreateAsync(new PermissionItem
                {
                    ApplicationId = residentapplicationId,
                    Groups = "文件",
                    Id = "DeleteDealFile",
                    Name = "删除成交信息文件"
                }, CancellationToken.None);
            }
            if (_permissionItemManager.FindByIdAsync("DeleteCustomerFile", CancellationToken.None).Result == null)
            {
                await _permissionItemManager.CreateAsync(new PermissionItem
                {
                    ApplicationId = residentapplicationId,
                    Groups = "文件",
                    Id = "DeleteCustomerFile",
                    Name = "删除客户文件"
                }, CancellationToken.None);
            }





            #endregion

            string customerapplicationId = _applicationManager.FindByClientIdAsync("customerManager", CancellationToken.None).Result.Id;

            #region 客源
            if (_permissionItemManager.FindByIdAsync("CustomerCreate", CancellationToken.None).Result == null)
            {
                await _permissionItemManager.CreateAsync(new PermissionItem
                {
                    ApplicationId = customerapplicationId,
                    Groups = "客源操作",
                    Id = "CustomerCreate",
                    Name = "添加客户"
                }, CancellationToken.None);
            }
            if (_permissionItemManager.FindByIdAsync("CustomerRetrieve", CancellationToken.None).Result == null)
            {
                await _permissionItemManager.CreateAsync(new PermissionItem
                {
                    ApplicationId = customerapplicationId,
                    Groups = "客源查询",
                    Id = "CustomerRetrieve",
                    Name = "获取客源信息"
                }, CancellationToken.None);
            }
            if (_permissionItemManager.FindByIdAsync("CustomerDealRetrieve", CancellationToken.None).Result == null)
            {
                await _permissionItemManager.CreateAsync(new PermissionItem
                {
                    ApplicationId = customerapplicationId,
                    Groups = "成交信息查询",
                    Id = "CustomerDealRetrieve",
                    Name = "获取成交信息"
                }, CancellationToken.None);
            }
            if (_permissionItemManager.FindByIdAsync("CustomerDealRetrieve", CancellationToken.None).Result == null)
            {
                await _permissionItemManager.CreateAsync(new PermissionItem
                {
                    ApplicationId = customerapplicationId,
                    Groups = "成交信息统计",
                    Id = "CustomerDealStatistical",
                    Name = "获取成交统计信息"
                }, CancellationToken.None);
            }
            if (_permissionItemManager.FindByIdAsync("CustomerLossRetrieve", CancellationToken.None).Result == null)
            {
                await _permissionItemManager.CreateAsync(new PermissionItem
                {
                    ApplicationId = customerapplicationId,
                    Groups = "客户失效查询",
                    Id = "CustomerLossRetrieve",
                    Name = "获取客户失效信息"
                }, CancellationToken.None);
            }
            if (_permissionItemManager.FindByIdAsync("LossCustomer", CancellationToken.None).Result == null)
            {
                await _permissionItemManager.CreateAsync(new PermissionItem
                {
                    ApplicationId = customerapplicationId,
                    Groups = "客户失效操作",
                    Id = "LossCustomer",
                    Name = "拉无效"
                }, CancellationToken.None);
            }
            if (_permissionItemManager.FindByIdAsync("ActivateCustomer", CancellationToken.None).Result == null)
            {
                await _permissionItemManager.CreateAsync(new PermissionItem
                {
                    ApplicationId = customerapplicationId,
                    Groups = "客户失效操作",
                    Id = "ActivateCustomer",
                    Name = "激活客户"
                }, CancellationToken.None);
            }
            if (_permissionItemManager.FindByIdAsync("TransactionsRetrieve", CancellationToken.None).Result == null)
            {
                await _permissionItemManager.CreateAsync(new PermissionItem
                {
                    ApplicationId = customerapplicationId,
                    Groups = "报备查询",
                    Id = "TransactionsRetrieve",
                    Name = "客户报备信息"
                }, CancellationToken.None);
            }

            #endregion

            string salemanapplicationId = _applicationManager.FindByClientIdAsync("wx-ywygj", CancellationToken.None).Result.Id;

            #region 业务员

            if (_permissionItemManager.FindByIdAsync("SaleManBasis", CancellationToken.None).Result == null)
            {
                await _permissionItemManager.CreateAsync(new PermissionItem
                {
                    ApplicationId = salemanapplicationId,
                    Groups = "业务员",
                    Id = "SaleManBasis",
                    Name = "业务员基础"
                }, CancellationToken.None);
            }
            




            #endregion


            if (_permissionOrganizationManager.FindByIdAsync("admin", CancellationToken.None).Result.Count == 0)
            {
                await _permissionOrganizationManager.CreateAsync(new PermissionOrganization
                {
                    OrganizationId = OrganizationScopeDefines.All,
                    OrganizationScope = "admin"
                }, CancellationToken.None);
            }

            string roleid = role.Id;

            var rolePermissionList = _rolePermissionManager.FindByRoleIdAsync(roleid, CancellationToken.None).Result;
            if (rolePermissionList?.Count > 0)
            {
                await _rolePermissionManager.DeleteByRoleIdAsync(roleid, CancellationToken.None);
            }
            var items = await _permissionItemManager.GetList();
            List<RolePermission> list = new List<RolePermission>();
            foreach (var item in items)
            {
                list.Add(new RolePermission
                {
                    OrganizationScope = "admin",
                    PermissionId = item.Id,
                    RoleId = roleid
                });
            }
            //var role = await _roleManager.FindByNameAsync("admin");
            await _rolePermissionManager.CreateListAsync(role.Id, list, CancellationToken.None);

        }
    }
}
