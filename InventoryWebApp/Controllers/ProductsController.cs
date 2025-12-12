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
using System;
using System.Collections.Generic;
using System.Linq;

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
        // 2) إضافة منتج عادي
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

            // Factory Pattern
            var newProduct = ProductFactory.Create(
                product.ProductName,
                product.Quantity,
                product.Price,
                product.Barcode,
                product.Description ?? ""
            );

            _unitOfWork.ProductRepository.Insert(newProduct);

            _unitOfWork.WarehouseStockRepository.Insert(new WarehouseStock
            {
                WarehouseID = warehouseId,
                ProductID = newProduct.ProductID,
                Quantity = newProduct.Quantity
            });

            _unitOfWork.MovementRepository.Insert(new StockMovement
            {
                ProductID = newProduct.ProductID,
                WarehouseID = warehouseId,
                MovementType = "IN",
                Quantity = newProduct.Quantity,
                Date = DateTime.Now
            });

            _unitOfWork.SaveChanges();

            TempData["Message"] = "✔ تم إضافة المنتج بنجاح";
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

            TempData["Message"] = "✔ تم تعديل المنتج";
            return RedirectToAction("Index");
        }

        // ================================
        // 4) حذف منتج
        // ================================
        public IActionResult Delete(int id)
        {
            var product = _unitOfWork.ProductRepository.GetById(id);
            if (product == null)
                return NotFound();

            _unitOfWork.ProductRepository.Delete(id);
            _unitOfWork.SaveChanges();

            TempData["Message"] = "✔ تم حذف المنتج";
            return RedirectToAction("Index");
        }

        // ================================
        // 5) استنساخ منتج (Prototype)
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
        // 6) تفاصيل منتج
        // ================================
        public IActionResult Details(int id)
        {
            var product = _unitOfWork.ProductRepository.GetById(id);
            if (product == null)
                return NotFound();

            return View(product);
        }

        // ================================
        // 7) إنشاء منتج مركب (Builder + BOM)
        // ================================
        [HttpGet]
        public IActionResult BuildComposite()
        {
            var emptyProduct = new Product();

            ViewBag.Warehouses = _unitOfWork.WarehouseRepository.GetAll();
            ViewBag.Components = _unitOfWork.ProductRepository.GetAll();

            return View(emptyProduct);
        }


        [HttpPost]
        public IActionResult SaveComposite(
            string productName,
            int finalQuantity,
            int warehouseId,
            int[] componentIds,
            Dictionary<int, int> componentQuantities)
        {
            if (string.IsNullOrWhiteSpace(productName) ||
                finalQuantity <= 0 ||
                warehouseId <= 0 ||
                componentIds == null || componentIds.Length == 0)
            {
                TempData["Message"] = "❌ بيانات غير مكتملة";
                return RedirectToAction("BuildComposite");
            }

            try
            {
                _facade.CreateCompositeFromComponents(
                    productName,
                    finalQuantity,
                    warehouseId,
                    componentIds,
                    componentQuantities
                );

                TempData["Message"] = "✔ تم إنشاء المنتج المركب وخصم المكونات";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                TempData["Message"] = "❌ " + ex.Message;
                return RedirectToAction("BuildComposite");
            }
        }

        // ================================
        // 8) Strategy Pattern Test
        // ================================
        public IActionResult TestStrategy()
        {
            var calc = new PriceCalculator(new BasicPriceStrategy());
            ViewBag.Normal = calc.Calculate(100, 5);

            calc.SetStrategy(new QuantityDiscountStrategy());
            ViewBag.Discounted = calc.Calculate(100, 15);

            return View();
        }

        // ================================
        // 9) Decorator Pattern Test
        // ================================
        public IActionResult DecoratorTest()
        {
            IPriceComponent price = new BasePrice(100);
            price = new TaxDecorator(price);
            price = new ShippingDecorator(price);

            return Content($"Final Price = {price.GetPrice()}");
        }

        // ================================
        // 10) Composite Pattern Test
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
    }
}
