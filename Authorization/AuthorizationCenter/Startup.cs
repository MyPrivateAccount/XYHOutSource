using AspNet.Security.OpenIdConnect.Primitives;
using AuthorizationCenter.Models;
using AuthorizationCenter.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.PlatformAbstractions;
using OpenIddict.Core;
using OpenIddict.Models;
using Swashbuckle.AspNetCore.Swagger;
using System;
using System.Threading;
using System.Threading.Tasks;
using XYH.Core.Log;
using System.Linq;
using Microsoft.AspNetCore.DataProtection;
using StackExchange.Redis;
using AuthorizationCenter.Managers;
using Microsoft.AspNetCore.Hosting;
using AuthorizationCenter.Stores;
using AutoMapper;
using System.IO;
using AuthorizationCenter.DataSync;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.Logging;
using AspNet.Security.OpenIdConnect.Server;

namespace AuthorizationCenter
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            var configuration = new ConfigurationBuilder()
                .AddJsonFile("config.json")
                .AddEnvironmentVariables()
                .Build();

            string timeStr = configuration["TokenExpiresIn"];
            long expiresIn = 0;
            long.TryParse(timeStr ?? "", out expiresIn);
            if (expiresIn <= 0)
            {
                expiresIn = 3600;
            }
            string certPassword = configuration["CertPassword"];

            services.AddSingleton<IConfigurationRoot>(configuration);
            services.AddMvc();
            var redisSection = configuration.GetSection("Redis");
            services.AddDistributedRedisCache(options =>
            {
                redisSection.Bind(options);
            });
            services.AddDbContextPool<ApplicationDbContext>(options =>
            {
                options.UseMySql(configuration["Data:DefaultConnection:ConnectionString"]);
                options.UseOpenIddict();
            });
            //services.AddDbContext<OaDbContext>(options =>
            //{
            //    options.UseMySql(configuration["Data:DefaultConnection:OAConnectionString"]);
            //});
            // Register the Identity services.
            services.AddIdentity<Users, IdentityRole>(options =>
            {
                options.Password.RequireDigit = false;
                options.Password.RequiredLength = 6;
                options.Password.RequireLowercase = false;
                options.Password.RequireNonAlphanumeric = false;
                options.Password.RequireUppercase = false;

            }).AddEntityFrameworkStores<ApplicationDbContext>().AddDefaultTokenProviders();

            string redisConfiguration = redisSection["Configuration"];
            var redis = ConnectionMultiplexer.Connect(redisConfiguration);
            services.AddDataProtection(options =>
            {
                options.ApplicationDiscriminator = "AuthorizationCenter";
            }).PersistKeysToRedis(redis);

            // Configure Identity to use the same JWT claims as OpenIddict instead
            // of the legacy WS-Federation claims it uses by default (ClaimTypes),
            // which saves you from doing the mapping in your authorization controller.
            services.Configure<IdentityOptions>(options =>
            {
                options.ClaimsIdentity.UserNameClaimType = OpenIdConnectConstants.Claims.Name;
                options.ClaimsIdentity.RoleClaimType = OpenIdConnectConstants.Claims.Role;


            });

            string cerFile = System.IO.Path.Combine(AppContext.BaseDirectory, "xinyaohang.pfx");
            var cer = new System.Security.Cryptography.X509Certificates.X509Certificate2(cerFile, certPassword);

            services.AddAuthentication()
                .AddJwtBearer(options =>
                {
                    //options.Configuration.SigningKeys 
                    //   SecurityKey sk = ;
                    Microsoft.IdentityModel.Tokens.X509SecurityKey key = new X509SecurityKey(cer);
                    options.Configuration = new Microsoft.IdentityModel.Protocols.OpenIdConnect.OpenIdConnectConfiguration()
                    {

                    };


                    options.TokenValidationParameters.ValidateAudience = false;
                    options.TokenValidationParameters.ValidateIssuer = false;

                    options.Configuration.SigningKeys.Add(key);
                });




            // Register the OpenIddict services.
            services.AddOpenIddict(options =>
            {
                options.ApplicationType = typeof(Applications);
                options.AuthorizationType = typeof(Authorization);
                options.TokenType = typeof(Token);

               
                // Register the Entity Framework stores.
                options.AddEntityFrameworkCoreStores<ApplicationDbContext>();
                options.AddApplicationStore<ApplicationStore>();
                options.AddApplicationManager<ApplicationManager>();
                options.AddMvcBinders();
                // Enable the authorization, logout, token and userinfo endpoints.
                options.EnableAuthorizationEndpoint("/connect/authorize")
                       .EnableLogoutEndpoint("/connect/logout")
                       .EnableTokenEndpoint("/connect/token")
                       .EnableUserinfoEndpoint("/api/userinfo")
                       .EnableIntrospectionEndpoint("/connect/introspect");
                // Note: the Mvc.Client sample only uses the code flow and the password flow, but you
                // can enable the other flows if you need to support implicit or client credentials.
                options.AllowAuthorizationCodeFlow()
                       .AllowPasswordFlow()
                       .AllowRefreshTokenFlow()
                       .AllowImplicitFlow()
                       .AllowClientCredentialsFlow();
                options.AllowCustomFlow("openid");
                options.AllowCustomFlow("face"); //人脸登录
                options.SetRefreshTokenLifetime(TimeSpan.FromHours(48*2));
                // Make the "client_id" parameter mandatory when sending a token request.
                options.RequireClientIdentification();
                // When request caching is enabled, authorization and logout requests
                // are stored in the distributed cache by OpenIddict and the user agent
                // is redirected to the same page with a single parameter (request_id).
                // This allows flowing large OpenID Connect requests even when using
                // an external authentication provider like Google, Facebook or Twitter.
                options.EnableRequestCaching();
                // During development, you can disable the HTTPS requirement.
                options.DisableHttpsRequirement();
                // Note: to use JWT access tokens instead of the default
                // encrypted format, the following lines are required:
                //
                options.SetAccessTokenLifetime(TimeSpan.FromSeconds(expiresIn));

                options.UseJsonWebTokens();

                options.AddSigningCertificate(cer);
                // options.AddSigningCertificate()
                //options.AddEphemeralSigningKey();
            });


            //  builder.Services.TryAddScoped(
            //typeof(OpenIdConnectServerProvider),
            //    typeof(OpenIddictProvider<,,,>).MakeGenericType(
            //        /* TApplication: */ builder.ApplicationType,
            //        /* TAuthorization: */ builder.AuthorizationType,
            //        /* TScope: */ builder.ScopeType,
            //        /* TToken: */ builder.TokenType));

            services.AddScoped<OpenIdConnectServerProvider, OpenIddictProvider>();


            services.AddTransient<IEmailSender, AuthMessageSender>();
            services.AddTransient<ISmsSender, AuthMessageSender>();

            services.AddUserDefined();
            services.AddAutoMapper();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Info
                {
                    Version = "v1",
                    Title = "权限中心接口文档",
                    TermsOfService = "None",
                });
                c.IgnoreObsoleteActions();
                //Set the comments path for the swagger json and ui.
                var basePath = PlatformServices.Default.Application.ApplicationBasePath;
                var xmlPath = Path.Combine(basePath, "api.xml");
                c.IncludeXmlComments(xmlPath);
                c.OperationFilter<HttpHeaderOperation>(); // 添加httpHeader参数
            });
        }

        public void Configure(IApplicationBuilder app, ILoggerFactory loggerFactory)
        {
            loggerFactory.AddProvider(new XYHLoggerProvider());
           

            app.UseDeveloperExceptionPage();

            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS etc.), specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ApplicationGateway API V1");
                c.ShowRequestHeaders();
            });

            app.UseCors(options =>
            {
                options.AllowAnyHeader();
                options.AllowAnyMethod();
                options.AllowAnyOrigin();
                options.AllowCredentials();
            });
            app.Use(async (context, next) =>
            {
                if (context.Request.Headers.ContainsKey("X-Forwarded-Proto"))
                {
                    //如果存在SLB，且包含原始请求协议，将请求协议重写为原始协议
                    string proto = context.Request.Headers["X-Forwarded-Proto"].FirstOrDefault();
                    if (!String.IsNullOrEmpty(proto))
                    {
                        context.Request.Scheme = proto;
                    }
                }
                await next();
            });
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseMvcWithDefaultRoute();



            // Seed the database with the sample applications.
            // Note: in a real world application, this step should be part of a setup script.
            InitializeAsync(app.ApplicationServices, CancellationToken.None).GetAwaiter().GetResult();
        }

        private async Task InitializeAsync(IServiceProvider services, CancellationToken cancellationToken)
        {
            // Create a new service scope to ensure the database context is correctly disposed when this methods returns.
            using (var scope = services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {

                //var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                //await context.Database.EnsureCreatedAsync();
                //var manager = scope.ServiceProvider.GetRequiredService<ApplicationManager>();
                //var omanager = scope.ServiceProvider.GetRequiredService<OrganizationExpansionManager>();
                //var pmanager = scope.ServiceProvider.GetRequiredService<PermissionExpansionManager>();
                //var imanager = scope.ServiceProvider.GetRequiredService<InitManager>();
                //var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                //await omanager.Expansion();
                //await pmanager.Expansion();

                //if (await manager.FindByClientIdAsync("apigetway", cancellationToken) == null)
                //{
                //    var application = new Applications
                //    {
                //        ClientId = "apigetway",
                //        Type = "confidential",
                //        //ClientSecret = "123456",
                //        DisplayName = "API网关",
                //        ApplicationType = "pc",
                //        PostLogoutRedirectUris = "http://localhost:5001",
                //        RedirectUris = "http://localhost:5001/callback"
                //    };
                //    await manager.CreateAsync(application, "123456",cancellationToken);
                //}
                //if (await manager.FindByClientIdAsync("authorization", cancellationToken) == null)
                //{
                //    var application = new Applications
                //    {
                //        ClientId = "authorization",
                //        Type = "public",
                //        //ClientSecret = "123456",
                //        DisplayName = "权限中心",
                //        ApplicationType = "pc",
                //        PostLogoutRedirectUris = "http://localhost:5001",
                //        RedirectUris = "http://localhost:5001/callback"
                //    };
                //    await manager.CreateAsync(application, cancellationToken);
                //}
                //if (await manager.FindByClientIdAsync("wx", cancellationToken) == null)
                //{
                //    var application = new Applications
                //    {
                //        ClientId = "wx",
                //        Type = "public",
                //        //ClientSecret = "123456",
                //        DisplayName = "微信",
                //        ApplicationType = "wx",
                //        PostLogoutRedirectUris = "http://localhost:5001",
                //        RedirectUris = "http://localhost:5001/callback"
                //    };
                //    await manager.CreateAsync(application, cancellationToken);
                //}
                //if (await manager.FindByClientIdAsync("nwf", cancellationToken) == null)
                //{
                //    var application = new Applications
                //    {
                //        ClientId = "nwf",
                //        Type = "public",
                //        //ClientSecret = "123456",
                //        DisplayName = "NWF",
                //        ApplicationType = "type",
                //        PostLogoutRedirectUris = "http://localhost:5001",
                //        RedirectUris = "http://localhost:5001/callback"
                //    };
                //    await manager.CreateAsync(application, cancellationToken);
                //}
                //if (await manager.FindByClientIdAsync("basedata", cancellationToken) == null)
                //{
                //    var application = new Applications
                //    {
                //        ClientId = "basedata",
                //        Type = "public",
                //        //ClientSecret = "123456",
                //        DisplayName = "基础数据",
                //        ApplicationType = "pc",
                //        PostLogoutRedirectUris = "http://localhost:5001",
                //        RedirectUris = "http://localhost:5001/callback"
                //    };
                //    await manager.CreateAsync(application, cancellationToken);
                //}
                //if (await manager.FindByClientIdAsync("residenttool", cancellationToken) == null)
                //{
                //    var application = new Applications
                //    {
                //        ClientId = "residenttool",
                //        Type = "public",
                //        //ClientSecret = "123456",
                //        DisplayName = "驻场工具",
                //        ApplicationType = "pc",
                //        PostLogoutRedirectUris = "http://localhost:5001",
                //        RedirectUris = "http://localhost:5001/callback"
                //    };
                //    await manager.CreateAsync(application, cancellationToken);
                //}
                //await imanager.InitDate();
                //confidential
                //    await manager.CreateAsync(application, cancellationToken);
                //}



            }

        }
    }
}
