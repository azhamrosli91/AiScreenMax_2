using Azure.Identity;
using Microsoft.Graph;
using MaxSys.Interface;
using MaxSystemWebSite.Models.SETTING;
using E_Template.Helpers;
using MaxSystemWebSite.Models.USER;

namespace MaxSystemWebSite.Helpers.Graph
{
    public class GH_UserProfile : IUserProfile
    {
        // Settings object
        private static Settings? _settings;

        // Client secret credential
        private static ClientSecretCredential? _clientSecretCredential;
        // Client configured with application authentication
        private static GraphServiceClient? _appClient;

        public readonly IConfiguration _configuration;

        public GH_UserProfile(IConfiguration configuration)
        {
            _configuration = configuration;


            if (string.IsNullOrEmpty(_configuration["Settings:TenantId"]) ||
               string.IsNullOrEmpty(_configuration["Settings:ClientId"]) ||
               string.IsNullOrEmpty(_configuration["Settings:ClientSecret"]) ||
               string.IsNullOrEmpty(_configuration.GetSection("Settings:GraphUserScopes").Get<string[]>()[0]))
            {
                throw new ArgumentException("Invalid Graph API settings");
            }

            // Initialize the credential using client secret
            _clientSecretCredential = new ClientSecretCredential(
            _configuration["Settings:TenantId"],
            _configuration["Settings:ClientId"],
                _configuration["Settings:ClientSecret"]
            );

            // Initialize Graph client with the credential and desired scopes
            _appClient = new GraphServiceClient(_clientSecretCredential, new[] { _configuration.GetSection("Settings:GraphUserScopes").Get<string[]>()[0] });

        }

        public void InitGraph(SETTING_EMAIL settings)
        {

            // Ensure that required properties are not null
            if (string.IsNullOrEmpty(settings.TENANT_ID) ||
                string.IsNullOrEmpty(settings.CLIENT_ID) ||
                string.IsNullOrEmpty(settings.CLIENT_SECRET))
            {
                throw new ArgumentException("Invalid Graph API settings");
            }

            // Initialize the credential using client secret
            _clientSecretCredential = new ClientSecretCredential(
                settings.TENANT_ID,
                settings.CLIENT_ID,
                settings.CLIENT_SECRET
            );

            // Initialize Graph client with the credential and desired scopes
            _appClient = new GraphServiceClient(_clientSecretCredential, new[] { settings.GRAPH_USER });
        }

        #region "USER PROFILE"
        public async Task<(bool success, string message, string? base64Image)> GetUserProfilePhotoAsync(string userEmail)
        {
            try
            {
                if (_appClient == null)
                {
                    return (false, "Graph has not been initialized for app authentication", null);
                }

                if (string.IsNullOrWhiteSpace(userEmail))
                {
                    return (false, "User email is required", null);
                }

                // Call the Graph API to get the user's photo
                var photoStream = await _appClient.Users[userEmail].Photo.Content.GetAsync();

                if (photoStream == null)
                {
                    return (false, "No profile photo found", null);
                }

                using (var memoryStream = new MemoryStream())
                {
                    await photoStream.CopyToAsync(memoryStream);
                    var imageBytes = memoryStream.ToArray();
                    var base64Image = Convert.ToBase64String(imageBytes);
                    var mimeType = "image/jpeg"; // Default Microsoft Graph format

                    return (true, "Profile photo retrieved successfully", $"data:{mimeType};base64,{base64Image}");
                }
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}", null);
            }
        }

        public async Task<(bool success, string message, List<USER_PROFILE> ListData)> GetUserList()
        {
            try
            {
                if (_appClient == null)
                {
                    return (false, "Graph has not been initialized for app authentication", null);
                }

                var users = new List<USER_PROFILE>();

                var page = await _appClient.Users.GetAsync(req =>
                {
                    req.QueryParameters.Select = new[] { "displayName", "mail", "userPrincipalName" };
                    req.QueryParameters.Top = 150;
                });

                while (page != null)
                {
                    foreach (var user in page.Value)
                    {
                        users.Add(new USER_PROFILE
                        {
                            Display_Name = user.DisplayName,
                            Email = user.Mail ?? user.UserPrincipalName,
                            Profile_Photo = await GetUserPhotoBase64(user.UserPrincipalName)
                        });
                    }

                    if (string.IsNullOrEmpty(page.OdataNextLink))
                        break;

                    // ✅ Correct way to get next page in SDK v5
                    page = await _appClient.Users
                        .WithUrl(page.OdataNextLink)
                        .GetAsync();
                }

                return (true, "User list fetched successfully", users);
            }
            catch (Exception ex)
            {
                return (false, $"Error: {ex.Message}", null);
            }
        }


        private string ExtractSkipToken(string nextLink)
        {
            var uri = new Uri(nextLink);
            var query = System.Web.HttpUtility.ParseQueryString(uri.Query);
            return query["$skiptoken"];
        }
        private async Task<string> GetUserPhotoBase64(string userPrincipalName)
        {
            if (string.IsNullOrWhiteSpace(userPrincipalName)) return null;

            try
            {
                var stream = await _appClient.Users[userPrincipalName].Photo.Content.GetAsync();
                if (stream == null) return null;

                using var ms = new MemoryStream();
                await stream.CopyToAsync(ms);
                var base64 = Convert.ToBase64String(ms.ToArray());
                return $"data:image/jpeg;base64,{base64}";
            }
            catch
            {
                // Skip if user has no photo or permission error
                return null;
            }
        }


        #endregion
    }
}
