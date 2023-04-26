using WebProject.Models;

namespace WebProject.Repositories
{
    public interface IOrderRepository : IRepository<order>
    {
        void update(order orders);
        void PaymentStatus(int id, string status);
    }
}
