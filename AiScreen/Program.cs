using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using MaxSys.Interface;
using MaxSys.Helpers;
using BaseWebApi.Interface;
using BaseSQL.Interface;
using BaseWebApi.Repository;
using Component_TableListing.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Authorization;
using System.Globalization;
using Component_TableListing.Services;
using E_Template.Services;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.Identity.Web;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using MaxSystemWebSite.Services;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Builder;
using E_Template.Helpers;
using MaxSystemWebSite.Helpers.Graph;
using Microsoft.AspNetCore.HttpOverrides;
using Microsoft.AspNetCore.Http.Features;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddControllersWithViews();
builder.Services.AddScoped<IWebApi, WebApi>();
builder.Services.AddScoped<IDapper, BaseSQL.Repository.BaseSQL>();
builder.Services.AddScoped<ISQL, BaseSQL.Repository.BaseSQL>();
builder.Services.AddScoped<IDapper_Oracle, BaseSQL.Repository.BaseOracle>();
builder.Services.AddScoped<IErrorLog, BaseSQL.Repository.BaseSQL>();
builder.Services.AddScoped<IJWTToken, JwtToken>();
builder.Services.AddScoped<ITable, TableService>();
builder.Services.AddScoped<IAuthenticator, BaseSQL.Repository.BaseSQL>();
builder.Services.AddSingleton<IConfiguration>(provider => builder.Configuration);
builder.Services.AddScoped<UserProfileService>();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<CommonMethod>();
builder.Services.AddSingleton<IBotFrameworkHttpAdapter, AdapterWithErrorHandler>();
builder.Services.AddTransient<IBot, ChatBot>();
builder.Services.AddSingleton<IEmail, GH_Email>();
builder.Services.AddTransient<ISharePoint, GH_SharePoint>();
builder.Services.AddTransient<IUserProfile, GH_UserProfile>();

builder.Services.AddControllersWithViews(options =>
{
    options.Filters.Add<SessionExpireAttribute>();
});
builder.Services.AddControllers().AddJsonOptions(options =>
{
    options.JsonSerializerOptions.PropertyNamingPolicy = null;
});

// Register EmailService
builder.Services.AddSingleton<IEmailService, EmailService_STMP>();
builder.Services.AddHttpContextAccessor();

// Authentication and Identity
var provider = builder.Services.BuildServiceProvider();
var _configuration = provider.GetRequiredService<IConfiguration>();

var googleClientId = _configuration["Google_Auth:ClientId"];
var googleClientSecret = _configuration["Google_Auth:ClientSecrect"];

builder.Services.AddAuthentication(options =>
{
    options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = OpenIdConnectDefaults.AuthenticationScheme;
})
.AddGoogle(options =>
{
    options.ClientId = googleClientId;
    options.ClientSecret = googleClientSecret;
    options.CallbackPath = "/signin-google";
})

.AddJwtBearer(options =>
{
    options.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var token = context.HttpContext.Request.Cookies["jwt"];
            if (!string.IsNullOrEmpty(token))
            {
                context.Token = token;
            }
            return Task.CompletedTask;
        },
        OnAuthenticationFailed = context =>
        {
            Console.WriteLine("Authentication failed: " + context.Exception.Message);
            return Task.CompletedTask;
        },
        OnTokenValidated = context =>
        {
            Console.WriteLine("Token is valid.");
            return Task.CompletedTask;
        }
    };

    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
        ValidAudience = builder.Configuration["JwtSettings:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration.GetSection("JwtSettings").GetValue<string>("Key").ToString()))
    };
})
.AddMicrosoftIdentityWebApp(builder.Configuration.GetSection("AzureAd"));

//builder.Services.AddRazorPages().AddMvcOptions(options =>
//{
//    var policy = new AuthorizationPolicyBuilder()
//                  .RequireAuthenticatedUser()
//                  .Build();
//    options.Filters.Add(new AuthorizeFilter(policy));
//});

// CORS Configuration
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins("https://localhost", "https://your-production-domain.com") // Add production domain here
            .AllowAnyHeader()
            .AllowAnyMethod();
    });
});
builder.Services.Configure<MicrosoftIdentityOptions>(options =>
{
    options.SignedOutCallbackPath = "/SignOut";
});

// Set default culture
var cultureInfo = new CultureInfo("en-US")
{
    DateTimeFormat = { ShortDatePattern = "dd/MM/yyyy", LongDatePattern = "dd/MM/yyyy HH:mm:ss" }
};
CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

builder.Services.AddAuthorization();

var app = builder.Build();

// Request localization
app.UseRequestLocalization(new RequestLocalizationOptions
{
    DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture(cultureInfo),
    SupportedCultures = new List<CultureInfo> { cultureInfo },
    SupportedUICultures = new List<CultureInfo> { cultureInfo }
});

// Error pages
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseForwardedHeaders(new ForwardedHeadersOptions
{
    ForwardedHeaders = ForwardedHeaders.XForwardedProto
});

// Remove X-Frame-Options
app.Use(async (context, next) =>
{
    context.Response.Headers.Remove("X-Frame-Options");
    await next();
});

// Apply CORS Policy
app.UseCors("AllowFrontend");

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=AiResume}/{action=Index}/{id?}");

app.Run();
