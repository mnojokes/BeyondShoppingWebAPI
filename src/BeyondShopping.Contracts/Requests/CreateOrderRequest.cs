﻿using BeyondShopping.Contracts.Objects;

namespace BeyondShopping.Contracts.Requests;

public record CreateOrderRequest
{
    public int UserId { get; init; } = default;
    public List<ItemData> Items { get; init; } = new();

    public CreateOrderRequest() { }
    public CreateOrderRequest(int userId, List<ItemData> items)
        => (UserId, Items) = (userId, items);
}
