using BaseSQL.Interface;
using BaseWebApi.Interface;
using MaxSys.Helpers;
using MaxSys.Interface;
using Microsoft.AspNetCore.Mvc;
using System.Text.Encodings.Web;
using System.Text.Json;

namespace MaxSystemWebSite.Controllers.DE
{
    public class OCR_CarPlateController : BaseController
    {
        private readonly HtmlEncoder _htmlEncoder;
        private readonly ILogger<OCR_CarPlateController> _logger;
        private readonly IActionResult _result;
        private readonly IJWTToken _jwtToken;
        private readonly ISQL _SQL;
        private readonly IDapper_Oracle _dapper_Oracle;
        private readonly UserProfileService _userProfileService;
        private readonly ISharePoint _sharePoint;
        private readonly IHttpClientFactory _clientFactory;

        public OCR_CarPlateController(ILogger<OCR_CarPlateController> logger, IConfiguration configuration, IWebApi webApi,
           IDapper dapper, IAuthenticator authenticator, UserProfileService userProfileService, ISharePoint sharePoint, IHttpClientFactory clientFactory)
            : base(configuration, webApi, dapper, authenticator) // Call the base constructor
        {
            _logger = logger;
            _userProfileService = userProfileService;
            _clientFactory = clientFactory;
        }

        public IActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> UploadProxy()
        {
            var apiUrl = "https://ocrplatecar-b9f2andfd2hrb8hk.southeastasia-01.azurewebsites.net/ocr";

            try
            {
                var form = await Request.ReadFormAsync();
                var file = form.Files.FirstOrDefault();

                if (file == null || file.Length == 0)
                    return BadRequest("No image uploaded.");

                using var httpClient = new HttpClient();
                using var formContent = new MultipartFormDataContent();

                // Add image file
                using var stream = file.OpenReadStream();
                var fileContent = new StreamContent(stream);
                fileContent.Headers.ContentType = new System.Net.Http.Headers.MediaTypeHeaderValue(file.ContentType);
                formContent.Add(fileContent, "image", file.FileName);

                // Add OCR parameters
                formContent.Add(new StringContent(form["detail"].ToString()), "detail");
                formContent.Add(new StringContent(form["text_threshold"].ToString()), "text_threshold");
                formContent.Add(new StringContent(form["low_text"].ToString()), "low_text");
                formContent.Add(new StringContent(form["link_threshold"].ToString()), "link_threshold");
                formContent.Add(new StringContent(form["grayscale"].ToString()), "grayscale");

                // Send request
                var response = await httpClient.PostAsync(apiUrl, formContent);
                var content = await response.Content.ReadAsStringAsync();

                if (!response.IsSuccessStatusCode)
                {
                    return StatusCode((int)response.StatusCode, new
                    {
                        error = "OCR API failed",
                        response = content
                    });
                }

                // Parse and extract values BEFORE disposing JsonDocument
                string hex;
                List<string> textsList;
                using (var jsonDoc = JsonDocument.Parse(content))
                {
                    var root = jsonDoc.RootElement;

                    if (!root.TryGetProperty("image", out var imageElement))
                        return BadRequest("Missing 'image' in OCR API response.");

                    hex = imageElement.GetString() ?? "";

                    if (string.IsNullOrWhiteSpace(hex) || hex.Length % 2 != 0)
                        return BadRequest("Invalid image hex received.");

                    textsList = root.TryGetProperty("texts", out var textsElement) && textsElement.ValueKind == JsonValueKind.Array
                        ? textsElement.EnumerateArray().Select(t => t.GetString()).Where(t => t != null).ToList()!
                        : new List<string>();
                }

                // Convert hex to image bytes
                byte[] imageBytes = Enumerable.Range(0, hex.Length / 2)
                    .Select(i => Convert.ToByte(hex.Substring(i * 2, 2), 16))
                    .ToArray();

                // Save to wwwroot/temp
                var fileName = $"{Guid.NewGuid():N}.jpg";
                var folderPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "temp");
                Directory.CreateDirectory(folderPath); // ensure folder exists
                var filePath = Path.Combine(folderPath, fileName);
                await System.IO.File.WriteAllBytesAsync(filePath, imageBytes);

                // Return URL
                var imageUrl = $"/temp/{fileName}";
                return Ok(new
                {
                    imageUrl,
                    texts = textsList
                });
            }
            catch (Exception ex)
            {
                Console.WriteLine("UploadProxy Error: " + ex);

                return StatusCode(500, new
                {
                    error = "Internal server error",
                    message = ex.Message,
                    details = ex.ToString()
                });
            }
        }


        private static byte[] ConvertHexToBytes(string hex)
        {
            int length = hex.Length;
            byte[] bytes = new byte[length / 2];
            for (int i = 0; i < length; i += 2)
            {
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            }
            return bytes;
        }

    }
}
