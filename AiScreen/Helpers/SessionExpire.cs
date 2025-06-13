using Azure;
using Base.Model;
using BaseSQL.Interface;
using MaxSys.Models.MM;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Graph.Models;
using System.Data;
using System.Diagnostics;
using System.Diagnostics.Metrics;
using System.DirectoryServices.Protocols;
using System.Net;
using System.Security.Claims;
using System.Web;

namespace MaxSys.Helpers
{
    public class SessionExpireAttribute : ActionFilterAttribute
    {
        //private readonly ILogger<SessionExpireAttribute> _logger;
        private readonly IConfiguration _configuration;
        private readonly IDapper _dapper;
        private readonly IAuthenticator _authenticator;
        public SessionExpireAttribute(IConfiguration configuration, IDapper dapper, IAuthenticator authenticator)
        {
            _configuration = configuration;
            _dapper = dapper;
            _authenticator = authenticator;
        }
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            var controller = context.Controller as BaseController;



            var actionDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            if (actionDescriptor != null)
            {
                var hasAllowAnonymous = actionDescriptor.MethodInfo.GetCustomAttributes(typeof(AllowAnonymousAttribute), true).Any();
                var hasNoSessionExpire = actionDescriptor.MethodInfo.GetCustomAttributes(typeof(NoSessionExpireAttribute), true).Any();

                if (hasAllowAnonymous || hasNoSessionExpire)
                {
                    return;
                }
            }

            var cookies = context.HttpContext.Request.Cookies;
            

            string USER_TOKEN = "";
            string PROFILE_IMAGE = "";
            string EMAIL = "";
            string ID_MM_USER = "";
            string NAME = "";
            string AUTH_TYPE = "";

            (bool status,string data) returnCookies = GetCookies(context, "PROFILE_IMAGE");
            if (returnCookies.status == true && string.IsNullOrEmpty(returnCookies.data) == false) {
                PROFILE_IMAGE = returnCookies.data;
            }

            returnCookies = GetCookies(context, "USER_TOKEN");
            if (returnCookies.status == true && string.IsNullOrEmpty(returnCookies.data) == false)
            {
                USER_TOKEN = returnCookies.data;
            }

            returnCookies = GetCookies(context, "EMAIL");
            if (returnCookies.status == true && string.IsNullOrEmpty(returnCookies.data) == false)
            {
                EMAIL = returnCookies.data;
            }

            returnCookies = GetCookies(context, "ID_MM_USER");
            if (returnCookies.status == true && string.IsNullOrEmpty(returnCookies.data) == false)
            {
                ID_MM_USER = returnCookies.data;
            }

            returnCookies = GetCookies(context, "NAME");
            if (returnCookies.status == true && string.IsNullOrEmpty(returnCookies.data) == false)
            {
                NAME = returnCookies.data;
            }

            returnCookies = GetCookies(context, "AUTH_TYPE");
            if (returnCookies.status == true && string.IsNullOrEmpty(returnCookies.data) == false)
            {
                AUTH_TYPE = returnCookies.data;
            }

            if (controller != null)
            {

                controller.USER_TOKEN = USER_TOKEN;
                controller.PROFILE_IMAGE = PROFILE_IMAGE;
                controller.EMAIL = EMAIL;
                controller.ID_MM_USER = ID_MM_USER;
                controller.NAME = NAME;
                controller.AUTH_TYPE = AUTH_TYPE;
                //controller.Setting_Setup = this.setting_Setup;
            }

            base.OnActionExecuting(context);
        }
        private bool IsCookieExpired(string cookieValue)
        {
            // Parse the cookie value or handle it as needed
            // For example, if your cookie value contains a creation timestamp:
            if (DateTime.TryParse(cookieValue, out var creationDate))
            {
                // Set the fixed expiration duration (e.g., 15 minutes)
                var expirationDuration = TimeSpan.FromMinutes(15);
                var expirationDate = creationDate + expirationDuration;
                return expirationDate < DateTime.Now;
            }
            // Handle other cases if necessary
            return false; // Default behavior if parsing fails
        }
        public override void OnActionExecuted(ActionExecutedContext context)
        {
            //_logger.LogInformation($"Action {context.ActionDescriptor.DisplayName} executed in {executionTime} ms.");
        }
        private (bool Status,string data) GetCookies(ActionExecutingContext context,string CookiesName) {
            string data = "";
            if (context.HttpContext.Request.Cookies.TryGetValue(CookiesName, out var PROFILE_IMAGEcookieValue))
            {
                if (IsCookieExpired(PROFILE_IMAGEcookieValue))
                {
                    data = "";
                    context.HttpContext.Response.Cookies.Delete(CookiesName);
                    string redirectTo = "~/Home/Index";
                    var currentUrl = context.HttpContext.Request.Path;
                    redirectTo = $"{redirectTo}?ReturnUrl={HttpUtility.UrlEncode(currentUrl)}";
                    context.Result = new RedirectResult(redirectTo);
                    return (false, data);
                }
                else
                {
                    data = PROFILE_IMAGEcookieValue.ToString();
                }
            }
            return (true,data);
        }
    }
}
