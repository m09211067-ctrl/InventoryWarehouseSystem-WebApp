using InventoryWebApp.Models;

namespace InventoryWebApp.Patterns.Strategy
{
    public class LargestCapacityStrategy : IWarehouseSelectionStrategy
    {
        public Warehouse SelectWarehouse(List<Warehouse> warehouses)
        {
            return warehouses.OrderByDescending(w => w.Capacity).FirstOrDefault();
        }
    }
}
