using Base.Model;
using BaseSQL.Interface;
using BaseWebApi.Interface;
using MaxSys.Interface;
using E_Template.Helpers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SmartTemplateCore.Models.Common;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Text;
using System.Text.Encodings.Web;
using MaxSys.Helpers;
using MaxSys.Models;
using LoginViewModel = Base.Model.LoginViewModel;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Bot.Builder;
using Microsoft.Graph.Models;
using MaxSystemWebSite.Models.SETTING;
using Microsoft.Graph.Models.TermStore;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Text.Json;

namespace MaxSys.Controllers
{
    [Authorize]
    public class HomeController : BaseController
    {
        private readonly HtmlEncoder _htmlEncoder;
        private readonly ILogger<HomeController> _logger;
        private readonly IActionResult _result;
        private readonly IJWTToken _jwtToken;
        private readonly ISQL _SQL;
        private readonly IDapper_Oracle _dapper_Oracle;
        private readonly IWebHostEnvironment _environment;
        private readonly IBotFrameworkHttpAdapter _adapter;
        private readonly IBot _bot;
        private readonly IEmail _emailService;
        private readonly IHttpClientFactory _httpClientFactory;

        public HomeController(ILogger<HomeController> logger, IConfiguration configuration, IWebApi webApi, 
            IDapper dapper, IJWTToken jWTToken, ISQL sql, 
            IDapper_Oracle dapper_Oracle, HtmlEncoder htmlEncoder, IAuthenticator authenticator, IWebHostEnvironment environment,
            IBotFrameworkHttpAdapter adapter, IBot bot,IEmail emailService, IHttpClientFactory httpClientFactory)
        : base(configuration, webApi, dapper, authenticator) // Call the base constructor
        {
            _logger = logger;
            _jwtToken = jWTToken;
            _SQL = sql;
            _htmlEncoder = htmlEncoder;
            _dapper_Oracle = dapper_Oracle;
            _emailService = emailService;
            _environment = environment;
            _adapter = adapter;
            _bot = bot;
            _httpClientFactory = httpClientFactory;
        }

        [HttpGet]
        [NoSessionExpire]
        [AllowAnonymous]
        public async Task<IActionResult> TestPage() 
        {
            return View();
        }

        [HttpGet]
        [NoSessionExpire]
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Login(string ReturnUrl = "")
        {
            //reset cookies
            Response.Cookies.Delete("ACL_JSON");
            Response.Cookies.Delete("USER_ID");
            Response.Cookies.Delete("USER_NAME");
            Response.Cookies.Delete("COMPANY_CODE");
            Response.Cookies.Delete("USER_EMAIL");
            Response.Cookies.Delete("USER_ID_NAME");
            Response.Cookies.Delete("SOLAT_ZONE");
            Response.Cookies.Delete("JWTToken");
            Response.Cookies.Delete("JWTRefreshToken");
            Response.Cookies.Delete("ACCESS_LEVEL"); 
            Response.Cookies.Append("ReturnUrl", ReturnUrl ?? "", new CookieOptions { Expires = DateTime.Now.AddMinutes(15) });

            AuthenticatorModel authenticatorModel = new AuthenticatorModel();
            authenticatorModel.RETURN_URL = ReturnUrl ?? "";
            return View(authenticatorModel);
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(AuthenticatorModel model)
        {
            try
            {
                if (model == null || string.IsNullOrEmpty(model.LOGIN_ID) == true || string.IsNullOrEmpty(model.PASSWORD))
                {
                    ViewBag.Validate = false;
                    return Json(new { success = false, redirectUrl = "/Home/Index" });
                }

                var modelData = new LoginViewModel
                {
                    Email = model.LOGIN_ID,
                    Password = model.PASSWORD,
                    RememberMe = true
                };

                (bool success, string message, Base.Model.Login_Response userModel) returnAuth = await _authenticator.LOGIN(modelData);

                if (returnAuth.success == true)
                {
                    
                    var cookieOptions = new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = false,  // Set to true for production with HTTPS
                        Expires = DateTime.UtcNow.AddYears(30),
                        Path = "/",  // Makes the cookie accessible across the entire app
                        SameSite = SameSiteMode.Strict  // Prevents the cookie from being sent in cross-site requests
                    };

                    Response.Cookies.Append("USER_TOKEN", returnAuth.userModel.JWTToken, cookieOptions);
                    Response.Cookies.Append("EMAIL", returnAuth.userModel.Email, cookieOptions);
                    Response.Cookies.Append("USER_ID", returnAuth.userModel.User_ID, cookieOptions);
                    Response.Cookies.Append("NAME", returnAuth.userModel.Name, cookieOptions);
                    //Response.Cookies.Append("ID_MM_USER", mm_user.user.ID_MM_USER.ToString(), cookieOptions);
                    //Response.Cookies.Append("ID_MM_COMPANY", mm_company.com.ID_MM_COMPANY.ToString(), cookieOptions);
                    //Response.Cookies.Append("MM_COMPANY_NAME", mm_company.com.MM_COMPANY_NAME, cookieOptions);
                    //Response.Cookies.Append("POSTCODE", mm_company.com.POSTCODE, cookieOptions);
                    //Response.Cookies.Append("CITY", mm_company.com.CITY, cookieOptions);
                    //Response.Cookies.Append("STATE", mm_company.com.STATE, cookieOptions);
                    //Response.Cookies.Append("COUNTRY", mm_company.com.COUNTRY, cookieOptions);
                    //Response.Cookies.Append("SOLAT_ZONE", mm_company.com.SOLAT_ZONE, cookieOptions);
                    //Response.Cookies.Append("COMPANY_CODE", mm_company.com.CODE, cookieOptions);
                     Response.Cookies.Append("ACCESS_LEVEL", "3", cookieOptions);


                    
                    //if (!string.IsNullOrEmpty(mm_user.user.PROFILE_IMAGE))
                    //{
                    //    // Get the full physical path of the file
                    //    var fullPath = Path.Combine(_environment.WebRootPath, mm_user.user.PROFILE_IMAGE.TrimStart('/'));

                    //    // Check if the file exists
                    //    if (System.IO.File.Exists(fullPath))
                    //    {
                    //        Response.Cookies.Append("PROFILE_IMAGE", mm_user.user.PROFILE_IMAGE, cookieOptions);
                    //    }
                    //    else {
                    //        Response.Cookies.Append("PROFILE_IMAGE", "/Images/user-avatar.png", cookieOptions);
                    //    }        
                    //}
                    //else {
                    //    Response.Cookies.Append("PROFILE_IMAGE", "/Images/user-avatar.png", cookieOptions);
                    //}

                    Response.Cookies.Append("AUTH_TYPE", "JWT", cookieOptions);

                    Response.Cookies.Append("jwt", returnAuth.userModel.JWTToken, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = false,  // Set to true for production with HTTPS
                        Expires = DateTime.UtcNow.AddYears(30),
                        Path = "/",  // Makes the cookie accessible across the entire app
                        SameSite = SameSiteMode.Strict  // Prevents the cookie from being sent in cross-site requests
                    });

                    ViewBag.Validate = true;

                    if (!string.IsNullOrWhiteSpace(model.RETURN_URL))
                        return Json(new { success = true, redirectUrl = model.RETURN_URL });


                    return Json(new { success = true, redirectUrl = "/MM_UserProfile/Index" });
                }
                else
                {

                    ViewBag.Validate = false;
                    return Json(new { success = false, redirectUrl = "/Home/Index" });
                }



            }
            catch (Exception ex)
            {
                ViewBag.Validate = false;
            }
            return Json(new { success = false, redirectUrl = "/Home/Index" });
        }

        [HttpPost]
        [AllowAnonymous]
        public async Task<IActionResult> Logout(string authenticationType = "JWT")
        {
            if (HttpContext.Request.Cookies.ContainsKey("jwt"))
            {
                Response.Cookies.Delete("jwt");
            }
            Response.Cookies.Append("jwt", "", new CookieOptions
            {
                Expires = DateTimeOffset.UtcNow.AddDays(-1) // Expire immediately
            });

            var deleteCookieOptions = new CookieOptions
            {
                Path = "/",          // Match the path used during append
                HttpOnly = true,     // Ensure this matches the original cookie's settings
                Secure = true        // Match Secure setting if the original cookie was Secure
            };
            Response.Cookies.Delete("ACL_JSON", deleteCookieOptions);
            Response.Cookies.Delete("USER_TOKEN", deleteCookieOptions);
            Response.Cookies.Delete("EMAIL", deleteCookieOptions);
            Response.Cookies.Delete("USER_ID", deleteCookieOptions);
            Response.Cookies.Delete("ID_MM_USER", deleteCookieOptions);
            Response.Cookies.Delete("NAME", deleteCookieOptions);
            Response.Cookies.Delete("ID_MM_COMPANY", deleteCookieOptions);
            Response.Cookies.Delete("MM_COMPANY_NAME", deleteCookieOptions);
            Response.Cookies.Delete("WORK_NO", deleteCookieOptions);
            Response.Cookies.Delete("AUTH_TYPE", deleteCookieOptions);
            Response.Cookies.Delete("SOLAT_ZONE", deleteCookieOptions);
            Response.Cookies.Delete("COMPANY_CODE", deleteCookieOptions);
            Response.Cookies.Delete("ACCESS_LEVEL", deleteCookieOptions);
            Response.Cookies.Delete("PROFILE_IMAGE", deleteCookieOptions);

            Response.Cookies.Append("ReturnUrl", "", new CookieOptions { Expires = DateTime.Now.AddMinutes(-1) });

            // Sign out from both Microsoft Identity and local session
            HttpContext.SignOutAsync();

            if (authenticationType == "OPENID")
            {

                //return SignOut(new AuthenticationProperties
                //{
                //    RedirectUri = "/"

                //}, OpenIdConnectDefaults.AuthenticationScheme, CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
                await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme, new AuthenticationProperties
                {
                    RedirectUri = "/"
                });


                await HttpContext.SignOutAsync("Cookies");

                return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public IActionResult Sidebar()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Chatbot([FromBody] ChatRequest request)
        {
            try
            {
                var client = new HttpClient();
                string apiKey = _configuration["ChatGPT:SecretKey"];
                string modelType = _configuration["ChatGPT:Model"];
                double temperature = double.TryParse(_configuration["ChatGPT:Temperature"], out var value) ? value : 0.7;

                string filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "Template", "MelissaPrompt.txt");

                string strPromtAi = "";
                if (System.IO.File.Exists(filePath))
                {
                    strPromtAi = await System.IO.File.ReadAllTextAsync(filePath);
                }

                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", apiKey);

                request.Messages.Add(new MessageChatBot("system", strPromtAi));

                var body = new
                {
                    model = modelType,
                    messages = request.Messages,
                    temperature = temperature
                };

                var response = await client.PostAsync(
                    "https://api.openai.com/v1/chat/completions",
                    new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json"));

                if (response.IsSuccessStatusCode)
                {
                    var json = await response.Content.ReadAsStringAsync();
                    dynamic data = JsonConvert.DeserializeObject(json);
                    string reply = data.choices[0].message.content;
                    return Ok(reply);
                }

                var errorBody = await response.Content.ReadAsStringAsync();
                return StatusCode(500, $"OpenAI error: {errorBody}");
            }
            catch (Exception ex)
            {
                return StatusCode(500, $"Server error: {ex.Message}");
            }
            
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ContactSubmit(IFormCollection form)
        {
            var name = form["name"];
            var email = form["email"];
            var subject = form["subject"];
            var message = form["message"];

            var emailBody = $"<p><strong>Name:</strong> {name}</p>" +
                            $"<p><strong>Email:</strong> {email}</p>" +
                            $"<p><strong>Message:</strong></p><p>{message}</p>";

            var modelTemp = new Emai_TemplateSent
            {
                Recipient = new List<Recipient>
                {
                     new Recipient
                     {
                         EmailAddress = new EmailAddress
                         {
                             Address = "hr@maxsys.com.my",
                             Name = "Muhammad Azham Bin Rosli"
                         }
                     }
                 },
                CC = new List<Recipient>
                {
                     new Recipient
                     {
                         EmailAddress = new EmailAddress
                         {
                             Address = "azham@maxsys.com.my",
                             Name = "Muhammad Azham Bin Rosli"
                         }
                     },
                     new Recipient
                     {
                         EmailAddress = new EmailAddress
                         {
                             Address = "shazwanie@maxsys.com.my",
                             Name = "Shazwanie (HR)"
                         }
                     },
                     new Recipient
                     {
                         EmailAddress = new EmailAddress
                         {
                             Address = "afina@maxsys.com.my",
                             Name = "Afina (HR)"
                         }
                     }
                 },
                Subject = $"Contact From : {name} {subject}",
                subTemplate = $"<p>Subject: {subject}</p>" +
                        $"<p>Contact Name: {name}</p>" +
                        $"<p>Email: {email}</p>" +
                        $"<p>Message: {message}</p>",
                WORD_REPLACE = new List<(string ori, string replace)>
                 {
                     ("[NAME]", name),
                     ("[EMAIL]", email),
                 },
                Attachments = new List<Emai_TemplateSent.EmailAttachment>()
            };


            modelTemp.mainTemplate = await modelTemp.EmailBodyTemplate();
            modelTemp.bodyContent = modelTemp.mainTemplate.Replace("[BODY]", modelTemp.subTemplate);
            var wordResult = modelTemp.WordReplacer(modelTemp.bodyContent);
            if (wordResult.Item1)
            {
                modelTemp.bodyContent = wordResult.Item2;
            }
            SETTING_EMAIL settingEmail = new SETTING_EMAIL();

            settingEmail.TENANT_ID = _configuration["Settings:TenantId"];
            settingEmail.CLIENT_ID = _configuration["Settings:ClientId"];
            settingEmail.CLIENT_SECRET = _configuration["Settings:ClientSecret"];
            settingEmail.GRAPH_USER = _configuration.GetSection("Settings:GraphUserScopes").Get<string[]>()[0];



            modelTemp.Setting_Setup = new Setting_Setup();
            modelTemp.Setting_Setup.SMTP_ACCOUNT = "hr@maxsys.com.my";
            modelTemp.WORD_REPLACE = new List<(string ori, string replace)>();
            modelTemp.WORD_REPLACE.Add(("[NAME]", ""));
            modelTemp.WORD_REPLACE.Add(("[HELP_DESK_EMAIL]", "hr@maxsys.com.my"));
            modelTemp.WORD_REPLACE.Add(("[APPLICATION_NAME]", "CONTACT FROM CUSTOMER"));
            modelTemp.WORD_REPLACE.Add(("[URL]", $"www.azhamrosli.com"));

            _emailService.InitGraph(settingEmail);

            (bool status, string message) result = await _emailService.SendEmailAsync(modelTemp);

            if (!result.status)
            {
                return Json(new { success = false, message = $"Failed to send message. {result.message}" });
            }

            return Json(new { success = true, message = "Your message has been sent. Thank you!" });
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ApplyJob(IFormCollection form)
        {
            var name = form["applicant-name"];
            var tel = form["applicant-tel"];
            var email = form["applicant-email"];
            var position = form["applicant-position"];
            var salary = form["expected-salary"];
            var description = form["applicant-description"];
            var transportation = form["transportation"];
            var noticeperiod = form["notice-period"];
            var resume = form.Files["applicant-resume"];

            byte[] fileBytes = null;
            string fileName = null;

            if (resume != null && resume.Length > 0)
            {
                var allowedExtensions = new[] { ".pdf", ".docx", ".jpg", ".jpeg", ".png" };
                var fileExtension = Path.GetExtension(resume.FileName).ToLowerInvariant();

                if (!allowedExtensions.Contains(fileExtension))
                {
                    return BadRequest("Unsupported file format.");
                }

                using (var ms = new MemoryStream())
                {
                    await resume.CopyToAsync(ms);
                    fileBytes = ms.ToArray();
                    fileName = resume.FileName;
                }
            }
            var modelTemp = new Emai_TemplateSent
            {
                Recipient = new List<Recipient>
                {
                     new Recipient
                     {
                         EmailAddress = new EmailAddress
                         {
                             Address = "hr@maxsys.com.my",
                             Name = "Muhammad Azham Bin Rosli"
                         }
                     }
                 },
                CC = new List<Recipient>
                {
                     new Recipient
                     {
                         EmailAddress = new EmailAddress
                         {
                             Address = "azham@maxsys.com.my",
                             Name = "Muhammad Azham Bin Rosli"
                         }
                     },
                     new Recipient
                     {
                         EmailAddress = new EmailAddress
                         {
                             Address = "shazwanie@maxsys.com.my",
                             Name = "Shazwanie (HR)"
                         }
                     },
                     new Recipient
                     {
                         EmailAddress = new EmailAddress
                         {
                             Address = "afina@maxsys.com.my",
                             Name = "Afina (HR)"
                         }
                     }
                 },
                Subject = $"Application Job for {position} - {name}",
                 subTemplate = $"<p>Applicant Name: {name}</p>" +
                        $"<p>Telephone: {tel}</p>" +
                        $"<p>Email: {email}</p>" +
                        $"<p>Position Applied: {position}</p>" +
                        $"<p>Expected Salary: {salary}</p>" +
                        $"<p>Transporation: {transportation}</p>" +
                        $"<p>Notice Period: {noticeperiod}</p>" +
                        $"<p>Description: {description}</p>",
                 WORD_REPLACE = new List<(string ori, string replace)>
                 {
                     ("[NAME]", name),
                     ("[EMAIL]", email),
                     ("[POSITION]", position),
                     ("[SALARY]", salary),
                     ("[DESCRIPTION]", description)
                 },
                Attachments = new List<Emai_TemplateSent.EmailAttachment>()
            };

            if (fileBytes != null && fileName != null)
            {
                modelTemp.Attachments.Add(new Emai_TemplateSent.EmailAttachment
                {
                    FileName = fileName,
                    FileContent = fileBytes,
                    ContentType = resume.ContentType
                });
            }

            modelTemp.mainTemplate = await modelTemp.EmailBodyTemplate();
            modelTemp.bodyContent = modelTemp.mainTemplate.Replace("[BODY]", modelTemp.subTemplate);
            var wordResult = modelTemp.WordReplacer(modelTemp.bodyContent);
            if (wordResult.Item1)
            {
                modelTemp.bodyContent = wordResult.Item2;
            }

            SETTING_EMAIL settingEmail = new SETTING_EMAIL();

            settingEmail.TENANT_ID = _configuration["Settings:TenantId"];
            settingEmail.CLIENT_ID = _configuration["Settings:ClientId"];
            settingEmail.CLIENT_SECRET = _configuration["Settings:ClientSecret"];
            settingEmail.GRAPH_USER = _configuration.GetSection("Settings:GraphUserScopes").Get<string[]>()[0];



            modelTemp.Setting_Setup = new Setting_Setup();
            modelTemp.Setting_Setup.SMTP_ACCOUNT = "hr@maxsys.com.my";
            modelTemp.WORD_REPLACE = new List<(string ori, string replace)>();
            modelTemp.WORD_REPLACE.Add(("[NAME]", ""));
            modelTemp.WORD_REPLACE.Add(("[HELP_DESK_EMAIL]", "hr@maxsys.com.my"));
            modelTemp.WORD_REPLACE.Add(("[APPLICATION_NAME]", "JOB APPLICATION"));
            modelTemp.WORD_REPLACE.Add(("[URL]", $"www.azhamrosli.com"));

            _emailService.InitGraph(settingEmail);

            (bool status, string message) result = await _emailService.SendEmailAsync(modelTemp);

            if (!result.status)
            {
                return Json(new { success = false, message = $"Failed to send email. {result.message}" });
            }

            return Json(new { success = true, message = "Application submitted and email sent." });
  
        }


        [HttpGet]
        [NoSessionExpire]
        [AllowAnonymous]
        public async Task<IActionResult> Functions()
        {
            return View();
        }


        [HttpGet]
        [NoSessionExpire]
        [AllowAnonymous]
        public async Task<IActionResult> EmailSummarize()
        {
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> SummarizeEmail(string emailBody)
        {
            string apiKey = _configuration["ChatGPT:SecretKey"];
            string modelType = _configuration["ChatGPT:Model"];
            double temperature = double.TryParse(_configuration["ChatGPT:Temperature"], out var value) ? value : 0.7;

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var payload = new
            {
                model = modelType,
                messages = new[]
                {
                    new { role = "system", content = "You are an assistant that specializes in summarizing emails in a clear and concise way. Extract the key points, action items, and important information from the following email. The summary should be brief, professional, and easy to understand." },
                    new { role = "user", content = "Please summarize the following email. Focus on the main points, key decisions, action items, and any important dates or names. The summary should be concise and suitable for someone who needs a quick understanding of the email content.\n\nEmail:\n\n" + emailBody }
                },
                temperature = temperature
            };

            var jsonPayload = JsonConvert.SerializeObject(payload);
            var response = await client.PostAsync("https://api.openai.com/v1/chat/completions",
                new StringContent(jsonPayload, Encoding.UTF8, "application/json"));

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest(new { error = "Failed to summarize email." });
            }

            var result = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(result);
            var summary = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return Ok(new { summary }); // Return JSON like { "summary": "..." }
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> ReplyEmailPolish(string emailBody, string userName)
        {
            string apiKey = _configuration["ChatGPT:SecretKey"];
            string modelType = _configuration["ChatGPT:Model"];
            double temperature = double.TryParse(_configuration["ChatGPT:Temperature"], out var value) ? value : 0.7;

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", apiKey);

            var payload = new
            {
                model = modelType,
                messages = new[]
                {
                    new {
                        role = "system",
                        content = $"You are an expert writing assistant. Rephrase the user's email to sound professional and polite. At the end, sign off the message with 'Best regards, {userName}'. Do not add subject lines or extra greetings."
                    },
                    new {
                        role = "user",
                        content = "Please polish the following email:\n\n" + emailBody
                    }
                },
                temperature = temperature
            };



            var jsonPayload = JsonConvert.SerializeObject(payload);
            var response = await client.PostAsync("https://api.openai.com/v1/chat/completions",
                new StringContent(jsonPayload, Encoding.UTF8, "application/json"));

            if (!response.IsSuccessStatusCode)
            {
                return BadRequest(new { error = "Failed to polish email." });
            }

            var result = await response.Content.ReadAsStringAsync();
            using var doc = JsonDocument.Parse(result);
            var polishedText = doc.RootElement
                .GetProperty("choices")[0]
                .GetProperty("message")
                .GetProperty("content")
                .GetString();

            return Ok(new { polishedText }); // Return JSON like { "polishedText": "..." }
        }


        [AllowAnonymous]
        [Route("api/messages")]
        [HttpPost, HttpGet]
        public async Task ChatbotTeamAsync()
        {
            await _adapter.ProcessAsync(Request, Response, _bot);
        }
        public async Task<IActionResult> AuditTrail(int ID = 0, string TableName = "", string ReturnURL = "")
        {
            if (string.IsNullOrEmpty(TableName)) {
                return View();
            }

            List<SqlParameter> _pMssql = new List<SqlParameter>();
            _pMssql.Add(new SqlParameter("@TABLE", TableName));
            _pMssql.Add(new SqlParameter("@KEY_VALUE", ID));

            (bool status, string message, DataTable dt)  result = await _SQL.PSP_COMMON_SQL("PSP_GET_AUDIT_TRAIL", CommandType.StoredProcedure,BaseSQL.Enum.BaseEnum.ExecutionType.ExecuteReader, _pMssql);

            if (result.status == false) { return Redirect(ReturnURL); }
            List<string> keyNames = result.dt.Columns.Cast<DataColumn>()
            .Select(column => column.ColumnName)
            .ToList();

            string jsonResult = JsonConvert.SerializeObject(result);

            AuditTrailModel dataModel = new AuditTrailModel
            {
                JsonData = jsonResult,
                ListData = keyNames
            };

            ViewBag.JsonResult = dataModel.JsonData;
            ViewBag.KeyNames = dataModel.ListData;

            return View();
        }

        //[AllowAnonymous]
        //public async Task<IActionResult> GetSidebar()
        //{
        //    //List<AclResource> sidebarItems = null;
        //    //(bool success, string message, List<AclResource> item) side_bar = await _dapper.PSP_COMMON_DAPPER<AclResource>("PSP_ACL_RESOURCE", CommandType.StoredProcedure, new { USER_ID  , ID_MM_COMPANY });

        //    //if (side_bar.success == true && side_bar.item != null)
        //    //{
        //    //    sidebarItems = side_bar.item;
        //    //}
        //    //else
        //    //{
        //    //    string filePath = $"{Directory.GetCurrentDirectory()}\\wwwroot\\json\\sidebar.json";
        //    //    string jsonText = "";
        //    //    if (System.IO.File.Exists(filePath))
        //    //    {
        //    //        jsonText = System.IO.File.ReadAllText(filePath);
        //    //    }

        //    //    // Deserialize JSON to a list of objects
        //    //    sidebarItems = JsonConvert.DeserializeObject<List<AclResource>>(jsonText);
        //    //}

        //    //// Filter based on userAccessLevel
        //    //var filteredSidebar = sidebarItems
        //    //    .Where(item => item.ACCESS_LEVEL <= ACCESS_LEVEL && item.LAYER == 1)
        //    //    .OrderBy(item => item.SEQ)
        //    //    .ToList();

        //    //if (filteredSidebar != null)
        //    //{
        //    //    foreach (var item in filteredSidebar)
        //    //    {
        //    //        if (item.ListChild == null)
        //    //        {
        //    //            item.ListChild = new List<AclResource>();

        //    //            var childSidebar = sidebarItems
        //    //                .Where(data => data.ACCESS_LEVEL <= ACCESS_LEVEL && data.RESOURCE_PARENT_ID == item.ID_ACL_RESOURCE)
        //    //                .OrderBy(data => data.SEQ)
        //    //                .ToList();

        //    //            if (childSidebar != null && childSidebar.Count > 0)
        //    //            {
        //    //                item.ListChild = childSidebar;
        //    //            }
        //    //        }
        //    //    }
        //    //}

        //    //// Serialize back to JSON
        //    //var settings = new JsonSerializerSettings
        //    //{
        //    //    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        //    //};

        //    //var filteredJson = JsonConvert.SerializeObject(filteredSidebar, settings);

        //    return "";
        //}

        //public async Task<IActionResult> GetSidebar()
        //{
        //    List<AclResource> sidebarItems = null;
        //    (bool success, string message, List<AclResource> item) side_bar = await _dapper.PSP_COMMON_DAPPER<AclResource>("PSP_ACL_RESOURCE", CommandType.StoredProcedure, new { USER_ID = EMAIL });

        //    if (side_bar.success == true && side_bar.item != null)
        //    {
        //        sidebarItems = side_bar.item;
        //    }
        //    else
        //    {
        //        string filePath = $"{Directory.GetCurrentDirectory()}\\wwwroot\\json\\sidebar.json";
        //        string jsonText = "";
        //        if (System.IO.File.Exists(filePath))
        //        {
        //            jsonText = System.IO.File.ReadAllText(filePath);
        //        }

        //        Deserialize JSON to a list of objects
        //       sidebarItems = JsonConvert.DeserializeObject<List<AclResource>>(jsonText);
        //    }


        //    Filter based on userAccessLevel
        //    var filteredSidebar = sidebarItems
        //        .Where(item => item.ACCESS_LEVEL <= ACCESS_LEVEL && item.LAYER == 1)
        //        .ToList();

        //    if (filteredSidebar != null)
        //    {

        //        foreach (var item in filteredSidebar)
        //        {
        //            if (item.ListChild == null)
        //            {
        //                item.ListChild = new List<AclResource>();

        //                var childSidebar = sidebarItems
        //                             .Where(data => data.ACCESS_LEVEL <= ACCESS_LEVEL && data.RESOURCE_PARENT_ID == item.ID_ACL_RESOURCE)
        //                             .ToList();
        //                if (childSidebar != null && childSidebar.Count > 0)
        //                {
        //                    item.ListChild = childSidebar;
        //                }
        //            }
        //        }
        //    }
        //    Serialize back to JSON
        //    var settings = new JsonSerializerSettings
        //    {
        //        ReferenceLoopHandling = ReferenceLoopHandling.Ignore
        //    };

        //    var filteredJson = JsonConvert.SerializeObject(filteredSidebar, settings);



        //    return Json(filteredJson);

        //}

        [HttpGet]
        [NoSessionExpire]
        public IActionResult RenderComponent(string ComponentName = "")
        {
            
            return ViewComponent(ComponentName);
        }

        [HttpPost]
        public string GetEcryptstring(string key = "")
        {
            var success = true;
            var message = "";

            string base64String = CommonMethod.ConvertStringTo64base(key);

            var data = new { success, message, data = base64String };
            string returnJson = JsonConvert.SerializeObject(data);
            return returnJson;
        }

        [HttpPost]
        public string GetDecryptstring(string key = "")
        {
            var success = true;
            var message = "";

            string text = CommonMethod.Convert64baseToString(key);

            var data = new { success, message, data = text };
            string returnJson = JsonConvert.SerializeObject(data);
            return returnJson;
        }



        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

       

        [AllowAnonymous]
        public async Task<string> GetSettingSetup()
        {
            try
            {
                (bool success, string message, Setting_Setup model) setup = await _dapper.PSP_COMMON_DAPPER_SINGLE<Setting_Setup>("PSP_SETTING_SETUP", CommandType.StoredProcedure, new { USER_ID });

                return JsonConvert.SerializeObject(new { success = setup.success, message = setup.message, data = setup.model });

            }
            catch (Exception ex)
            {
                return JsonConvert.SerializeObject(new { success = false, message = ex.Message });
            }

        }
    }
}
