using AppSecurityPracticalAssignment210733T.Models;
using AppSecurityPracticalAssignment210733T.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using System.ComponentModel.DataAnnotations;
using System.Text.Encodings.Web;
using System.Text;
using AppSecurityPracticalAssignment210733T.Services;
using AppSecurityPracticalAssignment210733T.Models;
using AppSecurityPracticalAssignment210733T.ViewModels;

namespace AppSecurityPracticalAssignment210733T.Pages
{
    [BindProperties]
    public class ForgetPasswordModel : PageModel
    {
        public readonly UserManager<CustomerUser> _userManager;

        public readonly EmailSenderService _emailSender;

        public ChangePassword CModel;

        public ForgetPasswordModel(EmailSenderService emailSender, UserManager<CustomerUser> userManager)
        {
            _emailSender = emailSender;
            _userManager = userManager;

        }

        [BindProperty]
        [Required]
        public string Email { get; set; }
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {

                return Page();
            }
            var user = await _userManager.FindByEmailAsync(Email);

            TempData["FlashMessage.Text"] = $"A reset password email will be sent to {Email} if it is valid";
            TempData["FlashMessage.Type"] = "info";

            if (user == null)
            {
                
                return Page();
            }

            var code = await _userManager.GeneratePasswordResetTokenAsync(user);
            code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
            
            var link = $"https://localhost:44334/ChangePassword?code={code}&username={user.UserName}";

            await _emailSender.SendEmailAsync(Email, "Reset Password", "Reset your password with this link "+ $"{link}.");

            



            return Page();
        }
    }
}
