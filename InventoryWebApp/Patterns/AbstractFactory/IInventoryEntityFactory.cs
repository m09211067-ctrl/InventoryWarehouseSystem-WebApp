using InventoryWebApp.Models;

namespace InventoryWebApp.Patterns.AbstractFactory
{
    public interface IInventoryEntityFactory
    {
        Product CreateProduct(string name, int qty, decimal price, string barcode, string description);
        WarehouseStock CreateWarehouseStock(int productId, int warehouseId, int qty);
      StockMovement CreateMovement(int productId, string movementType, int quantity, int warehouseId);

    }
}
