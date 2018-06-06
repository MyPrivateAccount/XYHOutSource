using ApplicationCore;
using ApplicationCore.Plugin;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using XYHHumanPlugin.Models;
using XYHHumanPlugin.Stores;
using XYHHumanPlugin.Managers;
using ApplicationCore.Managers;

namespace XYHHumanPlugin
{
    public class Plugin : PluginBase
    {
        public override string PluginID
        {
            get
            {
                return "e7acec14-a68b-4116-b9a0-7d07be69de58";
            }
        }

        public override string PluginName
        {
            get
            {
                return "人事数据";
            }
        }

        public override string Description
        {
            get
            {
                return "人事据插件";
            }
        }


        public override Task<ResponseMessage> Init(ApplicationContext context)
        {
            //context.Services.AddDbContext<BaseDataDbContext>(options => options.UseMySql("Server=server-d01;database=xinyaohang;uid=root;pwd=root;"));
            context.Services.AddDbContext<HumanDbContext>(options => options.UseMySql(context.ConnectionString), ServiceLifetime.Scoped);
            context.Services.AddScoped<IHumanManageStore, HumanManageStore>();
            context.Services.AddScoped<IOrganizationExpansionStore, OrganizationExpansionStore>();
            context.Services.AddScoped<HumanManager>();
            context.Services.AddScoped<MonthManager>();
            context.Services.AddScoped<StationManager>();
            context.Services.AddScoped<SalaryManager>();
            context.Services.AddScoped<BlackManager>();
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
