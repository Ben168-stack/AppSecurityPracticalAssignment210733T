using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AppSecurityPracticalAssignment210733T.Models;
using AppSecurityPracticalAssignment210733T.ViewModels;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using AppSecurityPracticalAssignment210733T.Services;

namespace AppSecurityPracticalAssignment210733T.Pages
{
    public class LoginModel : PageModel
    {
        private readonly GoogleCaptchaService _googleCaptchaService;

        [BindProperty]
        public Login LModel { get; set; }


        public AuditLog AuditLog { get; set; } = new();


        private readonly SignInManager<CustomerUser> signInManager;
        private readonly UserManager<CustomerUser> userManager;

        private readonly AuditLogService _auditLogService;
        public LoginModel(SignInManager<CustomerUser> signInManager, GoogleCaptchaService googleCaptchaService, AuditLogService auditLogService, UserManager<CustomerUser> userManager)
        {
            this.signInManager = signInManager;
            _googleCaptchaService = googleCaptchaService;
            _auditLogService = auditLogService;
            this.userManager = userManager;
        }
        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            //Verify response token with Google

            var captchaResult = await _googleCaptchaService.VerifyToken(LModel.Token);

            // If human verification fails return page
            if (!captchaResult)
            {
                return Page();
            }

            if (ModelState.IsValid)
            {
                // Check for users detected on multiple browsers
                var user = await userManager.FindByEmailAsync(LModel.Email);
                if (user != null && await userManager.CheckPasswordAsync(user, LModel.Password))
                {
                    await userManager.UpdateSecurityStampAsync(user);
                }
                var identityResult = await signInManager.PasswordSignInAsync(LModel.Email, LModel.Password,
                LModel.RememberMe, true);


                if (identityResult.IsLockedOut)
                {
                    TempData["FlashMessage.Type"] = "danger";
                    TempData["FlashMessage.Text"] = string.Format("Too Many Attempts, your account is currently lockout please try again later.");
                }
                if (identityResult.Succeeded)
                {
                    HttpContext.Session.SetString("email", LModel.Email);

                    TempData["FlashMessage.Type"] = "success";
                    TempData["FlashMessage.Text"] = string.Format("You Logged in Successfully, Welcome {0}", LModel.Email);
                    AuditLog.Description = $"{LModel.Email} has logged in.";
                    AuditLog.LoggedEmailUser = LModel.Email;
                    AuditLog.Action = "Login";
                    _auditLogService.AddAuditLog(AuditLog);
                    return RedirectToPage("Index");
                }
                ModelState.AddModelError("", "Username or Password incorrect");
            }



            return Page();
        }

        public async Task<IActionResult> OnPostGoogleauthAsync()
        {


            return new ChallengeResult("Google",signInManager.ConfigureExternalAuthenticationProperties("Google", "/google"));
        }
    }


}
