using AppSecurityPracticalAssignment210733T.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Security.Claims;

namespace AppSecurityPracticalAssignment210733T.Pages
{
    
    public class googleModel : PageModel
    {
        private readonly AuthDbContext _authDbContext;
        private SignInManager<CustomerUser> _signInManager;
        private UserManager<CustomerUser> _userManager;

        public googleModel(AuthDbContext authDbContext, SignInManager<CustomerUser> signInManager, UserManager<CustomerUser> userManager)
        {
            _authDbContext = authDbContext;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGet()
        {
            
            var info = await _signInManager.GetExternalLoginInfoAsync();

            var signInResult = await _signInManager.ExternalLoginSignInAsync(info.LoginProvider,
                info.ProviderKey, isPersistent: true, bypassTwoFactor: true);

            var email = info.Principal.FindFirstValue(ClaimTypes.Email);
            
            if (signInResult.Succeeded)
            {
                HttpContext.Session.SetString("email", email);
                TempData["FlashMessage.Type"] = "success";
                TempData["FlashMessage.Text"] = string.Format("You Logged in Successfully, Welcome {0}", info.Principal.FindFirstValue(ClaimTypes.Email));
                return Redirect("/");
            }
            else
            {
                
                if (email != null)
                {
                    var user = await _userManager.FindByEmailAsync(email);
                    if (user == null)
                    {
                        TempData["FlashMessage.Type"] = "danger";
                        TempData["FlashMessage.Text"] = string.Format("You have not registered an account with us with this email.");
                        return Redirect("/Login");
                    }
                    
                    
                    await _userManager.AddLoginAsync(user, info); //adds exteranl login info that links the email together 
                    await _signInManager.SignInAsync(user, isPersistent: true);
                    HttpContext.Session.SetString("email", email);
                    TempData["FlashMessage.Type"] = "success";
                    TempData["FlashMessage.Text"] = string.Format("You Logged in Successfully, Welcome {0}", info.Principal.FindFirstValue(ClaimTypes.Email));
                    return Redirect("/");
                }
            }

            TempData["FlashMessage.Type"] = "danger";
            TempData["FlashMessage.Text"] = string.Format("You have not registered an account with us with this email.");
            return Redirect("/Login");
        }
    }
}
