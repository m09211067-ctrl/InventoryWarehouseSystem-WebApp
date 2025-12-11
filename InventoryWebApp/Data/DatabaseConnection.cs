using System.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace InventoryWebApp.Data
{
    public class DatabaseConnection
    {
        private readonly string _connectionString;

        public DatabaseConnection(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("DefaultConnection");
        }

        // الدالة الأساسية
        public SqlConnection CreateConnection()
        {
            return new SqlConnection(_connectionString);
        }

        // الدالة المطلوبة من كل الـ Repositories
        public SqlConnection GetConnection()
        {
            return CreateConnection();
        }
    }
}
