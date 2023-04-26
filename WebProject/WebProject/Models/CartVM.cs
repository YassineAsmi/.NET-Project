namespace WebProject.Models
{
    public class CartVM
    {
        public order Order { get; set; }
        public IEnumerable<order_product> ListCart { get; set; }
        public orderDetail OrderDetail { get; set; }

        public IEnumerable<order_product> OrderedProducts { get; set; }
    }
}
