using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using SimplCommerce.Infrastructure.Modules;

namespace SimplCommerce.Module.TestModuleEric01
{
    public class ModuleInitializer : IModuleInitializer
    {
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            //throw new NotImplementedException();
        }

        public void ConfigureServices(IServiceCollection serviceCollection)
        {
            //throw new NotImplementedException();
        }
    }
}
