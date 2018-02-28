using ApplicationCore.Interface;
using ApplicationCore.Plugin;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore
{
    public class ApplicationContext
    {
        public List<Assembly> AdditionalAssembly { get; set; }

        public IServiceCollection Services { get; protected set; }

        public static ApplicationContext Current { get; private set; }

        public IServiceProvider Provider { get; set; }

        public ApplicationConfig Config { get; protected set; }
        public IPluginFactory PluginFactory { get; set; }
        public string ConnectionString { get; set; }

        public string NWFUrl { get; set; }
        public string NWFExamineCallbackUrl { get; set; }
        public string ExamineUrl { get; set; }
        public string BuildingExamineCallbackUrl { get; set; }
        public string UpdateExamineCallbackUrl { get; set; }
        public string FileServerRoot { get; set; }

        public string MessageServerUrl { get; set; }
        public ApplicationContext(IServiceCollection serviceContainer)
        {
            Current = this;
            Services = serviceContainer;
        }

        public async virtual Task<bool> Init()
        {
            return true;
        }

        public async virtual Task<bool> Start()
        {
            return true;
        }

        public async virtual Task<bool> Stop()
        {
            return true;
        }


    }
}
