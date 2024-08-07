using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.EntityFrameworkCore;
using restaurant_demo_website.Data;
using restaurant_demo_website.Models;
using restaurant_demo_website.Services;
using Microsoft.AspNetCore.Identity;
using System.Globalization;
using System.Diagnostics;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
//var connectionString = builder.Configuration.GetConnectionString("DefaultConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
//builder.Services.AddDbContext<ApplicationDbContext>(options =>
//    options.UseSqlServer(connectionString));

//builder.Services.AddDefaultIdentity<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true).AddEntityFrameworkStores<ApplicationDbContext>();
//builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//builder.Services.AddIdentityCore<IdentityUser>(options => options.SignIn.RequireConfirmedAccount = true)
//    .AddEntityFrameworkStores<ApplicationDbContext>();

builder.Services.AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromMinutes(20);
        options.SlidingExpiration = true;
        options.LoginPath = "/Identity/Account/Login";
        options.LogoutPath ="/Identity/Account/logout";
    });

builder.Services.AddHttpContextAccessor();

builder.Services.AddHttpClient<EntitiesRequest>(opt=> {
    opt.Timeout = TimeSpan.FromMinutes(10);
});

builder.Services.AddSession(opt=>{
    opt.IdleTimeout = TimeSpan.FromSeconds(10);
    opt.Cookie.HttpOnly = true;
    opt.Cookie.IsEssential = true;
});
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();

builder.Services.AddScoped<IEntitiesRequest, EntitiesRequest>();
builder.Services.AddScoped<IMapService, MapService>();


builder.Services.AddScoped<ShoppingCart>();
builder.Services.AddMemoryCache();
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.UseExceptionHandler("/Error");
}
else
{
    app.UseExceptionHandler("/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

try
{
    var culture = await new GetCultureName(builder.Configuration, new HttpClient()).GetNameAsync();
    var localization = new RequestLocalizationOptions
    {
        DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture(culture),
        CultureInfoUseUserOverride = true,
        ApplyCurrentCultureToResponseHeaders = true,
        SupportedCultures = new[] { new CultureInfo(culture) },
        SupportedUICultures = new[] { new CultureInfo(culture) }
    };
    app.UseRequestLocalization(localization);
}
catch (Exception ex)
{

    Debug.WriteLine(ex.Message);
}

app.UseAuthentication();

app.UseSession();
app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();

app.Run();
