using WebProject.Models;

namespace WebProject.Repositories
{
	public interface IProduct_OrderRepository : IRepository<order_product>
	{
        void IncrementCartItem(order_product cartItem, int quantity);
        void Update(order_product order_products);
	}
}
