using AppSecurityPracticalAssignment210733T.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AppSecurityPracticalAssignment210733T.Pages
{
    [BindProperties]
    public class ChangePasswordModel : PageModel
    {
        private readonly SignInManager<CustomerUser> signInManager;
        private readonly UserManager<CustomerUser> userManager;
        
        public ChangePasswordModel(SignInManager<CustomerUser> signInManager, UserManager<CustomerUser> userManager)
        {
            this.signInManager = signInManager;
            this.userManager = userManager;
        }

        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Compare(nameof(Password), ErrorMessage = "Passwords must match")]
        public string ConfirmPassword { get; set; }
        public void OnGet()
        {

        }

        public async Task<IActionResult> OnPostAsync(string code, string username)
        {
            Console.WriteLine("Password Stuff");
            Console.WriteLine(Password);
            Console.WriteLine(ConfirmPassword);
            Console.WriteLine($"Username: {username} & code: {code}");
            if (!ModelState.IsValid)
            {
                TempData["FlashMessage.Text"] = "Passwords do not match";
                TempData["FlashMessage.Type"] = "danger";
                return Page();
            }
            var user = await userManager.FindByNameAsync(username);
            var checkHashedPasswords = new PasswordHasher<CustomerUser>();

            if (user == null)
            {
                TempData["FlashMessage.Text"] = "Invalid Tokens";
                TempData["FlashMessage.Type"] = "danger";
                return Redirect("/");
            }

            if (user.timeBeforePasswordReset > DateTime.Now)
            {
                TempData["FlashMessage.Text"] = $"You just reset your password please wait for 3 minutes before setting a new password ";
                TempData["FlashMessage.Type"] = "danger";
                return Page();
            }

            if (checkHashedPasswords.VerifyHashedPassword(user, user.pastPassword1, ConfirmPassword) == PasswordVerificationResult.Success)
            {
                TempData["FlashMessage.Text"] = "New password can't be same as last 2 passwords";
                TempData["FlashMessage.Type"] = "danger";
                return Page();
            }

            if (checkHashedPasswords.VerifyHashedPassword(user, user.pastPassword2, ConfirmPassword) == PasswordVerificationResult.Success)
            {
                TempData["FlashMessage.Text"] = "New password can't be same as last 2 passwords";
                TempData["FlashMessage.Type"] = "danger";
                return Page();
            }

            if (checkHashedPasswords.VerifyHashedPassword(user, user.PasswordHash, ConfirmPassword) == PasswordVerificationResult.Success)
            {
                TempData["FlashMessage.Text"] = "New Password Cannot be same as Current Password";
                TempData["FlashMessage.Type"] = "danger";
                return Page();
            }
            user.pastPassword2 = user.pastPassword1;
            user.pastPassword1 = user.PasswordHash;
            user.timeBeforePasswordReset = DateTime.Now.AddMinutes(3);
            user.maxPasswordAge = DateTime.Now.AddDays(30); //DateTime.Now.AddSeconds(10);

            var token = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(code));
            var result = await userManager.ResetPasswordAsync(user, token, Password);

            if (!result.Succeeded)
            {
                TempData["FlashMessage.Text"] = "Invalid Tokens";
                TempData["FlashMessage.Type"] = "danger";
                return Page();
            }
            await userManager.UpdateAsync(user);
            TempData["FlashMessage.Text"] = "Successfully reset password!";
            TempData["FlashMessage.Type"] = "success";
            return RedirectToPage("/Login");

        }
    }
}
