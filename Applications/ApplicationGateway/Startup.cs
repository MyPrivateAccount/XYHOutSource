using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using ApplicationCore;
using System.Threading;
using ApplicationCore.Interface;
using ApplicationCore.Plugin;
using AutoMapper;
using StackExchange.Redis;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using ApplicationCore.Models;
using Microsoft.AspNetCore.Identity;
using AspNet.Security.OAuth.Validation;
using System.IO;
using Microsoft.Extensions.PlatformAbstractions;
using Swashbuckle.AspNetCore.Swagger;

namespace ApplicationGateway
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private static ApplicationContextImpl applicationContext = null;

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("config.json")
                .AddEnvironmentVariables()
                .Build();
            services.AddSingleton<IConfigurationRoot>(configuration);
            services.AddMvc();

            services.AddDbContext<CoreDbContext>(options =>
            {
                options.UseMySql(configuration["Data:DefaultConnection:ConnectionString"]);
            });

            var redisSection = configuration.GetSection("Redis");
            services.AddDistributedRedisCache(options =>
            {
                redisSection.Bind(options);
            });

            string redisConfiguration = redisSection["Configuration"];
            var redis = ConnectionMultiplexer.Connect(redisConfiguration);
            services.AddDataProtection(options =>
            {
                options.ApplicationDiscriminator = "ApplicationGateway";
            }).PersistKeysToRedis(redis);

            string authUrl = configuration["AuthUrl"];
            string clientId = configuration["ClientID"];
            string clientSecret = configuration["ClientSecret"];
            services.AddAuthentication(options =>
                {
                    options.DefaultScheme = OAuthValidationDefaults.AuthenticationScheme;

                })
                .AddOAuthIntrospection(options =>
                {
                    options.Authority = new Uri(authUrl);
                    //options.Audiences.Add("*");
                    options.ClientId = clientId;
                    options.ClientSecret = clientSecret;
                    options.RequireHttpsMetadata = false;
                    //options.SaveToken = true;
                    //options.MetadataAddress = new Uri("http://server-d01:5000/.well-known/openid-configuration");
                });

            services.Configure<IdentityOptions>(options =>
            {
                options.ClaimsIdentity.UserNameClaimType = "name";
                options.ClaimsIdentity.UserIdClaimType = "sub";
                options.ClaimsIdentity.RoleClaimType = "role";
            });
            applicationContext = new ApplicationContextImpl(services);
            //applicationContext.PluginConfigStorage = new DefaultPluginConfigStorage();
            applicationContext.PluginFactory = new DefaultPluginFactory();

            applicationContext.ConnectionString = configuration["Data:DefaultConnection:ConnectionString"];
            applicationContext.NWFUrl = configuration["NWFUrl"];
            applicationContext.ExamineUrl = configuration["ExamineUrl"];
            applicationContext.FileServerRoot = configuration["FileServerRoot"];
            applicationContext.BuildingExamineCallbackUrl = configuration["BuildingExamineCallbackUrl"];
            applicationContext.NWFExamineCallbackUrl = configuration["NWFExamineCallbackUrl"];
            applicationContext.UpdateExamineCallbackUrl = configuration["UpdateExamineCallbackUrl"];
            applicationContext.MessageServerUrl = configuration["MessageServerUrl"];

            var environment = services.FirstOrDefault(x => x.ServiceType == typeof(IHostingEnvironment))?.ImplementationInstance;
            var apppart = services.FirstOrDefault(x => x.ServiceType == typeof(ApplicationPartManager))?.ImplementationInstance;
            if (apppart != null)
            {
                ApplicationPartManager apm = apppart as ApplicationPartManager;
                //所有附件程序集
                ApplicationContextImpl ac = ApplicationContext.Current as ApplicationContextImpl;
                ac.AdditionalAssembly.ForEach((a) =>
                {
                    apm.ApplicationParts.Add(new AssemblyPart(a));
                });
            }
            bool InitIsOk = applicationContext.Init().Result;

            services.AddUserDefined();

            //services.AddSwaggerGen();
            //services.ConfigureSwaggerGen(options =>
            //{
            //    options.SingleApiVersion(new Swashbuckle.Swagger.Model.Info
            //    {
            //        Version = "v1",
            //        Title = "My Web Application",
            //        Description = "RESTful API for My Web Application",
            //        TermsOfService = "None"
            //    });
            //    options.IncludeXmlComments(Path.Combine(PlatformServices.Default.Application.ApplicationBasePath,
            //        "ApplicationGateway.xml")); // 注意：此处替换成所生成的XML documentation的文件名。
            //    options.DescribeAllEnumsAsStrings();
            //});

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "应用网关接口文档",
                    TermsOfService = "None",
                });
                //Set the comments path for the swagger json and ui.
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var xmlPath = Path.Combine(basePath, "Plugin", "XYHCustomerPlugin.xml");
                var xmlPath2 = Path.Combine(basePath, "Plugin", "XYHShopsPlugin.xml");
                var xmlPath3 = Path.Combine(basePath, "Plugin", "XYHBaseDataPlugin.xml");
                var xmlPath4 = Path.Combine(basePath, "Plugin", "ExamineCenterPlugin.xml");
                var xmlPath5 = Path.Combine(basePath, "Plugin", "MessageServerPlugin.xml");
                var xmlPath6 = Path.Combine(basePath, "Plugin", "XYHStatisticalPlugin.xml");
                c.IncludeXmlComments(xmlPath);
                c.IncludeXmlComments(xmlPath2);
                c.IncludeXmlComments(xmlPath3);
                c.IncludeXmlComments(xmlPath4);
                c.IncludeXmlComments(xmlPath5);
                c.IncludeXmlComments(xmlPath6);
                c.OperationFilter<HttpHeaderOperation>(); // 添加httpHeader参数
            });
            services.ConfigureSwaggerGen(options =>
            {
                options.CustomSchemaIds(x => x.FullName);
            });


            //插件加载之后引用
            services.AddAutoMapper();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddProvider(new XYHLoggerProvider());
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            app.UseCors(options =>
            {
                options.AllowAnyHeader();
                options.AllowAnyMethod();
                options.AllowAnyOrigin();
                options.AllowCredentials();
            });
            if (env.IsDevelopment())
            {
                app.UseSwagger();
                // Enable middleware to serve swagger-ui (HTML, JS, CSS etc.), specifying the Swagger JSON endpoint.
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ApplicationGateway API V1");
                    c.ShowRequestHeaders();
                });
            }
            app.UseMvc();

            app.UseStaticFiles();

            app.UseAuthentication();

            app.UseMvcWithDefaultRoute();

            InitializeAsync(app.ApplicationServices, CancellationToken.None).GetAwaiter().GetResult();
        }

        private async Task InitializeAsync(IServiceProvider services, CancellationToken cancellationToken)
        {
            // Create a new service scope to ensure the database context is correctly disposed when this methods returns.
            using (var scope = services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                applicationContext.Provider = scope.ServiceProvider;

                //var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                //var pluginFactory = scope.ServiceProvider.GetRequiredService<IPluginFactory>();

            }
            bool startIsOk = applicationContext.Start().Result;
        }

    }
}
