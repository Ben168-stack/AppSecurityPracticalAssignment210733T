using AppSecurityPracticalAssignment210733T.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AppSecurityPracticalAssignment210733T.Pages
{
    [Authorize]
    [BindProperties]
    public class UpdatePasswordModel : PageModel
    {
        private readonly SignInManager<CustomerUser> _signInManager;
        private readonly UserManager<CustomerUser> _userManager;

        public UpdatePasswordModel(SignInManager<CustomerUser> signInManager, UserManager<CustomerUser> userManager)
        {
            _signInManager = signInManager;
            _userManager = userManager;
        }
        [Required]
        [DataType(DataType.Password)]
        public string OldPassword { get; set; }

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

        public async Task<IActionResult> OnPostAsync()
        {

            if (!ModelState.IsValid)
            {
                TempData["FlashMessage.Text"] = "Passwords do not match";
                TempData["FlashMessage.Type"] = "danger";
                return Page();
            }
          
            var user = await _userManager.GetUserAsync(User);
            var checkHashedPasswords = new PasswordHasher<CustomerUser>();

            if (user == null)
            {
                TempData["FlashMessage.Text"] = "Invalid User";
                TempData["FlashMessage.Type"] = "danger";
                return Page();
            }

            if(user.timeBeforePasswordReset > DateTime.Now)
            {
                TempData["FlashMessage.Text"] = $"You just reset your password please wait for 3 minutes before setting a new password ";
                TempData["FlashMessage.Type"] = "danger";
                return Page();
            }

            if(checkHashedPasswords.VerifyHashedPassword(user, user.pastPassword1, ConfirmPassword) == PasswordVerificationResult.Success || checkHashedPasswords.VerifyHashedPassword(user, user.pastPassword2, ConfirmPassword) == PasswordVerificationResult.Success)
            {
                TempData["FlashMessage.Text"] = "New password can't be same as last 2 passwords";
                TempData["FlashMessage.Type"] = "danger";
                return Page();
            }         

            if(checkHashedPasswords.VerifyHashedPassword(user, user.PasswordHash, ConfirmPassword) == PasswordVerificationResult.Success)
            {
                TempData["FlashMessage.Text"] = "New Password Cannot be same as Current Password";
                TempData["FlashMessage.Type"] = "danger";
                return Page();
            }
            user.pastPassword2 = user.pastPassword1;
            user.pastPassword1 = user.PasswordHash;
            user.timeBeforePasswordReset = DateTime.Now.AddMinutes(3);
            user.maxPasswordAge = DateTime.Now.AddDays(30); //DateTime.Now.AddSeconds(10);
            
            var result = await _userManager.ChangePasswordAsync(user, OldPassword,Password);


            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
                
                return Page();
            }

            

            

            await _userManager.UpdateAsync(user);

            TempData["FlashMessage.Text"] = "Successfully update password! You have been logged out to ensure your password change works.";
            TempData["FlashMessage.Type"] = "success";
            return RedirectToPage("/Login");
        }
    }
}
