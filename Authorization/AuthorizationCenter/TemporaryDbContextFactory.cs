using AuthorizationCenter.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthorizationCenter
{
    public class TemporaryDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            var builder = new DbContextOptionsBuilder<ApplicationDbContext>();
            IConfigurationRoot configuration = new ConfigurationBuilder()
              .AddEnvironmentVariables()
              .AddJsonFile("config.json")
              .Build();

            builder.UseOpenIddict();
            builder.UseMySql(configuration["Data:DefaultConnection:ConnectionString"]);
            return new ApplicationDbContext(builder.Options);
        }
    }
}
