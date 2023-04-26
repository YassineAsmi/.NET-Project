using System.ComponentModel.DataAnnotations;

namespace WebProject.Models
{
    public class product
    {
        [Key]
        public int productid { get; set; }
		[Display(Name = "Name")]
		public string name { get; set; }
		[Display(Name = "Description")]
		public string description { get; set; }
		[Display(Name = "QTE")]
		public int quantity { get; set; }
		[Display(Name = "Price")]
		public double price { get; set; }
        [Display(Name = "URL Image")]
		public string ?image { get; set; }
        //Relationship
        public List<order_product> ?order_products { get; set; }
    }
}
