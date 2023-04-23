using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebProject.Models
{
    public class order_product
    {
        
        public int orderid { get; set; }
        public order order { get; set; }
        
        public int productid { get; set; }
        public product product { get; set; }
        public int quantity { get; set; }
        //Relationship user
        public string userid { get; set; }
        [ValidateNever]
        [ForeignKey("userid")]
        public user users { get; set; }
    }
}
