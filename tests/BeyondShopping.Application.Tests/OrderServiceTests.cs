using BeyondShopping.Application.Services;
using BeyondShopping.Application.Validators;
using BeyondShopping.Contracts;
using BeyondShopping.Contracts.Objects;
using BeyondShopping.Contracts.Requests;
using BeyondShopping.Contracts.Responses;
using BeyondShopping.Core.Exceptions;
using BeyondShopping.Core.Interfaces;
using BeyondShopping.Core.Models;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Moq;
using System.Data;

namespace BeyondShopping.Application.Tests;

public class OrderServiceTests
{
    private Mock<IConfiguration> _configurationMock;
    private Mock<IUserRepository> _userRepositoryMock;
    private Mock<IOrderRepository> _orderRepositoryMock;
    private Mock<IOrderItemRepository> _orderItemRepositoryMock;

    private const int _validIdFrom = 1;
    private const int _validIdTo = 10;
    private const string _expiryTimeMinutes = "120";

    private readonly OrderService _orderService;

    public OrderServiceTests()
    {
        _configurationMock = new Mock<IConfiguration>();
        _userRepositoryMock = new Mock<IUserRepository>();
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _orderItemRepositoryMock = new Mock<IOrderItemRepository>();

        _configurationMock.Setup(c => c["PendingOrderExpiryTimeMinutes"]).Returns(_expiryTimeMinutes);

        _userRepositoryMock.Setup(r => r.Get(It.Is<int>(id => id >= _validIdFrom && id <= _validIdTo))).Returns((int id) => Task.FromResult<UserData?>(new UserData(id, "TestName", "TestUsername", "TestEmail")));
        _userRepositoryMock.Setup(r => r.Get(It.Is<int>(id => id < _validIdFrom || id > _validIdTo))).Returns(Task.FromResult<UserData?>(null));

        _orderRepositoryMock.Setup(r => r.OpenConnectionAndStartTransaction()).Returns(new Mock<IDbTransaction>().Object);
        _orderRepositoryMock.Setup(r => r.Create(It.IsAny<OrderDataModel>(), It.IsAny<IDbTransaction>())).Returns(Task.FromResult(new OrderDataModel(1, 1, OrderStatus.Pending, DateTime.UtcNow)));
        _orderRepositoryMock.Setup(r => r.UpdateStatus(It.Is<OrderStatusModel>(osm => osm.Id >= _validIdFrom && osm.Id <= _validIdTo))).Returns((OrderStatusModel osm) => Task.FromResult(new OrderDataModel(osm.Id, 0, osm.Status, DateTime.UtcNow)));
        _orderRepositoryMock.Setup(r => r.UpdateStatus(It.Is<OrderStatusModel>(osm => osm.Id < _validIdFrom || osm.Id > _validIdTo))).Throws<InvalidOperationException>();
        _orderRepositoryMock.Setup(r => r.Get(It.Is<int>(id => id >= _validIdFrom && id <= _validIdTo))).Returns((int id) => Task.FromResult<OrderDataModel?>(new OrderDataModel(id, 0, OrderStatus.Pending, DateTime.UtcNow)));
        _orderRepositoryMock.Setup(r => r.Get(It.Is<int>(id => id < _validIdFrom || id > _validIdTo))).Returns(Task.FromResult<OrderDataModel?>(null));

        _orderService = new OrderService(
            _configurationMock.Object,
            _userRepositoryMock.Object,
            _orderRepositoryMock.Object,
            _orderItemRepositoryMock.Object,
            new CreateOrderRequestValidator(),
            new IdValidator()
            );
    }

    [Fact]
    public async Task CreateOrder_GivenValidRequest_ReturnsOrderResponse()
    {
        int testUserId = 1;
        List<OrderItem> testItems = new List<OrderItem>()
        {
            new OrderItem(1, 1),
            new OrderItem(2, 2),
            new OrderItem(3, 3)
        };

        CreateOrderRequest request = new CreateOrderRequest(testUserId, testItems);
        Func<Task> act = async () => await _orderService.CreateOrder(request);

        await act.Should().NotThrowAsync();
        _orderRepositoryMock.Verify(r => r.OpenConnectionAndStartTransaction(), Times.Once());
        _orderRepositoryMock.Verify(r => r.Create(It.IsAny<OrderDataModel>(), It.IsAny<IDbTransaction>()), Times.Once());
        _orderItemRepositoryMock.Verify(r => r.Create(It.IsAny<OrderItemModel>(), It.IsAny<IDbTransaction>()), Times.Exactly(testItems.Count));
        _orderRepositoryMock.Verify(r => r.CloseConnectionAndCommit(It.IsAny<IDbTransaction>()));
    }

    // CreateOrder
    [Fact]
    public async Task CreateOrder_GivenRequestWithInvalidUserId_ThrowsDataValidationException()
    {
        int invalidId = _validIdFrom - 1;
        CreateOrderRequest request = new CreateOrderRequest(invalidId, new List<OrderItem>() { new OrderItem(_validIdFrom, 1) });
        Func<Task> act = async () => await _orderService.CreateOrder(request);

        await act.Should().ThrowAsync<DataValidationException>();
        _orderRepositoryMock.Verify(r => r.OpenConnectionAndStartTransaction(), Times.Never());
    }

    [Fact]
    public async Task CreateOrder_GivenRequestWithEmptyItemsList_ThrowsDataValidationException()
    {
        CreateOrderRequest request = new CreateOrderRequest(_validIdFrom, new List<OrderItem>());
        Func<Task> act = async () => await _orderService.CreateOrder(request);

        await act.Should().ThrowAsync<DataValidationException>();
        _orderRepositoryMock.Verify(r => r.OpenConnectionAndStartTransaction(), Times.Never());
    }

    [Fact]
    public async Task CreateOrder_GivenRequestWithInvalidItemId_ThrowsDataValidationException()
    {
        int invalidId = _validIdFrom - 1;
        CreateOrderRequest request = new CreateOrderRequest(_validIdFrom, new List<OrderItem>() { new OrderItem(invalidId, 1) });
        Func<Task> act = async () => await _orderService.CreateOrder(request);

        await act.Should().ThrowAsync<DataValidationException>();
        _orderRepositoryMock.Verify(r => r.OpenConnectionAndStartTransaction(), Times.Never());
    }

    [Fact]
    public async Task CreateOrder_GivenRequestWithInvalidItemQuantity_ThrowsDataValidationException()
    {
        CreateOrderRequest request = new CreateOrderRequest(_validIdFrom, new List<OrderItem>() { new OrderItem(_validIdFrom, 0) });
        Func<Task> act = async () => await _orderService.CreateOrder(request);

        await act.Should().ThrowAsync<DataValidationException>();
        _orderRepositoryMock.Verify(r => r.OpenConnectionAndStartTransaction(), Times.Never());
    }

    [Fact]
    public async Task CreateOrder_GivenNonExistentUserId_ThrowsUserNotFoundException()
    {
        int nonexistentId = _validIdTo + 1;
        CreateOrderRequest request = new CreateOrderRequest(nonexistentId, new List<OrderItem>() { new OrderItem(_validIdFrom, 1) });
        Func<Task> act = async () => await _orderService.CreateOrder(request);

        await act.Should().ThrowAsync<UserNotFoundException>();
        _orderRepositoryMock.Verify(r => r.OpenConnectionAndStartTransaction(), Times.Never());
    }

    [Fact]
    public async Task CreateOrder_GivenNonExistentItemId_ThrowsItemNotFoundException()
    {
        int nonexistentId = _validIdTo + 1;
        CreateOrderRequest request = new CreateOrderRequest(_validIdFrom, new List<OrderItem>() { new OrderItem(nonexistentId, 1) });
        Func<Task> act = async () => await _orderService.CreateOrder(request);

        await act.Should().ThrowAsync<ItemNotFoundException>();
        _orderRepositoryMock.Verify(r => r.OpenConnectionAndStartTransaction(), Times.Never());
    }

    // CompleteOrder
    [Fact]
    public async Task CompleteOrder_GivenValidExistingOrderId_ReturnsOrderResponse()
    {
        Func<Task<OrderResponse>> act = async () => await _orderService.CompleteOrder(_validIdFrom);
        var callResult = await act.Should().NotThrowAsync();

        OrderResponse result = callResult.Which;
        result.Should().NotBeNull();
        result.Id.Should().Be(_validIdFrom);
        result.Status.Should().Be(OrderStatus.Completed);
    }

    [Fact]
    public async Task CompleteOrder_GivenInvalidId_ThrowsDataValidationException()
    {
        int invalidId = _validIdFrom - 1;
        Func<Task> act = async () => await _orderService.CompleteOrder(invalidId);

        await act.Should().ThrowAsync<DataValidationException>();
        _orderRepositoryMock.Verify(r => r.UpdateStatus(It.IsAny<OrderStatusModel>()), Times.Never());
    }

    [Fact]
    public async Task CompleteOrder_GivenNonExistentOrderId_ThrowsOrderNotFoundException()
    {
        int nonexistentId = _validIdTo + 1;
        Func<Task> act = async () => await _orderService.CompleteOrder(nonexistentId);

        await act.Should().ThrowAsync<OrderNotFoundException>();
        _orderRepositoryMock.Verify(r => r.Get(nonexistentId), Times.Once());
        _orderRepositoryMock.Verify(r => r.UpdateStatus(It.IsAny<OrderStatusModel>()), Times.Never());
    }

    [Fact]
    public async Task CompleteOrder_GivenExpiredOrderId_ThrowsOrderNotFoundException()
    {
        int minutesToExpiry = int.Parse(_expiryTimeMinutes);
        _orderRepositoryMock.Setup(r => r.Get(_validIdFrom)).Returns(Task.FromResult<OrderDataModel?>(new OrderDataModel(_validIdFrom, 0, OrderStatus.Pending, DateTime.UtcNow.AddMinutes(-minutesToExpiry))));
        Func<Task> act = async () => await _orderService.CompleteOrder(_validIdFrom);

        await act.Should().ThrowAsync<OrderNotFoundException>();
        _orderRepositoryMock.Verify(r => r.Get(_validIdFrom), Times.Once());
        _orderRepositoryMock.Verify(r => r.UpdateStatus(It.IsAny<OrderStatusModel>()), Times.Never());
    }
}
