using System;
using GatewayInterface;
using Microsoft.Extensions.DependencyInjection;
using System.Threading.Tasks;
using ApplicationCore;
using ApplicationCore.Plugin;
using XYHContractPlugin.Controllers;

namespace XYHContractInterfacePlugin
{
    public class Plugin: PluginBase
    {
        public override string Description => "合同SDK";

        public override string PluginID => "d2bdc636-0904-4d95-9f53-e0c3d57cfc69";

        public override string PluginName => "合同对外接口";


        public override Task<ResponseMessage> Init(ApplicationContext context)
        {
            //context.Services.AddScoped<IShopsInterface, ShopsInterfaceImplement>();
            //context.Services.AddScoped<BuildingsController>();
            //context.Services.AddScoped<BuildingRuleController>();
            //context.Services.AddScoped<ShopsController>();
            //context.Services.AddScoped<UpdateRecordController>();
            context.Services.AddScoped<IContractInterface, ContractInterfaceImplement>();
            //context.Services.AddScoped<ContractInfoController>();
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
