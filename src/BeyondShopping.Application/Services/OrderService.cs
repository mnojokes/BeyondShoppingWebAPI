using BeyondShopping.Application.Validators;
using BeyondShopping.Contracts;
using BeyondShopping.Contracts.Objects;
using BeyondShopping.Contracts.Requests;
using BeyondShopping.Contracts.Responses;
using BeyondShopping.Core.Exceptions;
using BeyondShopping.Core.Interfaces;
using BeyondShopping.Core.Models;
using BeyondShopping.Core.Utilities;
using FluentValidation.Results;
using Moq;
using System.Data;

namespace BeyondShopping.Application.Services;

public class OrderService
{
    private readonly IUserRepository _userRepository;
    private readonly IOrderRepository _orderRepository;
    private readonly IOrderItemRepository _orderItemRepository;
    private readonly IItemRepository _itemRepository;
    private readonly CreateOrderRequestValidator _createOrderRequestValidator;
    private readonly IdValidator _idValidator;

    public OrderService(
        IUserRepository userRepository,
        IOrderRepository orderRepository,
        IOrderItemRepository orderItemRepository,
        CreateOrderRequestValidator createOrderRequestValidator,
        IdValidator idValidator)
    {
        _userRepository = userRepository;
        _orderRepository = orderRepository;
        _orderItemRepository = orderItemRepository;
        _createOrderRequestValidator = createOrderRequestValidator;
        _idValidator = idValidator;

        ///// Create a mock item repository. Replace with proper dependency injection if actual item inventory keeping is implemented.
        Mock<IItemRepository> itemRepoMock = new Mock<IItemRepository>();
        itemRepoMock.Setup(r => r.Get(It.Is<int>(id => id >= 1 && id <= 100))).Returns((int id) => Task.FromResult<OrderItem?>(new OrderItem(id, id + 10)));
        itemRepoMock.Setup(r => r.Get(It.Is<int>(id => id < 1 || id > 100))).Returns(Task.FromResult<OrderItem?>(null));
        _itemRepository = itemRepoMock.Object;
        //////////
    }

    public async Task<OrderResponse> CreateOrder(CreateOrderRequest request)
    {
        ValidateOrderRequest(request);
        await ValidateUser(request.UserId);
        await ValidateItems(request.Items);

        OrderDataModel? response = null;
        using (IDbTransaction? transaction = _orderRepository.OpenConnectionAndStartTransaction())
        {
            if (transaction == null)
            {
                throw new InvalidOperationException("Failed to initialize database transaction.");
            }

            response = await _orderRepository.Create(new OrderDataModel(0, request.UserId, OrderStatus.Pending, DateTime.UtcNow));
            foreach (var item in request.Items!)
            {
                await _orderItemRepository.Create(new OrderItemModel(response.Id, item.Id, item.Quantity));
            }

            _orderRepository.CloseConnectionAndCommit(transaction);
        }

        return new OrderResponse(response.Id, response.Status, response.CreatedAt);
    }

    public async Task<OrderResponse> CompleteOrder(int id)
    {
        ValidateId(id);

        OrderDataModel response = await _orderRepository.UpdateStatus(new OrderStatusModel(id, OrderStatus.Completed));
        return new OrderResponse()
        {
            Id = response.Id,
            Status = response.Status,
            CreatedAt = response.CreatedAt
        };
    }

    public async Task<OrderResponseList> GetUserOrders(int userId)
    {
        ValidateId(userId);
        await ValidateUser(userId);

        List<OrderDataModel> orders = (await _orderRepository.Get(userId)).ToList();

        return new OrderResponseList()
        {
            Orders = orders.Select(o => new OrderResponse()
            {
                Id = o.Id,
                Status = o.Status,
                CreatedAt = o.CreatedAt
            }).ToList()
        };
    }

    public async Task CleanupExpiredOrders(int minutesOldToCleanUp)
    {
        List<OrderDataModel> expiredOrders = (await _orderRepository.Get(DateTime.UtcNow.AddMinutes(-minutesOldToCleanUp), OrderStatus.Pending)).ToList();
        if (expiredOrders.Count == 0)
        {
            return;
        }

        using (IDbTransaction? transaction = _orderRepository.OpenConnectionAndStartTransaction())
        {
            if (transaction == null)
            {
                throw new InvalidOperationException("Failed to initialize database transaction.");
            }

            foreach (OrderDataModel order in expiredOrders)
            {
                await _orderItemRepository.Delete(order.Id, transaction);
                await _orderRepository.Delete(order.Id, transaction);
            }

            _orderRepository.CloseConnectionAndCommit(transaction);
        }
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
        UserData? user = null;
        try
        {
            user = await _userRepository.Get(id);
        }
        catch (Exception ex)
        {
            throw new Exception($"Error validating user data: {ex.Message}");
        }

        if (user is null)
        {
            throw new UserNotFoundException($"User with id {id} does not exist.");
        }
    }

    private async Task ValidateItems(List<OrderItem> items)
    {
        List<int> invalidIds = new List<int>();

        try
        {
            foreach (OrderItem item in items)
            {
                OrderItem? dbItem = await _itemRepository.Get(item.Id);
                if (dbItem is null)
                {
                    invalidIds.Add(item.Id);
                }
            }
        }
        catch (Exception ex)
        {
            throw new Exception($"Error validating item(s): {ex.Message}");
        }

        if (invalidIds.Count != 0)
        {
            throw new ItemNotFoundException($"Item id(s) not found: {string.Join(", ", invalidIds)}");
        }
    }
}
