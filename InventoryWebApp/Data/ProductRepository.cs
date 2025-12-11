using System.Collections.Generic;
using System.Data.SqlClient;
using InventoryWebApp.Models;

namespace InventoryWebApp.Data
{
    public class ProductRepository
    {
        private readonly DatabaseConnection _db;

        public ProductRepository(DatabaseConnection db)
        {
            _db = db;
        }

        // قراءة جميع المنتجات مع الكمية والسعر
        public List<Product> GetAll()
        {
            var products = new List<Product>();

            using (SqlConnection conn = _db.CreateConnection())
            {
                conn.Open();

                string query = @"
                    SELECT ProductID, ProductName, Quantity, Price, Barcode, Description
                    FROM Products";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                using (SqlDataReader reader = cmd.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        var p = new Product
                        {
                            ProductID   = reader.GetInt32(0),
                            ProductName = reader.GetString(1),
                            Quantity    = reader.GetInt32(2),
                            Price       = reader.GetDecimal(3),
                            Barcode     = reader.IsDBNull(4) ? "" : reader.GetString(4),
                            Description = reader.IsDBNull(5) ? "" : reader.GetString(5)
                        };

                        products.Add(p);
                    }
                }
            }

            return products;
        }

        // إدخال منتج جديد مع إرجاع/تحديث ProductID
        public void Insert(Product product)
        {
            using (SqlConnection conn = _db.CreateConnection())
            {
                conn.Open();

                string query = @"
                    INSERT INTO Products (ProductName, Quantity, Price, Barcode, Description)
                    OUTPUT INSERTED.ProductID
                    VALUES (@name, @qty, @price, @barcode, @desc);";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@name", product.ProductName);
                    cmd.Parameters.AddWithValue("@qty", product.Quantity);
                    cmd.Parameters.AddWithValue("@price", product.Price);
                    cmd.Parameters.AddWithValue("@barcode", product.Barcode ?? "");
                    cmd.Parameters.AddWithValue("@desc", product.Description ?? "");

                    int newId = (int)cmd.ExecuteScalar();
                    product.ProductID = newId;
                }
            }
        }

        public Product? GetById(int id)
        {
            using (SqlConnection conn = _db.CreateConnection())
            {
                conn.Open();

                string query = @"SELECT ProductID, ProductName, Quantity, Price, Barcode, Description 
                                 FROM Products WHERE ProductID = @id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);

                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            return new Product
                            {
                                ProductID   = reader.GetInt32(0),
                                ProductName = reader.GetString(1),
                                Quantity    = reader.GetInt32(2),
                                Price       = reader.GetDecimal(3),
                                Barcode     = reader.GetString(4),
                                Description = reader.IsDBNull(5) ? "" : reader.GetString(5)
                            };
                        }
                    }
                }
            }
            return null;
        }

        public void Update(Product product)
        {
            using (SqlConnection conn = _db.CreateConnection())
            {
                conn.Open();

                string query = @"
                    UPDATE Products 
                    SET ProductName = @name, 
                        Quantity    = @qty, 
                        Price       = @price, 
                        Barcode     = @barcode, 
                        Description = @desc
                    WHERE ProductID = @id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", product.ProductID);
                    cmd.Parameters.AddWithValue("@name", product.ProductName);
                    cmd.Parameters.AddWithValue("@qty", product.Quantity);
                    cmd.Parameters.AddWithValue("@price", product.Price);
                    cmd.Parameters.AddWithValue("@barcode", product.Barcode ?? "");
                    cmd.Parameters.AddWithValue("@desc", product.Description ?? "");

                    cmd.ExecuteNonQuery();
                }
            }
        }

        public void Delete(int id)
        {
            using (SqlConnection conn = _db.CreateConnection())
            {
                conn.Open();

                string query = "DELETE FROM Products WHERE ProductID = @id";

                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@id", id);
                    cmd.ExecuteNonQuery();
                }
            }
        }
    }
}
