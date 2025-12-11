using System.Collections.Generic;

namespace InventoryWebApp.Models
{
    public class DashboardViewModel
    {
        public int ProductCount { get; set; }
        public int WarehouseCount { get; set; }
        public int MovementCount { get; set; }
        public int TotalStock { get; set; }

        public List<Product> LowStockProducts { get; set; } = new();
    }
}
