using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.Build.Framework;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebProject.Models
{
    public class orderDetail
    {
        public int Id { get; set; }
        [Required]
        public int orderid { get; set; }
        [ValidateNever]
        public order order { get; set; }
        [Required]
        public int productid { get; set; }
        [ValidateNever]
        public product product { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public string Address { get; set; }
        public double price { get; set; }
        public int quantity { get; set; }
        public double? total { get; set; }
        public string userid { get; set; }
        [ValidateNever]
        [ForeignKey("userid")]
        public user users { get; set; }
        // Add a one-to-many relationship with order_product
        public ICollection<order_product> OrderProducts { get; set; }
       // public IEnumerable<order_product> OrderedProducts { get; set; }
    }
}
