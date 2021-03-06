﻿using ApplicationCore;
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
            context.Services.AddScoped<HumanManager>();
            context.Services.AddScoped<HumanInfoManager>();
            context.Services.AddScoped<MonthManager>();
            context.Services.AddScoped<StationManager>();
            context.Services.AddScoped<SalaryManager>();
            context.Services.AddScoped<BlackManager>();
            context.Services.AddScoped<AttendanceManager>();
            context.Services.AddScoped<PermissionExpansionManager>();
            context.Services.AddScoped<RewardPunishmentManager>();

            context.Services.AddScoped<HumanInfoManager>();
            context.Services.AddScoped<IHumanInfoStore, HumanInfoStore>();

            context.Services.AddScoped<HumanInfoBlackManager>();
            context.Services.AddScoped<IHumanInfoBlackStore, HumanInfoBlackStore>();

            context.Services.AddScoped<IHumanInfoChangeStore, HumanInfoChangeStore>();
            context.Services.AddScoped<HumanInfoChange>();
            context.Services.AddScoped<IHumanInfoAdjustmentStore, HumanInfoAdjustmentStore>();
            context.Services.AddScoped<HumanInfoAdjustmentManager>();
            context.Services.AddScoped<IHumanInfoLeaveStore, HumanInfoLeaveStore>();
            context.Services.AddScoped<HumanInfoLeaveManager>();
            context.Services.AddScoped<IHumanInfoPartPositionStore, HumanInfoPartPositionStore>();
            context.Services.AddScoped<HumanInfoPartPositionManager>();
            context.Services.AddScoped<IHumanInfoRegularStore, HumanInfoRegularStore>();
            context.Services.AddScoped<HumanInfoRegularManager>();

            context.Services.AddScoped<IHumanPositionStore, HumanPositionStore>();
            context.Services.AddScoped<HumanPositionManager>();

            context.Services.AddScoped<IPositionSalaryStore, PositionSalaryStore>();
            context.Services.AddScoped<PositionSalaryManager>();

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
