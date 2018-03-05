//using AuthorizationCenter;
//using AuthorizationCenter.Dto;
//using AuthorizationCenter.Managers;
//using AuthorizationCenter.Models;
//using AuthorizationCenter.Stores;
//using Microsoft.AspNetCore.Builder.Internal;
//using Microsoft.AspNetCore.Identity;
//using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore;
//using Microsoft.Extensions.Configuration;
//using Microsoft.Extensions.DependencyInjection;
//using Microsoft.Extensions.Logging;
//using Microsoft.Extensions.Options;
//using OpenIddict.Core;
//using OpenIddict.Models;
using System;
using System.Threading;
using Xunit;

namespace AuthorizationTest
{
    public class UnitTest
    {
        //[Fact]
        //public async void ExpansionTest()
        //{
        //    var configuration = new ConfigurationBuilder()
        //        .AddJsonFile("config.json")
        //        .AddEnvironmentVariables()
        //        .Build();

        //    IServiceCollection services = new ServiceCollection();
        //    services.AddUserDefined();
        //    services.AddIdentity<Users, Roles>();
        //    services.AddOpenIddict();

        //    services.AddDbContextPool<ApplicationDbContext>(options =>
        //    {
        //        options.UseMySql(configuration["Data:DefaultConnection:ConnectionString"]);
        //        options.UseOpenIddict();
        //    });
        //    IServiceProvider serviceProvider = services.BuildServiceProvider();
        //    ExtendUserManager<Users> extendUserManager = new ExtendUserManager<Users>(
        //        serviceProvider.GetService<IUserStore<Users>>(),
        //        serviceProvider.GetService<IOptions<IdentityOptions>>(),
        //        serviceProvider.GetService<IPasswordHasher<Users>>(),
        //        serviceProvider.GetServices<IUserValidator<Users>>(),
        //        serviceProvider.GetServices<IPasswordValidator<Users>>(),
        //        serviceProvider.GetService<ILookupNormalizer>(),
        //        serviceProvider.GetService<IdentityErrorDescriber>(),
        //        serviceProvider,
        //        serviceProvider.GetService<ILogger<UserManager<Users>>>()
        //        );
        //    RoleManager<Roles> roleManager = new RoleManager<Roles>(
        //        serviceProvider.GetService<IRoleStore<Roles>>(),
        //        serviceProvider.GetServices<IRoleValidator<Roles>>(),
        //        serviceProvider.GetService<ILookupNormalizer>(),
        //        serviceProvider.GetService<IdentityErrorDescriber>(),
        //        serviceProvider.GetService<ILogger<RoleManager<Roles>>>()
        //        );
        //    OpenIddictApplicationManager<OpenIddict.Models.OpenIddictApplication> openIddictApplicationManager = new OpenIddictApplicationManager<OpenIddictApplication>(
        //        serviceProvider.GetService<IOpenIddictApplicationStore<OpenIddictApplication>>(),
        //        serviceProvider.GetService<ILogger<OpenIddictApplicationManager<OpenIddictApplication>>>()
        //        );
        //    PermissionItemManager permissionItemManager = new PermissionItemManager(
        //        serviceProvider.GetService<IPermissionItemStore>()
        //        );
        //    OrganizationsManager organizationsManager = new OrganizationsManager(
        //        serviceProvider.GetService<IOrganizationStore>()
        //        );
        //    OrganizationExpansionManager organizationExpansionManager = new OrganizationExpansionManager(
        //        serviceProvider.GetService<IOrganizationExpansionStore>(),
        //        serviceProvider.GetService<IOrganizationStore>()
        //        );
        //    RolePermissionManager rolePermissionManager = new RolePermissionManager(
        //        serviceProvider.GetService<IRolePermissionStore>()
        //        );
        //    PermissionOrganizationManager permissionOrganizationManager = new PermissionOrganizationManager(
        //        serviceProvider.GetService<IPermissionOrganizationStore>()
        //        );

        //    PermissionExpansionManager permissionExpansionManager = new PermissionExpansionManager(
        //        serviceProvider.GetService<IPermissionExpansionStore>(),
        //        serviceProvider.GetService<IPermissionItemStore>(),
        //        serviceProvider.GetService<IRolePermissionStore>(),
        //        serviceProvider.GetService<UserStore<Users>>(),
        //        serviceProvider.GetService<IPermissionOrganizationStore>(),
        //        serviceProvider.GetService<IOrganizationExpansionStore>(),
        //        serviceProvider.GetService<IRoleStore<Roles>>()
        //        );
        //    await organizationExpansionManager.Expansion();
        //    await permissionExpansionManager.Expansion();
        //}


        //[Fact]
        //public async void AddTestData()
        //{
        //    var configuration = new ConfigurationBuilder()
        //        .AddJsonFile("config.json")
        //        .AddEnvironmentVariables()
        //        .Build();

        //    IServiceCollection services = new ServiceCollection();
        //    services.AddUserDefined();
        //    services.AddIdentity<Users, Roles>();
        //    services.AddOpenIddict();

        //    services.AddDbContextPool<ApplicationDbContext>(options =>
        //    {
        //        options.UseMySql(configuration["Data:DefaultConnection:ConnectionString"]);
        //        options.UseOpenIddict();
        //    });
        //    IServiceProvider serviceProvider = services.BuildServiceProvider();
        //    ExtendUserManager<Users> extendUserManager = new ExtendUserManager<Users>(
        //        serviceProvider.GetService<IUserStore<Users>>(),
        //        serviceProvider.GetService<IOptions<IdentityOptions>>(),
        //        serviceProvider.GetService<IPasswordHasher<Users>>(),
        //        serviceProvider.GetServices<IUserValidator<Users>>(),
        //        serviceProvider.GetServices<IPasswordValidator<Users>>(),
        //        serviceProvider.GetService<ILookupNormalizer>(),
        //        serviceProvider.GetService<IdentityErrorDescriber>(),
        //        serviceProvider,
        //        serviceProvider.GetService<ILogger<UserManager<Users>>>()
        //        );
        //    RoleManager<Roles> roleManager = new RoleManager<Roles>(
        //        serviceProvider.GetService<IRoleStore<Roles>>(),
        //        serviceProvider.GetServices<IRoleValidator<Roles>>(),
        //        serviceProvider.GetService<ILookupNormalizer>(),
        //        serviceProvider.GetService<IdentityErrorDescriber>(),
        //        serviceProvider.GetService<ILogger<RoleManager<Roles>>>()
        //        );
        //    OpenIddictApplicationManager<OpenIddict.Models.OpenIddictApplication> openIddictApplicationManager = new OpenIddictApplicationManager<OpenIddictApplication>(
        //        serviceProvider.GetService<IOpenIddictApplicationStore<OpenIddictApplication>>(),
        //        serviceProvider.GetService<ILogger<OpenIddictApplicationManager<OpenIddictApplication>>>()
        //        );
        //    PermissionItemManager permissionItemManager = new PermissionItemManager(
        //        serviceProvider.GetService<IPermissionItemStore>()
        //        );
        //    OrganizationsManager organizationsManager = new OrganizationsManager(
        //        serviceProvider.GetService<IOrganizationStore>()
        //        );
        //    PermissionOrganizationManager permissionOrganizationManager = new PermissionOrganizationManager(
        //        serviceProvider.GetService<IPermissionOrganizationStore>()
        //        );

        //    OrganizationExpansionManager organizationExpansionManager = new OrganizationExpansionManager(
        //        serviceProvider.GetService<IOrganizationExpansionStore>(),
        //        serviceProvider.GetService<IOrganizationStore>()
        //        );
        //    RolePermissionManager rolePermissionManager = new RolePermissionManager(
        //        serviceProvider.GetService<IRolePermissionStore>()
        //        );
        //    PermissionExpansionManager permissionExpansionManager = new PermissionExpansionManager(
        //        serviceProvider.GetService<IPermissionExpansionStore>(),
        //        serviceProvider.GetService<IPermissionItemStore>(),
        //        serviceProvider.GetService<IRolePermissionStore>(),
        //        serviceProvider.GetService<UserStore<Users>>(),
        //        serviceProvider.GetService<IPermissionOrganizationStore>(),
        //        serviceProvider.GetService<IOrganizationExpansionStore>(),
        //        serviceProvider.GetService<IRoleStore<Roles>>()
        //        );
        //    #region ---------------------添加测试数据---------------------------
        //    //添加用户测试数据
        //    await extendUserManager.CreateAsync(new Users()
        //    {
        //        Id = "zhangsan",
        //        UserName = "zhangsan",
        //        PasswordHash = "123456",
        //        Email = "zhangsan@qq.com",
        //        TrueName = "张三",
        //        OrganizationId = "3",
        //        FilialeId = "2"
        //    });
        //    await extendUserManager.CreateAsync(new Users()
        //    {
        //        Id = "lisi",
        //        UserName = "lisi",
        //        PasswordHash = "123456",
        //        Email = "lisi@qq.com",
        //        TrueName = "李四",
        //        OrganizationId = "3",
        //        FilialeId = "2"
        //    });
        //    await extendUserManager.CreateAsync(new Users()
        //    {
        //        Id = "wangwu",
        //        UserName = "wangwu",
        //        PasswordHash = "123456",
        //        Email = "wangwu@qq.com",
        //        TrueName = "王五",
        //        OrganizationId = "8",
        //        FilialeId = "6"
        //    });
        //    await extendUserManager.CreateAsync(new Users()
        //    {
        //        Id = "zhaoliu",
        //        UserName = "zhaoliu",
        //        PasswordHash = "123456",
        //        Email = "zhaoliu@qq.com",
        //        TrueName = "赵六",
        //        OrganizationId = "9",
        //        FilialeId = "6"
        //    });
        //    //添加组织架构测试数据
        //    await organizationsManager.CreateAsync(new Organization()
        //    {
        //        Id = "1",
        //        ParentId = "0",
        //        OrganizationName = "总部"
        //    }, CancellationToken.None);
        //    await organizationsManager.CreateAsync(new Organization()
        //    {
        //        Id = "2",
        //        ParentId = "1",
        //        OrganizationName = "重庆分公司"
        //    }, CancellationToken.None);
        //    await organizationsManager.CreateAsync(new Organization()
        //    {
        //        Id = "3",
        //        ParentId = "2",
        //        OrganizationName = "重庆行政"
        //    }, CancellationToken.None);
        //    await organizationsManager.CreateAsync(new Organization()
        //    {
        //        Id = "4",
        //        ParentId = "2",
        //        OrganizationName = "重庆财务"
        //    }, CancellationToken.None);
        //    await organizationsManager.CreateAsync(new Organization()
        //    {
        //        Id = "5",
        //        ParentId = "2",
        //        OrganizationName = "重庆IT"
        //    }, CancellationToken.None);
        //    await organizationsManager.CreateAsync(new Organization()
        //    {
        //        Id = "6",
        //        ParentId = "1",
        //        OrganizationName = "成都分公司"
        //    }, CancellationToken.None);
        //    await organizationsManager.CreateAsync(new Organization()
        //    {
        //        Id = "7",
        //        ParentId = "6",
        //        OrganizationName = "成都行政"
        //    }, CancellationToken.None);
        //    await organizationsManager.CreateAsync(new Organization()
        //    {
        //        Id = "8",
        //        ParentId = "6",
        //        OrganizationName = "成都财务"
        //    }, CancellationToken.None);
        //    await organizationsManager.CreateAsync(new Organization()
        //    {
        //        Id = "9",
        //        ParentId = "6",
        //        OrganizationName = "成都IT"
        //    }, CancellationToken.None);
        //    await organizationsManager.CreateAsync(new Organization()
        //    {
        //        Id = "10",
        //        ParentId = "1",
        //        OrganizationName = "信息战略中心"
        //    }, CancellationToken.None);

        //    //添加角色测试数据
        //    await roleManager.CreateAsync(new Roles()
        //    {
        //        Id = "guanliyuan",
        //        Name = "管理员",
        //        OrganizationId = "1"
        //    });
        //    await roleManager.CreateAsync(new Roles()
        //    {
        //        Id = "zongjingli",
        //        Name = "总经理",
        //        OrganizationId = "1"
        //    });
        //    await roleManager.CreateAsync(new Roles()
        //    {
        //        Id = "fengongsijingli",
        //        Name = "分公司经理",
        //        OrganizationId = "1"
        //    });
        //    await roleManager.CreateAsync(new Roles()
        //    {
        //        Id = "bumenjingli",
        //        Name = "部门经理",
        //        OrganizationId = "1"
        //    });
        //    //添加应用测试数据
        //    await openIddictApplicationManager.CreateAsync(new OpenIddictApplication()
        //    {
        //        ClientId = "aaaaaa",
        //        DisplayName = "房源管理",
        //        Id = "1",
        //        Type = "public",
        //        LogoutRedirectUri = "http://localhost:5001/signout-callback-oidc",
        //        RedirectUri = "http://localhost:8080/callback"
        //    }, CancellationToken.None);
        //    await openIddictApplicationManager.CreateAsync(new OpenIddictApplication()
        //    {
        //        ClientId = "bbbbbb",
        //        DisplayName = "客源管理",
        //        Id = "2",
        //        Type = "public",
        //        LogoutRedirectUri = "http://localhost:5001/signout-callback-oidc",
        //        RedirectUri = "http://localhost:8080/callback"
        //    }, CancellationToken.None);
        //    //添加应用权限项测试数据
        //    await permissionItemManager.CreateAsync(new PermissionItem()
        //    {
        //        ApplicationId = "1",
        //        Id = "HouseIndex",
        //        Groups = "基础操作",
        //        Name = "房源查看"
        //    }, CancellationToken.None);
        //    await permissionItemManager.CreateAsync(new PermissionItem()
        //    {
        //        ApplicationId = "1",
        //        Id = "HouseAdd",
        //        Groups = "基础操作",
        //        Name = "房源录入"
        //    }, CancellationToken.None);
        //    await permissionItemManager.CreateAsync(new PermissionItem()
        //    {
        //        ApplicationId = "1",
        //        Id = "HouseDelete",
        //        Groups = "基础操作",
        //        Name = "房源删除"
        //    }, CancellationToken.None);
        //    await permissionItemManager.CreateAsync(new PermissionItem()
        //    {
        //        ApplicationId = "1",
        //        Id = "HouseUpdate",
        //        Groups = "基础操作",
        //        Name = "房源修改"
        //    }, CancellationToken.None);
        //    await permissionItemManager.CreateAsync(new PermissionItem()
        //    {
        //        ApplicationId = "1",
        //        Id = "HouseSearch",
        //        Groups = "基础操作",
        //        Name = "房源搜索"
        //    }, CancellationToken.None);
        //    await permissionItemManager.CreateAsync(new PermissionItem()
        //    {
        //        ApplicationId = "2",
        //        Id = "CustomerIndex",
        //        Groups = "基础操作",
        //        Name = "客源查看"
        //    }, CancellationToken.None);
        //    await permissionItemManager.CreateAsync(new PermissionItem()
        //    {
        //        ApplicationId = "2",
        //        Id = "CustomerAdd",
        //        Groups = "基础操作",
        //        Name = "客源录入"
        //    }, CancellationToken.None);
        //    await permissionItemManager.CreateAsync(new PermissionItem()
        //    {
        //        ApplicationId = "2",
        //        Id = "CustomerDelete",
        //        Groups = "基础操作",
        //        Name = "客源删除"
        //    }, CancellationToken.None);
        //    await permissionItemManager.CreateAsync(new PermissionItem()
        //    {
        //        ApplicationId = "2",
        //        Id = "CustomerUpdate",
        //        Groups = "基础操作",
        //        Name = "客源修改"
        //    }, CancellationToken.None);
        //    await permissionItemManager.CreateAsync(new PermissionItem()
        //    {
        //        ApplicationId = "2",
        //        Id = "CustomerSearch",
        //        Groups = "基础操作",
        //        Name = "客源搜索"
        //    }, CancellationToken.None);


        //    await extendUserManager.AddToRoleAsync(await extendUserManager.FindByIdAsync("zhangsan"), "管理员");
        //    await extendUserManager.AddToRoleAsync(await extendUserManager.FindByIdAsync("zhangsan"), "部门经理");
        //    await extendUserManager.AddToRoleAsync(await extendUserManager.FindByIdAsync("lisi"), "分公司经理");
        //    await extendUserManager.AddToRoleAsync(await extendUserManager.FindByIdAsync("wangwu"), "分公司经理");
        //    await extendUserManager.AddToRoleAsync(await extendUserManager.FindByIdAsync("wangwu"), "部门经理");

        //    await permissionOrganizationManager.CreateAsync(new PermissionOrganization()
        //    {
        //        OrganizationScope = "guanliyuanscope",
        //        OrganizationId = "1",
        //    }, CancellationToken.None);
        //    await permissionOrganizationManager.CreateAsync(new PermissionOrganization()
        //    {
        //        OrganizationScope = "zongjingli",
        //        OrganizationId = "1",
        //    }, CancellationToken.None);
        //    await permissionOrganizationManager.CreateAsync(new PermissionOrganization()
        //    {
        //        OrganizationScope = "fengongsijingli",
        //        OrganizationId = OrganizationScopeDefines.Filiale,
        //    }, CancellationToken.None);
        //    await permissionOrganizationManager.CreateAsync(new PermissionOrganization()
        //    {
        //        OrganizationScope = "bumenjingli",
        //        OrganizationId = OrganizationScopeDefines.Department,
        //    }, CancellationToken.None);

        //    //添加角色权限测试数据
        //    await rolePermissionManager.CreateAsync(new RolePermission()
        //    {
        //        OrganizationScope = "guanliyuanscope",
        //        RoleId = "guanliyuan",
        //        PermissionId = "HouseIndex"
        //    }, CancellationToken.None);
        //    await rolePermissionManager.CreateAsync(new RolePermission()
        //    {
        //        OrganizationScope = "guanliyuanscope",
        //        RoleId = "guanliyuan",
        //        PermissionId = "HouseAdd"
        //    }, CancellationToken.None);
        //    await rolePermissionManager.CreateAsync(new RolePermission()
        //    {
        //        OrganizationScope = "guanliyuanscope",
        //        RoleId = "guanliyuan",
        //        PermissionId = "HouseDelete"
        //    }, CancellationToken.None);
        //    await rolePermissionManager.CreateAsync(new RolePermission()
        //    {
        //        OrganizationScope = "guanliyuanscope",
        //        RoleId = "guanliyuan",
        //        PermissionId = "HouseUpdate"
        //    }, CancellationToken.None);

        //    await rolePermissionManager.CreateAsync(new RolePermission()
        //    {
        //        OrganizationScope = "zongjingli",
        //        RoleId = "zongjingli",
        //        PermissionId = "HouseIndex"
        //    }, CancellationToken.None);
        //    await rolePermissionManager.CreateAsync(new RolePermission()
        //    {
        //        OrganizationScope = "zongjingli",
        //        RoleId = "zongjingli",
        //        PermissionId = "CustomerIndex"
        //    }, CancellationToken.None);

        //    await rolePermissionManager.CreateAsync(new RolePermission()
        //    {
        //        OrganizationScope = "fengongsijingli",
        //        RoleId = "fengongsijingli",
        //        PermissionId = "HouseDelete"
        //    }, CancellationToken.None);
        //    await rolePermissionManager.CreateAsync(new RolePermission()
        //    {
        //        OrganizationScope = "fengongsijingli",
        //        RoleId = "fengongsijingli",
        //        PermissionId = "CustomerDelete"
        //    }, CancellationToken.None);

        //    await rolePermissionManager.CreateAsync(new RolePermission()
        //    {
        //        OrganizationScope = "bumenjingli",
        //        RoleId = "bumenjingli",
        //        PermissionId = "CustomerIndex"
        //    }, CancellationToken.None);
        //    await rolePermissionManager.CreateAsync(new RolePermission()
        //    {
        //        OrganizationScope = "bumenjingli",
        //        RoleId = "bumenjingli",
        //        PermissionId = "CustomerAdd"
        //    }, CancellationToken.None);

        //    #endregion
        //}

        //[Fact]
        //public async void DeleteTestData()
        //{
        //    var configuration = new ConfigurationBuilder()
        //        .AddJsonFile("config.json")
        //        .AddEnvironmentVariables()
        //        .Build();

        //    IServiceCollection services = new ServiceCollection();
        //    services.AddUserDefined();
        //    services.AddIdentity<Users, Roles>();
        //    services.AddOpenIddict();

        //    services.AddDbContextPool<ApplicationDbContext>(options =>
        //    {
        //        options.UseMySql(configuration["Data:DefaultConnection:ConnectionString"]);
        //        options.UseOpenIddict();
        //    });
        //    IServiceProvider serviceProvider = services.BuildServiceProvider();
        //    ExtendUserManager<Users> extendUserManager = new ExtendUserManager<Users>(
        //        serviceProvider.GetService<IUserStore<Users>>(),
        //        serviceProvider.GetService<IOptions<IdentityOptions>>(),
        //        serviceProvider.GetService<IPasswordHasher<Users>>(),
        //        serviceProvider.GetServices<IUserValidator<Users>>(),
        //        serviceProvider.GetServices<IPasswordValidator<Users>>(),
        //        serviceProvider.GetService<ILookupNormalizer>(),
        //        serviceProvider.GetService<IdentityErrorDescriber>(),
        //        serviceProvider,
        //        serviceProvider.GetService<ILogger<UserManager<Users>>>()
        //        );
        //    RoleManager<Roles> roleManager = new RoleManager<Roles>(
        //        serviceProvider.GetService<IRoleStore<Roles>>(),
        //        serviceProvider.GetServices<IRoleValidator<Roles>>(),
        //        serviceProvider.GetService<ILookupNormalizer>(),
        //        serviceProvider.GetService<IdentityErrorDescriber>(),
        //        serviceProvider.GetService<ILogger<RoleManager<Roles>>>()
        //        );
        //    OpenIddictApplicationManager<OpenIddict.Models.OpenIddictApplication> openIddictApplicationManager = new OpenIddictApplicationManager<OpenIddictApplication>(
        //        serviceProvider.GetService<IOpenIddictApplicationStore<OpenIddictApplication>>(),
        //        serviceProvider.GetService<ILogger<OpenIddictApplicationManager<OpenIddictApplication>>>()
        //        );
        //    PermissionItemManager permissionItemManager = new PermissionItemManager(
        //        serviceProvider.GetService<IPermissionItemStore>()
        //        );
        //    OrganizationsManager organizationsManager = new OrganizationsManager(
        //        serviceProvider.GetService<IOrganizationStore>()
        //        );
        //    OrganizationExpansionManager organizationExpansionManager = new OrganizationExpansionManager(
        //        serviceProvider.GetService<IOrganizationExpansionStore>(),
        //        serviceProvider.GetService<IOrganizationStore>()
        //        );
        //    RolePermissionManager rolePermissionManager = new RolePermissionManager(
        //        serviceProvider.GetService<IRolePermissionStore>()
        //        );
        //    PermissionExpansionManager permissionExpansionManager = new PermissionExpansionManager(
        //        serviceProvider.GetService<IPermissionExpansionStore>(),
        //        serviceProvider.GetService<IPermissionItemStore>(),
        //        serviceProvider.GetService<IRolePermissionStore>(),
        //        serviceProvider.GetService<UserStore<Users>>(),
        //        serviceProvider.GetService<IPermissionOrganizationStore>(),
        //        serviceProvider.GetService<IOrganizationExpansionStore>(),
        //        serviceProvider.GetService<IRoleStore<Roles>>()
        //        );


        //    foreach (var item in await extendUserManager.Users.ToListAsync())
        //    {
        //        await extendUserManager.DeleteAsync(item);
        //    }
        //    await organizationsManager.DeleteAsync(await organizationsManager.FindByIdAsync("1", CancellationToken.None), CancellationToken.None);
        //    await organizationsManager.DeleteAsync(await organizationsManager.FindByIdAsync("2", CancellationToken.None), CancellationToken.None);
        //    await organizationsManager.DeleteAsync(await organizationsManager.FindByIdAsync("3", CancellationToken.None), CancellationToken.None);
        //    await organizationsManager.DeleteAsync(await organizationsManager.FindByIdAsync("4", CancellationToken.None), CancellationToken.None);
        //    await organizationsManager.DeleteAsync(await organizationsManager.FindByIdAsync("5", CancellationToken.None), CancellationToken.None);
        //    await organizationsManager.DeleteAsync(await organizationsManager.FindByIdAsync("6", CancellationToken.None), CancellationToken.None);
        //    await organizationsManager.DeleteAsync(await organizationsManager.FindByIdAsync("7", CancellationToken.None), CancellationToken.None);
        //    await organizationsManager.DeleteAsync(await organizationsManager.FindByIdAsync("8", CancellationToken.None), CancellationToken.None);
        //    await organizationsManager.DeleteAsync(await organizationsManager.FindByIdAsync("9", CancellationToken.None), CancellationToken.None);
        //    await organizationsManager.DeleteAsync(await organizationsManager.FindByIdAsync("10", CancellationToken.None), CancellationToken.None);
        //    foreach (var item in await permissionItemManager.FindByApplicationAsync("1", CancellationToken.None))
        //    {
        //        await permissionItemManager.DeleteAsync(item, CancellationToken.None);
        //    }
        //    foreach (var item in await permissionItemManager.FindByApplicationAsync("2", CancellationToken.None))
        //    {
        //        await permissionItemManager.DeleteAsync(item, CancellationToken.None);
        //    }
        //    await openIddictApplicationManager.DeleteAsync(await openIddictApplicationManager.FindByIdAsync("1", CancellationToken.None), CancellationToken.None);
        //    await openIddictApplicationManager.DeleteAsync(await openIddictApplicationManager.FindByIdAsync("2", CancellationToken.None), CancellationToken.None);
        //    foreach (var item in await roleManager.Roles.ToListAsync())
        //    {
        //        await roleManager.DeleteAsync(item);
        //    }






        //}

    }
}
