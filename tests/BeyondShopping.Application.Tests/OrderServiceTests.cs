using BeyondShopping.Application.Services;
using BeyondShopping.Application.Validators;
using BeyondShopping.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Moq;

namespace BeyondShopping.Application.Tests;

public class OrderServiceTests
{
    private Mock<IHttpClientFactory> _httpClientFactoryMock;
    private Mock<IOrderRepository> _orderRepositoryMock;
    private Mock<IOrderItemRepository> _orderItemRepositoryMock;

    private readonly OrderService _orderService;

    public OrderServiceTests()
    {
        Mock<IConfiguration> configurationMock = new Mock<IConfiguration>();
        configurationMock.Setup(x => x["UserDataRepositoryAddress"]).Returns("ConnectionString");

        _httpClientFactoryMock = new Mock<IHttpClientFactory>();
        _orderRepositoryMock = new Mock<IOrderRepository>();
        _orderItemRepositoryMock = new Mock<IOrderItemRepository>();

        _orderService = new OrderService(
            configurationMock.Object,
            _httpClientFactoryMock.Object,
            _orderRepositoryMock.Object,
            _orderItemRepositoryMock.Object,
            new CreateOrderRequestValidator(),
            new IdValidator()
            );
    }

    [Fact]
    public void Test1()
    {

    }
}