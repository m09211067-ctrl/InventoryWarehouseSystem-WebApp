using Microsoft.AspNetCore.Mvc;
using InventoryWebApp.Patterns;
using InventoryWebApp.Data;

namespace InventoryWebApp.Controllers
{
    public class StockController : Controller
    {
        private readonly InventoryFacade _facade;
        private readonly ProductRepository _productRepo;
        private readonly WarehouseRepository _warehouseRepo;
        private readonly WarehouseStockRepository _stockRepo;
        private readonly MovementRepository _movementRepo;

        public StockController(
            InventoryFacade facade,
            ProductRepository productRepo,
            WarehouseRepository warehouseRepo,
            WarehouseStockRepository stockRepo,
            MovementRepository movementRepo)
        {
            _facade = facade;
            _productRepo = productRepo;
            _warehouseRepo = warehouseRepo;
            _stockRepo = stockRepo;
            _movementRepo = movementRepo;
        }

        // ------------------------------------------------------
        // 1) STOCK DASHBOARD
        // ------------------------------------------------------
        public IActionResult Index()
        {
            var products = _productRepo.GetAll();
            var warehouses = _warehouseRepo.GetAll();
            var totalStock = _stockRepo.GetAllStockQuantity();
            var movements = _movementRepo.GetAll();

            ViewBag.TotalStock = totalStock;

            return View(new
            {
                Products = products,
                Warehouses = warehouses,
                Movements = movements
            });
        }

        // ------------------------------------------------------
        // 2) ADD STOCK PAGE
        // ------------------------------------------------------
        [HttpGet]
        public IActionResult Add()
        {
            ViewBag.Products = _productRepo.GetAll();
            ViewBag.Warehouses = _warehouseRepo.GetAll();
            return View();
        }

        [HttpPost]
        public IActionResult Add(int productId, int warehouseId, int quantity)
        {
            _facade.AddStock(productId, warehouseId, quantity);

            TempData["msg"] = "Stock Added Successfully!";
            return RedirectToAction("Index");
        }

        // ------------------------------------------------------
        // 3) REMOVE STOCK
        // ------------------------------------------------------
        [HttpGet]
        public IActionResult Remove()
        {
            ViewBag.Products = _productRepo.GetAll();
            ViewBag.Warehouses = _warehouseRepo.GetAll();
            return View();
        }

        [HttpPost]
        public IActionResult Remove(int productId, int warehouseId, int quantity)
        {
            bool success = _facade.RemoveStock(productId, warehouseId, quantity);

            if (!success)
            {
                TempData["error"] = "Not Enough Stock!";
                return RedirectToAction("Remove");
            }

            TempData["msg"] = "Stock Removed Successfully!";
            return RedirectToAction("Index");
        }

        // ------------------------------------------------------
        // 4) TRANSFER STOCK
        // ------------------------------------------------------
        [HttpGet]
        public IActionResult Transfer()
        {
            ViewBag.Products = _productRepo.GetAll();
            ViewBag.Warehouses = _warehouseRepo.GetAll();
            return View();
        }

        [HttpPost]
        public IActionResult Transfer(int productId, int fromWarehouseId, int toWarehouseId, int quantity)
        {
            if (fromWarehouseId == toWarehouseId)
            {
                TempData["error"] = "Warehouses cannot be the same!";
                return RedirectToAction("Transfer");
            }

            bool success = _facade.TransferStock(productId, fromWarehouseId, toWarehouseId, quantity);

            if (!success)
            {
                TempData["error"] = "Not Enough Stock in Source Warehouse!";
                return RedirectToAction("Transfer");
            }

            TempData["msg"] = "Stock Transferred Successfully!";
            return RedirectToAction("Index");
        }

        // ------------------------------------------------------
        // 5) MOVEMENT HISTORY
        // ------------------------------------------------------
        public IActionResult Movements()
        {
            var movements = _movementRepo.GetAll();
            return View(movements);
        }
    }
}
