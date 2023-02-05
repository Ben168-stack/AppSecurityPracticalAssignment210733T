using AppSecurityPracticalAssignment210733T.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace AppSecurityPracticalAssignment210733T.Pages
{
    [Authorize]
    public class IndexModel : PageModel
    {
        private readonly ILogger<IndexModel> _logger;
        private readonly SignInManager<CustomerUser> _signInManager;
        private readonly UserManager<CustomerUser> _userManager;
        public IndexModel(ILogger<IndexModel> logger, SignInManager<CustomerUser> signInManager, UserManager<CustomerUser> userManager)
        {
            _logger = logger;
            _signInManager = signInManager;
            _userManager = userManager;
        }

        public string AboutMeUnEncoded { get; set; }

        public async Task<IActionResult> OnGet()
        {
            
            if (HttpContext.Session.GetString("email").IsNullOrEmpty())
            {
                TempData["FlashMessage.Type"] = "success";
                TempData["FlashMessage.Text"] = string.Format("Your session has timeout");
                await _signInManager.SignOutAsync();
                HttpContext.Session.Clear();
                return RedirectToPage("Login");
            }
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                var decodeUserAboutMe = Convert.FromBase64String(user.aboutMe ?? "empty");
                var unencodedAboutMe = Encoding.UTF8.GetString(decodeUserAboutMe);
                AboutMeUnEncoded = unencodedAboutMe;
            }
            else
            {
                AboutMeUnEncoded = "Hi I am Member";
            }
            

            return Page();
        }
    }
}