using BeyondShopping.Core.Interfaces;
using BeyondShopping.Infrastructure.Repositories;
using Dapper;
using DbUp;
using Microsoft.Extensions.DependencyInjection;
using Npgsql;
using System.Data;
using System.Reflection;

namespace BeyondShopping.Infrastructure;

public static class DependencyInjection
{
    public static void AddInfrastructure(this IServiceCollection services, string dbConnection)
    {
        if (string.IsNullOrEmpty(dbConnection))
        {
            throw new ArgumentNullException(nameof(dbConnection));
        }

        EnsureDatabase.For.PostgresqlDatabase(dbConnection);
        var result = DeployChanges.To
            .PostgresqlDatabase(dbConnection)
            .WithScriptsEmbeddedInAssembly(Assembly.GetExecutingAssembly())
            .LogToConsole()
            .Build()
            .PerformUpgrade();

        DefaultTypeMap.MatchNamesWithUnderscores = true;

        if (!result.Successful)
        {
            throw new InvalidOperationException("Error initializing Postgre database.");
        }

        services.AddScoped<IDbConnection>(sp => new NpgsqlConnection(dbConnection));
        services.AddScoped<IOrderRepository, OrderRepository>();
        services.AddScoped<IOrderItemRepository, OrderItemRepository>();
    }
}
