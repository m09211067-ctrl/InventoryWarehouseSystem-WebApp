using InventoryWebApp.Models;

namespace InventoryWebApp.Patterns.Strategy
{
    public class LeastUsedStrategy : IWarehouseSelectionStrategy
    {
        public Warehouse SelectWarehouse(List<Warehouse> warehouses)
        {
            return warehouses.OrderBy(w => w.CurrentLoad).FirstOrDefault();
        }
    }
}
