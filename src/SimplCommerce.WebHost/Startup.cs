using System;
using System.Linq;
using System.Text.Encodings.Web;
using System.Text.Unicode;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Razor;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.WebEncoders;
using Microsoft.OpenApi.Models;
using SimplCommerce.Infrastructure;
using SimplCommerce.Infrastructure.Data;
using SimplCommerce.Infrastructure.Modules;
using SimplCommerce.Infrastructure.Web;
using SimplCommerce.Module.Core.Data;
using SimplCommerce.Module.Core.Extensions;
using SimplCommerce.Module.Localization.Extensions;
using SimplCommerce.Module.Localization.TagHelpers;
using SimplCommerce.WebHost.Extensions;

namespace SimplCommerce.WebHost
{
    public class Startup
    {
        private readonly IWebHostEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration, IWebHostEnvironment hostingEnvironment)
        {
            _configuration = configuration;
            _hostingEnvironment = hostingEnvironment;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            GlobalConfiguration.WebRootPath = _hostingEnvironment.WebRootPath;
            GlobalConfiguration.ContentRootPath = _hostingEnvironment.ContentRootPath;

            /*During the application startup, the host will scan for all the *.dll in the Modules folder and load them up using AssemblyLoadContext. 
             * These assemblies will be then registered to MVC Core by the AddApplicationPart method.*/
            /*
            assemblies 讀取，讀進程序中，尚未實體化 
            */
            services.AddModules(_hostingEnvironment.ContentRootPath);

            services.Configure<CookiePolicyOptions>(options =>
            {
                // This lambda determines whether user consent for non-essential cookies is needed for a given request.
                options.CheckConsentNeeded = context => true;
                options.MinimumSameSitePolicy = SameSiteMode.None;
            });

            // 資料庫 DataStore
            services.AddCustomizedDataStore(_configuration);

            services.AddCustomizedIdentity(_configuration);
            services.AddHttpClient();

            services.AddTransient(typeof(IRepository<>), typeof(Repository<>));
            services.AddTransient(typeof(IRepositoryWithTypedId<,>), typeof(RepositoryWithTypedId<,>));
            services.AddScoped<SlugRouteValueTransformer>();

            services.AddCustomizedLocalization();

            services.AddCustomizedMvc(GlobalConfiguration.Modules);
            services.Configure<RazorViewEngineOptions>(
                options => { options.ViewLocationExpanders.Add(new ThemeableViewLocationExpander()); });
            services.Configure<WebEncoderOptions>(options =>
            {
                options.TextEncoderSettings = new TextEncoderSettings(UnicodeRanges.All);
            });
            services.AddScoped<ITagHelperComponent, LanguageDirectionTagHelperComponent>();
            services.AddTransient<IRazorViewRenderer, RazorViewRenderer>();
            services.AddAntiforgery(options => options.HeaderName = "X-XSRF-Token");
            services.AddCloudscribePagination();

            #region 這裡進行 Modules 的 載入與準備
            /*
             這裡才是真正的 assemblies 實體化
                A. Scoped	1	Specifies that a new instance of the service will be created for each scope.
                B. Singleton	0	Specifies that a single instance of the service will be created.
                C. Transient	2	Specifies that a new instance of the service will be created every time it is requested.
             */
            foreach (var module in GlobalConfiguration.Modules)
            {
                var moduleInitializerType = module.Assembly.GetTypes()
                   .FirstOrDefault(t => typeof(IModuleInitializer).IsAssignableFrom(t));
                if ((moduleInitializerType != null) && (moduleInitializerType != typeof(IModuleInitializer)))
                {
                    /*
                     System.Activator
                     */
                    var moduleInitializer = (IModuleInitializer)Activator.CreateInstance(moduleInitializerType);

                    // Microsoft.Extensions.DependencyInjection.ServiceLifetime提供了三種類型 , 實體化
                    services.AddSingleton(typeof(IModuleInitializer), moduleInitializer);

                    // 對於實體化後的物件，進行 Configure
                    moduleInitializer.ConfigureServices(services);
                }
            }

            #endregion

            services.AddScoped<ServiceFactory>(p => p.GetService);
            services.AddScoped<IMediator, Mediator>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "SimplCommerce API", Version = "v1" });
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseDatabaseErrorPage();
            }
            else
            {
                app.UseWhen(
                    context => !context.Request.Path.StartsWithSegments("/api"),
                    a => a.UseExceptionHandler("/Home/Error")
                );
                app.UseHsts();
            }

            app.UseWhen(
                context => !context.Request.Path.StartsWithSegments("/api"),
                a => a.UseStatusCodePagesWithReExecute("/Home/ErrorWithCode/{0}")
            );

            app.UseHttpsRedirection();
            app.UseCustomizedStaticFiles(env);
            app.UseRouting();
            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "SimplCommerce API V1");
            });
            app.UseCookiePolicy();
            app.UseCustomizedIdentity();
            app.UseCustomizedRequestLocalization();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapDynamicControllerRoute<SlugRouteValueTransformer>("/{**slug}");
                endpoints.MapControllerRoute(
                    name: "areas",
                    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
                endpoints.MapControllerRoute(
                    name: "default",
                    pattern: "{controller=Home}/{action=Index}/{id?}");
            });

            var moduleInitializers = app.ApplicationServices.GetServices<IModuleInitializer>();
            foreach (var moduleInitializer in moduleInitializers)
            {
                moduleInitializer.Configure(app, env);
            }
        }
    }
}
