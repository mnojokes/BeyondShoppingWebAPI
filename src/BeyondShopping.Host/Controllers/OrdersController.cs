using BeyondShopping.Contracts.Requests;
using BeyondShopping.Contracts.Responses;
using Microsoft.AspNetCore.Mvc;
using System.Runtime.Versioning;

#pragma warning disable 1591

namespace BeyondShopping.Host.Controllers;

[ApiController]
public class OrdersController : Controller
{
#pragma warning restore 1591
    /// <summary>
    /// Create a new order.
    /// </summary>
    /// <remarks>
    /// Important:<br />
    /// * If the order is not completed (paid) withing two hours, it is automagically canceled;<br />
    /// * Billions of users with weird names and stock photos from the internet trust us, so we're definitely legit!
    /// </remarks>
    /// <response code="200">Order created</response>
    /// <response code="400">Bad request, faulty order data</response>
    /// <response code="500">Server error, beyond the comprehension of any living soul</response>
    [HttpPost("orders")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(OrderResponse), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<IActionResult> Create([FromBody] CreateOrderRequest order)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Complete the order.
    /// </summary>
    /// <remarks>
    /// Marks the order as complete unless it has been less than 2 hours since its creation.<br />
    /// </remarks>
    /// <response code="200">Order completed</response>
    /// <response code="400">Bad request, order expired</response>
    /// <response code="404">Not found, order by that id does not exist</response>
    /// <response code="500">Server error, beyond the comprehension of any living soul</response>
    [HttpPut("orders/{id}/complete")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(ErrorResponse), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 400)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<IActionResult> CompleteOrder([FromRoute] int id)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Get all orders for the user.
    /// </summary>
    /// <remarks>
    /// Shows just how much money we scammed off of a particular user.<br />
    /// </remarks>
    /// <response code="200">Orders successfully retrieved</response>
    /// <response code="404">Not found, user by that id does not exist</response>
    /// <response code="500">Server error, beyond the comprehension of any living soul</response>
    [HttpGet("orders")]
    [Produces("application/json")]
    [ProducesResponseType(typeof(OrderResponseList), 200)]
    [ProducesResponseType(typeof(ErrorResponse), 404)]
    [ProducesResponseType(typeof(ErrorResponse), 500)]
    public async Task<IActionResult> Get([FromQuery] int userId)
    {
        throw new NotImplementedException();
    }
}
