using AppSecurityPracticalAssignment210733T.Models;
using System.ComponentModel.DataAnnotations;

namespace AppSecurityPracticalAssignment210733T.ViewModels
{
    public class Register
    {
        [Required]
        [RegularExpression("^[a-zA-Z0-9]+$", ErrorMessage = "Please remove any special characters in your name")]
        public string? fullName { get; set; }

        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Please enter a valid credit card number")]
        public string? creditCardNo { get; set; }
        
        [Required]
        public string? gender { get; set; }

        [Required]
        [RegularExpression("^[0-9]*$", ErrorMessage = "Please enter a valid phone number")]
        public string? mobileNo { get; set; }

        [Required]
        [RegularExpression("^[a-zA-Z0-9]+$", ErrorMessage = "Please remove any special characters in your address")]
        public string? deliveryAddress { get; set; }


        public string? imageURL { get; set; }

        [Required]
        public string? aboutMe { get; set; }

        [Required]
        [DataType(DataType.EmailAddress)]
        [RegularExpression("^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$", ErrorMessage = "Please enter a valid email")]
        public string Email { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Password and confirmation password does not match")]
        public string ConfirmPassword { get; set; }
    }
}
