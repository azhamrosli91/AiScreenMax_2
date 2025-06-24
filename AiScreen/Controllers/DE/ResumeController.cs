using Base.Model;
using BaseSQL.Interface;
using BaseWebApi.Interface;
using Dapper;
using MaxSys.Controllers;
using MaxSys.Helpers;
using MaxSys.Interface;
using MaxSys.Models;
using MaxSys.Models.DE;
using MaxSystemWebSite.Controllers.DE;
using MaxSystemWebSite.Controllers.MM;
using MaxSystemWebSite.Models.DE;
using MaxSystemWebSite.Models.EMAIL;
using MaxSystemWebSite.Models.SETTING;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Bot.Builder;
using Microsoft.Bot.Builder.Integration.AspNet.Core;
using Microsoft.Extensions.Logging;
using Microsoft.Graph.Models;
using SmartTemplateCore.Models.Common;
using System;
using System.Collections.Generic;
using System.Data;
using System.Net;
using System.Net.Http;
using System.Net.Mail;
using System.Security.Claims;
using System.Text.Encodings.Web;
using System.Text.Json;
using System.Threading.Tasks;

namespace MaxSystemWebSite.Controllers.DE
{
    public class ResumeController : BaseController
    {
        private readonly HtmlEncoder _htmlEncoder;
        private readonly ILogger<ResumeController> _logger;
        private readonly IActionResult _result;
        private readonly IJWTToken _jwtToken;
        private readonly ISQL _SQL;
        private readonly IDapper_Oracle _dapper_Oracle;
        private readonly IWebHostEnvironment _environment;
        private readonly IBotFrameworkHttpAdapter _adapter;
        private readonly IBot _bot;
        private readonly IEmail _emailService;
        private readonly IHttpClientFactory _httpClientFactory;

        public ResumeController(ILogger<ResumeController> logger, IConfiguration configuration, IWebApi webApi,
            IDapper dapper, IJWTToken jWTToken, ISQL sql,
            IDapper_Oracle dapper_Oracle, HtmlEncoder htmlEncoder, IAuthenticator authenticator, IWebHostEnvironment environment,
            IBotFrameworkHttpAdapter adapter, IBot bot, IEmail emailService, IHttpClientFactory httpClientFactory)
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

        public IActionResult Index()
        {

            return View();
        }

        public IActionResult Guide()
        {
            return View();
        }
        public IActionResult History()
        {
            return View();
        }

        public IActionResult ShortListed()
        {
            return View();
        }

        public IActionResult Subscription()
        {
            return View();
        }

        public IActionResult HelpCenter()
        {
            return View();
        }

        public IActionResult Checkout()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadProxy([FromForm] IFormFile file, [FromForm] string jobDesc)
        {
            if (file == null || string.IsNullOrWhiteSpace(jobDesc))
            {
                return BadRequest("Missing file or job description");
            }

            var client = _httpClientFactory.CreateClient();
            using var content = new MultipartFormDataContent();

            // ✅ Correct field name for API
            content.Add(new StringContent(jobDesc), "job_desc");

            var fileContent = new StreamContent(file.OpenReadStream());
            fileContent.Headers.ContentDisposition = new System.Net.Http.Headers.ContentDispositionHeaderValue("form-data")
            {
                Name = "file",
                FileName = file.FileName
            };
            fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);

            content.Add(fileContent, "file", file.FileName);

            var response = await client.PostAsync("https://airesumeapi-amfpg4anayejfbdx.southeastasia-01.azurewebsites.net/evaluate-resume", content);
            var responseBody = await response.Content.ReadAsStringAsync();

            return Content(responseBody, "application/json");
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

        [HttpPost]
        public async Task<IActionResult> SendInterviewEmail([FromBody] InterviewDataModel data)
        {
            try
            {
                var senderEmail = User.FindFirst("preferred_username")?.Value
               ?? User.FindFirst(ClaimTypes.Email)?.Value;

                var senderName = User.Identity?.Name ?? "User";

                var malaysiaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("Singapore Standard Time");
                var parsedDateTime = DateTime.Parse(data.Datetime); // Convert string to DateTime
                var malaysiaTime = TimeZoneInfo.ConvertTimeFromUtc(parsedDateTime.ToUniversalTime(), malaysiaTimeZone);
                string formattedDate = malaysiaTime.ToString("dd/MM/yy hh:mm tt") + " GMT+8";

                var subject = $"Interview Scheduled for {data.Candidate.Name}";
                var subContent = $@"
            <p><strong>Candidate:</strong> {data.Candidate.Name} ({data.Candidate.Email})</p>
            <p><strong>Interview Date:</strong> {formattedDate}</p>
            <p><strong>Location:</strong> {data.Location}</p>
            <p><strong>Interviewers:</strong></p>
            <ul>
                {string.Join("", data.Interviewers.Select(i => $"<li>{i.Name} ({i.Email})</li>"))}
            </ul>";

                var modelTemp = new Emai_TemplateSent
                {
                    Recipient = new List<Recipient>
    {
        new Recipient
        {
            EmailAddress = new EmailAddress
            {
                Address = data.Candidate.Email,
                Name = data.Candidate.Name
            }
        }
    },
                    CC = data.Interviewers
    .Select(i => new Recipient
    {
        EmailAddress = new EmailAddress
        {
            Address = i.Email,
            Name = i.Name
        }
    })
    .Append(new Recipient
    {
        EmailAddress = new EmailAddress
        {
            Address = senderEmail,
            Name = senderName
        }
    })
    .ToList(),
                    Subject = subject,
                    subTemplate = subContent,
                    WORD_REPLACE = new List<(string ori, string replace)>
    {
        ("[NAME]", data.Candidate.Name),
        ("[EMAIL]", data.Candidate.Email),
        ("[APPLICATION_NAME]", "Interview Notification"),
        (" [HELP_DESK_EMAIL]", _configuration["Settings:HelpDeskEmail"])
    },
                    Attachments = new List<Emai_TemplateSent.EmailAttachment>()
                };

                // Optional: load the HTML body template
                modelTemp.mainTemplate = await modelTemp.EmailBodyTemplate();
                modelTemp.bodyContent = modelTemp.mainTemplate.Replace("[BODY]", modelTemp.subTemplate);
                var wordResult = modelTemp.WordReplacer(modelTemp.bodyContent);
                if (wordResult.Item1) modelTemp.bodyContent = wordResult.Item2;

                // Settings for Microsoft Graph
                var settingEmail = new SETTING_EMAIL
                {
                    TENANT_ID = _configuration["Settings:TenantId"],
                    CLIENT_ID = _configuration["Settings:ClientId"],
                    CLIENT_SECRET = _configuration["Settings:ClientSecret"],
                    GRAPH_USER = _configuration.GetSection("Settings:GraphUserScopes").Get<string[]>()[0]
                };

                modelTemp.Setting_Setup = new Setting_Setup
                {
                    SMTP_ACCOUNT = "hr@maxsys.com.my"
                };

                _emailService.InitGraph(settingEmail);

                (bool status, string message) = await _emailService.SendEmailAsync(modelTemp);

                if (!status)
                {
                    return StatusCode(500, new { success = false, message });
                }

                return Ok(new { success = true, sentTo = data.Candidate.Email });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

    }
}
