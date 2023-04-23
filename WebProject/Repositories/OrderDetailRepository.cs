using WebProject.Data;
using WebProject.Models;

namespace WebProject.Repositories
{
    public class OrderDetailRepository : Repository<orderDetail>, IOrderDetailRepository
    {
        private AppDbContext _context;
        public OrderDetailRepository(AppDbContext context) : base(context)
        {
            _context = context;
        }
        public void update(orderDetail orderDetails)
        {
            var objFromDb = _context.orderDetails.FirstOrDefault(s => s.orderid == orderDetails.orderid && s.productid == orderDetails.productid);
            if (objFromDb != null)
            {
                objFromDb.price = orderDetails.price;
                objFromDb.quantity = orderDetails.quantity;
            }
        }
    }
}
