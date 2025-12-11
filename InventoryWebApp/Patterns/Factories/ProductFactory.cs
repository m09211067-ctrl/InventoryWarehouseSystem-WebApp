using InventoryWebApp.Models;

namespace InventoryWebApp.Patterns
{
    public static class ProductFactory
    {
        public static Product Create(string name, int quantity, decimal price, string barcode, string description)
        {
            return new Product
            {
                ProductName = name,
                Quantity = quantity,
                Price = price,
                Barcode = barcode,
                Description = description
            };
        }
    }
}
