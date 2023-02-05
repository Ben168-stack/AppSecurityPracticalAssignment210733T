using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AppSecurityPracticalAssignment210733T.Models;
using AppSecurityPracticalAssignment210733T.Services;
using Microsoft.AspNetCore.Authorization;

namespace AppSecurityPracticalAssignment210733T.Pages
{
    [Authorize]
    public class LogoutModel : PageModel
    {
        private readonly SignInManager<CustomerUser> signInManager;
        private readonly UserManager<CustomerUser> _userManager;
        public AuditLog AuditLog { get; set; } = new AuditLog();
        private readonly AuditLogService _auditLogService;
        public LogoutModel(SignInManager<CustomerUser> signInManager , AuditLogService auditLogService, UserManager<CustomerUser> userManager)
        {
            this.signInManager = signInManager;
            _auditLogService = auditLogService;
            _userManager = userManager;
        }
        
        public async Task<IActionResult> OnPostLogoutAsync()
        {
            var user = await _userManager.GetUserAsync(User);
           
            AuditLog.Description = $"{user.Email} has logged out.";
            AuditLog.LoggedEmailUser = user.Email;
            AuditLog.Action = "Logout";
            _auditLogService.AddAuditLog(AuditLog);
            await signInManager.SignOutAsync();
            TempData["FlashMessage.Type"] = "success";
            TempData["FlashMessage.Text"] = string.Format("You have logged out successfully");
            
            return RedirectToPage("Login");
        }
        public async Task<IActionResult> OnPostDontLogoutAsync()
        {

            
            return RedirectToPage("Index");
        }
        public void OnGet()
        {
        }
    }
}
