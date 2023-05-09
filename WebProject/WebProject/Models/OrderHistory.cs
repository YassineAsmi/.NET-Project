
using System.ComponentModel.DataAnnotations;

namespace WebProject.Models
{
    public class OrderHistory
    {
        [Key]
        public int orderHistoryId { get; set; }
        public string UserId { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public DateTime OrderDate { get; set; }
        public string OrderStatus { get; set; }

        public virtual user User { get; set; }
        public virtual product Product { get; set; }
    }

}
