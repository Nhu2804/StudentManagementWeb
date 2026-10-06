using Sieve.Models;
using Sieve.Services;
using StudentManagementWeb.Core.Sieve;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StudentManagementWeb.Core;
using StudentManagementWeb.Core.DbContexts;
using StudentManagementWeb.Core.SharedServices;

namespace StudentManagementWeb.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("Default")
            ?? throw new InvalidOperationException("Thiếu ConnectionStrings:Default trong cấu hình.");

        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(connectionString, npgsql =>
                // Migrations nằm trong tầng Core (thư mục Migrations)
                npgsql.MigrationsAssembly(typeof(CoreAssemblyMarker).Assembly.FullName)));

        services.AddScoped<IStudentCodeGenerator, StudentCodeGenerator>();

        services.Configure<SieveOptions>(options =>
        {
            options.ThrowExceptions = false;       // lọc sai cú pháp thì bỏ qua, không văng lỗi
            options.IgnoreNullsOnNotEqual = true;
        });
        services.AddScoped<ISieveProcessor, AppSieveProcessor>();

        return services;
    }
}