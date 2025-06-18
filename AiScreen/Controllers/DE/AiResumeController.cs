using BaseSQL.Interface;
using BaseWebApi.Interface;
using MaxSys.Helpers;
using MaxSys.Interface;
using MaxSystemWebSite.Controllers.MM;
using MaxSystemWebSite.Models.DE;
using Microsoft.AspNetCore.Mvc;
using System.Net.Mail;
using System.Net;
using System.Text.Encodings.Web;
using MaxSys.Models;
using MaxSystemWebSite.Models.SETTING;
using Microsoft.Graph.Models;
using SmartTemplateCore.Models.Common;
using System.Security.Claims;

namespace MaxSystemWebSite.Controllers.DE
{
    public class AiResumeController : BaseController
    {
        private readonly HtmlEncoder _htmlEncoder;
        private readonly ILogger<AiResumeController> _logger;
        private readonly IActionResult _result;
        private readonly IJWTToken _jwtToken;
        private readonly ISQL _SQL;
        private readonly IDapper_Oracle _dapper_Oracle;
        private readonly UserProfileService _userProfileService;
        private readonly ISharePoint _sharePoint;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IEmail _emailService;

        public AiResumeController(ILogger<AiResumeController> logger, IConfiguration configuration, IWebApi webApi,
           IDapper dapper, IAuthenticator authenticator, UserProfileService userProfileService, ISharePoint sharePoint, IHttpClientFactory clientFactory,IEmail emailService)
            : base(configuration, webApi, dapper, authenticator) // Call the base constructor
        {
            _logger = logger;
            _userProfileService = userProfileService;
            _clientFactory = clientFactory;
            _emailService = emailService;
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

        [HttpPost]
        public async Task<IActionResult> UploadProxy([FromForm] IFormFile file, [FromForm] string jobDesc)
        {
            if (file == null || string.IsNullOrWhiteSpace(jobDesc))
            {
                return BadRequest("Missing file or job description");
            }

            var client = _clientFactory.CreateClient();
            using var content = new MultipartFormDataContent();

            // ✅ Correct field name for API
            content.Add(new StringContent(jobDesc), "job_desc");
            content.Add(new StringContent(EMAIL), "user_id");

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
