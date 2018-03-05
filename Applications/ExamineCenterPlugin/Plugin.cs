using ApplicationCore;
using ApplicationCore.Plugin;
using ExamineCenterPlugin.Controllers;
using ExamineCenterPlugin.Managers;
using ExamineCenterPlugin.Models;
using ExamineCenterPlugin.Stores;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;

namespace ExamineCenterPlugin
{
    public class Plugin : PluginBase
    {
        public override string PluginID
        {
            get
            {
                return "60daa98b-dbfa-41d0-b718-4bb050b9e6e8";
            }
        }

        public override string PluginName
        {
            get
            {
                return "审核管理";
            }
        }

        public override string Description
        {
            get
            {
                return "审核管理插件";
            }
        }


        public override Task<ResponseMessage> Init(ApplicationContext context)
        {
            context.Services.AddDbContext<ExamineDbContext>(options => { options.UseMySql(context.ConnectionString); }, ServiceLifetime.Scoped);

            context.Services.AddScoped<IExamineFlowStore, ExamineFlowStore>();
            context.Services.AddScoped<IExamineNoticeStore, ExamineNoticeStore>();
            context.Services.AddScoped<ExamineFlowManager>();
            context.Services.AddScoped<ExamineNoticeManager>();

            context.Services.AddScoped<ExamineFlowController>();


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
