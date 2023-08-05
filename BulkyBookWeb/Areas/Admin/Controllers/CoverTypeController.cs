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
    public class CoverTypeController : Controller
    {
        #region USANDO UNIT OF WORK

        private readonly IUnitOfWork uow;

        public CoverTypeController(IUnitOfWork _uow)
        {
            uow = _uow;
        }

        public IActionResult Index()
        {
            IEnumerable<CoverType> coverTyoelist = uow.CoverType.GetAll();
            Console.WriteLine(coverTyoelist.ToArray());
            return View(coverTyoelist);
        }

        // GET
        public IActionResult Create()
        {
            return View();
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(CoverType obj)
        {
            if (!ModelState.IsValid)
            {
                return View(obj);
            }

            uow.CoverType.Add(obj);
            uow.Save();

            TempData["success"] = "Cover Type guardado exitosamente";

            return RedirectToAction("Index");
        }

        // GET
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var coverType = uow.CoverType.GetFirstOrDefault(c => c.Id == id);

            if (coverType == null)
            {
                return NotFound();
            }

            return View(coverType);
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(CoverType obj)
        {
            if (!ModelState.IsValid)
            {
                return View(obj);
            }

            uow.CoverType.Update(obj);
            uow.Save();

            TempData["success"] = "Cover Type editado exitosamente";

            return RedirectToAction("Index");
        }

        [HttpGet]
        public IActionResult Delete(int? id)
        {
            if (id == null || id == 0)
            {
                return NotFound();
            }

            var coverType = uow.CoverType.GetFirstOrDefault(c => c.Id == id);

            if (coverType == null)
            {
                return NotFound();
            }

            return View(coverType);
        }

        // POST
        [HttpPost, ActionName("Delete")] 
        [ValidateAntiForgeryToken]
        public IActionResult DeleteCoverType(int? id)
        {
            var coverType = uow.CoverType.GetFirstOrDefault(c => c.Id == id);

            if (coverType == null)
            {
                return NotFound();
            }

            uow.CoverType.Remove(coverType);
            uow.Save();

            TempData["success"] = "Cover Type eliminado exitosamente";

            return RedirectToAction("Index");
        }

        #endregion
    }
}
