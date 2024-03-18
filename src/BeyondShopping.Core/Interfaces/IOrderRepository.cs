﻿using BeyondShopping.Core.Models;
using System.Data;

namespace BeyondShopping.Core.Interfaces;

public interface IOrderRepository
{
    Task<OrderDataModel> Create(OrderDataModel order, IDbTransaction? transaction = null);
    Task<OrderDataModel?> UpdateStatus(OrderStatusModel status);
    Task<IEnumerable<OrderDataModel>> Get(int userId);
    Task<IEnumerable<OrderDataModel>> Get(DateTime before, string? withStatus = null);
    Task Delete(int id, IDbTransaction? transaction = null);
    IDbTransaction? OpenConnectionAndStartTransaction();
    void CloseConnectionAndCommit(IDbTransaction transaction);
}
