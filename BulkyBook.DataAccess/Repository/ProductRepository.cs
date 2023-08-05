using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository
{
    public class ProductRepository : Repository<Product>, IProductRepository
    {
        private readonly ApplicationDbContext db;

        public ProductRepository(ApplicationDbContext _db) : base(_db)
        {
            db = _db;
        }

        public void Update(Product product)
        {
            //db.Products.Update(product);

            var prodFromDb = db.Products.FirstOrDefault(p => p.Id == product.Id);

            if (prodFromDb != null)
            {
                prodFromDb.Title = product.Title;
                prodFromDb.Description = product.Description;
                prodFromDb.Isbn = product.Isbn;
                prodFromDb.Author = product.Author;
                prodFromDb.Price = product.Price;
                prodFromDb.ListPrice = product.ListPrice;
                prodFromDb.Price50 = product.Price50;
                prodFromDb.Price100 = product.Price100;
                prodFromDb.CategoryId = product.CategoryId;
                prodFromDb.CoverTypeId = product.CoverTypeId;

                if (product.ImageUrl != null)
                {
                    prodFromDb.ImageUrl = product.ImageUrl;
                }
            }
        }
    }
}
