using WebProject.Data;
using WebProject.Models;

namespace WebProject.Repositories
{
    public class OrderHistoryRepository : Repository<OrderHistory>, IOrderHistoryRepository
    {
        public AppDbContext Context { get; set; }
        public OrderHistoryRepository(AppDbContext context) : base(context)
        {
            Context = context;
        }

        public void update(OrderHistory orderHistory)
        {
          var objFromDb = Context.OrderHistories.FirstOrDefault(s => s.orderHistoryId == orderHistory.orderHistoryId && s.ProductId == orderHistory.ProductId);
          if (objFromDb != null)
            {
                objFromDb.OrderStatus = orderHistory.OrderStatus;
                objFromDb.Quantity = orderHistory.Quantity;
            }
        }
    }
}
