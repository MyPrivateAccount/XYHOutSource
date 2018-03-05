using System;
using System.Threading.Tasks;
using ApplicationCore.Plugin;
using ApplicationCore;
using XYHShopsPlugin.Models;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using XYHShopsPlugin.Stores;
using XYHShopsPlugin.Managers;
using Microsoft.Extensions.Configuration;

namespace XYHShopsPlugin
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
                return "房源管理";
            }
        }

        public override string Description
        {
            get
            {
                return "房源管理插件";
            }
        }


        public override Task<ResponseMessage> Init(ApplicationContext context)
        {
            context.Services.AddDbContext<ShopsDbContext>(options => { options.UseMySql(context.ConnectionString); }, ServiceLifetime.Scoped);

            context.Services.AddScoped<IBuildingBaseInfoStore, BuildingBaseInfoStore>();
            context.Services.AddScoped<IBuildingFacilitiesStore, BuildingFacilitiesStore>();
            context.Services.AddScoped<IBuildingShopInfoStore, BuildingShopInfoStore>();
            context.Services.AddScoped<IBuildingsStore, BuildingsStore>();
            context.Services.AddScoped<IFileInfoStore, FileInfoStore>();
            context.Services.AddScoped<IShopsFileScopeStore, ShopsFileScopeStore>();
            context.Services.AddScoped<IShopBaseInfoStore, ShopBaseInfoStore>();
            context.Services.AddScoped<IShopFacilitiesStore, ShopFacilitiesStore>();
            context.Services.AddScoped<IShopLeaseInfoStore, ShopLeaseInfoStore>();
            context.Services.AddScoped<IShopsStore, ShopsStore>();
            context.Services.AddScoped<IBuildingFileScopeStore, BuildingFileScopeStore>();
            context.Services.AddScoped<IBuildingFavoritesStore, BuildingFavoriteStore>();
            context.Services.AddScoped<IShopsFavoriteStore, ShopsFavoriteStore>();
            context.Services.AddScoped<IBuildingRecommendStore, BuildingRecommendStore>();
            context.Services.AddScoped<IBuildingNoStore, BuildingNoStore>();
            context.Services.AddScoped<IBuildingRuleStore, BuildingRuleStore>();
            context.Services.AddScoped<IUpdateRecordFileScopeStore, UpdateRecordFileScopeStore>();

            context.Services.AddScoped<BuildingBaseInfoManager>();
            context.Services.AddScoped<BuildingFacilitiesManager>();
            context.Services.AddScoped<BuildingShopInfoManager>();
            context.Services.AddScoped<BuildingsManager>();
            context.Services.AddScoped<FileInfoManager>();
            context.Services.AddScoped<FileScopeManager>();
            context.Services.AddScoped<ShopBaseInfoManager>();
            context.Services.AddScoped<ShopFacilitiesManager>();
            context.Services.AddScoped<ShopLeaseInfoManager>();
            context.Services.AddScoped<ShopsManager>();
            context.Services.AddScoped<BuildingFavoriteManager>();
            context.Services.AddScoped<ShopsFavoriteManager>();
            context.Services.AddScoped<BuildingRecommendManager>();
            context.Services.AddScoped<BuildingNoManager>();
            context.Services.AddScoped<BuildingTimingManager>();
            context.Services.AddScoped<BuildingRuleManager>();

            context.Services.AddScoped<IUpdateRecordStore, UpdateRecordStore>();
            context.Services.AddScoped<UpdateRecordManager>();

            context.Services.AddScoped<IBuildingNoticeFileScopeStore, BuildingNoticeFileScopeStore>();
            context.Services.AddScoped<IBuildingNoticeStore, BuildingNoticeStore>();

            context.Services.AddScoped<BuildingNoticeManager>();

            context.Services.AddScoped<IOrganizationExpansionStore, OrganizationExpansionStore>();
            context.Services.AddScoped<BuildingTimingManager>();
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
