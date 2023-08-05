using BulkyBook.DataAccess;
using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using BulkyBook.Models.ViewModels;
using BulkyBook.Utility;
using BulkyBook.Utility.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyBookWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = SD.Role_Admin)]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork uow;
        private readonly IWebHostEnvironment host;
        private readonly IFileStorage fileStorage;
        private readonly string container = "products";

        public ProductController(IUnitOfWork _uow, IWebHostEnvironment _host, IFileStorage _fileStorage)
        {
            uow = _uow;
            host = _host;
            fileStorage = _fileStorage;
        }

        public IActionResult Index()
        {
            return View();
        }

        // GET
        public IActionResult Upsert(int? id)
        {
            ProductVM productVM = new()
            {
                Product = new(),
                CategoryList = uow.Category.GetAll().Select(
                    c => new SelectListItem
                    {
                        Text = c.Name,
                        Value = c.Id.ToString()
                    }
                ),
                CoverTypeList = uow.CoverType.GetAll().Select(
                    c => new SelectListItem
                    {
                        Text = c.Name,
                        Value = c.Id.ToString()
                    }
                )
            };

            if (id == null || id == 0)
            {
                return View(productVM);
            }
            else
            {
                productVM.Product = uow.Product.GetFirstOrDefault(p => p.Id == id);
                return View(productVM);
            }
        }

        // POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Upsert(ProductVM obj, IFormFile? file)
        {
            if (!ModelState.IsValid)
            {
                return View(obj);
            }

            if (file != null)
            {
                using (var memoryStrem = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStrem);
                    var content = memoryStrem.ToArray();
                    var extension = Path.GetExtension(file.FileName);

                    if (obj.Product.ImageUrl != null)
                    {
                        obj.Product.ImageUrl = await fileStorage.EditFile(content, extension, container,
                                                                 obj.Product.ImageUrl, file.ContentType);
                    }
                    else
                    {
                        obj.Product.ImageUrl = await fileStorage.SaveFile(content, extension, container,
                                                                 file.ContentType);
                    }   
                }
            }

            if (obj.Product.Id == 0)
            {
                uow.Product.Add(obj.Product);
            }
            else
            {
                uow.Product.Update(obj.Product);
            }

            uow.Save();

            TempData["success"] = "Producto guardado exitosamente";

            return RedirectToAction("Index");
        }

        #region API CALLS


        [HttpGet]
        public IActionResult GetAll()
        {
            var productList = uow.Product.GetAll(includeProperties: "Category,CoverType");
          
            var products = productList.Select(p => new {
                Id = p.Id,
                Title = p.Title, 
                Description = p.Description,
                Isbn = p.Isbn,
                Price = p.Price.ToString("#,###.00"),
                Author = p.Author,
                Category = p.Category.Name
            });

            return Json(new { data = products });
        }

        [HttpPost]
        public async Task<IActionResult> Delete(int? id)
        {
            var product = uow.Product.GetFirstOrDefault(p => p.Id == id);

            if (product == null)
            {
                return Json(new { success = false, message = "Error while deleting" });
            }

            await fileStorage.DeleteFile(product.ImageUrl, container);
 
            uow.Product.Remove(product);
            uow.Save();

            return Json(new { success = true, message = "Product deleted successfully" });
        }

        #endregion
    }
}
