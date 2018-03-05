using AuthorizationCenter;
using AuthorizationCenter.Interface;
using AuthorizationCenter.Managers;
using AuthorizationCenter.Models;
using AuthorizationCenter.Stores;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using OpenIddict.Core;
using OpenIddict.EntityFrameworkCore;
using OpenIddict.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Microsoft.Extensions.DependencyInjection
{
    public static class IServiceCollectionExtensions
    {
        public static UserDefinedBuilder AddUserDefined(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            //services.AddSingleton<ApplicationDbContext>();
            services.AddScoped<PermissionItemManager>();
            services.AddScoped<PermissionExpansionManager>();
            services.AddScoped<OrganizationExpansionManager>();
            services.AddScoped<InitManager>();
            services.AddScoped<OrganizationsManager>();
            services.AddScoped<RolePermissionManager>();
            services.AddScoped<RoleManager<Roles>>();
            services.AddScoped<ApplicationManager>();
            services.AddScoped<PermissionOrganizationManager>();

            services.AddScoped<ExtendUserStore<ApplicationDbContext>>();
            services.AddScoped<ExtendUserManager<Users>>();
            services.AddScoped<UserStore<Users>, ExtendUserStore<ApplicationDbContext>>();
            services.AddScoped<DbContext, ApplicationDbContext>();

            services.AddScoped<IPermissionExpansionStore, PermissionExpansionStore<ApplicationDbContext>>();
            services.AddScoped<IExtendUserStore, ExtendUserStore<ApplicationDbContext>>();
            services.AddScoped<IOrganizationExpansionStore, OrganizationExpansionStore<ApplicationDbContext>>();

            services.AddScoped<IOrganizationStore, OrganizationStore<ApplicationDbContext>>();
            services.AddScoped<IPermissionOrganizationStore, PermissionOrganizationStore<ApplicationDbContext>>();
            services.AddScoped<IRolePermissionStore, RolePermissionStore<ApplicationDbContext>>();
            services.AddScoped<IPermissionItemStore, PermissionItemStore<ApplicationDbContext>>();

            services.AddScoped<IUserStore<Users>, ExtendUserStore<ApplicationDbContext>>();
            services.AddScoped<IRoleStore<Roles>, RoleStore<Roles>>();
            services.AddScoped<IOpenIddictApplicationStore<Applications>, OpenIddictApplicationStore<Applications, Authorization, Token, ApplicationDbContext, string>>();
            services.AddScoped<IOpenIddictApplicationStore<OpenIddictApplication>, OpenIddictApplicationStore<OpenIddictApplication, OpenIddictAuthorization, OpenIddictToken, ApplicationDbContext, string>>();

            services.AddScoped<ApplicationStore>();

            services.AddScoped<OpenIddictAuthorizationManager<Authorization>>();
            services.AddScoped<OpenIddictTokenManager<Token>>();
            services.AddScoped<OpenIddictApplicationManager<ApplicationDbContext>>();

            services.AddScoped<IRoleApplicationStore, RoleApplicationStore<ApplicationDbContext>>();
            services.AddScoped<RoleApplicationManager>();

            services.AddScoped<IUserRoleStore, UserRoleStore<ApplicationDbContext>>();

            services.AddScoped<IUserLoginLogStore, UserLoginLogStore<ApplicationDbContext>>();
            services.AddScoped<UserLoginLogManager>();

            services.AddScoped<IUserExtensionsStore, UserExtensionsStore<ApplicationDbContext>>();
            services.AddScoped<IUserExtensionsManager, UserExtensionsManager>();

            return new UserDefinedBuilder(services);
        }


    }
}
