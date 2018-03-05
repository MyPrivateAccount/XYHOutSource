using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using AuthorizationCenter.Models;
using AuthorizationCenter.Managers;
using AuthorizationCenter.ViewModels;
using OpenIddict.Core;
using OpenIddict.Models;
using System.Threading;
using AuthorizationCenter.Filters;
using Microsoft.AspNetCore.Authorization;
using AspNet.Security.OAuth.Validation;
using AuthorizationCenter.Dto;
using Microsoft.AspNetCore.Identity;
using Newtonsoft.Json;

namespace AuthorizationCenter.Controllers
{
    [Authorize(AuthenticationSchemes = OAuthValidationDefaults.AuthenticationScheme)]
    [Produces("application/json")]
    [Route("api/RolePermissions")]
    public class RolePermissionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly RoleManager<Roles> _roleManager;
        private readonly ExtendUserManager<Users> _extendUserManager;
        private readonly RolePermissionManager _rolePermissionManager;
        private readonly PermissionItemManager _permissionItemManager;
        private readonly OrganizationsManager _organizationsManager;
        private readonly OrganizationExpansionManager _organizationExpansionManager;
        private readonly PermissionExpansionManager _permissionExpansionManager;
        private readonly PermissionOrganizationManager _permissionOrganizationManager;
        private readonly OpenIddictApplicationManager<OpenIddictApplication> _applicationManager;

        public RolePermissionsController(ApplicationDbContext context,
            RoleManager<Roles> roleManager,
            ExtendUserManager<Users> extendUserManager,
            RolePermissionManager rolePermissionManager,
            PermissionItemManager permissionItemManager,
            OrganizationsManager organizationsManager,
            OrganizationExpansionManager organizationExpansionManager,
            PermissionExpansionManager permissionExpansionManager,
            PermissionOrganizationManager permissionOrganizationManager,
        OpenIddictApplicationManager<OpenIddictApplication> applicationManager)
        {
            _context = context;
            _roleManager = roleManager;
            _extendUserManager = extendUserManager;
            _rolePermissionManager = rolePermissionManager;
            _permissionItemManager = permissionItemManager;
            _organizationsManager = organizationsManager;
            _organizationExpansionManager = organizationExpansionManager;
            _permissionOrganizationManager = permissionOrganizationManager;
            _applicationManager = applicationManager;
            _permissionExpansionManager = permissionExpansionManager;
        }

        // GET: api/RolePermissions/5
        [HttpGet("{id}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "AuthorizationPermission" })]
        public async Task<ResponseMessage<List<ApplicationPermissionModel>>> GetRolePermissions(string userId, [FromRoute] string id)
        {
            ResponseMessage<List<ApplicationPermissionModel>> response = new ResponseMessage<List<ApplicationPermissionModel>>();
            var roles = await _roleManager.FindByIdAsync(id);
            if (roles == null)
            {
                response.Code = ResponseCodeDefines.NotFound;
                return response;
            }
            if (roles.Type != RoleType.Public)
            {
                if (!await _permissionExpansionManager.HavePermission(userId, "RoleRetrieve", roles.OrganizationId))
                {
                    response.Code = ResponseCodeDefines.NotAllow;
                    response.Message = "没有权限";
                    return response;
                }
                if (!await _permissionExpansionManager.HavePermission(userId, "AuthorizationPermission"))
                {
                    response.Code = ResponseCodeDefines.NotAllow;
                    return response;
                }
            }
            try
            {
                var rolePermission = await _rolePermissionManager.FindByRoleIdAsync(id, HttpContext.RequestAborted);
                var list = new List<ApplicationPermissionModel>();
                response.Extension = await ConvertToModel(rolePermission, list, HttpContext.RequestAborted);
            }
            catch (Exception)
            {
                response.Code = ResponseCodeDefines.ServiceError;
            }
            return response;
        }

        // PUT: api/RolePermissions/5
        [HttpPut("{id}")]
        [TypeFilter(typeof(CheckPermission), Arguments = new object[] { "AuthorizationPermission" })]
        public async Task<ResponseMessage> PutRolePermissions(string userId, [FromRoute] string id, [FromBody] List<ApplicationPermissionModel> permissionModelList)
        {
            ResponseMessage response = new ResponseMessage();
            if (!ModelState.IsValid)
            {
                response.Code = ResponseCodeDefines.ModelStateInvalid;
                return response;
            }
            var roles = await _roleManager.FindByIdAsync(id);
            if (roles == null)
            {
                response.Code = ResponseCodeDefines.NotFound;
                return response;
            }
            List<string> organizationIds = new List<string>();
            //判断是否公共角色
            if (roles.Type == RoleType.Public)
            {
                organizationIds = await _permissionExpansionManager.GetOrganizationOfPermission(userId, "PublicRolePermission");
                if (organizationIds?.Count == 0)
                {
                    response.Code = ResponseCodeDefines.NotAllow;
                    response.Message = "没有权限";
                    return response;
                }
            }
            else
            {
                if (!await _permissionExpansionManager.HavePermission(userId, "RoleRetrieve", roles.OrganizationId))
                {
                    response.Code = ResponseCodeDefines.NotAllow;
                    response.Message = "没有权限";
                    return response;
                }
                organizationIds = await _permissionExpansionManager.GetOrganizationOfPermission(userId, "AuthorizationPermission");
                if (organizationIds?.Count == 0)
                {
                    response.Code = ResponseCodeDefines.NotAllow;
                    response.Message = "没有权限";
                    return response;
                }
            }
            try
            {
                //List<ApplicationPermissionModel> permissionModelListCopy = JsonConvert.DeserializeObject<List<ApplicationPermissionModel>>(JsonConvert.SerializeObject(permissionModelList));
                var rolePermissionList = new List<RolePermission>();
                var user = await _extendUserManager.FindByIdAsync(userId);
                foreach (var item in permissionModelList)
                {
                    //遍历权限项
                    foreach (var permission in item.Permissions)
                    {
                        //创建一个组织scope
                        var ScopeId = Guid.NewGuid().ToString();
                        var organizationScopeList = new List<PermissionOrganization>();

                        bool hasAll = permission.Organizations.Any(a => a.OrganizationId == OrganizationScopeDefines.All);
                        bool hasDepartment = permission.Organizations.Any(a => a.OrganizationId == OrganizationScopeDefines.Department);
                        bool hasFiliale = permission.Organizations.Any(a => a.OrganizationId == OrganizationScopeDefines.Filiale);
                        if (hasAll)
                        {
                            permission.Organizations = new List<OrganizationScopeModel> { new OrganizationScopeModel { OrganizationId = OrganizationScopeDefines.All, OrganizationName = "全部" } };
                        }
                        else if (hasFiliale && hasDepartment)
                        {
                            var deparment = permission.Organizations.Where(a => a.OrganizationId == OrganizationScopeDefines.Department).FirstOrDefault();
                            permission.Organizations.Remove(deparment);
                        }
                        //如果不是全部，则过滤相同的组织
                        //if (!hasAll)
                        //{
                            permission.Organizations = await OrganizationFilter(new List<OrganizationScopeModel>(), permission.Organizations);
                        //}
                        foreach (var organization in permission.Organizations)
                        {
                            //排除未包含本组织
                            if (organization.OrganizationId == OrganizationScopeDefines.Department)
                            {
                                if (!organizationIds.Contains(user.OrganizationId))
                                {
                                    continue;
                                }
                            }
                            //排除未包含分公司
                            else if (organization.OrganizationId == OrganizationScopeDefines.Filiale)
                            {
                                if (!organizationIds.Contains(user.FilialeId))
                                {
                                    continue;
                                }
                            }
                            //排除未包含全部
                            else if (organization.OrganizationId == OrganizationScopeDefines.All)
                            {
                                if (!organizationIds.Contains("0"))
                                {
                                    continue;
                                }
                            }
                            //排除未包含权限的组织
                            else if (!organizationIds.Contains(organization.OrganizationId))
                            {
                                continue;
                            }
                            organizationScopeList.Add(new PermissionOrganization() { OrganizationScope = ScopeId, OrganizationId = organization.OrganizationId });
                        }
                        if (!organizationScopeList.Any(a => a.OrganizationScope == ScopeId))
                        {
                            continue;
                        }
                        await _permissionOrganizationManager.CreateListAsync(organizationScopeList, HttpContext.RequestAborted);
                        rolePermissionList.Add(new RolePermission()
                        {
                            RoleId = id,
                            PermissionId = permission.PermissionId,
                            OrganizationScope = ScopeId
                        });
                    }
                }
                if (rolePermissionList.Count > 0)
                {
                    await _rolePermissionManager.CreateListAsync(id, rolePermissionList, HttpContext.RequestAborted);
                    await _permissionExpansionManager.UpdatePermissionAsync(roles.NormalizedName, rolePermissionList);
                }
            }
            catch (DbUpdateConcurrencyException)
            {
                response.Code = ResponseCodeDefines.ServiceError;
            }
            return response;
        }

        private async Task<List<ApplicationPermissionModel>> ConvertToModel(List<RolePermission> permissionList, List<ApplicationPermissionModel> apList, CancellationToken cancellationToken)
        {
            //TODO
            if (permissionList.Count() != 0)
            {
                //获取首个权限项的详情
                var permission = await _permissionItemManager.FindByIdAsync(permissionList[0].PermissionId, cancellationToken);
                if (permission != null)
                {
                    var model = new ApplicationPermissionModel();
                    model.ApplicationId = permission.ApplicationId;
                    //model.ApplicationName = (await _applicationManager.FindByIdAsync(permission.ApplicationId, cancellationToken)).DisplayName;
                    model.Permissions = new List<PermissionModel>();

                    //获取该权限项所在应用下的所有权限项
                    var array = await _permissionItemManager.FindByApplicationAsync(permission.ApplicationId, cancellationToken);

                    for (int i = permissionList.Count - 1; i >= 0; i--)
                    {
                        var r = array.Where(a => a.Id == permissionList[i].PermissionId).FirstOrDefault();
                        if (r != null)
                        {
                            var permissionOrganizations = await _permissionOrganizationManager.FindByIdAsync(permissionList[i].OrganizationScope, cancellationToken);
                            var pmodel = new PermissionModel()
                            {
                                PermissionId = r.Id,
                                PermissionName = r.Name,
                                Organizations = (from a in permissionOrganizations select new OrganizationScopeModel() { OrganizationId = a.OrganizationId }).ToList()
                            };
                            model.Permissions.Add(pmodel);
                            permissionList.Remove(permissionList[i]);
                        }
                    }
                    apList.Add(model);
                    await ConvertToModel(permissionList, apList, cancellationToken);
                }
                return apList;
            }
            return new List<ApplicationPermissionModel>();
        }

        private async Task<List<OrganizationScopeModel>> OrganizationFilter(List<OrganizationScopeModel> newlist, List<OrganizationScopeModel> list)
        {
            if (list?.Count > 0)
            {
                var sons = await _organizationExpansionManager.GetAllSonIdAsync(list[0].OrganizationId);
                var parents = await _organizationExpansionManager.GetAllParentIdAsync(list[0].OrganizationId);
                var listid = list.Select(a => a.OrganizationId);
                bool haveParent = parents?.Any(a => listid.Contains(a)) ?? false;
                bool havaSon = sons?.Any(a => listid.Contains(a)) ?? false;
                if (!havaSon && !haveParent)
                {
                    newlist.Add(list[0]);
                    list.RemoveAt(0);
                }
                else
                {
                    if (havaSon)
                    {
                        list.RemoveAll(new Predicate<OrganizationScopeModel>(a => sons.Contains(a.OrganizationId)));
                    }
                    if (haveParent)
                    {
                        list.RemoveAt(0);
                    }
                }
                return await OrganizationFilter(newlist, list);
            }
            else
            {
                return newlist;
            }
        }
    }
}