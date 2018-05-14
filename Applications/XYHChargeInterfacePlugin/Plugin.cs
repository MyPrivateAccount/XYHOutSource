using ApplicationCore;
using ApplicationCore.Plugin;
using GatewayInterface;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using XYHChargePlugin.Controllers;

namespace XYHChargeInterfacePlugin
{
    public class Plugin : PluginBase
    {
        public override string Description => "费用SDK";

        public override string PluginID => "d835654e-3997-4d2c-a4eb-9207aede326e";
        // {}
        // {D835654E-3997-4D2C-A4EB-9207AEDE326E}
    
        public override string PluginName => "费用对外接口";


        public override Task<ResponseMessage> Init(ApplicationContext context)
        {
            //context.Services.AddScoped<IShopsInterface, ShopsInterfaceImplement>();
            //context.Services.AddScoped<BuildingsController>();
            //context.Services.AddScoped<BuildingRuleController>();
            //context.Services.AddScoped<ShopsController>();
            //context.Services.AddScoped<UpdateRecordController>();
            context.Services.AddScoped<IChargeInterface, ChargeInterfaceImplement>();
            context.Services.AddScoped<ChargeInfoController>();
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
