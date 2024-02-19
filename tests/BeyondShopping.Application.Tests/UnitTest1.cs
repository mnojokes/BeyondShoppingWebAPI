using BeyondShopping.Application.Services;
using BeyondShopping.Application.Validators;
using BeyondShopping.Core.Interfaces;
using Microsoft.Extensions.Configuration;
using Moq;

namespace BeyondShopping.Application.Tests;

public class UnitTest1
{
    private IConfiguration _configurationMock;
    private IHttpClientFactory _httpClientFactoryMock;
    private IOrderRepository _orderRepositoryMock;
    private IItemRepository _itemRepositoryMock;
    private IdValidator _idValidator;
    private CreateOrderRequestValidator _createOrderRequestValidator;

    private readonly OrderService _orderService;

    public UnitTest1()
    {
        _configurationMock = new Mock<IConfiguration>().Object;
        _httpClientFactoryMock = new Mock<IHttpClientFactory>().Object;
        _orderRepositoryMock = new Mock<IOrderRepository>().Object;
        _itemRepositoryMock = new Mock<IItemRepository>().Object;
        _idValidator = new IdValidator();
        _createOrderRequestValidator = new CreateOrderRequestValidator();

        // TODO: set up mock object methods

        _orderService = new OrderService(
            _configurationMock,
            _httpClientFactoryMock,
            _orderRepositoryMock,
            _itemRepositoryMock,
            _idValidator,
            _createOrderRequestValidator
            );
    }


    [Fact]
    public void Test1()
    {

    }
}