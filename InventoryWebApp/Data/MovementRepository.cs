using System.Data.SqlClient;
using InventoryWebApp.Models;

namespace InventoryWebApp.Data
{
    public class MovementRepository
    {
        private readonly DatabaseConnection _db;

        public MovementRepository(DatabaseConnection db)
        {
            _db = db;
        }

        // إضافة حركة جديدة
        public void Insert(StockMovement movement)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            string sql = @"
                INSERT INTO StockMovement
                    (ProductID, WarehouseID, MovementType, Quantity, Date)
                VALUES
                    (@ProductID, @WarehouseID, @MovementType, @Quantity, @Date);";

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@ProductID", movement.ProductID);
            cmd.Parameters.AddWithValue("@WarehouseID", movement.WarehouseID);
            cmd.Parameters.AddWithValue("@MovementType", movement.MovementType);
            cmd.Parameters.AddWithValue("@Quantity", movement.Quantity);
            cmd.Parameters.AddWithValue("@Date", movement.Date);

            cmd.ExecuteNonQuery();
        }

        // جميع الحركات
        public List<StockMovement> GetAll()
        {
            var list = new List<StockMovement>();

            using var conn = _db.GetConnection();
            conn.Open();

            string sql = @"SELECT MovementID, ProductID, WarehouseID,
                                  MovementType, Quantity, Date
                           FROM StockMovement
                           ORDER BY Date DESC";

            using var cmd = new SqlCommand(sql, conn);
            using var reader = cmd.ExecuteReader();

            while (reader.Read())
            {
                list.Add(new StockMovement
                {
                    MovementID = (int)reader["MovementID"],
                    ProductID = (int)reader["ProductID"],
                    WarehouseID = (int)reader["WarehouseID"],
                    MovementType = reader["MovementType"].ToString() ?? "",
                    Quantity = (int)reader["Quantity"],
                    Date = (DateTime)reader["Date"]
                });
            }

            return list;
        }

        // حركات منتج معيّن
        public List<StockMovement> GetByProduct(int productId)
        {
            var list = new List<StockMovement>();

            using var conn = _db.GetConnection();
            conn.Open();

            string sql = @"SELECT MovementID, ProductID, WarehouseID,
                                  MovementType, Quantity, Date
                           FROM StockMovement
                           WHERE ProductID = @ProductID
                           ORDER BY Date DESC";

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@ProductID", productId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new StockMovement
                {
                    MovementID = (int)reader["MovementID"],
                    ProductID = (int)reader["ProductID"],
                    WarehouseID = (int)reader["WarehouseID"],
                    MovementType = reader["MovementType"].ToString() ?? "",
                    Quantity = (int)reader["Quantity"],
                    Date = (DateTime)reader["Date"]
                });
            }

            return list;
        }

        // حركات مخزن معيّن
        public List<StockMovement> GetByWarehouse(int warehouseId)
        {
            var list = new List<StockMovement>();

            using var conn = _db.GetConnection();
            conn.Open();

            string sql = @"SELECT MovementID, ProductID, WarehouseID,
                                  MovementType, Quantity, Date
                           FROM StockMovement
                           WHERE WarehouseID = @WarehouseID
                           ORDER BY Date DESC";

            using var cmd = new SqlCommand(sql, conn);
            cmd.Parameters.AddWithValue("@WarehouseID", warehouseId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new StockMovement
                {
                    MovementID = (int)reader["MovementID"],
                    ProductID = (int)reader["ProductID"],
                    WarehouseID = (int)reader["WarehouseID"],
                    MovementType = reader["MovementType"].ToString() ?? "",
                    Quantity = (int)reader["Quantity"],
                    Date = (DateTime)reader["Date"]
                });
            }

            return list;
        }

        // ----------- Added for compatibility --------------
        public void AddMovement(StockMovement movement)
        {
            Insert(movement);
        }

        public List<StockMovement> GetAllMovements()
        {
            return GetAll();
        }
    }
}
