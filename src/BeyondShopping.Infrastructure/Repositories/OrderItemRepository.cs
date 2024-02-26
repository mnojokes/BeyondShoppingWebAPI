using BeyondShopping.Core.Interfaces;
using BeyondShopping.Core.Models;
using Dapper;
using System.Data;

namespace BeyondShopping.Infrastructure.Repositories;

public class OrderItemRepository : IOrderItemRepository
{
    private readonly IDbConnection _dbConnection;

    public OrderItemRepository(IDbConnection dbConnection)
    {
        _dbConnection = dbConnection;
    }

    public async Task Create(OrderItemModel orderItem, IDbTransaction? transaction = null)
    {
        string query = @"INSERT INTO orders_items (order_id, item_id, quantity)
                        VALUES (@orderId, @itemId, @quantity)";

        var queryParameters = new
        {
            orderId = orderItem.OrderId,
            itemId = orderItem.ItemId,
            quantity = orderItem.Quantity,
        };

        await _dbConnection.ExecuteAsync(query, queryParameters, transaction);
    }
    public async Task Delete(int orderId, IDbTransaction? transaction = null)
    {
        string query = @"DELETE FROM orders_items
                        WHERE order_id = @id";

        var queryParameters = new
        {
            id = orderId
        };

        await _dbConnection.ExecuteAsync(query, queryParameters, transaction);
    }
}
