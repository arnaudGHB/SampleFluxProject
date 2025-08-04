using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
//using SixLabors.ImageSharp.Formats.Png;

namespace CBS.AccountManagement.Helper
{
    // Utility class for file-related operations.
    public static class FileData
    {
        // Asynchronously saves an uploaded file to the specified subfolder in the web root path.
        public static async Task<string> SaveFileAsync(IFormFile file, string subfolder, IHostingEnvironment _hostingEnvironment)
        {
            // Check if the file is valid
            if (file == null || file.Length <= 0)
            {
                return null;
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

        // Asynchronously saves a file from base64 encoded source data to the specified path.
        public static async Task SaveFile(string path, string source)
        {
            // Delete the existing file if it exists
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            // Convert base64 encoded data to bytes and write to the file asynchronously
            string base64 = source.Split(',').LastOrDefault();
            if (!string.IsNullOrWhiteSpace(base64))
            {
                byte[] bytes = Convert.FromBase64String(base64);
                await File.WriteAllBytesAsync(path, bytes);
            }
        }

        // Asynchronously saves a thumbnail version of an image file to the specified path.
        public static async Task SaveThumbnailFile(string path, string source)
        {
            // Delete the existing file if it exists
            if (File.Exists(path))
            {
                File.Delete(path);
            }

            // Extract file name and base64 encoded data from the source string
            var fileName = path.Split(Path.DirectorySeparatorChar).LastOrDefault();
            string base64 = source.Split(',').LastOrDefault();
            //if (!string.IsNullOrWhiteSpace(base64))
            //{
            //    // Convert base64 encoded data to bytes and create an IFormFile
            //    byte[] bytes = Convert.FromBase64String(base64);
            //    var stream = new MemoryStream(bytes);
            //    IFormFile file = new FormFile(stream, 0, bytes.Length, fileName, fileName);

            //    // Load the image, resize it to a thumbnail, and save it
            //    using var image = await Image.LoadAsync(file.OpenReadStream());
            //    image.Mutate(x => x.Resize(100, 100));
            //    using var outputStream = new MemoryStream();
            //    image.Save(outputStream, new PngEncoder());
                await File.WriteAllBytesAsync(path, null);
            //}
        }

        // Deletes a file at the specified path if it exists.
        public static void DeleteFile(string path)
        {
            if (File.Exists(path))
            {
                File.Delete(path);
            }
        }
    }
}