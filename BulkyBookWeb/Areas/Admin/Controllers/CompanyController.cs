using BulkyBook.DataAccess;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class CompanyController : Controller
    {
        private readonly IUnitOfWork uow;

        public CompanyController(IUnitOfWork _uow)
        {
            uow = _uow;
        }

        public IActionResult Index()
        {
            return View();
        }

        // GET
        public IActionResult Upsert(int? id)
        {
            Company company = new();

            if (id == null || id == 0)
            {
                return View(company);
            }
            else
            {
                company = uow.Company.GetFirstOrDefault(c => c.Id == id);
                return View(company);
            }
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Upsert(Company obj)
        {
            if (!ModelState.IsValid)
            {
                return View(obj);
            }

            if (obj.Id == 0) 
            {
                uow.Company.Add(obj);
                TempData["success"] = "Company created successfully";
            }
            else  
            {
                uow.Company.Update(obj);
                TempData["success"] = "Company updated successfully";
            }
            
            uow.Save();

            return RedirectToAction("Index");
        }

        #region API CALLS

        [HttpGet]
        public IActionResult GetAll()
        {
            var companyList = uow.Company.GetAll();
            return Json(new { data = companyList });
        }

        [HttpPost]
        public IActionResult Delete(int? id)
        {
            var company = uow.Company.GetFirstOrDefault(c => c.Id == id);

            if (company == null)
            {
                return Json(new { success = false, message = "Error whle deleting" });
            }

            uow.Company.Remove(company);
            uow.Save();

            return Json(new { success = true, message = "Company deleted successfully" });
        }

        #endregion
    }
}
