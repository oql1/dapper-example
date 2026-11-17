using Microsoft.AspNetCore.Mvc;
using Orders.Api.Models;
using Orders.Api.Models.Dtos;
using Orders.Api.Services;

namespace Orders.Api.Controllers;

[ApiController]
[Route("api/orders")]
public class OrderController : ControllerBase
{
    private readonly IOrderService _orderService;

    public OrderController(IOrderService orderService)
    {
      _orderService = orderService;
    }

    [HttpPost]
    public async Task<IActionResult> CreateOrder(
        CreateOrderDto createOrderDto,
        CancellationToken cancellationToken)
    {
      try
      {
        var orderId = await _orderService.CreateAsync(createOrderDto, cancellationToken);
        return CreatedAtAction(nameof(GetOrderById), new { id = orderId }, new { OrderId = orderId });
      }
      catch (OrderServiceException exception)
      {
        return ToErrorResponse(exception);
      }
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> UpdateOrder(
        int id,
        UpdateOrderDto updateOrderDto,
        CancellationToken cancellationToken)
    {
      try
      {
        await _orderService.UpdateAsync(id, updateOrderDto, cancellationToken);
        return NoContent();
      }
      catch (OrderServiceException exception)
      {
        return ToErrorResponse(exception);
      }
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<Order>>> GetOrders(
        [FromQuery] string? status,
        [FromQuery] int? customerId,
        [FromQuery] string? orderNumber,
        CancellationToken cancellationToken)
    {
      try
      {
        return Ok(await _orderService.GetAllAsync(status, customerId, orderNumber, cancellationToken));
      }
      catch (OrderServiceException exception)
      {
        return ToErrorResponse(exception);
      }
    }

    [HttpGet("{id:int}")]
    public async Task<IActionResult> GetOrderById(int id, CancellationToken cancellationToken)
    {
      try
      {
        var order = await _orderService.GetByIdAsync(id, cancellationToken);
        return order is null ? NotFound(new { Message = "La orden no existe." }) : Ok(order);
      }
      catch (OrderServiceException exception)
      {
        return ToErrorResponse(exception);
      }
    }

    [HttpGet("total")]
    public async Task<IActionResult> GetTotalOrders(CancellationToken cancellationToken)
    {
      try
      {
        var totalOrders = await _orderService.GetCountAsync(cancellationToken);
        return Ok(new { totalOrders });
      }
      catch (OrderServiceException exception)
      {
        return ToErrorResponse(exception);
      }
    }

    [HttpGet("finished")]
    public async Task<IActionResult> GetFinishedOrders(CancellationToken cancellationToken)
    {
      try
      {
        return Ok(await _orderService.GetFinishedAsync(cancellationToken));
      }
      catch (OrderServiceException exception)
      {
        return ToErrorResponse(exception);
      }
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> DeleteOrder(int id, CancellationToken cancellationToken)
    {
      try
      {
        await _orderService.DeleteAsync(id, cancellationToken);
        return NoContent();
      }
      catch (OrderServiceException exception)
      {
        return ToErrorResponse(exception);
      }
    }

    private ObjectResult ToErrorResponse(OrderServiceException exception)
    {
      var errors = exception.StatusCode == StatusCodes.Status400BadRequest
        ? new[] { new FieldError(string.Empty, new[] { exception.Message }) }
        : Array.Empty<FieldError>();

      return StatusCode(exception.StatusCode,
        new ApiErrorResponse(exception.StatusCode, exception.Message, errors));
    }
}
