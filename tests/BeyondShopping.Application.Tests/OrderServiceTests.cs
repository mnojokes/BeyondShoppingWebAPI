using BeyondShopping.Application.Services;
using BeyondShopping.Application.Validators;
using BeyondShopping.Contracts;
using BeyondShopping.Contracts.Objects;
using BeyondShopping.Contracts.Requests;
using BeyondShopping.Core.Exceptions;
using BeyondShopping.Core.Interfaces;
using BeyondShopping.Core.Models;
using FluentAssertions;
using Moq;
using System.Data;

namespace BeyondShopping.Application.Tests;

public class OrderServiceTests
{
    private Mock<IUserRepository> _userRepositoryMock;
    private Mock<IOrderRepository> _orderRepositoryMock;
    private Mock<IOrderItemRepository> _orderItemRepositoryMock;

    private readonly OrderService _orderService;

    public OrderServiceTests()
    {
        _userRepositoryMock = new Mock<IUserRepository>();
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _orderItemRepositoryMock = new Mock<IOrderItemRepository>();

        _userRepositoryMock.Setup(r => r.Get(It.Is<int>(id => id >= 1 && id <= 10))).Returns((int id) => Task.FromResult<UserData?>(new UserData(id, "TestName", "TestUsername", "TestEmail")));
        _userRepositoryMock.Setup(r => r.Get(It.Is<int>(id => id < 1 || id > 10))).Returns(Task.FromResult<UserData?>(null));

        _orderRepositoryMock.Setup(r => r.OpenConnectionAndStartTransaction()).Returns(new Mock<IDbTransaction>().Object);
        _orderRepositoryMock.Setup(r => r.Create(It.IsAny<OrderDataModel>(), It.IsAny<IDbTransaction>())).Returns(Task.FromResult(new OrderDataModel(1, 1, OrderStatus.Pending, DateTime.Now)));

        _orderService = new OrderService(
            _userRepositoryMock.Object,
            _orderRepositoryMock.Object,
            _orderItemRepositoryMock.Object,
            new CreateOrderRequestValidator(),
            new IdValidator()
            );
    }

    [Fact]
    public async Task CreateOrder_GivenRequestWithInvalidUserId_ThrowsDataValidationException()
    {
        CreateOrderRequest request = new CreateOrderRequest(-1, new List<OrderItem>() { new OrderItem(1, 1) });
        Func<Task> act = async () => await _orderService.CreateOrder(request);

        await act.Should().ThrowAsync<DataValidationException>();
        _orderRepositoryMock.Verify(r => r.OpenConnectionAndStartTransaction(), Times.Never());
    }

    [Fact]
    public async Task CreateOrder_GivenRequestWithEmptyItemsList_ThrowsDataValidationException()
    {
        CreateOrderRequest request = new CreateOrderRequest(1, new List<OrderItem>());
        Func<Task> act = async () => await _orderService.CreateOrder(request);

        await act.Should().ThrowAsync<DataValidationException>();
        _orderRepositoryMock.Verify(r => r.OpenConnectionAndStartTransaction(), Times.Never());
    }

    [Fact]
    public async Task CreateOrder_GivenRequestWithInvalidItemId_ThrowsDataValidationException()
    {
        CreateOrderRequest request = new CreateOrderRequest(1, new List<OrderItem>() { new OrderItem(-1, 1) });
        Func<Task> act = async () => await _orderService.CreateOrder(request);

        await act.Should().ThrowAsync<DataValidationException>();
        _orderRepositoryMock.Verify(r => r.OpenConnectionAndStartTransaction(), Times.Never());
    }

    [Fact]
    public async Task CreateOrder_GivenRequestWithInvalidItemQuantity_ThrowsDataValidationException()
    {
        CreateOrderRequest request = new CreateOrderRequest(1, new List<OrderItem>() { new OrderItem(1, 0) });
        Func<Task> act = async () => await _orderService.CreateOrder(request);

        await act.Should().ThrowAsync<DataValidationException>();
        _orderRepositoryMock.Verify(r => r.OpenConnectionAndStartTransaction(), Times.Never());
    }

    [Fact]
    public async Task CreateOrder_GivenNonExistentUserId_ThrowsUserNotFoundException()
    {
        CreateOrderRequest request = new CreateOrderRequest(11, new List<OrderItem>() { new OrderItem(1, 1) });
        Func<Task> act = async () => await _orderService.CreateOrder(request);

        await act.Should().ThrowAsync<UserNotFoundException>();
        _orderRepositoryMock.Verify(r => r.OpenConnectionAndStartTransaction(), Times.Never());
    }

    [Fact]
    public async Task CreateOrder_GivenNonExistentItemId_ThrowsItemNotFoundException()
    {
        CreateOrderRequest request = new CreateOrderRequest(1, new List<OrderItem>() { new OrderItem(101, 1) });
        Func<Task> act = async () => await _orderService.CreateOrder(request);

        await act.Should().ThrowAsync<ItemNotFoundException>();
        _orderRepositoryMock.Verify(r => r.OpenConnectionAndStartTransaction(), Times.Never());
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
}
