using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Plugin
{
    public interface IPlugin
    {
        string PluginID { get; }

        string PluginName { get; }

        string Description { get; }

        int Order { get; }


        Task<ResponseMessage> Init(ApplicationContext context);

        Task<ResponseMessage> Start(ApplicationContext context);

        Task<ResponseMessage> Stop(ApplicationContext context);

        Task OnMainConfigChanged(ApplicationContext context, ApplicationConfig newConfig);
    }
}
