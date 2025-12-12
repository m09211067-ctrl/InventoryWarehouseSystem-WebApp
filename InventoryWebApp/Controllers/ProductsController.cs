using Microsoft.AspNetCore.Mvc;
using InventoryWebApp.Data;
using InventoryWebApp.Models;
using InventoryWebApp.Patterns;
using InventoryWebApp.Patterns.Prototype;
using InventoryWebApp.Patterns.Decorator;
using InventoryWebApp.Patterns.Strategy;
using InventoryWebApp.Patterns.Composite;
using InventoryWebApp.Patterns.ChainOfResponsibility;
using InventoryWebApp.Patterns.Builder;

namespace InventoryWebApp.Controllers
{
    public class ProductsController : Controller
    {
        private readonly UnitOfWork _unitOfWork;
        private readonly InventoryFacade _facade;

        public ProductsController(UnitOfWork uow, InventoryFacade facade)
        {
            _unitOfWork = uow;
            _facade = facade;
        }

        // ================================
        // 1) عرض جميع المنتجات
        // ================================
        public IActionResult Index()
        {
            var products = _unitOfWork.ProductRepository.GetAll();
            return View(products);
        }

        // ================================
        // 2) صفحة إضافة منتج
        // ================================
        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Warehouses = _unitOfWork.WarehouseRepository.GetAll();
            return View();
        }

        [HttpPost]
        public IActionResult Create(Product product, int warehouseId)
        {
            if (!ModelState.IsValid)
                return View(product);

            // Chain of Responsibility Validation
            var h1 = new CheckNameHandler();
            var h2 = new CheckBarcodeHandler();
            var h3 = new CheckPriceHandler();
            var h4 = new CheckQuantityHandler();

            h1.SetNext(h2).SetNext(h3).SetNext(h4);

            try
            {
                h1.Handle(product);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", ex.Message);
                return View(product);
            }

            // ---- Factory Pattern لإنشاء المنتج ----
            var newProduct = ProductFactory.Create(
                product.ProductName,
                product.Quantity,
                product.Price,
                product.Barcode,
                product.Description ?? ""
            );

            // إضافة المنتج في قاعدة البيانات
            _unitOfWork.ProductRepository.Insert(newProduct);

            // ---- إضافة المنتج داخل المخزن المختار ----
            var stock = new WarehouseStock
            {
                WarehouseID = warehouseId,
                ProductID = newProduct.ProductID,
                Quantity = newProduct.Quantity
            };

            _unitOfWork.WarehouseStockRepository.Insert(stock);

            // ---- تسجيل حركة دخول للمخزن ----
            var movement = new StockMovement
            {
                ProductID = newProduct.ProductID,
                WarehouseID = warehouseId,
                MovementType = "IN",
                Quantity = newProduct.Quantity,
                Date = DateTime.Now
            };

            _unitOfWork.MovementRepository.Insert(movement);

            _unitOfWork.SaveChanges();

            TempData["Message"] = "✔ تم إضافة المنتج بنجاح إلى المخزن";
            return RedirectToAction("Index");
        }


        // ================================
        // 3) تعديل منتج
        // ================================
        [HttpGet]
        public IActionResult Edit(int id)
        {
            var product = _unitOfWork.ProductRepository.GetById(id);

            if (product == null)
                return NotFound();

            return View(product);
        }

        [HttpPost]
        public IActionResult Edit(Product product)
        {
            if (!ModelState.IsValid)
                return View(product);

            _unitOfWork.ProductRepository.Update(product);
            _unitOfWork.SaveChanges();

            TempData["Message"] = "✔ تم تعديل المنتج بنجاح";
            return RedirectToAction("Index");
        }

        // ================================
        // 4) حذف المنتج
        // ================================
        public IActionResult Delete(int id)
        {
            var product = _unitOfWork.ProductRepository.GetById(id);

            if (product == null)
                return NotFound();

            _unitOfWork.ProductRepository.Delete(id);
            _unitOfWork.SaveChanges();

            TempData["Message"] = "✔ تم حذف المنتج بنجاح";
            return RedirectToAction("Index");
        }

        // ================================
        // 5) نسخ منتج (Prototype Pattern)
        // ================================
        public IActionResult Clone(int id)
        {
            var original = _unitOfWork.ProductRepository.GetById(id);
            if (original == null)
                return NotFound();

            var clone = original.Clone();

            _unitOfWork.ProductRepository.Insert(clone);
            _unitOfWork.SaveChanges();

            TempData["Message"] = "✔ تم إنشاء نسخة من المنتج";
            return RedirectToAction("Index");
        }


        // ================================
        // 6) اختبار الاستراتيجيات
        // ================================
        public IActionResult TestStrategy()
        {
            var calc = new PriceCalculator(new BasicPriceStrategy());
            var normal = calc.Calculate(100, 5);

            calc.SetStrategy(new QuantityDiscountStrategy());
            var discounted = calc.Calculate(100, 15);

            ViewBag.Normal = normal;
            ViewBag.Discounted = discounted;

            return View();
        }

        // ================================
        // 7) Decorator Pattern Test
        // ================================
        public IActionResult DecoratorTest()
        {
            IPriceComponent price = new BasePrice(100);
            price = new TaxDecorator(price);
            price = new ShippingDecorator(price);

            return Content($"Final Price = {price.GetPrice()}");
        }

        // ================================
        // 8) Composite Pattern Test
        // ================================
        public IActionResult CompositeTest()
        {
            var keyboard = new SingleProduct("Keyboard", 150);
            var mouse = new SingleProduct("Mouse", 100);

            var bundle = new CompositeProduct("Bundle");
            bundle.Add(keyboard);
            bundle.Add(mouse);

            return Content($"Bundle Price = {bundle.GetPrice()}");
        }

        public IActionResult Details(int id)
        {
            var product = _unitOfWork.ProductRepository.GetById(id);

            if (product == null)
                return NotFound();

            return View(product);
        }

        public IActionResult CreateComposite(int warehouseId)
        {
            _Facade.AddCompositeProduct(warehouseId);

            TempData["Message"] = "✔ تم إضافة منتج مركب بنجاح";
            return RedirectToAction("Index");
        }



        public IActionResult BuildComposite()
        {
            var composite = _facade.BuildComputerProduct();
            ViewBag.Warehouses = _unitOfWork.WarehouseRepository.GetAll();
            ViewBag.Components = _unitOfWork.ProductRepository.GetAll();
            return View(composite);
        }

        [HttpPost]
        public IActionResult SaveComposite(
      string productName,
      int finalQuantity,
      int warehouseId,
      int[] componentIds,
      Dictionary<int, int> componentQuantities)
        {
            // تحقق مبدئي بسيط (بدون منطق أعمال)
            if (string.IsNullOrWhiteSpace(productName) ||
                finalQuantity <= 0 ||
                warehouseId <= 0 ||
                componentIds == null || componentIds.Length == 0)
            {
                TempData["Message"] = "❌ بيانات غير مكتملة لإنشاء المنتج المركب";
                return RedirectToAction("BuildComposite");
            }

            try
            {
                // تمرير كل شيء للـ Facade (هو من يدير المنطق)
                _facade.CreateCompositeFromComponents(
                    productName,
                    finalQuantity,
                    warehouseId,
                    componentIds,
                    componentQuantities
                );

                TempData["Message"] = "✔ تم إنشاء المنتج المركب وخصم المكونات من المخزون";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                // أي خطأ أعمال (كمية غير كافية…)
                TempData["Message"] = "❌ " + ex.Message;
                return RedirectToAction("BuildComposite");
            }
        }




    }
}
