using InventoryWebApp.Models;
using InventoryWebApp.Data;
using InventoryWebApp.Patterns.Factories;

namespace InventoryWebApp.Patterns
{
    public class WarehouseFacade
    {
        private readonly WarehouseRepository _warehouseRepo;
        private readonly WarehouseStockRepository _stockRepo;
        private readonly ProductRepository _productRepo;

        public WarehouseFacade(
            WarehouseRepository warehouseRepo, 
            WarehouseStockRepository stockRepo,
            ProductRepository productRepo)
        {
            _warehouseRepo = warehouseRepo;
            _stockRepo     = stockRepo;
            _productRepo   = productRepo;
        }

        public void AddWarehouseWithInitialStock(Warehouse warehouse)
        {
            // 1) إضافة المخزن
            int newId = _warehouseRepo.Insert(warehouse);

            // 2) جلب كل المنتجات الحالية
            var products = _productRepo.GetAll();

            // 3) إنشاء سجل مخزون ابتدائي 0 لكل منتج
            foreach (var p in products)
            {
                var stock = WarehouseStockFactory.Create(newId, p.ProductID, 0);
                _stockRepo.Insert(stock);
            }

            // مستقبلاً: يمكن إضافة Log أو حركة أو Notifications
        }
    }
}
