using BeyondShopping.Application.Validators;
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
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IOrderRepository _orderRepository;
    private readonly IdValidator _idValidator;
    private readonly CreateOrderRequestValidator _createOrderRequestValidator;
    private readonly int _cleanupMinutes;
    private readonly string _userDataRepositoryAddress;

    public OrderService(
        IConfiguration configuration,
        IHttpClientFactory clientFactory,
        IOrderRepository orderRepository,
        IdValidator idValidator,
        CreateOrderRequestValidator createOrderRequestValidator)
    {
        _httpClientFactory = clientFactory;
        _orderRepository = orderRepository;
        _idValidator = idValidator;
        _createOrderRequestValidator = createOrderRequestValidator;

        string expiryPeriodSection = "PendingOrderExpiryTimeMinutes";
        _cleanupMinutes = int.Parse(configuration[expiryPeriodSection] ??
            throw new ArgumentNullException(expiryPeriodSection));

        string userDataRepoSection = "UserDataRepositoryAddress";
        _userDataRepositoryAddress = configuration[userDataRepoSection] ??
            throw new ArgumentNullException(userDataRepoSection);
    }

    public async Task<OrderResponse> CreateOrder(CreateOrderRequest request)
    {
        ValidateOrderRequest(request);
        await ValidateUser(request.UserId);

        OrderDataModel response = await _orderRepository.Create(
            new OrderDataModel(0, request.UserId, "Pending", DateTime.UtcNow));

        return new OrderResponse(response.Id, response.Status, response.CreatedAt);
    }

    public async Task<OrderResponse> CompleteOrder(int id)
    {
        ValidateId(id);

        OrderDataModel response = await _orderRepository.UpdateStatus(new OrderStatusModel(id, "Completed"));
        return new OrderResponse(response.Id, response.Status, response.CreatedAt);
    }

    public async Task<OrderResponseList> GetUserOrders(int userId)
    {
        ValidateId(userId);
        await ValidateUser(userId);

        List<OrderDataModel> orders = (await _orderRepository.Get(userId)).ToList();

        return new OrderResponseList(orders.Select(o => new OrderResponse(o.UserId, o.Status, o.CreatedAt)).ToList());
    }

    public async Task CleanupExpiredOrders()
    {
        await _orderRepository.CleanupOlderThan(DateTime.UtcNow.AddMinutes(-_cleanupMinutes));
    }

    private void ValidateId(int id)
    {
        ValidationResult result = _idValidator.Validate(id);
        if (!result.IsValid)
        {
            throw new DataValidationException(ValidationErrorUtility.GetAllValidationErrorMessages(result));
        }
    }

    private void ValidateOrderRequest(CreateOrderRequest order)
    {
        ValidationResult result = _createOrderRequestValidator.Validate(order);
        if (!result.IsValid)
        {
            throw new DataValidationException(ValidationErrorUtility.GetAllValidationErrorMessages(result));
        }
    }

    private async Task ValidateUser(int id)
    {
        HttpClient client = _httpClientFactory.CreateClient("ClientWithExponentialBackoff");

        try
        {
            var response = await client.GetAsync($"{_userDataRepositoryAddress}/{id}");
            response.EnsureSuccessStatusCode();
        }
        catch (HttpRequestException)
        {
            throw new UserNotFoundException($"User with id {id} does not exist.");
        }
        catch
        {
            throw new Exception("Error validating user data.");
        }
    }
}
