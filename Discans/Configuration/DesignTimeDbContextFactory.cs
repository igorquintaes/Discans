using Discans.Shared.Database;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;

namespace Discans.Configuration
{
    public class DesignTimeDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
    {
        public AppDbContext CreateDbContext(string[] args) => new AppDbContext(
            new DbContextOptionsBuilder<AppDbContext>().UseMySql(GetConnectionString()).Options);

        private string GetConnectionString() => 
            Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")?.ToLower() == "production"
                ? Environment.GetEnvironmentVariable("CONN")
                : new ConfigurationBuilder()
                        .SetBasePath(Directory.GetCurrentDirectory())
                        .AddJsonFile("config.json")
                        .Build()
                        .GetValue<string>("CONN");
    }
}
