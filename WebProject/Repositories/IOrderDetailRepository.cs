using WebProject.Models;

namespace WebProject.Repositories
{
    public interface IOrderDetailRepository : IRepository<orderDetail> 
    {
        void update(orderDetail orderDetails);
    }
}
