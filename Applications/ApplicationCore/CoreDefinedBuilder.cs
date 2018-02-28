using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;

namespace ApplicationCore
{
    public class CoreDefinedBuilder
    {
        public CoreDefinedBuilder(IServiceCollection services)
        {
            Services = services ?? throw new ArgumentNullException(nameof(services));
        }
        IServiceCollection Services { get; }
    }
}
