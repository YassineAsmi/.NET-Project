using WebProject.Models;

namespace WebProject.Repositories
{
    public interface IProductRepository : IRepository<product>
    {
        void Update(product products);
    }
}
