using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace AppSecurityPracticalAssignment210733T.Models
{
    public class CustomerUser : IdentityUser
    {
        [Required]
        public string? fullName { get; set; }

        [Required]
        public string? creditCardNo { get; set; }

        [Required]
        public string? gender { get; set; }

        [Required]
        public string? mobileNo { get; set; }

        [Required]
        public string? deliveryAddress { get; set; }

        
        public string? imageURL { get; set; } 

        [Required]
        public string? aboutMe { get; set; }

        
        public string? pastPassword1 { get; set; } = "";
        
        public string? pastPassword2 { get; set; } = "";
        
        public DateTime? timeBeforePasswordReset { get; set; } = null;

        public DateTime? maxPasswordAge { get; set; } = DateTime.Now.AddDays(30); //DateTime.Now.AddSeconds(10);


    }
}
