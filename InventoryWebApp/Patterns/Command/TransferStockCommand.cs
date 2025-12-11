using InventoryWebApp.Data;
using InventoryWebApp.Models;

namespace InventoryWebApp.Patterns.Command
{
    public class TransferStockCommand : ICommand
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly int _fromWarehouseId;
        private readonly int _toWarehouseId;
        private readonly int _productId;
        private readonly int _quantity;

        public TransferStockCommand(UnitOfWork unitOfWork, int fromWh, int toWh, int productId, int quantity)
        {
            _unitOfWork = unitOfWork;
            _fromWarehouseId = fromWh;
            _toWarehouseId = toWh;
            _productId = productId;
            _quantity = quantity;
        }

       public void Execute()
{
    _unitOfWork.WarehouseStockRepository.DecreaseStock(_fromWarehouseId, _productId, _quantity);
    _unitOfWork.WarehouseStockRepository.IncreaseStock(_toWarehouseId, _productId, _quantity);

    _unitOfWork.MovementRepository.Insert(new StockMovement
    {
        ProductID = _productId,
        WarehouseID = _fromWarehouseId,
        Quantity = _quantity,
        MovementType = "OUT",
        Date = DateTime.Now
    });

    _unitOfWork.MovementRepository.Insert(new StockMovement
    {
        ProductID = _productId,
        WarehouseID = _toWarehouseId,
        Quantity = _quantity,
        MovementType = "IN",
        Date = DateTime.Now
    });

    _unitOfWork.SaveChanges();
}

        public void Undo()
        {
            _unitOfWork.WarehouseStockRepository.IncreaseStock(_fromWarehouseId, _productId, _quantity);
            _unitOfWork.WarehouseStockRepository.DecreaseStock(_toWarehouseId, _productId, _quantity);
            _unitOfWork.SaveChanges();
        }
    }
}
