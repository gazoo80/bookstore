using BulkyBook.DataAccess.Repository.IRepository;
using BulkyBook.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository
{
    public class ShoppingCartRepository : Repository<ShoppingCart>, IShoppingCartRepository
    {
        private readonly ApplicationDbContext db;

        public ShoppingCartRepository(ApplicationDbContext _db) : base(_db)
        {
            db = _db;
        }

        public IEnumerable<ShoppingCart> GetAllForUserId(string userId, string? includeProperties = null)
        {
            IQueryable<ShoppingCart> query = db.ShoppingCarts;
            query = query.Where(sc => sc.ApplicationUserId == userId);

            if (includeProperties is not null)
            {
                var properties = includeProperties.Split(new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                foreach (var property in properties)
                {
                    query = query.Include(property);
                }
            }

            return query.ToList();
        }

        public void Update(ShoppingCart sCart)
        {
            db.ShoppingCarts.Update(sCart);
        }

        public int DecrementCount(ShoppingCart sCart, int count)
        {
            sCart.Count -= count;
            return sCart.Count;
        }

        public int IncrementCount(ShoppingCart sCart, int count)
        {
            sCart.Count += count;
            return sCart.Count;
        }
    }
}
