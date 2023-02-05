using AppSecurityPracticalAssignment210733T.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace AppSecurityPracticalAssignment210733T.Pages
{
    public class TwoFactorAuthenticationModel : PageModel
    {
        private readonly SignInManager<CustomerUser> _signInManager;
        private readonly UserManager<CustomerUser> _userManager;
        public void OnGet()
        {
        }
    }
}
