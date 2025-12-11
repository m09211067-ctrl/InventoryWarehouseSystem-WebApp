using InventoryWebApp.Data;
using InventoryWebApp.Models;
using InventoryWebApp.Patterns.Strategy;
using InventoryWebApp.Patterns.Observer;

namespace InventoryWebApp.Patterns.Template
{
    public abstract class StockOperationTemplate
    {
        protected readonly WarehouseStockRepository _stockRepo;
        protected readonly MovementRepository _movementRepo;
        protected readonly IStockStrategy _strategy;
        protected readonly StockSubject _subject;

        protected StockOperationTemplate(
            WarehouseStockRepository stockRepo,
            MovementRepository movementRepo,
            IStockStrategy strategy,
            StockSubject subject)
        {
            _stockRepo = stockRepo;
            _movementRepo = movementRepo;
            _strategy = strategy;
            _subject = subject;
        }

        public void Execute(Product product, int warehouseId, int quantity)
        {
            // 1) التحقق
            ValidateInput(product, quantity);

            // 2) تنفيذ العملية
            ApplyOperation(product.ProductID, warehouseId, quantity);

            // 3) تسجيل الحركة
            SaveMovement(product.ProductID, warehouseId, quantity);

            // 4) إشعار المراقبين
            _subject.Notify($"Stock changed for Product: {product.ProductName}");
        }

        protected abstract void ApplyOperation(int productId, int warehouseId, int quantity);

        protected virtual void ValidateInput(Product product, int quantity)
        {
            if (product == null)
                throw new Exception("Product cannot be null!");

            if (quantity <= 0)
                throw new Exception("Quantity must be greater than zero!");
        }

        protected virtual void SaveMovement(int productId, int warehouseId, int quantity)
        {
            var movement = new StockMovement
            {
                ProductID = productId,
                WarehouseID = warehouseId,
                Quantity = quantity,
                MovementType = "AUTO",
                Date = DateTime.Now
            };

            _movementRepo.Insert(movement);
        }
    }
}
