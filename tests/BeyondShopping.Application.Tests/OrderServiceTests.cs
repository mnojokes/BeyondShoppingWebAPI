using BeyondShopping.Application.Services;
using BeyondShopping.Application.Validators;
using BeyondShopping.Contracts.Objects;
using BeyondShopping.Contracts.Requests;
using BeyondShopping.Core.Exceptions;
using BeyondShopping.Core.Interfaces;
using Moq;

namespace BeyondShopping.Application.Tests;

public class OrderServiceTests
{
    private Mock<IUserRepository> _userRepositoryMock;
    private Mock<IOrderRepository> _orderRepositoryMock;
    private Mock<IOrderItemRepository> _orderItemRepositoryMock;

    private readonly OrderService _orderService;

    public OrderServiceTests()
    {
        //Mock<IConfiguration> configurationMock = new Mock<IConfiguration>();
        //configurationMock.Setup(x => x["UserDataRepositoryAddress"]).Returns("ConnectionString");

        _userRepositoryMock = new Mock<IUserRepository>();
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _orderItemRepositoryMock = new Mock<IOrderItemRepository>();

        _orderService = new OrderService(
            _userRepositoryMock.Object,
            _orderRepositoryMock.Object,
            _orderItemRepositoryMock.Object,
            new CreateOrderRequestValidator(),
            new IdValidator()
            );
    }

    [Fact]
    public async Task CreateOrder_GivenInvalidRequest_ThrowsDataValidationException()
    {
        CreateOrderRequest request = new CreateOrderRequest(-1, new List<ItemData>());
        await Assert.ThrowsAsync<DataValidationException>(async () => await _orderService.CreateOrder(request));
        _orderRepositoryMock.Verify(r => r.OpenConnectionAndStartTransaction(), Times.Never());
    }
}
