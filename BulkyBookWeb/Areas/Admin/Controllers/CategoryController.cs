using BulkyBook.DataAccess;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CategoryController : Controller
    {
        #region USANDO UNIT OF WORK

        private readonly IUnitOfWork uow;

        public CategoryController(IUnitOfWork _uow)
        {
            uow = _uow;
        }

        public IActionResult Index()
        {
            IEnumerable<Category> categorylist = uow.Category.GetAll();
            Console.WriteLine(categorylist.ToArray());
            return View(categorylist);
        }

        // GET
        public IActionResult Create()
        {
            return View();
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(Category obj)
        {
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("Name", "DisplayOrder y Name no pueden ser iguales");
            }

            if (!ModelState.IsValid)
            {
                return View(obj);
            }

            uow.Category.Add(obj);
            uow.Save();

            TempData["success"] = "Categoria guardada exitosamente";

            return RedirectToAction("Index");
        }

        // GET
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var category = uow.Category.GetFirstOrDefault(c => c.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(Category obj)
        {
            if (obj.Name == obj.DisplayOrder.ToString())
            {
                ModelState.AddModelError("Name", "DisplayOrder y Name no pueden ser iguales");
            }

            if (!ModelState.IsValid)
            {
                return View(obj);
            }

            uow.Category.Update(obj);
            uow.Save();

            TempData["success"] = "Categoria editada exitosamente";

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var category = uow.Category.GetFirstOrDefault(c => c.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST
        [HttpPost, ActionName("Delete")] 
        [ValidateAntiForgeryToken]
        public IActionResult DeleteCategory(int? id)
        {
            var category = uow.Category.GetFirstOrDefault(c => c.Id == id);

            if (category == null)
            {
                return NotFound();
            }

            uow.Category.Remove(category);
            uow.Save();

            TempData["success"] = "Categoria eliminada exitosamente";

            return RedirectToAction("Index");
        }

        #endregion
    }
}
