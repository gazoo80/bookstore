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
    public class OrderHeaderRepository : Repository<OrderHeader>, IOrderHeaderRepository
    {
        private readonly ApplicationDbContext db;

        public OrderHeaderRepository(ApplicationDbContext _db) : base(_db)
        {
            db = _db;
        }

        public void Update(OrderHeader orderHeader)
        {
            db.OrderHeaders.Update(orderHeader);
        }

        public void UpdateStatus(int id, string orderStatus, string? paymentStatus = null)
        {
            var orderFromDb = db.OrderHeaders.FirstOrDefault(oh => oh.Id == id);

            if (orderFromDb != null) 
            {
                orderFromDb.OrderStatus = orderStatus;

                if (paymentStatus != null)
                {
                    orderFromDb.PaymentStatus = paymentStatus;
                }
            }
        }

        public void UpdateStripePaymentId(int id, string sessionId, string paymentIntentId)
        {
            var orderFromDb = db.OrderHeaders.FirstOrDefault(oh => oh.Id == id);

            if (orderFromDb != null)
            {
                orderFromDb.SessionId = sessionId;
                orderFromDb.PaymentIntentId = paymentIntentId;
            }
        }
    }
}
