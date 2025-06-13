namespace E_Template.Helpers
{
    using Microsoft.AspNetCore.Hosting;
    using System.IO;

    public class FileService
    {
        private readonly IWebHostEnvironment _environment;

        public FileService(IWebHostEnvironment environment)
        {
            _environment = environment;
        }

        public bool CheckFileExistsInWwwroot(string relativePath)
        {
            if (string.IsNullOrEmpty(relativePath)) { 
                return false;
            }
            string sanitizedRelativePath = relativePath.TrimStart('/'); // Remove leading slash
            string fullPath = Path.Combine(_environment.WebRootPath, sanitizedRelativePath);

            return File.Exists(fullPath);
        }
    }

}
