using InventoryWebApp.Data;

namespace InventoryWebApp.Data
{
    public class UnitOfWork
    {
        public ProductRepository ProductRepository { get; }
        public WarehouseRepository WarehouseRepository { get; }
        public WarehouseStockRepository WarehouseStockRepository { get; }
        public MovementRepository MovementRepository { get; }

        public UnitOfWork(
            ProductRepository productRepository,
            WarehouseRepository warehouseRepository,
            WarehouseStockRepository warehouseStockRepository,
            MovementRepository movementRepository)
        {
            ProductRepository        = productRepository;
            WarehouseRepository      = warehouseRepository;
            WarehouseStockRepository = warehouseStockRepository;
            MovementRepository       = movementRepository;
        }

        // حاليًا نستخدم ADO.NET مباشرًا، فكل أمر يُنفذ فورًا.
        // يمكن لاحقًا إضافة Transactions هنا لو أحببتِ.
        public void SaveChanges()
        {
            // لا شيء حاليًا
        }
    }
}
