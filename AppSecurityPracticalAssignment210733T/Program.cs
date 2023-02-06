using AppSecurityPracticalAssignment210733T.Models;
using AppSecurityPracticalAssignment210733T.Services;
using AppSecurityPracticalAssignment210733T.Settings;
using Microsoft.AspNetCore.Authentication.Google;
using Microsoft.AspNetCore.Identity;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<IdentityOptions>(opts =>
{
    opts.Lockout.AllowedForNewUsers = true;
    opts.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromSeconds(10);
    opts.Lockout.MaxFailedAccessAttempts = 3;
});

//Service to check multiple browsers
builder.Services.Configure<SecurityStampValidatorOptions>(x =>
{
    x.ValidationInterval = TimeSpan.Zero;
});


// Add services to the container.
builder.Services.AddRazorPages();
builder.Services.Configure<GoogleCaptchaConfig>(builder.Configuration.GetSection("GoogleReCaptcha"));
builder.Services.AddTransient(typeof(GoogleCaptchaService));
builder.Services.AddDbContext<AuthDbContext>();
builder.Services.AddIdentity<CustomerUser, IdentityRole>().AddEntityFrameworkStores<AuthDbContext>().AddDefaultTokenProviders();
builder.Services.AddDataProtection();


//Services
builder.Services.AddScoped<AuditLogService>();
builder.Services.AddScoped<EmailSenderService>();
builder.Services.AddScoped<PrepopulateUserRoleService>();
builder.Services.AddScoped<PasswordScoreService>();

var emailConfig = builder.Configuration
        .GetSection("SmtpEmail")
        .Get<EmailConfiguration>();
builder.Services.AddSingleton(emailConfig);

var emailNameConfig = builder.Configuration
        .GetSection("EmailSender")
        .Get<EmailName>();
builder.Services.AddSingleton(emailNameConfig);

var adminPasswordConfig = builder.Configuration
        .GetSection("AdminPassword")
        .Get<AdminPassword>();
builder.Services.AddSingleton(adminPasswordConfig);



builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10);
});

builder.Services.ConfigureApplicationCookie(config =>
{
    config.LoginPath = "/Login";
});

builder.Services.Configure<IdentityOptions>(options =>
{
    //options.User.RequireUniqueEmail = true;
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequireNonAlphanumeric = true;
    options.Password.RequiredLength = 12;
});

//Google Service Extension

builder.Services.AddAuthentication().AddGoogle(x=>
    { 
        x.ClientId = builder.Configuration["Authentication:Google:ClientId"];
        x.ClientSecret = builder.Configuration["Authentication:Google:ClientSecret"];
        x.CallbackPath = "/signin-google";
    }
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseSession();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseStatusCodePagesWithRedirects("/errors/{0}");

app.UseRouting();

app.UseAuthentication();

app.UseAuthorization();

app.MapRazorPages();

app.Run();
