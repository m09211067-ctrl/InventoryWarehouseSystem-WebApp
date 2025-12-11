using System;
using InventorySystem.Models;
using InventorySystem.Services;

namespace InventorySystem
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ProductService service = new ProductService();

            Console.WriteLine("📌 TESTING PRODUCT CRUD");
            Console.WriteLine("--------------------------");

            // 1) Add Product Test
            var newProduct = new Product
            {
                ProductName = "Test Product",
                Quantity = 5,
                Price = 12.50m,
                Barcode = "ABC123"
            };

            bool added = service.AddProduct(newProduct);
            Console.WriteLine(added ? "✔ Added Successfully" : "❌ Add Failed");

            // 2) List Products
            var allProducts = service.GetAllProducts();
            Console.WriteLine("\n📦 All Products:");
            foreach (var p in allProducts)
            {
                Console.WriteLine($"{p.ProductID} - {p.ProductName} - {p.Quantity} - {p.Price} - {p.Barcode}");
            }

            Console.WriteLine("\nاضغطي أي زر للخروج...");
            Console.ReadKey();
        }
    }
}
