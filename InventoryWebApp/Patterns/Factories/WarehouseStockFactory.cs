using InventoryWebApp.Models;

namespace InventoryWebApp.Patterns.Factories
{
    public static class WarehouseStockFactory
    {
        public static WarehouseStock Create(int warehouseId, int productId, int quantity)
        {
            return new WarehouseStock
            {
                WarehouseID = warehouseId,
                ProductID = productId,
                Quantity = quantity
            };
        }
    }
}
