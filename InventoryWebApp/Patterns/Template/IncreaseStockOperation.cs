using InventoryWebApp.Data;
using InventoryWebApp.Models;
using InventoryWebApp.Patterns.Strategy;
using InventoryWebApp.Patterns.Observer;

namespace InventoryWebApp.Patterns.Template
{
    public class IncreaseStockOperation : StockOperationTemplate
    {
        public IncreaseStockOperation(
            WarehouseStockRepository stockRepo,
            MovementRepository movementRepo,
            IStockStrategy strategy,
            StockSubject subject)
            : base(stockRepo, movementRepo, strategy, subject) { }

        protected override void ApplyOperation(int productId, int warehouseId, int quantity)
        {
            // 1) جلب الكمية الحالية
            int currentQty = _stockRepo.GetStock(productId, warehouseId);

            // 2) Strategy حساب الكمية الجديدة
            int newQty = _strategy.Apply(currentQty, quantity);

            // 3) جلب السجل من القاعدة
            var stock = _stockRepo.GetByProductAndWarehouse(productId, warehouseId);

            // 4) إذا ما في سجل → إنشاء جديد
            if (stock == null)
            {
                stock = new WarehouseStock
                {
                    ProductID = productId,
                    WarehouseID = warehouseId,
                    Quantity = newQty
                };
                _stockRepo.Insert(stock);
            }
            else
            {
                // 5) إذا في سجل → تحديث
                stock.Quantity = newQty;
                _stockRepo.Update(stock);
            }
        }
    }
}
