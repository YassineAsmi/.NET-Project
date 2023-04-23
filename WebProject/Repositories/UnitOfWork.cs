using WebProject.Data;

namespace WebProject.Repositories
{
	public class UnitOfWork : IUnitOfWork
	{
		private AppDbContext _context;
		public IProductRepository product { get; private set; }
        public IProduct_OrderRepository product_order { get; private set; }
		public IOrderDetailRepository orderDetail { get; private set; }
        public IOrderRepository order { get; private set; }
		public IUserRepository user { get; private set; }
        public UnitOfWork(AppDbContext db)
		{
			_context = db;
			product = new ProductRepository(_context);
            order = new OrderRepository(_context);
			orderDetail = new OrderDetailRepository(_context);
            product_order = new Product_OrderRepository(_context);
			user = new UserRepository(_context);
        }

		public void Save()
		{
		_context.SaveChanges();
		}
	}
}
