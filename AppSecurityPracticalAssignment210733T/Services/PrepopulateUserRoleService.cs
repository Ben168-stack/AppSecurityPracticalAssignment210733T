using AppSecurityPracticalAssignment210733T.Models;
using AppSecurityPracticalAssignment210733T.Settings;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;
using System.Text;

namespace AppSecurityPracticalAssignment210733T.Services
{
    public class PrepopulateUserRoleService
    {
        private readonly SignInManager<CustomerUser> _signInManager;
        private readonly UserManager<CustomerUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly AdminPassword _adminPassword;

        public PrepopulateUserRoleService(SignInManager<CustomerUser> signInManager, UserManager<CustomerUser> userManager, RoleManager<IdentityRole> roleManager,AdminPassword adminPassword)
        {
            _signInManager = signInManager;
            _userManager = userManager;
            _roleManager = roleManager;
            _adminPassword = adminPassword;
        }

        const string adminEmail = "admin@mail.com";
        public async Task AdminAccountPrepopulateDB()
        {
            //do the role later
            IdentityRole role = await _roleManager.FindByNameAsync("Admin");
            if (role is null)
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));

            }
            var dataProtectionProvider = DataProtectionProvider.Create("EncryptData");
            var protector = dataProtectionProvider.CreateProtector("MySecretKey");
            var encodeUserAboutMe = Encoding.UTF8.GetBytes("This is an Admin Account");
            var encodedStringUserAboutMe = Convert.ToBase64String(encodeUserAboutMe);
            if (await _userManager.FindByEmailAsync(adminEmail) is null)
            {
               
                var adminUser = new CustomerUser()
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    fullName = "Admin",
                    creditCardNo = protector.Protect("123456789"),
                    gender = "Male",
                    mobileNo = "987654321",
                    deliveryAddress = "Singapore",
                    imageURL = "/uploads/ProfileDefault.jpg",
                    aboutMe = encodedStringUserAboutMe,

                };

                var result = await _userManager.CreateAsync(adminUser, _adminPassword.Password);
                if (result.Succeeded)
                {
                    if (!await _userManager.IsInRoleAsync(adminUser, "Admin")) await _userManager.AddToRoleAsync(adminUser, "Admin");
                }

            }

        }



    }
    
}
