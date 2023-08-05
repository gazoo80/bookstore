using BulkyBook.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BulkyBook.DataAccess.Repository.IRepository
{
    public interface IShoppingCartRepository : IRepository<ShoppingCart>
    {
        IEnumerable<ShoppingCart> GetAllForUserId(string userId, string? includeProperties = null);
        void Update(ShoppingCart sCart);
        int IncrementCount(ShoppingCart sCart, int count);
        int DecrementCount(ShoppingCart sCart, int count);
    }
}
