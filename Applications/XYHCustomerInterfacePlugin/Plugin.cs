using ApplicationCore;
using ApplicationCore.Plugin;
using GatewayInterface;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using XYHCustomerPlugin.Controllers;

namespace XYHCustomerInterfacePlugin
{
    public class Plugin : PluginBase
    {
        public override string Description => "客源SDK";

        public override string PluginID => "2af8ce41-1c08-4566-9e69-279a1219c5c7";

        public override string PluginName => "客源对外接口";


        public override Task<ResponseMessage> Init(ApplicationContext context)
        {
            context.Services.AddScoped<ICustomerInterface, CustomerInterfaceImplement>();
            context.Services.AddScoped<CustomerInfoController>();
            context.Services.AddScoped<CustomerDealController>();
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
