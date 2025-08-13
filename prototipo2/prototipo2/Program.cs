using prototipo2.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authentication.Cookies;
using prototipo2.Servicios;
using prototipo2.Services;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);
// Servicios
builder.Services.AddControllersWithViews();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(1);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddControllersWithViews();
builder.Services.AddSession();
builder.Services.AddScoped<IUtilitarios, Utilitarios>();
builder.Services.AddHttpClient<PayPalService>();
builder.Services.AddScoped<PayPalService>();

// Configuración JWT
var key = Encoding.UTF8.GetBytes(builder.Configuration["Start:LlaveSegura"]);

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
})
.AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuerSigningKey = true,
        IssuerSigningKey = new SymmetricSecurityKey(key),
        ValidateIssuer = false,
        ValidateAudience = false,
        ClockSkew = TimeSpan.Zero // No dar tiempo extra
    };
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();
app.UseSession();

app.MapControllerRoute(
     name: "default",
    pattern: "{controller=Cliente}/{action=InicioSesion}/{id?}");

app.MapControllerRoute(
    name: "catalogo",
    pattern: "Catalogo/{action=Index}/{id?}",
    defaults: new { controller = "Catalogo" });

app.Run();

app.MapControllerRoute(
    name: "catalogo",
    pattern: "catalogo",
    defaults: new { controller = "Catalogo", action = "Index" }
);