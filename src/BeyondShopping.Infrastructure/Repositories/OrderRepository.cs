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

    public async Task<OrderDataModel> Create(OrderDataModel order, IDbTransaction? transaction = null)
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

        return await _dbConnection.QuerySingleAsync<OrderDataModel>(query, queryParameters, transaction);
    }

    public async Task<OrderDataModel?> UpdateStatus(OrderStatusModel status)
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

        return await _dbConnection.QuerySingleOrDefaultAsync<OrderDataModel>(query, queryParameters);
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

    public async Task<IEnumerable<OrderDataModel>> Get(DateTime before, string? withStatus = null)
    {
        string query = @"SELECT * FROM orders
                        WHERE created_at <= @date";

        if (!string.IsNullOrEmpty(withStatus))
        {
            query += " AND status = @status";
        }

        var queryParameters = new
        {
            date = before,
            status = withStatus
        };

        return await _dbConnection.QueryAsync<OrderDataModel>(query, queryParameters);
    }

    public async Task Delete(int id, IDbTransaction? transaction = null)
    {
        string query = @"DELETE FROM orders
                        WHERE id = @id";

        var queryParameters = new
        {
            id = id
        };

        await _dbConnection.ExecuteAsync(query, queryParameters, transaction);
    }

    public IDbTransaction? OpenConnectionAndStartTransaction()
    {
        _dbConnection.Open();
        return _dbConnection.BeginTransaction();
    }

    public void CloseConnectionAndCommit(IDbTransaction transaction)
    {
        string errMessage = string.Empty;
        try
        {
            transaction.Commit();
        }
        catch
        {
            errMessage = "Database failure: changes reverted.";
            try
            {
                transaction.Rollback();
            }
            catch
            {
                errMessage = "Database failure: transaction rollback failed.";
            }

            _dbConnection.Close();
        }

        if (!string.IsNullOrEmpty(errMessage))
        {
            throw new InvalidOperationException(errMessage);
        }
    }
}
