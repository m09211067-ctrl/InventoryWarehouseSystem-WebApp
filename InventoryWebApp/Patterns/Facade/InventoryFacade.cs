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

            // 🔥 FIXED HERE
            _subject.Notify(product.ProductName);
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

            // 🔥 FIXED HERE
            _subject.Notify(product.ProductName);

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

            // 🔥 FIXED HERE
            _subject.Notify(product.ProductName);

            return true;
        }


        public void AddCompositeProduct(int warehouseId)
        {
            // Builder
            var builder = new CompositeProductBuilder();
            var director = new ProductDirector();
            var product = director.BuildComputer(builder);

            // إدراج المنتج
            _uow.ProductRepository.Insert(product);

            // إدخاله في المخزن
            var stock = new WarehouseStock
            {
                WarehouseID = warehouseId,
                ProductID = product.ProductID,
                Quantity = product.Quantity
            };

            _uow.WarehouseStockRepository.Insert(stock);

            // تسجيل حركة
            var movement = new StockMovement
            {
                ProductID = product.ProductID,
                WarehouseID = warehouseId,
                MovementType = "IN",
                Quantity = product.Quantity,
                Date = DateTime.Now
            };

            _uow.MovementRepository.Insert(movement);

            _uow.SaveChanges();
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

        public void CreateCompositeFromComponents(
    string productName,
    int finalQuantity,
    int warehouseId,
    int[] componentIds,
    Dictionary<int, int> componentQuantities)
        {
            // 1) جلب المنتجات (المكونات)
            var components = new List<Product>();
            foreach (var compId in componentIds)
            {
                var product = _uow.ProductRepository.GetById(compId);
                if (product == null)
                    throw new Exception("❌ أحد المكونات غير موجود");

                components.Add(product);
            }

            // 2) التحقق من توفر الكميات في المخزن
            foreach (var comp in components)
            {
                if (!componentQuantities.TryGetValue(comp.ProductID, out int qtyPerUnit))
                    throw new Exception($"❌ لم يتم تحديد كمية للمكوّن: {comp.ProductName}");

                int totalRequired = qtyPerUnit * finalQuantity;

                int available = _uow.WarehouseStockRepository
                    .GetStock(comp.ProductID, warehouseId);

                if (available < totalRequired)
                {
                    throw new Exception(
                        $"❌ الكمية غير كافية للمكوّن {comp.ProductName} " +
                        $"(المتوفر: {available}, المطلوب: {totalRequired})"
                    );
                }
            }



            // 3) حساب السعر النهائي (BOM)
            decimal totalPrice = 0;

            foreach (var comp in components)
            {
                if (!componentQuantities.TryGetValue(comp.ProductID, out int qtyPerUnit))
                    throw new Exception($"❌ كمية المكوّن {comp.ProductName} غير محددة");

                if (comp.Price <= 0)
                    throw new Exception($"❌ سعر المكوّن {comp.ProductName} غير صالح");

                // سعر المكوّن داخل وحدة واحدة
                totalPrice += comp.Price * qtyPerUnit;
            }

            // ضرب في عدد الوحدات المنتجة
            totalPrice *= finalQuantity;




            // 4) بناء المنتج المركب (Builder)
            var builder = new CompositeProductBuilder();
            builder.SetName(productName);
            builder.SetPrice(totalPrice);
            builder.SetQuantity(finalQuantity);
            builder.SetBarcode($"CMP_{Guid.NewGuid().ToString().Substring(0, 8)}");
            builder.SetDescription("منتج مركب (BOM)");
            foreach (var comp in components)
            {
                int qtyPerUnit = componentQuantities[comp.ProductID];
                builder.AddComponent($"{comp.ProductName} × {qtyPerUnit}");
            }

            var compositeProduct = builder.Build();

            // 5) حفظ المنتج المركب
            _uow.ProductRepository.Insert(compositeProduct);

            // 6) إدخال المنتج المركب إلى المخزن
            _uow.WarehouseStockRepository.Insert(new WarehouseStock
            {
                WarehouseID = warehouseId,
                ProductID = compositeProduct.ProductID,
                Quantity = finalQuantity
            });

            // تسجيل حركة دخول للمنتج المركب
            _uow.MovementRepository.Insert(new StockMovement
            {
                ProductID = compositeProduct.ProductID,
                WarehouseID = warehouseId,
                MovementType = "IN",
                Quantity = finalQuantity,
                Date = DateTime.Now
            });

            // 7) خصم المكونات من المخزون + تسجيل الحركات
            foreach (var comp in components)
            {
                int totalRequired =
                    componentQuantities[comp.ProductID] * finalQuantity;

                _uow.WarehouseStockRepository.DecreaseStock(
                    warehouseId,
                    comp.ProductID,
                    totalRequired
                );

                _uow.MovementRepository.Insert(new StockMovement
                {
                    ProductID = comp.ProductID,
                    WarehouseID = warehouseId,
                    MovementType = "OUT",
                    Quantity = totalRequired,
                    Date = DateTime.Now
                });
            }

            // 8) حفظ كل التغييرات
            _uow.SaveChanges();
        }

    }
}
