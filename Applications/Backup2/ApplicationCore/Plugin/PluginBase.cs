using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ApplicationCore.Plugin
{
    public abstract class PluginBase : IPlugin
    {
        public static readonly int DefaultOrder = 1000;

        public abstract string Description { get; }

        public virtual int Order
        {
            get
            {
                return DefaultOrder;
            }
        }
        public abstract string PluginID { get; }
        public abstract string PluginName { get; }

        public virtual Task<ResponseMessage> Init(ApplicationContext context)
        {
            return Task.FromResult(new ResponseMessage());
        }
        public virtual Task<ResponseMessage> Start(ApplicationContext context)
        {
            return Task.FromResult(new ResponseMessage());
        }
        public virtual Task<ResponseMessage> Stop(ApplicationContext context)
        {
            return Task.FromResult(new ResponseMessage());
        }


        public virtual Task OnMainConfigChanged(ApplicationContext context, ApplicationConfig newConfig)
        {
            return Task.CompletedTask;
        }
    }
}
