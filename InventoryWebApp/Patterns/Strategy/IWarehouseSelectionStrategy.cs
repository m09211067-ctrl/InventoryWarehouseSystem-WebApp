using InventoryWebApp.Models;

namespace InventoryWebApp.Patterns.Strategy
{
    public interface IWarehouseSelectionStrategy
    {
        Warehouse SelectWarehouse(List<Warehouse> warehouses);
    }
}
