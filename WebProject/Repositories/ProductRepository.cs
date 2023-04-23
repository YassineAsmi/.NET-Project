using WebProject.Data;
using WebProject.Models;

namespace WebProject.Repositories
{
	public class ProductRepository : Repository<product>, IProductRepository
	{
		private AppDbContext _context;
		public ProductRepository(AppDbContext context) : base(context)
		{
			_context = context;
		}

		public void Update(product products)
		{
			var objFromDb = _context.products.FirstOrDefault(s => s.productid == products.productid);
			if (objFromDb != null)
			{
				objFromDb.name = products.name;
				objFromDb.price = products.price;
				objFromDb.description = products.description;
				if (products.image != null)
				{
					objFromDb.image = products.image;
				}
			}
		}
	}
}
