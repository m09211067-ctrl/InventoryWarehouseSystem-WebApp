using InventoryWebApp.Data;
using InventoryWebApp.Models;
using InventoryWebApp.Patterns.AbstractFactory;
using InventoryWebApp.Patterns.Adapter;
using InventoryWebApp.Patterns.Observer;
using InventoryWebApp.Patterns.Singleton;
using InventoryWebApp.Patterns.Strategy;
using InventoryWebApp.Patterns.Builder;
using InventoryWebApp.Patterns.Command;

namespace InventoryWebApp.Patterns
{
    public class InventoryFacade
    {
        private readonly UnitOfWork _uow;
        private readonly IInventoryEntityFactory _factory;
        private readonly ICurrencyService _currencyService;
        private readonly StockSubject _subject;

        private readonly CommandInvoker _invoker = new CommandInvoker();

        public InventoryFacade(
            UnitOfWork uow,
            IInventoryEntityFactory factory,
            ICurrencyService currencyService)
        {
            _uow = uow;
            _factory = factory;
            _currencyService = currencyService;

            _subject = StockSubject.Instance;
            _subject.Attach(new LowStockObserver());
        }

        // --------------------------------------------------------
        // 1) إضافة منتج
        // --------------------------------------------------------
        public void AddNewProduct(string name, int qty, decimal price, string barcode, string description)
        {
            var product = _factory.CreateProduct(name, qty, price, barcode, description);
            _uow.ProductRepository.Insert(product);

            var stock = _factory.CreateWarehouseStock(product.ProductID, 1, qty);
            _uow.WarehouseStockRepository.Insert(stock);

            var movement = _factory.CreateMovement(product.ProductID, "IN", qty, 1);
            _uow.MovementRepository.Insert(movement);

            LogManager.Instance.Write(
                $"[NEW PRODUCT] ID={product.ProductID}, Name={product.ProductName}, Qty={qty}");
        }

        // --------------------------------------------------------
        // 2) Prototype
        // --------------------------------------------------------
        public Product CloneProduct(Product original)
        {
            var clone = original.Clone();

            LogManager.Instance.Write(
                $"[CLONE] Original={original.ProductName}, New={clone.ProductName}");

            return clone;
        }

        // --------------------------------------------------------
        // 3) Adapter
        // --------------------------------------------------------
        public decimal ConvertPriceToLocal(decimal usdPrice)
        {
            var rate = _currencyService.GetExchangeRate();
            return usdPrice * rate;
        }

        // --------------------------------------------------------
        // 4) Add Stock (Command)
        // --------------------------------------------------------
        public void AddStock(int productId, int warehouseId, int qty)
        {
            var cmd = new AddStockCommand(_uow, warehouseId, productId, qty);
            _invoker.Execute(cmd);

            LogManager.Instance.Write(
                $"[STOCK INCREASE] Product={productId}, Qty={qty}, Warehouse={warehouseId}");

            var product = _uow.ProductRepository.GetById(productId);
            _subject.Notify(product);
        }

        // --------------------------------------------------------
        // 5) Remove Stock (Command)
        // --------------------------------------------------------
        public bool RemoveStock(int productId, int warehouseId, int qty)
        {
            int available = _uow.WarehouseStockRepository.GetStock(productId, warehouseId);
            if (available < qty)
                return false;

            var cmd = new RemoveStockCommand(_uow, warehouseId, productId, qty);
            _invoker.Execute(cmd);

            LogManager.Instance.Write(
                $"[STOCK DECREASE] Product={productId}, Qty={qty}, Warehouse={warehouseId}");

            var product = _uow.ProductRepository.GetById(productId);
            _subject.Notify(product);

            return true;
        }

        // --------------------------------------------------------
        // 6) Transfer Stock (Command)
        // --------------------------------------------------------
        public bool TransferStock(int productId, int fromWr, int toWr, int qty)
        {
            int available = _uow.WarehouseStockRepository.GetStock(productId, fromWr);
            if (available < qty)
                return false;

            var cmd = new TransferStockCommand(_uow, fromWr, toWr, productId, qty);
            _invoker.Execute(cmd);

            LogManager.Instance.Write(
                $"[TRANSFER] Product={productId}, Qty={qty}, From={fromWr} To={toWr}");

            var product = _uow.ProductRepository.GetById(productId);
            _subject.Notify(product);

            return true;
        }

        // --------------------------------------------------------
        // 7) Builder
        // --------------------------------------------------------
        public Product BuildComputerProduct()
        {
            var builder = new CompositeProductBuilder();
            var director = new ProductDirector();
            return director.BuildComputer(builder);
        }
    }
}
