using ApplicationCore;
using ApplicationCore.Managers;
using ApplicationCore.Plugin;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Threading.Tasks;
using XYHCustomerPlugin.Managers;
using XYHCustomerPlugin.Models;
using XYHCustomerPlugin.Stores;

namespace XYHCustomerPlugin
{
    public class Plugin : PluginBase
    {
        public override string PluginID
        {
            get
            {
                return "e51acc0f-32dc-4409-b5d2-1db518269cba";
            }
        }

        public override string PluginName
        {
            get
            {
                return "客源管理";
            }
        }

        public override string Description
        {
            get
            {
                return "客源管理插件";
            }
        }


        public override Task<ResponseMessage> Init(ApplicationContext context)
        {
            context.Services.AddDbContext<CustomerDbContext>(options => { options.UseMySql(context.ConnectionString); }, ServiceLifetime.Scoped);
            //SaveChangesAync中会导致程序卡死
            //options.EnableSensitiveDataLogging();
            context.Services.AddScoped<CustomerInfoManager>();
            context.Services.AddScoped<ICustomerInfoStore, CustomerInfoStore>();
            context.Services.AddScoped<CustomerDemandManager>();
            context.Services.AddScoped<ICustomerDemandStore, CustomerDemandStore>();
            context.Services.AddScoped<AboutLookManager>();
            context.Services.AddScoped<IAboutLookStore, AboutLookStore>();
            context.Services.AddScoped<CustomerReportManager>();
            context.Services.AddScoped<ICustomerReportStore, CustomerReportStore>();
            context.Services.AddScoped<CustomerFollowUpManager>();
            context.Services.AddScoped<ICustomerFollowUpStore, CustomerFollowUpStore>();

            context.Services.AddScoped<BeltLookManager>();
            context.Services.AddScoped<IBeltLookStore, BeltLookStore>();

            context.Services.AddScoped<CustomerTransactionsFollowUpManager>();
            context.Services.AddScoped<ICustomerTransactionsFollowUpStore, CustomerTransactionsFollowUpStore>();

            context.Services.AddScoped<CustomerTransactionsManager>();
            context.Services.AddScoped<ICustomerTransactionsStore, CustomerTransactionsStore>();

            //context.Services.AddScoped<CustomerPhoneManager>();
            context.Services.AddScoped<ICustomerPhoneStore, CustomerPhoneStore>();

            context.Services.AddScoped<CustomerHandOverManager>();

            context.Services.AddScoped<ICustomerPoolStore, CustomerPoolStore>();

            context.Services.AddScoped<ICustomerPoolDefineStore, CustomerPoolDefineStore>();

            context.Services.AddScoped<CustomerLossManager>();
            context.Services.AddScoped<ICustomerLossStore, CustomerLossStore>();

            context.Services.AddScoped<FileInfoManager>();
            context.Services.AddScoped<IFileInfoStore, FileInfoStore>();

            context.Services.AddScoped<CustomerDealManager>();
            context.Services.AddScoped<ICustomerDealStore, CustomerDealStore>();

            context.Services.AddScoped<FileScopeManager>();
            context.Services.AddScoped<IDealFileScopeStore, DealFileScopeStore>();

            context.Services.AddScoped<CustomerFilescopeManager>();
            context.Services.AddScoped<ICustomerFilescopeStore, CustomerFilescopeStore>();

            context.Services.AddScoped<CustomerTimingManager>();

            context.Services.AddScoped<CustomerPoolManager>();

            context.Services.AddScoped<CustomerPoolDefineManager>();
            context.Services.AddScoped<PermissionExpansionManager>();

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
