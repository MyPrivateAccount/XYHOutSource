using ApplicationCore;
using ApplicationCore.Plugin;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using XYHBaseDataPlugin.Managers;
using XYHBaseDataPlugin.Models;
using XYHBaseDataPlugin.Stores;

namespace XYHBaseDataPlugin
{
    public class Plugin : PluginBase
    {
        public override string PluginID
        {
            get
            {
                return "dc0fb4d7-ee92-4718-9524-6c42c600554e";
            }
        }

        public override string PluginName
        {
            get
            {
                return "基础数据";
            }
        }

        public override string Description
        {
            get
            {
                return "基础数据插件";
            }
        }


        public override Task<ResponseMessage> Init(ApplicationContext context)
        {
            //context.Services.AddDbContext<BaseDataDbContext>(options => options.UseMySql("Server=server-d01;database=xinyaohang;uid=root;pwd=root;"));
            context.Services.AddDbContext<BaseDataDbContext>(options => options.UseMySql(context.ConnectionString));
            context.Services.AddScoped<IDictionaryGroupStore, DictionaryGroupStore>();
            context.Services.AddScoped<IDictionaryDefineStore, DictionaryDefineStore>();
            context.Services.AddScoped<DictionaryGroupManager>();
            context.Services.AddScoped<DictionaryDefineManager>();
            context.Services.AddScoped<UserTypeValueManager>();


            context.Services.AddScoped<IAreaDefineStore, AreaDefineStore>();
            context.Services.AddScoped<IUserTypeValueStore, UserTypeValueStore>();
            context.Services.AddScoped<AreaDefineManager>();

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
