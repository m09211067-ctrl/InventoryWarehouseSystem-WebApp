using InventoryWebApp.Data;
using InventoryWebApp.Models;
using InventoryWebApp.Patterns.Strategy;
using InventoryWebApp.Patterns.Observer;

namespace InventoryWebApp.Patterns.Template
{
    public class DecreaseStockOperation : StockOperationTemplate
    {
        public DecreaseStockOperation(
            WarehouseStockRepository stockRepo,
            MovementRepository movementRepo,
            IStockStrategy strategy,
            StockSubject subject)
            : base(stockRepo, movementRepo, strategy, subject) { }

        protected override void ApplyOperation(int productId, int warehouseId, int quantity)
        {
            int currentQty = _stockRepo.GetStock(productId, warehouseId);

            int newQty = _strategy.Apply(currentQty, -quantity);

            var stock = _stockRepo.GetByProductAndWarehouse(productId, warehouseId);

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
                stock.Quantity = newQty;
                _stockRepo.Update(stock);
            }
        }

        protected override void SaveMovement(int productId, int warehouseId, int quantity)
        {
            var movement = new StockMovement
            {
                ProductID = productId,
                WarehouseID = warehouseId,
                Quantity = quantity,
                MovementType = "OUT",
                Date = DateTime.Now
            };

            _movementRepo.Insert(movement);
        }
    }
}
