using System.Data.SqlClient;
using InventoryWebApp.Models;

namespace InventoryWebApp.Data
{
    public class WarehouseStockRepository
    {
        private readonly DatabaseConnection _db;

        public WarehouseStockRepository(DatabaseConnection db)
        {
            _db = db;
        }

        // ========================================
        //   الحصول على كمية المنتج داخل المخزن
        // ========================================
        public int GetStock(int productId, int warehouseId)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            string query = @"SELECT Quantity FROM WarehouseStock 
                             WHERE ProductID = @p AND WarehouseID = @w";

            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@p", productId);
            cmd.Parameters.AddWithValue("@w", warehouseId);

            var result = cmd.ExecuteScalar();
            return result == null ? 0 : Convert.ToInt32(result);
        }

        // ========================================
        //   زيادة الكمية
        // ========================================
        public void IncreaseStock(int warehouseId, int productId, int quantity)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            // تحقق إن كان السجل موجودًا
            string checkQuery = @"SELECT Quantity FROM WarehouseStock
                                  WHERE WarehouseID = @w AND ProductID = @p";

            using var checkCmd = new SqlCommand(checkQuery, conn);
            checkCmd.Parameters.AddWithValue("@w", warehouseId);
            checkCmd.Parameters.AddWithValue("@p", productId);

            var result = checkCmd.ExecuteScalar();

            if (result == null)
            {
                // السجل غير موجود → إدراج جديد
                string insertQuery = @"INSERT INTO WarehouseStock 
                                      (WarehouseID, ProductID, Quantity)
                                       VALUES (@w, @p, @q)";

                using var insertCmd = new SqlCommand(insertQuery, conn);
                insertCmd.Parameters.AddWithValue("@w", warehouseId);
                insertCmd.Parameters.AddWithValue("@p", productId);
                insertCmd.Parameters.AddWithValue("@q", quantity);
                insertCmd.ExecuteNonQuery();
            }
            else
            {
                // السجل موجود → تحديث الكمية
                string updateQuery = @"UPDATE WarehouseStock
                                       SET Quantity = Quantity + @q
                                       WHERE WarehouseID = @w AND ProductID = @p";

                using var updateCmd = new SqlCommand(updateQuery, conn);
                updateCmd.Parameters.AddWithValue("@w", warehouseId);
                updateCmd.Parameters.AddWithValue("@p", productId);
                updateCmd.Parameters.AddWithValue("@q", quantity);
                updateCmd.ExecuteNonQuery();
            }
        }

        // ========================================
        //   إنقاص الكمية
        // ========================================
        public void DecreaseStock(int warehouseId, int productId, int quantity)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            string query = @"UPDATE WarehouseStock
                             SET Quantity = Quantity - @q
                             WHERE WarehouseID = @w AND ProductID = @p";

            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@w", warehouseId);
            cmd.Parameters.AddWithValue("@p", productId);
            cmd.Parameters.AddWithValue("@q", quantity);
            cmd.ExecuteNonQuery();
        }

        // ========================================
        //   دالة اختيارية لدمج المنطق
        // ========================================
        public void CreateOrUpdateStock(int warehouseId, int productId, int quantity)
        {
            int currentStock = GetStock(productId, warehouseId);

            if (currentStock == 0)
                IncreaseStock(warehouseId, productId, quantity);
            else
                IncreaseStock(warehouseId, productId, quantity);
        }

        public int GetAllStockQuantity()
        {
            using var conn = _db.GetConnection();
            conn.Open();

            string query = "SELECT SUM(Quantity) FROM WarehouseStock";

            using var cmd = new SqlCommand(query, conn);
            var result = cmd.ExecuteScalar();

            return result == DBNull.Value || result == null ? 0 : Convert.ToInt32(result);
        }
        // ========================================
        //     REQUIRED BY FACADE & COMMAND PATTERNS
        // ========================================

        // إنشاء سجل جديد
        public void Insert(WarehouseStock stock)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            string query = @"INSERT INTO WarehouseStock (WarehouseID, ProductID, Quantity)
                     VALUES (@w, @p, @q)";

            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@w", stock.WarehouseID);
            cmd.Parameters.AddWithValue("@p", stock.ProductID);
            cmd.Parameters.AddWithValue("@q", stock.Quantity);

            cmd.ExecuteNonQuery();
        }

        // تحديث سجل موجود
        public void Update(WarehouseStock stock)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            string query = @"UPDATE WarehouseStock
                     SET Quantity = @q
                     WHERE WarehouseID = @w AND ProductID = @p";

            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@w", stock.WarehouseID);
            cmd.Parameters.AddWithValue("@p", stock.ProductID);
            cmd.Parameters.AddWithValue("@q", stock.Quantity);

            cmd.ExecuteNonQuery();
        }

        // الحصول على سجل منتج داخل مخزن
        public WarehouseStock? GetByProductAndWarehouse(int productId, int warehouseId)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            string query = @"SELECT WarehouseID, ProductID, Quantity
                     FROM WarehouseStock
                     WHERE WarehouseID = @w AND ProductID = @p";

            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@w", warehouseId);
            cmd.Parameters.AddWithValue("@p", productId);

            using var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                return new WarehouseStock
                {
                    WarehouseID = warehouseId,
                    ProductID = productId,
                    Quantity = Convert.ToInt32(reader["Quantity"])
                };
            }

            return null;
        }

        // ========================================
        //   جلب كل المنتجات داخل مخزن معيّن
        // ========================================
        public List<WarehouseStock> GetProductsByWarehouse(int warehouseId)
        {
            var list = new List<WarehouseStock>();

            using var conn = _db.GetConnection();
            conn.Open();

            string query = @"
        SELECT 
            ws.WarehouseID,
            ws.ProductID,
            ws.Quantity,
            p.ProductName,
            p.Barcode,
            p.Price
        FROM WarehouseStock ws
        INNER JOIN Products p ON ws.ProductID = p.ProductID
        WHERE ws.WarehouseID = @wid
    ";

            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@wid", warehouseId);

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                list.Add(new WarehouseStock
                {
                    WarehouseID = (int)reader["WarehouseID"],
                    ProductID = (int)reader["ProductID"],
                    Quantity = Convert.ToInt32(reader["Quantity"]),
                    Product = new Product
                    {
                        ProductID = (int)reader["ProductID"],
                        ProductName = reader["ProductName"].ToString() ?? "",
                        Barcode = reader["Barcode"].ToString() ?? "",
                        Price = (decimal)reader["Price"]
                    }
                });
            }

            return list;
        }
        public void DeleteByProductId(int productId)
        {
            using var conn = _db.GetConnection();
            conn.Open();

            string query = "DELETE FROM WarehouseStock WHERE ProductID = @p";
            using var cmd = new SqlCommand(query, conn);
            cmd.Parameters.AddWithValue("@p", productId);
            cmd.ExecuteNonQuery();
        }



    }

}
