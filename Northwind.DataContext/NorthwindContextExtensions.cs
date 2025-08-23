using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Northwind.DataContext;

public static class NorthwindContextExtensions
{
    /// <summary>
    /// Adds NorthwindContext to the specified IServiceCollection. Uses the SqlServer database provider.
    /// </summary>
    /// <param name="services">The service collection.</param>
    /// <param name="connectionString">Set to override the default.</param>
    /// <returns>An IServiceCollection that can be used to add more services.</returns>
    public static IServiceCollection AddNorthwindContext(this IServiceCollection services, string? connectionString = null)
    {
        if (connectionString is null)
        {
            SqlConnectionStringBuilder builder = new();
            builder.DataSource = "CARLO-PC\\MSSQL_DEV_2022"; // SQL Edge in Docker.
            builder.InitialCatalog = "NorthwindMVC";
            builder.TrustServerCertificate = true;
            builder.MultipleActiveResultSets = true;
            // Because we want to fail faster. Default is 15 seconds.
            builder.ConnectTimeout = 3;
            // SQL Server authentication.
            builder.UserID = Environment.GetEnvironmentVariable("NORTHWINDMVC_USER");
            builder.Password = Environment.GetEnvironmentVariable("NORTHWINDMVC_PWD");
            connectionString = builder.ConnectionString;
        }

        services.AddDbContext<NorthwindMvcContext>(options =>
        {
            options.UseSqlServer(connectionString);
            options.LogTo(NorthwindContextLogger.WriteLine,
            new[] { Microsoft.EntityFrameworkCore.Diagnostics.RelationalEventId.CommandExecuting });
        },
        // Register with a transient lifetime to avoid concurrency
        // issues with Blazor Server projects.
            contextLifetime: ServiceLifetime.Transient,
            optionsLifetime: ServiceLifetime.Transient);

        return services;

    }
}
