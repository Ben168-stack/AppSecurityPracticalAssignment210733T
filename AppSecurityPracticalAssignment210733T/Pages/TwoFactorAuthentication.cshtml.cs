using AppSecurityPracticalAssignment210733T.Models;
using AppSecurityPracticalAssignment210733T.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Newtonsoft.Json.Linq;

namespace AppSecurityPracticalAssignment210733T.Pages
{
    [BindProperties]
    public class TwoFactorAuthenticationModel : PageModel
    {
        private readonly SignInManager<CustomerUser> _signInManager;
        private readonly UserManager<CustomerUser> _userManager;
        private readonly EmailSenderService _emailSenderService;
        private readonly AuditLogService _auditLogService;

        public string otp { get; set; }

        public AuditLog AuditLog { get; set; } = new();

        public TwoFactorAuthenticationModel(SignInManager<CustomerUser> signInManager, UserManager<CustomerUser> userManager, EmailSenderService emailSenderService, AuditLogService auditLogService)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _emailSenderService = emailSenderService;
            _auditLogService = auditLogService;
            
        }

        public async void OnGet(string email)
        {
            var user = await _userManager.FindByEmailAsync(email);
            if(user != null)
            {
                await _userManager.UpdateSecurityStampAsync(user);
                await _emailSenderService.SendEmailAsync(email, "Two Factor Authentication", $"Here is your OTP,{await _userManager.GenerateTwoFactorTokenAsync(user, TokenOptions.DefaultEmailProvider)} ");
            }
        }

        public async Task<IActionResult> OnPostAsync()
        {
            CustomerUser user = await _signInManager.GetTwoFactorAuthenticationUserAsync();

            
                var identityResult = await _signInManager.TwoFactorSignInAsync(TokenOptions.DefaultEmailProvider, otp, true, true);

                if (identityResult.Succeeded)
                {
                    TempData["FlashMessage.Type"] = "success";
                    TempData["FlashMessage.Text"] = string.Format("You Logged in Successfully, Welcome {0}", user.Email);
                    AuditLog.Description = $"{user.Email} has logged in.";
                    AuditLog.LoggedEmailUser = user.Email;
                    AuditLog.Action = "Login";
                    _auditLogService.AddAuditLog(AuditLog);
                HttpContext.Session.SetString("email", user.Email);
                return RedirectToPage("Index");
                }
                TempData["FlashMessage.Type"] = "danger";
                TempData["FlashMessage.Text"] = string.Format("You OTP Token is Invalid " );
                return Page();

            }
            
    }
}


