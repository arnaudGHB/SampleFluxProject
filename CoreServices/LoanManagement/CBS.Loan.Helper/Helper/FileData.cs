using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

namespace CBS.NLoan.Helper.Helper
{
    public class FileData
    {
        // Asynchronously saves an uploaded file to the specified subfolder in the web root path.
        public static async Task<string> SaveFileAsync(IFormFile file, string subfolder, IHostingEnvironment _hostingEnvironment)
        {
            // Check if the file is valid
            if (file == null || file.Length <= 0)
            {
                return null;
            }

            if (_hostingEnvironment.WebRootPath == null)
            {
                _hostingEnvironment.WebRootPath = _hostingEnvironment.ContentRootPath;
            }

            // Generate a unique file name and create the file path
            var uniqueFileName = Guid.NewGuid().ToString() + "_" + file.FileName;
            var uploadsFolder = Path.Combine(_hostingEnvironment.WebRootPath, subfolder);
            if (!Directory.Exists(uploadsFolder))
            {
                Directory.CreateDirectory(uploadsFolder);
            }
            var filePath = Path.Combine(uploadsFolder, uniqueFileName);

            // Save the file asynchronously
            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(fileStream);
            }
            return filePath;
        }
    }
}
