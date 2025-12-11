using Microsoft.AspNetCore.Mvc;
using InventoryWebApp.Data;
using InventoryWebApp.Models;

namespace InventoryWebApp.Controllers
{
    public class WarehousesController : Controller
    {
        private readonly UnitOfWork _unitOfWork;

        // 🤝 استخدام DI بدلاً من new UnitOfWork()
        public WarehousesController(UnitOfWork uow)
        {
            _unitOfWork = uow;
        }

        // عرض كل المخازن
        public IActionResult Index()
        {
            var warehouses = _unitOfWork.WarehouseRepository.GetAll();
            return View(warehouses);
        }

        // عرض صفحة إضافة مخزن جديد
        public IActionResult Create()
        {
            return View();
        }

        // إضافة مخزن جديد (POST)
        [HttpPost]
        public IActionResult Create(Warehouse warehouse)
        {
            if (!ModelState.IsValid)
            {
                return View(warehouse);
            }

            _unitOfWork.WarehouseRepository.Insert(warehouse);
            _unitOfWork.SaveChanges();

            TempData["Message"] = "✔ تم إضافة المخزن بنجاح";
            return RedirectToAction("Index");
        }

        // عرض صفحة تعديل مخزن
        public IActionResult Edit(int id)
        {
            var warehouse = _unitOfWork.WarehouseRepository.GetById(id);

            if (warehouse == null)
                return NotFound();

            return View(warehouse);
        }

        // تعديل مخزن (POST)
        [HttpPost]
        public IActionResult Edit(Warehouse warehouse)
        {
            if (!ModelState.IsValid)
            {
                return View(warehouse);
            }

            _unitOfWork.WarehouseRepository.Update(warehouse);
            _unitOfWork.SaveChanges();

            TempData["Message"] = "✔ تم تعديل بيانات المخزن بنجاح";
            return RedirectToAction("Index");
        }

        // صفحة تأكيد الحذف
        public IActionResult Delete(int id)
        {
            var warehouse = _unitOfWork.WarehouseRepository.GetById(id);

            if (warehouse == null)
                return NotFound();

            return View(warehouse);
        }

        // تنفيذ الحذف
        [HttpPost]
        public IActionResult ConfirmDelete(int id)
        {
            _unitOfWork.WarehouseRepository.Delete(id);
            _unitOfWork.SaveChanges();

            TempData["Message"] = "✔ تم حذف المخزن بنجاح";
            return RedirectToAction("Index");
        }
    }
}
