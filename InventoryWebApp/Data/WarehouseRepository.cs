using System.Data.SqlClient;
using InventoryWebApp.Models;

namespace InventoryWebApp.Data
{
    public class WarehouseRepository
    {
        private readonly DatabaseConnection _db;

        public WarehouseRepository(DatabaseConnection db)
        {
            _db = db;
        }

        // جلب جميع المخازن
        public List<Warehouse> GetAll()
        {
            var list = new List<Warehouse>();

            using var conn = _db.GetConnection();
            conn.Open();

            string query = "SELECT WarehouseID, WarehouseName, BranchName FROM Warehouses";
            using var cmd = new SqlCommand(query, conn);

            var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new Warehouse
                {
                    WarehouseID   = (int)reader["WarehouseID"],
                    WarehouseName = reader["WarehouseName"].ToString() ?? "",
                    BranchName    = reader["BranchName"].ToString() ?? ""
                });
            }

            return list;
        }

        // جلب مخزن حسب الرقم
        public Warehouse? GetById(int id)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            string query = @"SELECT WarehouseID, WarehouseName, BranchName
                             FROM Warehouses WHERE WarehouseID = @id";

            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", id);

            var reader = cmd.ExecuteReader();

            if (reader.Read())
            {
                return new Warehouse
                {
                    WarehouseID   = (int)reader["WarehouseID"],
                    WarehouseName = reader["WarehouseName"].ToString() ?? "",
                    BranchName    = reader["BranchName"].ToString() ?? ""
                };
            }

            return null;
        }

        // إضافة مخزن جديد (ترجع ID)
        public int Insert(Warehouse warehouse)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            string query = @"
                INSERT INTO Warehouses (WarehouseName, BranchName)
                OUTPUT INSERTED.WarehouseID
                VALUES (@name, @branch);";

            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@name", warehouse.WarehouseName);
            cmd.Parameters.AddWithValue("@branch", warehouse.BranchName);

            int newId = (int)cmd.ExecuteScalar();
            warehouse.WarehouseID = newId;
            return newId;
        }

        // تعديل بيانات مخزن
        public void Update(Warehouse warehouse)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            string query = @"UPDATE Warehouses
                             SET WarehouseName = @name, BranchName = @branch
                             WHERE WarehouseID = @id";

            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", warehouse.WarehouseID);
            cmd.Parameters.AddWithValue("@name", warehouse.WarehouseName);
            cmd.Parameters.AddWithValue("@branch", warehouse.BranchName);

            cmd.ExecuteNonQuery();
        }

        // حذف مخزن
        public void Delete(int id)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            string query = @"DELETE FROM Warehouses WHERE WarehouseID = @id";

            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", id);

            cmd.ExecuteNonQuery();
        }

        // جلب جميع المنتجات المرتبطة بمخزن معيّن (سجلات WarehouseStock)
        public List<WarehouseStock> GetAllProducts(int warehouseId)
        {
            var list = new List<WarehouseStock>();

            using var conn = _db.GetConnection();
            conn.Open();

            string query = @"SELECT WarehouseID, ProductID, Quantity
                             FROM WarehouseStock
                             WHERE WarehouseID = @id";

            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@id", warehouseId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new WarehouseStock
                {
                    WarehouseID = (int)reader["WarehouseID"],
                    ProductID   = (int)reader["ProductID"],
                    Quantity    = Convert.ToInt32(reader["Quantity"])
                });
            }

            return list;
        }
    }
}
