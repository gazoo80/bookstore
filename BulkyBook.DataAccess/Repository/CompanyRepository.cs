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
    public class CompanyRepository : Repository<Company>, ICompanyRepository
    {
        private readonly ApplicationDbContext db;

        public CompanyRepository(ApplicationDbContext _db) : base(_db)
        {
            db = _db;
        }

        public void Update(Company company)
        {
            db.Companies.Update(company);
        }
    }
}
