using System.ComponentModel.DataAnnotations;

namespace AppSecurityPracticalAssignment210733T.ViewModels
{
    public class ChangePassword
    {
        [Required]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }

        [Required]
        public string Code { get; set; }

        [Required, RegularExpression(@"^(?=.\d)(?=.[a-z])(?=.[A-Z])(?=.[^a-zA-Z0-9])(?!.*\s).{12,}$", ErrorMessage = "Min 12 chars, Use combination of lower-case, upper-case, Numbers and special characters")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Password and confirmation password does not match")]
        public string ConfirmPassword { get; set; }
    }
}
