using ApplicationCore;
using ApplicationCore.Plugin;
using GatewayInterface;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ExamineCenterInterfacePlugin
{
    public class Plugin : PluginBase
    {
        public override string Description => "审核中心对外提供的SDK";

        public override string PluginID => "8032a080-1887-4eb7-8d90-606e73ca57bc";

        public override string PluginName => "审核SDK";

        public override Task<ResponseMessage> Init(ApplicationContext context)
        {
            context.Services.AddScoped<IExamineInterface, ExamineInterfaceImplement>();
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
