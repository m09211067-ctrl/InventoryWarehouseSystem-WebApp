using InventoryWebApp.Data;
using InventoryWebApp.Models;

namespace InventoryWebApp.Patterns.Command
{
    public class RemoveStockCommand : ICommand
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly int _warehouseId;
        private readonly int _productId;
        private readonly int _quantity;

        public RemoveStockCommand(UnitOfWork unitOfWork, int warehouseId, int productId, int quantity)
        {
            _unitOfWork = unitOfWork;
            _warehouseId = warehouseId;
            _productId = productId;
            _quantity = quantity;
        }

        public void Execute()
        {
            _unitOfWork.WarehouseStockRepository.DecreaseStock(_warehouseId, _productId, _quantity);

            var movement = new StockMovement
            {
                ProductID = _productId,
                WarehouseID = _warehouseId,
                Quantity = _quantity,
                MovementType = "OUT",
                Date = DateTime.Now
            };

            _unitOfWork.MovementRepository.AddMovement(movement);
            _unitOfWork.SaveChanges();
        }

        public void Undo()
        {
            _unitOfWork.WarehouseStockRepository.IncreaseStock(_warehouseId, _productId, _quantity);
            _unitOfWork.SaveChanges();
        }
    }
}
