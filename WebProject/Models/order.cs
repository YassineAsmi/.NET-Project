
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebProject.Models
{
    public class order
    {
        [Key]
        public int orderid { get; set; }

        public double? total { get; set; }
        [Required]
        public DateTime? date_order { get; set; }
        public string? status { get; set; }
        public string userid { get; set; }
        [ValidateNever]
        public user users { get; set; }
        public string? payment_status { get; set; }
        public DateTime? date_payment { get; set; }
        //Relationship product
        [ValidateNever]
        public List<order_product> order_products { get; set; }

    }
}
