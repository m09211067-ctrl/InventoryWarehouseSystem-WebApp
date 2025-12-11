using InventoryWebApp.Patterns.Prototype;

namespace InventoryWebApp.Models
{
    public class Product : ICloneableProduct
    {
        public int ProductID { get; set; }
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public string Barcode { get; set; }
        public string? Description { get; set; }
        public List<string> Components { get; set; } = new();

        // Prototype Pattern Implementation
        public Product Clone()
        {
            return new Product
            {
                ProductID = 0,
                ProductName = this.ProductName ,
                Barcode = this.Barcode ,
                Price = this.Price,
                Quantity = this.Quantity,
                Description = this.Description,
                Components = new List<string>(this.Components)
            };
        }
    }
}
