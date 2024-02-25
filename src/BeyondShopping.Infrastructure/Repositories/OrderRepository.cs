using BeyondShopping.Core.Interfaces;
using BeyondShopping.Core.Models;
using Dapper;
using System.Data;

namespace BeyondShopping.Infrastructure.Repositories;

public class OrderRepository : IOrderRepository
{
    private readonly IDbConnection _dbConnection;
    public OrderRepository(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task CleanupOlderThan(DateTime time)
    {
        string query = @"DELETE FROM orders
                        WHERE status = @status AND created_at <= @time";

        var queryParameters = new
        {
            status = "Pending",
            time = time
        };

        await _dbConnection.ExecuteAsync(query, queryParameters);
    }

    public async Task<OrderDataModel> Create(OrderDataModel order)
    {
        string query = @"INSERT INTO orders (user_id, status, created_at)
                        VALUES (@user_id, @status, @created_at)
                        RETURNING *";

        var queryParameters = new
        {
            user_id = order.UserId,
            status = order.Status,
            created_at = order.CreatedAt
        };

        return await _dbConnection.QuerySingleAsync<OrderDataModel>(query, queryParameters);
    }

    public async Task<IEnumerable<OrderDataModel>> Get(int userId)
    {
        string query = @"SELECT * FROM orders
                        WHERE user_id = @user_id";

        var queryParameters = new
        {
            user_id = userId
        };

        return await _dbConnection.QueryAsync<OrderDataModel>(query, queryParameters);
    }

    public async Task<OrderDataModel> UpdateStatus(OrderStatusModel status)
    {
        string query = @"UPDATE orders
                        SET status = @status
                        WHERE id = @id
                        RETURNING *";

        var queryParameters = new
        {
            id = status.Id,
            status = status.Status
        };

        return await _dbConnection.QuerySingleAsync<OrderDataModel>(query, queryParameters);
    }
}
