using ApplicationCore;
using ApplicationCore.Plugin;
using GatewayInterface;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using XYHShopsPlugin.Controllers;

namespace XYHShopsInterfacePlugin
{
    public class Plugin : PluginBase
    {
        public override string Description => "房源SDK";

        public override string PluginID => "5c147750-d718-46eb-b0f7-7583be159de4";

        public override string PluginName => "房源对外接口";


        public override Task<ResponseMessage> Init(ApplicationContext context)
        {
            context.Services.AddScoped<IShopsInterface, ShopsInterfaceImplement>();
            context.Services.AddScoped<BuildingsController>();
            context.Services.AddScoped<BuildingRuleController>();
            context.Services.AddScoped<ShopsController>();
            context.Services.AddScoped<UpdateRecordController>();
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
