namespace WebProject.Repositories
{
    public interface IUnitOfWork
    {
        IProductRepository product { get; }
        IOrderRepository order { get; }
        IProduct_OrderRepository product_order { get; }
        IOrderDetailRepository orderDetail { get; }
        IUserRepository user { get; }
        //	IProduct_OrderRepository product_order { get; }
        void Save();
    }
}
