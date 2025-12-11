using InventoryWebApp.Data;
using InventoryWebApp.Models;
using Microsoft.AspNetCore.Mvc;

namespace InventoryWebApp.Controllers
{
    public class HomeController : Controller
    {
        private readonly UnitOfWork _unitOfWork;

        public HomeController(UnitOfWork uow)
        {
            _unitOfWork = uow;
        }
        public IActionResult Index()
        {
            var dashboard = new DashboardViewModel
            {
                ProductCount = _unitOfWork.ProductRepository.GetAll().Count,
                WarehouseCount = _unitOfWork.WarehouseRepository.GetAll().Count,
                MovementCount = _unitOfWork.MovementRepository.GetAll().Count,
                
                TotalStock = _unitOfWork.WarehouseStockRepository
                                        .GetAllStockQuantity(),

                LowStockProducts = _unitOfWork.ProductRepository
                    .GetAll()
                    .Where(p => p.Quantity < 10)
                    .ToList()
            };

            return View(dashboard);
        }
    }
}
