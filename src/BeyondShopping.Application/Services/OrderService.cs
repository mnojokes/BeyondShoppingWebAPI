﻿using BeyondShopping.Application.Validators;
using BeyondShopping.Contracts.Requests;
using BeyondShopping.Contracts.Responses;
using BeyondShopping.Core.Exceptions;
using BeyondShopping.Core.Interfaces;
using BeyondShopping.Core.Models;
using BeyondShopping.Core.Utilities;
using FluentValidation.Results;
using Microsoft.Extensions.Configuration;

namespace BeyondShopping.Application.Services;

public class OrderService
{
    private readonly IOrderRepository _orderRepository;
    private readonly IdValidator _idValidator;
    private readonly CreateOrderRequestValidator _createOrderRequestValidator;
    private readonly int _cleanupMinutes;

    public OrderService(
        IConfiguration configuration,
        IOrderRepository orderRepository,
        IdValidator idValidator,
        CreateOrderRequestValidator createOrderRequestValidator)
    {
        _orderRepository = orderRepository;
        _idValidator = idValidator;
        _createOrderRequestValidator = createOrderRequestValidator;

        string expiryPeriodSection = "PendingOrderExpiryTimeMinutes";
        _cleanupMinutes = int.Parse(configuration[expiryPeriodSection] ??
            throw new ArgumentNullException(expiryPeriodSection));
    }

    public async Task<OrderResponse> CreateOrder(CreateOrderRequest request)
    {
        ValidationResult result = _createOrderRequestValidator.Validate(request);
        if (!result.IsValid)
        {
            throw new DataValidationException(ValidationErrorUtility.GetAllValidationErrorMessages(result));
        }

        OrderDataModel response = await _orderRepository.Create(
            new OrderDataModel(0, request.UserId, "Pending", DateTime.UtcNow));

        return new OrderResponse(response.Id, response.Status, response.CreatedAt);
    }

    public async Task<OrderResponse> CompleteOrder(int id)
    {
        ValidationResult result = _idValidator.Validate(id);
        if (!result.IsValid)
        {
            throw new DataValidationException(ValidationErrorUtility.GetAllValidationErrorMessages(result));
        }

        OrderDataModel response = await _orderRepository.UpdateStatus(new OrderStatusModel(id, "Completed"));
        return new OrderResponse(response.Id, response.Status, response.CreatedAt);
    }

    public async Task<OrderResponseList> GetUserOrders(int userId)
    {
        ValidationResult result = _idValidator.Validate(userId);
        if (!result.IsValid)
        {
            throw new DataValidationException(ValidationErrorUtility.GetAllValidationErrorMessages(result));
        }

        List<OrderDataModel> orders = (await _orderRepository.Get(userId)).ToList();

        return new OrderResponseList(orders.Select(o => new OrderResponse(o.UserId, o.Status, o.CreatedAt)).ToList());
    }

    public async Task CleanupExpiredOrders()
    {
        await _orderRepository.CleanupOlderThan(DateTime.Now.AddMinutes(-_cleanupMinutes));
    }
}
