using ApplicationCore;
using ApplicationCore.Plugin;
using System;
using GatewayInterface;
using System.Threading.Tasks;
using XYHHumanPlugin.Controllers;
using Microsoft.Extensions.DependencyInjection;

namespace XYHHumanInterfacePlugin
{
    public class Plugin : PluginBase
    {
        public override string Description => "合同SDK";

        public override string PluginID => "8104363e-eb80-4082-94aa-8108030e397c";
        // {}

        public override string PluginName => "人事对外接口";


        public override Task<ResponseMessage> Init(ApplicationContext context)
        {
            //context.Services.AddScoped<IShopsInterface, ShopsInterfaceImplement>();
            //context.Services.AddScoped<BuildingsController>();
            //context.Services.AddScoped<BuildingRuleController>();
            //context.Services.AddScoped<ShopsController>();
            //context.Services.AddScoped<UpdateRecordController>();
            context.Services.AddScoped<IHumanInterface, HumanInterfaceImplement>();
            context.Services.AddScoped<HumanInfoController>();
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
