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
    public class CoverTypeRepository : Repository<CoverType>, ICoverTypeRepository
    {
        private readonly ApplicationDbContext db;

        public CoverTypeRepository(ApplicationDbContext _db) : base(_db)
        {
            db = _db;
        }

        public void Update(CoverType coverType)
        {
            db.CoverType.Update(coverType);
        }
    }
}
