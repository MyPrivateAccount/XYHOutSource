﻿using System;
using System.Threading.Tasks;
using ApplicationCore;
using ApplicationCore.Plugin;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using XYHContractPlugin.Models;
using XYHContractPlugin.Stores;
using XYHContractPlugin.Managers;
using ApplicationCore.Managers;

namespace XYHContractPlugin
{
    public class Plugin : PluginBase
    {
        public override string PluginID
        {
            get
            {
                return "7f5eebef-82a4-4493-ba9c-b255b30120b4";
            }
        }

        public override string PluginName
        {
            get
            {
                return "合同数据";
            }
        }

        public override string Description
        {
            get
            {
                return "合同数据插件";
            }
        }


        public override Task<ResponseMessage> Init(ApplicationContext context)
        {
            context.Services.AddDbContext<ContractDbContext>(options => options.UseMySql(context.ConnectionString), ServiceLifetime.Scoped);
            context.Services.AddScoped<IContractInfoStore, ContractInfoStore>();
            context.Services.AddScoped<IContractFileScopeStore, ContractFileScopeStore>();
            context.Services.AddScoped<IFileInfoStore, FileInfoStore>();
            context.Services.AddScoped<ContractListManager>();
            context.Services.AddScoped<ContractInfoManager>();
            context.Services.AddScoped<PermissionExpansionManager>();
        
            context.Services.AddScoped<XYHContractPlugin.Managers.FileScopeManager>();
            context.Services.AddScoped<FileInfoManager>();
            context.Services.AddScoped<ExtraInfoDataManager>();
            context.Services.AddScoped<IExtraDataInfoStrore, ExtraDataInfoStore>();
            context.Services.AddScoped<PermissionExpansionManager>();
            context.Services.AddScoped<UserManager>();
            context.Services.AddScoped<IOrganizationExpansionStore, OrganizationExpansionStore>();
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
