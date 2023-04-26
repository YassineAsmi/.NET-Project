using WebProject.Data;
using WebProject.Models;

namespace WebProject.Repositories
{
    public class OrderRepository : Repository<order>, IOrderRepository
    {
        private AppDbContext _context;
        public OrderRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }

        public void PaymentStatus(int id, string status)
        {
            var order = _context.orders.FirstOrDefault(s => s.orderid == id);
            order.payment_status = status;
            order.date_payment = DateTime.Now;
        }

        public void update(order orders)
        {
            var objFromDb = _context.orders.FirstOrDefault(s => s.orderid == orders.orderid);
            if (objFromDb != null)
            {
                objFromDb.total = orders.total;
                objFromDb.status = orders.status;
                objFromDb.date_order = orders.date_order;
                if (orders.payment_status != null)
                {
                    objFromDb.payment_status = orders.payment_status;
                    objFromDb.date_payment = DateTime.Now;
                }
            }
        }
    }
}
