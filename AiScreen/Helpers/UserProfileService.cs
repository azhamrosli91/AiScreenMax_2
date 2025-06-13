using Azure.Identity;
using Microsoft.Graph;
using System.Net.Http.Headers;

namespace MaxSys.Helpers
{
    public class UserProfileService
    {
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IConfiguration _configuration;
        public UserProfileService(IHttpClientFactory httpClientFactory, IConfiguration configuration)
        {
            _httpClientFactory = httpClientFactory;
            _configuration = configuration;
        }

        public async Task<byte[]> GetUserPhotoAsync(string userId)
        {
            try
            {
                var scopes = new[] { "https://graph.microsoft.com/.default" };
                var tenantId = _configuration["Settings:TenantId"];
                var clientId = _configuration["Settings:ClientId"];
                var clientSecret = _configuration["Settings:ClientSecret"];
                var clientSecretCredential = new ClientSecretCredential(
                                tenantId, clientId, clientSecret);
                var graphClient = new GraphServiceClient(clientSecretCredential, scopes);
                var users = await graphClient.Users.GetAsync();
                var photo = await graphClient.Users[userId].Photo.Content.GetAsync();
                if (photo != null)
                {
                    using (var memoryStream = new MemoryStream())
                    {
                        await photo.CopyToAsync(memoryStream);
                        return memoryStream.ToArray();
                    }
                }
                return null;



            }
            catch (Exception ex)
            {
                // Log the exception for troubleshooting
                Console.WriteLine($"Error fetching user photo: {ex.Message}");
                return null;
            }
        }
    }
}
