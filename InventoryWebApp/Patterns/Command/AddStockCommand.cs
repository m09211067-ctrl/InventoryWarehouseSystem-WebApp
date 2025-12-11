using InventoryWebApp.Data;
using InventoryWebApp.Models;

namespace InventoryWebApp.Patterns.Command
{
    public class AddStockCommand : ICommand
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly int _warehouseId;
        private readonly int _productId;
        private readonly int _quantity;

        public AddStockCommand(UnitOfWork unitOfWork, int warehouseId, int productId, int quantity)
        {
            _unitOfWork = unitOfWork;
            _warehouseId = warehouseId;
            _productId = productId;
            _quantity = quantity;
        }

        public void Execute()
        {
            // 1) تحديث الكمية في WarehouseStock
            _unitOfWork.WarehouseStockRepository.IncreaseStock(_warehouseId, _productId, _quantity);

            // 2) إنشاء حركة مخزون
            var movement = new StockMovement
            {
                ProductID = _productId,
                WarehouseID = _warehouseId,
                Quantity = _quantity,
                MovementType = "IN",
                Date = DateTime.Now
            };

            _unitOfWork.MovementRepository.Insert(movement);

            // 3) تنفيذ التغييرات دفعة واحدة
            _unitOfWork.SaveChanges();
        }

        public void Undo()
        {
            _unitOfWork.WarehouseStockRepository.DecreaseStock(_warehouseId, _productId, _quantity);
            _unitOfWork.SaveChanges();
        }
    }
}
