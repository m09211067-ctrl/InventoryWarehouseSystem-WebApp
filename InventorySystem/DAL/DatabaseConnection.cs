using System;
using System.Data.SqlClient;

namespace InventorySystem.DAL
{
    public class DatabaseConnection
    {
        // عدّلي اسم قاعدة البيانات إذا كان مختلف (InventoryDB)
        private readonly string connectionString =
            "Server=localhost,1433;Database=InventoryDB;User Id=SA;Password=YourPassword123!;TrustServerCertificate=True;";

        public SqlConnection GetConnection()
        {
            return new SqlConnection(connectionString);
        }

        // دالة بسيطة لاختبار الاتصال
        public bool TestConnection(out string message)
        {
            try
            {
                using (SqlConnection conn = GetConnection())
                {
                    conn.Open();
                    message = "✅ Connection Successful!";
                    return true;
                }
            }
            catch (Exception ex)
            {
                message = "❌ Connection Failed: " + ex.Message;
                return false;
            }
        }
    }
}
