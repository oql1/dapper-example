using System.Data;
using Dapper;
using Orders.Api.Models;
using Orders.Api.Models.Dtos;

namespace Orders.Api.Repositories;

public sealed class OrderRepository : IOrderRepository
{
    private readonly SqlDapperContext _context;

    public OrderRepository(SqlDapperContext context)
    {
        _context = context;
    }

    public async Task<int> CreateAsync(CreateOrderDto order, CancellationToken cancellationToken)
    {
        var parameters = new DynamicParameters(order);
        parameters.Add("@OrderId", dbType: DbType.Int32, direction: ParameterDirection.Output);

        await using var connection = _context.CreateConnection();
        var command = new CommandDefinition(
            "dbo.usp_TransportOrders_Create",
            parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        await connection.ExecuteAsync(command);
        return parameters.Get<int>("@OrderId");
    }

    public async Task UpdateAsync(int id, UpdateOrderDto order, CancellationToken cancellationToken)
    {
        var parameters = new DynamicParameters(order);
        parameters.Add("@OrderId", id, DbType.Int32);

        await using var connection = _context.CreateConnection();
        var command = new CommandDefinition(
            "dbo.usp_TransportOrders_Update",
            parameters,
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        await connection.ExecuteAsync(command);
    }

    public async Task<IReadOnlyList<Order>> GetAllAsync(
        string? status,
        int? customerId,
        string? orderNumber,
        CancellationToken cancellationToken)
    {
        await using var connection = _context.CreateConnection();
        var command = new CommandDefinition(
            "dbo.usp_TransportOrders_GetAll",
            new { Status = status, CustomerId = customerId, OrderNumber = orderNumber },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        var orders = await connection.QueryAsync<Order>(command);
        return orders.AsList();
    }

    public async Task<Order?> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
        await using var connection = _context.CreateConnection();
        var command = new CommandDefinition(
            "dbo.usp_TransportOrders_GetById",
            new { OrderId = id },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        return await connection.QuerySingleOrDefaultAsync<Order>(command);
    }

    public async Task<int> GetCountAsync(CancellationToken cancellationToken)
    {
        await using var connection = _context.CreateConnection();
        var command = new CommandDefinition(
            "SELECT COUNT(*) FROM dbo.TransportOrders;",
            commandType: CommandType.Text,
            cancellationToken: cancellationToken);

        return await connection.QuerySingleAsync<int>(command);
    }

    public async Task<IReadOnlyList<Order>> GetFinishedAsync(CancellationToken cancellationToken)
    {
        await using var connection = _context.CreateConnection();
        var command = new CommandDefinition(
            """
            SELECT
                o.OrderId,
                o.OrderNumber,
                o.CustomerId,
                c.CustomerCode,
                c.CompanyName,
                o.VehicleId,
                v.PlateNumber,
                v.VehicleType,
                o.OriginCity,
                o.DestinationCity,
                o.CargoDescription,
                o.WeightKg,
                o.FreightAmount,
                o.CurrencyCode,
                o.Status,
                o.PickupDate,
                o.DeliveryDate,
                o.Notes,
                o.CreatedAt,
                o.UpdatedAt
            FROM dbo.TransportOrders AS o
            INNER JOIN dbo.Customers AS c ON c.CustomerId = o.CustomerId
            LEFT JOIN dbo.Vehicles AS v ON v.VehicleId = o.VehicleId
            WHERE o.Status IN (N'Delivered', N'Cancelled')
            ORDER BY o.PickupDate DESC, o.OrderId DESC;
            """,
            commandType: CommandType.Text,
            cancellationToken: cancellationToken);

        var orders = await connection.QueryAsync<Order>(command);
        return orders.AsList();
    }

    public async Task DeleteAsync(int id, CancellationToken cancellationToken)
    {
        await using var connection = _context.CreateConnection();
        var command = new CommandDefinition(
            "dbo.usp_TransportOrders_Delete",
            new { OrderId = id },
            commandType: CommandType.StoredProcedure,
            cancellationToken: cancellationToken);

        await connection.ExecuteAsync(command);
    }
}
