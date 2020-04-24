using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace NbIotCmd
{
    public static class IocManager
    {
        public static IServiceCollection Services { get; private set; }

        public static IServiceProvider ServiceProvider { get; private set; }

        static IocManager()
        {
            Services = new ServiceCollection();
        }

        public static void Build()
        {
            if (ServiceProvider != null)
            {
                return;
            }

            ServiceProvider = Services.BuildServiceProvider();
        }
    }
}
