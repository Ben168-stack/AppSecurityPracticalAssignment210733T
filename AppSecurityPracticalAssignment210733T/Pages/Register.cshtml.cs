using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using AppSecurityPracticalAssignment210733T.Models;
using AppSecurityPracticalAssignment210733T.ViewModels;
using Microsoft.AspNetCore.DataProtection;
using System.Text;
using AppSecurityPracticalAssignment210733T.Services;

namespace AppSecurityPracticalAssignment210733T.Pages
{
    [BindProperties]
    public class RegisterModel : PageModel
    {
        private IWebHostEnvironment _environment;
        private UserManager<CustomerUser> _userManager { get; }
        private SignInManager<CustomerUser> _signInManager { get; }

        
        public Register RModel { get; set; } = new();

        
        public IFormFile? Upload { get; set; }

        public RegisterModel(UserManager<CustomerUser> userManager, SignInManager<CustomerUser> signInManager, IWebHostEnvironment environment)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _environment = environment;
        }

        public void OnGet()
        {
            
        }

        //Save data into the database
        public async Task<IActionResult> OnPostAsync()
        {
           
            if (ModelState.IsValid)
            {
                String password = RModel.Password;
                PasswordScore passwordStrengthScore = PasswordScoreService.CheckStrength(password);

                switch (passwordStrengthScore)
                {
                    case PasswordScore.Blank:
                    case PasswordScore.VeryWeak:
                    case PasswordScore.Weak:
                        // Show an error message to the user
                        TempData["FlashMessage.Type"] = "danger";
                        TempData["FlashMessage.Text"] = string.Format("Password is still weak");
                        break;
                    case PasswordScore.Medium:
                    case PasswordScore.Strong:
                    case PasswordScore.VeryStrong:
                        // Password deemed strong enough, allow user to be added to database etc
                        break;
                }

                var dataProtectionProvider = DataProtectionProvider.Create("EncryptData");
                var protector = dataProtectionProvider.CreateProtector("MySecretKey");


                if (Upload != null)
                {
                    if (Upload.Length > 2 * 1024 * 1024)
                    {
                        ModelState.AddModelError("Upload", "File size cannot exceed 2MB.");
                        return Page();
                    }
                    var uploadsFolder = "uploads";
                    var imageFile = Guid.NewGuid() + Path.GetExtension(Upload.FileName);
                    var imagePath = Path.Combine(_environment.ContentRootPath, "wwwroot", uploadsFolder, imageFile);
                    using var fileStream = new FileStream(imagePath, FileMode.Create);
                    await Upload.CopyToAsync(fileStream);
                    RModel.imageURL = string.Format("/{0}/{1}", uploadsFolder, imageFile);


                    
                }
                else
                {
                    RModel.imageURL = "/uploads/ProfileDefault.jpg";


                }
                var encodeUserAboutMe = Encoding.UTF8.GetBytes(RModel.aboutMe);
                var encodedStringUserAboutMe = Convert.ToBase64String(encodeUserAboutMe);

                var user = new CustomerUser()
                {
                    UserName = RModel.Email,
                    Email = RModel.Email,
                    fullName = RModel.fullName,
                    creditCardNo = protector.Protect(RModel.creditCardNo),
                    gender = RModel.gender,
                    mobileNo = RModel.mobileNo,
                    deliveryAddress = RModel.deliveryAddress,
                    imageURL = RModel.imageURL,
                    aboutMe = encodedStringUserAboutMe,

                };
                var result = await _userManager.CreateAsync(user, RModel.Password);

                if (result.Succeeded)
                {

                    HttpContext.Session.SetString("email", RModel.Email);
                    await _signInManager.SignInAsync(user, false);
                    TempData["FlashMessage.Type"] = "success";
                    TempData["FlashMessage.Text"] = string.Format("Account Registered Successfully, Welcome {0}", user.UserName);
                    return RedirectToPage("Index");
                }
                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return Page();
        }

    }
}
