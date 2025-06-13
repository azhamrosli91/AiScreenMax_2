using BaseSQL.Interface;
using BaseWebApi.Interface;
using MaxSys.Helpers;
using MaxSys.Interface;
using MaxSys.Models;
using MaxSystemWebSite.Controllers.PMO;
using MaxSystemWebSite.Models.EMAIL;
using MaxSystemWebSite.Models.SETTING;
using MaxSystemWebSite.Models.USER;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Graph.Models;
using System.Text.Encodings.Web;

namespace MaxSystemWebSite.Controllers.Email
{
    public class EmailController : BaseController
    {
        private readonly HtmlEncoder _htmlEncoder;
        private readonly ILogger<EmailController> _logger;
        private readonly IActionResult _result;
        private readonly IJWTToken _jwtToken;
        private readonly ISQL _SQL;
        private readonly IDapper_Oracle _dapper_Oracle;
        private readonly UserProfileService _userProfileService;
        private readonly ISharePoint _sharePoint;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IEmail _emailService;

        public EmailController(ILogger<EmailController> logger, IConfiguration configuration, IWebApi webApi,
           IDapper dapper, IAuthenticator authenticator, UserProfileService userProfileService, ISharePoint sharePoint, IHttpClientFactory clientFactory, IEmail emailService)
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
        [HttpPost]
        public async Task<IActionResult> SendComposeEmail([FromBody] EmailContent_Response model)
        {
            try
            {
                if (model == null || model.ToRecipients == null || model.ToRecipients.Count == 0 || model.From.EmailAddress == null)
                {
                    return Json(new { success = false, message = "To or From recipient(s) required." });
                }

                // Prepare email model to send
                var modelTemp = new Emai_TemplateSent
                {
                    Subject = model.Subject ?? "(No Subject)",
                    bodyContent = model.Content ?? "",
                    Setting_Setup = new Setting_Setup
                    {
                        SMTP_ACCOUNT = model.From.EmailAddress.Address
                    },

                    // Convert Microsoft.Graph.Recipient to internal Recipient model
                    Recipient = model.ToRecipients.Select(r => new Recipient
                    {
                        EmailAddress = new Microsoft.Graph.Models.EmailAddress
                        {
                            Address = r.EmailAddress?.Address,
                            Name = r.EmailAddress?.Name
                        }
                    }).ToList(),

                    CC = model.CcRecipients?.Select(r => new Recipient
                    {
                        EmailAddress = new Microsoft.Graph.Models.EmailAddress
                        {
                            Address = r.EmailAddress?.Address,
                            Name = r.EmailAddress?.Name
                        }
                    }).ToList(),

                    BCC = model.BccRecipients?.Select(r => new Recipient
                    {
                        EmailAddress = new Microsoft.Graph.Models.EmailAddress
                        {
                            Address = r.EmailAddress?.Address,
                            Name = r.EmailAddress?.Name
                        }
                    }).ToList(),

                    Attachments = model.AttachmentDto?.Select(a => new Emai_TemplateSent.EmailAttachment
                    {
                        FileName = a.Name,
                        ContentType = a.ContentType,
                        FileContent = ExtractBase64Bytes(a.ContentBytes)
                    }).ToList()

                };

                // Prepare Graph Email Configuration
                SETTING_EMAIL settingEmail = new SETTING_EMAIL
                {
                    TENANT_ID = _configuration["Settings:TenantId"],
                    CLIENT_ID = _configuration["Settings:ClientId"],
                    CLIENT_SECRET = _configuration["Settings:ClientSecret"],
                    GRAPH_USER = _configuration.GetSection("Settings:GraphUserScopes").Get<string[]>()[0]
                };

                _emailService.InitGraph(settingEmail);

                (bool status, string message) result = await _emailService.SendComposeEmailAsync(modelTemp);

                if (!result.status)
                {
                    return Json(new { success = false, message = $"Failed to send message. {result.message}" });
                }

                return Json(new { success = true, message = "Email sent successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = $"Exception occurred: {ex.Message}" });
            }
        }
        private byte[] ExtractBase64Bytes(string base64Data)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(base64Data))
                    return null;

                // If using data URI format: "data:<mime>;base64,<actualBase64>"
                if (base64Data.StartsWith("data:", StringComparison.OrdinalIgnoreCase))
                {
                    var parts = base64Data.Split(',');
                    if (parts.Length == 2)
                    {
                        return Convert.FromBase64String(parts[1]);
                    }
                }

                // If already just base64
                return Convert.FromBase64String(base64Data);
            }
            catch
            {
                return null;
            }
        }



    }
}
