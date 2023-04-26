using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace WebProject.Models
{
    public class user : IdentityUser
    {
        [Display(Name = "First Name")]
        public string first_name { get; set; }
        [Display(Name = "Last Name")]
        public string last_name { get; set; }
        [Display(Name = "Address")]
        public string? Address { get; set; }
        [Display(Name = "Phone Number")]
        public string? phone_number { get; set; }
        [Display(Name = "Payment Method")]
        public string? payment_method { get; set; }
        //Relationship
        public List<order>? orders { get; set; }
    }

}
