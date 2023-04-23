using WebProject.Data;
using WebProject.Models;

namespace WebProject.Repositories
{
	public class Product_OrderRepository : Repository<order_product>, IProduct_OrderRepository
	{
		private readonly AppDbContext _context;

		public Product_OrderRepository(AppDbContext context) : base(context)
		{
			_context = context;
		}

		public void Update(order_product order_products)
		{
			var objFromDb = _context.order_products.FirstOrDefault(s => s.orderid == order_products.orderid);
			if (objFromDb != null)
			{
				objFromDb.quantity = order_products.quantity;
			}
		}

        public void IncrementCartItem(order_product cartItem, int quantity)
        {
            // Check if the cartItem already exists in the database
            var existingCartItem = _context.order_products
                .FirstOrDefault(op => op.orderid == cartItem.orderid && op.productid == cartItem.productid);

            if (existingCartItem != null)
            {
                // If the cartItem exists, increment the quantity
                existingCartItem.quantity += quantity;
            }
            else
            {
                // If the cartItem does not exist, add it to the database
                _context.order_products.Add(cartItem);
            }

            // Save the changes
            _context.SaveChanges();
        }
    }
}
