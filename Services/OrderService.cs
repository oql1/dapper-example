using Microsoft.Data.SqlClient;
using Orders.Api.Models;
using Orders.Api.Models.Dtos;
using Orders.Api.Repositories;

namespace Orders.Api.Services;

public sealed class OrderService : IOrderService
{
    private readonly IOrderRepository _repository;

    public OrderService(IOrderRepository repository)
    {
        _repository = repository;
    }

    public Task<int> CreateAsync(CreateOrderDto order, CancellationToken cancellationToken) =>
        ExecuteAsync(() => _repository.CreateAsync(order, cancellationToken));

    public Task UpdateAsync(int id, UpdateOrderDto order, CancellationToken cancellationToken) =>
        ExecuteAsync(() => _repository.UpdateAsync(id, order, cancellationToken));

    public Task<IReadOnlyList<Order>> GetAllAsync(
        string? status,
        int? customerId,
        string? orderNumber,
        CancellationToken cancellationToken) =>
        ExecuteAsync(() => _repository.GetAllAsync(status, customerId, orderNumber, cancellationToken));

    public Task<Order?> GetByIdAsync(int id, CancellationToken cancellationToken) =>
        ExecuteAsync(() => _repository.GetByIdAsync(id, cancellationToken));

    public Task<int> GetCountAsync(CancellationToken cancellationToken) =>
        ExecuteAsync(() => _repository.GetCountAsync(cancellationToken));

    public Task<IReadOnlyList<Order>> GetFinishedAsync(CancellationToken cancellationToken) =>
        ExecuteAsync(() => _repository.GetFinishedAsync(cancellationToken));

    public Task DeleteAsync(int id, CancellationToken cancellationToken) =>
        ExecuteAsync(() => _repository.DeleteAsync(id, cancellationToken));

    private static async Task<T> ExecuteAsync<T>(Func<Task<T>> operation)
    {
        try
        {
            return await operation();
        }
        catch (SqlException exception)
        {
            throw Translate(exception);
        }
    }

    private static async Task ExecuteAsync(Func<Task> operation)
    {
        try
        {
            await operation();
        }
        catch (SqlException exception)
        {
            throw Translate(exception);
        }
    }

    private static OrderServiceException Translate(SqlException exception)
    {
        if (exception.Number == 50030)
        {
            return new OrderServiceException(StatusCodes.Status404NotFound, "La orden no existe.", exception);
        }

        if (exception.Number is 50037 or 2601 or 2627)
        {
            return new OrderServiceException(StatusCodes.Status409Conflict, "Ya existe una orden con esos datos únicos.", exception);
        }

        if ((exception.Number >= 50000 && exception.Number <= 50999) || exception.Number is 547 or 8115)
        {
            return new OrderServiceException(StatusCodes.Status400BadRequest, exception.Message, exception);
        }

        return new OrderServiceException(
            StatusCodes.Status500InternalServerError,
            "Ocurrió un error al acceder a las órdenes.",
            exception);
    }
}

public sealed class OrderServiceException : Exception
{
    public OrderServiceException(int statusCode, string message, Exception innerException)
        : base(message, innerException)
    {
        StatusCode = statusCode;
    }

    public int StatusCode { get; }
}
