using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using InventorySystem.DAL;
using InventorySystem.Models;

namespace InventorySystem.Services
{
    public class ProductService
    {
        private readonly DatabaseConnection _db;

        public ProductService()
        {
            _db = new DatabaseConnection();
        }

        // ---------------------------
        // 1) Get All Products
        // ---------------------------
        public List<Product> GetAllProducts()
        {
            var products = new List<Product>();
            string query = "SELECT ProductID, ProductName, Quantity, Price, Barcode FROM Products";

            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        products.Add(new Product
                        {
                            ProductID = reader.GetInt32(0),
                            ProductName = reader.GetString(1),
                            Quantity = reader.GetInt32(2),
                            Price = reader.GetDecimal(3),
                            Barcode = reader.GetString(4)
                        });
                    }
                }
            }

            return products;
        }

        // ---------------------------
        // 2) Add Product
        // ---------------------------
        public bool AddProduct(Product p)
        {
            string query = @"INSERT INTO Products (ProductName, Quantity, Price, Barcode)
                             VALUES (@ProductName, @Quantity, @Price, @Barcode)";

            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ProductName", p.ProductName);
                    cmd.Parameters.AddWithValue("@Quantity", p.Quantity);
                    cmd.Parameters.AddWithValue("@Price", p.Price);
                    cmd.Parameters.AddWithValue("@Barcode", p.Barcode);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // ---------------------------
        // 3) Update Product
        // ---------------------------
        public bool UpdateProduct(Product p)
        {
            string query = @"UPDATE Products
                             SET ProductName=@ProductName, Quantity=@Quantity,
                                 Price=@Price, Barcode=@Barcode
                             WHERE ProductID=@ProductID";

            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ProductID", p.ProductID);
                    cmd.Parameters.AddWithValue("@ProductName", p.ProductName);
                    cmd.Parameters.AddWithValue("@Quantity", p.Quantity);
                    cmd.Parameters.AddWithValue("@Price", p.Price);
                    cmd.Parameters.AddWithValue("@Barcode", p.Barcode);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }

        // ---------------------------
        // 4) Delete Product
        // ---------------------------
        public bool DeleteProduct(int productId)
        {
            string query = "DELETE FROM Products WHERE ProductID=@ProductID";

            using (SqlConnection conn = _db.GetConnection())
            {
                conn.Open();
                using (SqlCommand cmd = new SqlCommand(query, conn))
                {
                    cmd.Parameters.AddWithValue("@ProductID", productId);

                    return cmd.ExecuteNonQuery() > 0;
                }
            }
        }
    }
}
