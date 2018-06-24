using ApplicationCore;
using ApplicationCore.Managers;
using ApplicationCore.Plugin;
using ApplicationCore.Stores;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using XYHChargePlugin.Managers;
using XYHChargePlugin.Models;
using XYHChargePlugin.Stores;

namespace XYHChargePlugin
{
    public class Plugin : PluginBase
    {
        public override string PluginID
        {
            get
            {
                // {9DF858E9-8896-4EFD-82EE-4D6A1F5E9FA0}
                return "9df858e9-8896-4efd-82ee-4d6a1f5e9fa0";
            }
        }

        public override string PluginName
        {
            get
            {
                return "费用数据";
            }
        }

        public override string Description
        {
            get
            {
                return "费用插件";
            }
        }


        public override Task<ResponseMessage> Init(ApplicationContext context)
        {
            //context.Services.AddDbContext<ChargeDbContext>(options => options.UseMySql("Server=server-d01;database=xinyaohang;uid=root;pwd=root;"));
            context.Services.AddDbContext<ChargeDbContext>(options => options.UseMySql(context.ConnectionString), ServiceLifetime.Scoped);

            context.Services.AddScoped<ITransaction<ChargeDbContext>, Transaction<ChargeDbContext>>();
            context.Services.AddScoped<IChargeInfoStore, ChargeInfoStore>();
            context.Services.AddScoped<IOrganizationUtils, OrganizationUtils>();
            context.Services.AddScoped<ILimitInfoStore, LimitInfoStore>();
            context.Services.AddScoped<IBorrowingStore, BorrowingStore>();

            context.Services.AddScoped<ChargeManager>();
            context.Services.AddScoped<LimitManager>();
            context.Services.AddScoped<BorrowingManager>();
            context.Services.AddScoped<PermissionExpansionManager>();

            return base.Init(context);
        }


        public override Task<ResponseMessage> Start(ApplicationContext context)
        {
            return base.Start(context);
        }

        public override Task<ResponseMessage> Stop(ApplicationContext context)
        {
            return base.Stop(context);
        }
    }
}
