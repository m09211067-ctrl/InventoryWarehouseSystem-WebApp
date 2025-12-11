using InventoryWebApp.Models;

namespace InventoryWebApp.Patterns.Factories
{
    public static class WarehouseFactory
    {
        public static Warehouse Create(string warehouseName, string branchName)
        {
            return new Warehouse
            {
                WarehouseName = warehouseName,
                BranchName = branchName
            };
        }
    }
}
