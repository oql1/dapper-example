using Orders.Api.Models;
using Orders.Api.Models.Dtos;

namespace Orders.Api.Repositories;

public interface IOrderRepository
{
    Task<int> CreateAsync(CreateOrderDto order, CancellationToken cancellationToken);
    Task UpdateAsync(int id, UpdateOrderDto order, CancellationToken cancellationToken);
    Task<IReadOnlyList<Order>> GetAllAsync(
        string? status,
        int? customerId,
        string? orderNumber,
        CancellationToken cancellationToken);
    Task<Order?> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<int> GetCountAsync(CancellationToken cancellationToken);
    Task<IReadOnlyList<Order>> GetFinishedAsync(CancellationToken cancellationToken);
    Task DeleteAsync(int id, CancellationToken cancellationToken);
}
