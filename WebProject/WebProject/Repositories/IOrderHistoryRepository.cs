using WebProject.Models;

namespace WebProject.Repositories
{
    public interface IOrderHistoryRepository : IRepository<OrderHistory>
    {
        void update(OrderHistory orderHistory);
    }
}
