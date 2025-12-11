using InventoryWebApp.Models;

namespace InventoryWebApp.Patterns.AbstractFactory
{
    public class DefaultInventoryFactory : IInventoryEntityFactory
    {
        public Product CreateProduct(string name, int qty, decimal price, string barcode, string description)
        {
            return new Product
            {
                ProductName = name,
                Quantity = qty,
                Price = price,
                Barcode = barcode,
                Description = description
            };
        }

        public WarehouseStock CreateWarehouseStock(int productId, int warehouseId, int qty)
        {
            return new WarehouseStock
            {
                ProductID = productId,
                WarehouseID = warehouseId,
                Quantity = qty
            };
        }

        public StockMovement CreateMovement(int productId, string movementType, int quantity, int warehouseId)
        {
            return new StockMovement
            {
                ProductID = productId,
                WarehouseID = warehouseId,
                MovementType = movementType,
                Quantity = quantity,
                Date = DateTime.Now
            };
        }
    }
}
