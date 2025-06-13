using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using BaseSQL.Interface;
using System.Data;
using System.Security.Claims;
using BaseWebApi.Interface;
using System.Text.Encodings.Web;
using MaxSys.Interface;
using MaxSys.Helpers;
using E_Template.Helpers;
using System.Text;
using MaxSys.Models.MM;
using Base.Model;
using Microsoft.AspNet.Identity.EntityFramework;
using MaxSystemWebSite.Models.SETTING;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.Google;

namespace MaxSys.Controllers
{
    [Authorize]
    [AllowAnonymous]
    [NoSessionExpire]
    public class AccountController : BaseController
    {
        private readonly HtmlEncoder _htmlEncoder;
        private readonly ILogger<AccountController> _logger;
        private readonly IActionResult _result;
        private readonly IJWTToken _jwtToken;
        private readonly ISQL _SQL;
        private readonly IDapper_Oracle _dapper_Oracle;
        private readonly UserProfileService _userProfileService;
        private readonly IEmailService _emailService;
        private readonly IUserProfile _userProfile;

        public AccountController(ILogger<AccountController> logger, IConfiguration configuration, IWebApi webApi,
            IDapper dapper, IJWTToken jWTToken, ISQL sql,
            IDapper_Oracle dapper_Oracle, HtmlEncoder htmlEncoder, IAuthenticator authenticator, UserProfileService userProfileService, IEmailService emailService, IUserProfile userProfile)
        : base(configuration, webApi, dapper, authenticator) // Call the base constructor
        {
            _logger = logger;
            _jwtToken = jWTToken;
            _SQL = sql;
            _htmlEncoder = htmlEncoder;
            _dapper_Oracle = dapper_Oracle;
            _userProfileService = userProfileService;
            _emailService = emailService;
            _userProfile = userProfile;
        }

        [AllowAnonymous]
        [NoSessionExpire]
        public IActionResult Sign()
        {
            // Set the return URL to redirect after successful sign-in
            var returnUrl = Url.Action("SignIn", "Account");

            // Trigger the OpenID Connect authentication process and pass the return URL
            return Challenge(new AuthenticationProperties { RedirectUri = returnUrl }, OpenIdConnectDefaults.AuthenticationScheme);
           // return Challenge( OpenIdConnectDefaults.AuthenticationScheme);
        }

        [AllowAnonymous]
        [HttpGet("login-google")]
        public IActionResult LoginGoogle(string returnUrl = "/Account/SignIn")
        {
            var properties = new AuthenticationProperties { RedirectUri = returnUrl };
            return Challenge(properties, "Google");
        }

        [AllowAnonymous]
        public async Task<IActionResult> SignIn()
        {

            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Unauthorize", "Errors", new { message = "User.Identity.IsAuthenticated = false" });
            }

            var claims = User.Claims.Select(c => new { c.Type, c.Value });

            if (claims == null)
            {
                return RedirectToAction("Unauthorize", "Errors");
            }



            string email = claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value ??
                                           claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            string name = claims.FirstOrDefault(c => c.Type == "name")?.Value ??
                          claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ??
                          claims.FirstOrDefault(c => c.Type == "preferred_username")?.Value ??
                          claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;

            (bool success, string message, IdentityUser emp) employee = _dapper.PSP_COMMON_DAPPER_SINGLE_SYNC<IdentityUser>
                                  ("PSP_AUTH_GET_USER", CommandType.StoredProcedure, new { EMAIL = email });

            if (employee.success == false || employee.emp == null)
            {

                RegisterViewModel modelRegister = new RegisterViewModel
                {
                    Email = email,
                    Name = name,
                    Password = "MaxSystem@123",
                    ConfirmPassword = "MaxSystem@123",
                    ID_MM_COMPANY = "0",
                    MM_COMPANY_CODE = "MAXSYS"
                };
                (bool status, string messsage) users_register = _authenticator.REGISTER_USER_SYNC(modelRegister);

                if (users_register.status == false)
                {
                    return RedirectToAction("Unauthorize", "Errors", new { message = "employee is null or " + employee.message });
                }
                else
                {
                    employee = _dapper.PSP_COMMON_DAPPER_SINGLE_SYNC<IdentityUser>
                                      ("PSP_AUTH_GET_USER", CommandType.StoredProcedure, new { EMAIL = email });
                }

            }


            IdentityUser identityUser = new IdentityUser();
            identityUser.Id = Guid.NewGuid().ToString();
            identityUser.UserName = employee.emp.UserName.ToUpper();
            identityUser.Email = employee.emp.Email;

            string token = _authenticator.GenerateToken(identityUser);


            Response.Cookies.Append("ReturnUrl", "", new CookieOptions { Expires = DateTime.Now.AddMinutes(-1) });


            var cookieOptions = new CookieOptions
            {
                Path = "/",           // Ensure a consistent path
                SameSite = SameSiteMode.Unspecified // Specify how cookies should be handled
            };

            Response.Cookies.Append("USER_TOKEN", token, cookieOptions);
            Response.Cookies.Append("EMAIL", employee.emp.Email, cookieOptions);
            Response.Cookies.Append("ID_MM_USER", identityUser.Id, cookieOptions);
            Response.Cookies.Append("NAME", employee.emp.UserName.ToUpper(), cookieOptions);
            Response.Cookies.Append("AUTH_TYPE", "OPENID", cookieOptions);

            Response.Cookies.Append("jwt", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = true,  // Set to true for production with HTTPS
                Expires = DateTime.UtcNow.AddYears(30),
                Path = "/",  // Makes the cookie accessible across the entire app
                SameSite = SameSiteMode.Strict  // Prevents the cookie from being sent in cross-site requests
            });

            // Trigger the OpenID Connect authentication process and pass the return URL
            return Redirect("/AiResume/Index?refresh=1");
        }

        [AllowAnonymous]
        public async Task<IActionResult> GoogleCallback()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Unauthorize", "Errors", new { message = "Google login failed" });
            }

            var claims = User.Claims.Select(c => new { c.Type, c.Value });

            string email = claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
            string name = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value ?? email;

            if (string.IsNullOrEmpty(email))
            {
                return RedirectToAction("Unauthorize", "Errors", new { message = "Google did not return email" });
            }

            // Find user in database
            var employee = _dapper.PSP_COMMON_DAPPER_SINGLE_SYNC<IdentityUser>
                ("PSP_AUTH_GET_USER", CommandType.StoredProcedure, new { EMAIL = email });
            (bool success, string message, IdentityUser emp) = _dapper.PSP_COMMON_DAPPER_SINGLE_SYNC<IdentityUser>(
    "PSP_AUTH_GET_USER", CommandType.StoredProcedure, new { EMAIL = email });

            if (!success || emp == null)
            {
                // Register user if not exists
                var modelRegister = new RegisterViewModel
                {
                    Email = email,
                    Name = name,
                    Password = "MaxSystem@123",
                    ConfirmPassword = "MaxSystem@123",
                    ID_MM_COMPANY = "0",
                    MM_COMPANY_CODE = "MAXSYS"
                };

                if (!success || emp == null)
                {
                    return RedirectToAction("Unauthorize", "Errors", new { message = message });
                }

                // Re-fetch user
                employee = _dapper.PSP_COMMON_DAPPER_SINGLE_SYNC<IdentityUser>
                    ("PSP_AUTH_GET_USER", CommandType.StoredProcedure, new { EMAIL = email });
            }

            var identityUser = new IdentityUser
            {
                Id = Guid.NewGuid().ToString(),
                UserName = emp.UserName.ToUpper(),
                Email = emp.Email
            };

            var token = _authenticator.GenerateToken(identityUser);

            string pictureUrl = claims.FirstOrDefault(c => c.Type == "picture")?.Value;
            if (!string.IsNullOrEmpty(pictureUrl))
{
    Response.Cookies.Append("PROFILE_IMAGE", pictureUrl, new CookieOptions
    {
        Path = "/",
        Expires = DateTime.UtcNow.AddYears(1),
        SameSite = SameSiteMode.Lax,
        Secure = true // Only for HTTPS!
    });
}

            // Set cookies
            var cookieOptions = new CookieOptions
            {
                Path = "/",
                SameSite = SameSiteMode.Unspecified
            };

            Response.Cookies.Append("USER_TOKEN", token, cookieOptions);
            Response.Cookies.Append("EMAIL", emp.Email, cookieOptions);
            Response.Cookies.Append("ID_MM_USER", identityUser.Id, cookieOptions);
            Response.Cookies.Append("NAME", emp.UserName.ToUpper(), new CookieOptions
            {
                Path = "/",
                Expires = DateTimeOffset.UtcNow.AddYears(1),
                SameSite = SameSiteMode.Lax
            });
            Response.Cookies.Append("NAME", emp.UserName.ToUpper(), cookieOptions);
            Response.Cookies.Append("AUTH_TYPE", "GOOGLE", cookieOptions);

            Response.Cookies.Append("jwt", token, new CookieOptions
            {
                HttpOnly = true,
                Secure = false,
                Expires = DateTime.UtcNow.AddYears(30),
                Path = "/",
                SameSite = SameSiteMode.Strict
            });

            return Redirect("/AiResume/Index?refresh=1");
        }



        [HttpGet]
        [AllowAnonymous]
        [Route("signout-oidc")]
        public IActionResult SignOut()
        {
            // 🔥 Clear all cookies
            foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }

            // 🔁 Redirect after logout
            return SignOut(
                new AuthenticationProperties
                {
                    RedirectUri = Url.Action("Index", "AiResume", null, Request.Scheme) // e.g. https://localhost/AiResume/Index
                },
                OpenIdConnectDefaults.AuthenticationScheme,
                CookieAuthenticationDefaults.AuthenticationScheme
            );
        }


        [HttpGet]
        [AllowAnonymous]
        public IActionResult SignOut2(string page = "")
        {
            // 🔥 Clear all cookies
            foreach (var cookie in Request.Cookies.Keys)
            {
                Response.Cookies.Delete(cookie);
            }
            return Redirect("/AiResume/Index?refresh=1");

            // 🔁 Redirect after logout
            //return SignOut(
            //    new AuthenticationProperties
            //    {
            //        RedirectUri = Url.Action("Index", "AiResume", null, Request.Scheme) // e.g. https://localhost/AiResume/Index
            //    },
            //    OpenIdConnectDefaults.AuthenticationScheme,
            //    CookieAuthenticationDefaults.AuthenticationScheme
            //);
        }

        [HttpGet]
        [AllowAnonymous]
        [NoSessionExpire]
        public async Task<IActionResult> GetProfileImage(string email = "")
        {
            var photoBytes = await _userProfileService.GetUserPhotoAsync(email);

            if (photoBytes == null)
            {
                return Json(new { success = false, msg = "no profile photo." });
            }
            // Convert photo bytes to Base64
            var base64String = Convert.ToBase64String(photoBytes);
            string base64Image = $"data:image/jpeg;base64,{base64String}";

            return Json(new { success = true, msg = "Profile photo retrieved.", data = base64Image });
        }


        [HttpPost]
        [AllowAnonymous]
        [NoSessionExpire]
        public async Task<IActionResult> ForgotPasswordVerify(ChangePasswordViewModel model) 
        {
            try
            {
                if (model == null)
                {
                    return Json(new { success = false, message = "Data not found" });
                }

                if (string.IsNullOrEmpty(model.Email))
                {
                    return Json(new { success = false, message = "Sila masukkan email anda." });
                }

                if (string.IsNullOrEmpty(model.NewPassword) || string.IsNullOrEmpty(model.ConfirmPassword))
                {
                    return Json(new { success = false, message = "Sila masukkan kata laluan anda." });
                }


                byte[] data = Convert.FromBase64String(model.NewPassword);
                // Convert byte array to a normal string
                string normalString = Encoding.UTF8.GetString(data);
                model.NewPassword = normalString;

                byte[] data2 = Convert.FromBase64String(model.ConfirmPassword);
                // Convert byte array to a normal string
                string normalString2 = Encoding.UTF8.GetString(data2);
                model.ConfirmPassword = normalString2;


                (bool success, string message) returnAuth = await _authenticator.CHANGE_PASSWORD(model);

                if (returnAuth.success == false)
                {
                    return Json(new { success = false, message = "Failed to change password" });
                }
                else {
                    return Json(new { success = true, message = $"Successfully changes password." });
                }

            }
            catch (Exception ex)
            {

                return Json(new { success = false, message = ex.Message });
            }
        }
       

        [HttpGet]
        [AllowAnonymous]
        [NoSessionExpire]
        public async Task<IActionResult> ForgotPassword(string Email = "")
        {
            ChangePasswordViewModel model = new ChangePasswordViewModel();
            model.Email = Email;
            return View(model);
        }

        //void InitializeGraph(E_Template.Helpers.Settings settings)
        //{
        //    GraphHelper.InitializeGraphForAppAuth(settings);
        //}
    }
}
