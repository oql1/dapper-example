using Microsoft.Data.SqlClient;

namespace Orders.Api.Repositories;

public sealed class SqlDapperContext
{
    private readonly string _connectionString;

    public SqlDapperContext(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("TransportOrders")
            ?? throw new InvalidOperationException(
                "Falta configurar ConnectionStrings:TransportOrders.");
    }

    public SqlConnection CreateConnection() => new(_connectionString);
}
