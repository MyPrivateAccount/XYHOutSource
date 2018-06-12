using ApplicationCore.Managers;
using ApplicationCore.Models;
using ApplicationCore.Stores;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace ApplicationCore
{
    public static class ICoreServiceCollectionExtensions
    {
        public static CoreDefinedBuilder AddUserDefined(this IServiceCollection services)
        {
            if (services == null)
            {
                throw new ArgumentNullException(nameof(services));
            }
            services.AddScoped<OrganizationExpansionManager>();
            services.AddScoped<PermissionExpansionManager>();
            // services.AddScoped<Microsoft.AspNetCore.Identity.UserManager<Users>>();
            services.AddScoped<UserManager>();

            services.AddScoped<FeedbackManager>();
            services.AddScoped<IFeedbackStore, FeedbackStore>();


            //services.AddScoped<UserManager>();
            //services.AddScoped<DbContext, CoreDbContext>();

            services.AddSingleton<RestClient>();

            services.AddScoped<IOrganizationExpansionStore, OrganizationExpansionStore<CoreDbContext>>();
            services.AddScoped<IPermissionExpansionStore, PermissionExpansionStore<CoreDbContext>>();
            services.AddScoped<IUserStore, Stores.UserStore<CoreDbContext>>();

            return new CoreDefinedBuilder(services);
        }


    }
}
